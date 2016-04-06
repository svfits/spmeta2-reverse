﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using SPMeta2.Containers.Consts;
using SPMeta2.Containers.Services;
using SPMeta2.Containers.Utils;
using SPMeta2.CSOM.Standard.Services;
using SPMeta2.Definitions;
using SPMeta2.Exceptions;
using SPMeta2.Models;
using SPMeta2.Reverse.CSOM.Foundation.ReverseHosts;
using SPMeta2.Reverse.CSOM.Foundation.Services;
using SPMeta2.Reverse.Regression.Services;
using SPMeta2.Standard.Definitions.Taxonomy;
using SPMeta2.Standard.Syntax;
using SPMeta2.Utils;
using SPMeta2.Reverse.Tests.Services;
using SPMeta2.CSOM.ModelHosts;
using SPMeta2.Reverse.CSOM.Foundation.ReverseHandlers.Base;
using SPMeta2.Reverse.CSOM.Standard.Services;
using SPMeta2.Reverse.Services;
using SPMeta2.Reverse.Regression.Base;
using SPMeta2.Containers.Services.Rnd;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPMeta2.Reverse.Regression;
using SPMeta2.Containers.Assertion;
using SPMeta2.Reverse.Regression.Consts;
using System.IO;

namespace SPMeta2.Reverse.Tests.Base
{
    [Serializable]
    public class ReverseCoverageResult
    {
        public ReverseCoverageResult()
        {
            Properties = new List<ReverseCoveragePropertyResult>();
        }

        public DefinitionBase Model { get; set; }
        public string ModelFullClassName { get; set; }
        public string ModelShortClassName { get; set; }

        public List<ReverseCoveragePropertyResult> Properties { get; set; }
    }

    [Serializable]
    public class ReverseCoveragePropertyResult
    {
        public string SrcPropertyValue { get; set; }
        public string SrcPropertyName { get; set; }

        public string DstPropertyValue { get; set; }
        public string DstPropertyName { get; set; }

        public bool IsValid { get; set; }
        public string Message { get; set; }
    }

    [TestClass]
    public class ReverseTestBase
    {
        #region static

        protected static void DeleteAllSubWebs(Web web)
        {
            var context = web.Context;

            context.Load(web,
            website => website.Webs,
            website => website.Title);

            context.ExecuteQuery();
            for (int i = 0; i != web.Webs.Count; )
            {
                DeleteAllWebs(web.Webs[i]);
            }
        }
        private static void DeleteAllWebs(Web web)
        {
            var context = web.Context;

            context.Load(web,
                website => website.Webs,
                website => website.Title
            );

            context.ExecuteQuery();

            for (int i = 0; i != web.Webs.Count; )
            {
                DeleteAllWebs(web.Webs[i]);
            }

            web.DeleteObject();
            web.Update();

            context.ExecuteQuery();
        }

        static ReverseTestBase()
        {
            GlobalInternalInit();
        }

        private static void GlobalInternalInit()
        {
            RegressionAssertService.OnPropertyValidated += OnReversePropertyValidated;
        }

        private static void OnReversePropertyValidated(object sender, OnPropertyValidatedEventArgs e)
        {
            // huge TODO :)
            var reportsFolder = "../../../SPMeta2.Reverse.Tests/_coverage_reports";
            var readMeFolder = "../../../..";

            var validationResults = ReverseRegressionAssertService.ModelValidations;
            var uniqueResults = new List<ReverseCoverageResult>();

            foreach (var result in validationResults)
            {
                if (!uniqueResults.Any(r => r.Model.GetType() == result.Model.GetType()))
                {
                    var newResult = new ReverseCoverageResult();

                    newResult.Model = result.Model;

                    newResult.ModelFullClassName = result.Model.GetType().FullName;
                    newResult.ModelShortClassName = result.Model.GetType().Name;

                    foreach (var propResult in result.Properties)
                    {
                        var newPropResult = new ReverseCoveragePropertyResult();

                        if (propResult.Src != null)
                        {
                            newPropResult.SrcPropertyName = propResult.Src.Name;
                            newPropResult.SrcPropertyValue = ConvertUtils.ToString(propResult.Src.Value);
                        }

                        if (propResult.Dst != null)
                        {
                            newPropResult.DstPropertyName = propResult.Dst.Name;
                            newPropResult.DstPropertyValue = ConvertUtils.ToString(propResult.Dst.Value);
                        }

                        newPropResult.IsValid = propResult.IsValid;
                        newPropResult.Message = propResult.Message;

                        newResult.Properties.Add(newPropResult);
                    }

                    uniqueResults.Add(newResult);
                }
            }

            uniqueResults = uniqueResults.OrderBy(r => r.Model.GetType().Name)
                                         .ToList();

            var types = uniqueResults.Select(r => r.Model.GetType()).ToList();

            types.AddRange(uniqueResults.Select(r => r.Model.GetType()).ToList());

            types.Add(typeof(ModelValidationResult));
            types.Add(typeof(PropertyValidationResult));

            var xml = XmlSerializerUtils.SerializeToString(uniqueResults, types);
            System.IO.File.WriteAllText(reportsFolder + "/_m2.reverse-coverage.xml", xml);

            // save all reports for caching purposes
            foreach (var result in uniqueResults.OrderBy(s => s.ModelShortClassName))
            {
                var definitionName = result.ModelShortClassName;
                var fileName = string.Format("_m2.{0}.def-coverage.html", definitionName);

                var reportContent = GenerateDefinitionCoverateTable(result);
                System.IO.File.WriteAllText(reportsFolder + "/" + fileName, reportContent);
            }

            // generae the full report
            var report = string.Empty;
            report += "<div class='m-reverse-report-cnt'>";

            var files = Directory.GetFiles(reportsFolder, "*.def-coverage.html")
                                 .OrderBy(f => f);

            foreach (var file in files)
            {
                report += System.IO.File.ReadAllText(file);
            }

            report += "</div>";

            System.IO.File.WriteAllText(reportsFolder + "/_m2.reverse-coverage.html", report);

            // updating readme
            var readMeContent = System.IO.File.ReadAllText(readMeFolder + "/README-TEMPLATE.md");
            readMeContent = readMeContent.Replace("[[COVERAGE-REPORT]]", report);

            System.IO.File.WriteAllText(readMeFolder + "/README.md", readMeContent);
        }

        private static string GenerateDefinitionCoverateTable(ReverseCoverageResult result)
        {
            var report = string.Empty;

            report += string.Format("<h4>{0}</h4>", result.ModelShortClassName);

            report += "<table>";

            report += "<thead>";
            report += "<td>Property</td>";
            report += "<td>Support</td>";
            report += "<td>Comments</td>";
            report += "<thead>";

            report += "<tbody>";

            foreach (var propResult in result.Properties.OrderBy(p =>
            {
                var localPropName = p.SrcPropertyName;

                if (localPropName.Contains("."))
                    localPropName = localPropName.Split('.')[1];

                return localPropName;
            }))
            {
                var propName = propResult.SrcPropertyName;

                // method calls, such as 's.Scope.ToString()	'
                if (propName.Contains("."))
                    propName = propName.Split('.')[1];

                report += "<tr>";
                report += string.Format("<td>{0}</td>", propName);

                if (propResult.Message == SkipMessages.NotImplemented)
                {
                    report += string.Format("<td>{0}</td>", false);
                    report += string.Format("<td>{0}</td>", propResult.Message);
                }
                else
                {
                    report += string.Format("<td>{0}</td>", propResult.IsValid);
                    report += string.Format("<td>{0}</td>", propResult.Message);
                }

                report += "</tr>";
            }
            report += "</tbody>";

            report += "</table>";

            return report;
        }

        #endregion

        #region constructors

        public ReverseTestBase()
        {
            SiteUrl = RunnerEnvironmentUtils.GetEnvironmentVariables(EnvironmentConsts.O365_SiteUrls).First();

            UserName = RunnerEnvironmentUtils.GetEnvironmentVariable(EnvironmentConsts.O365_UserName);
            UserPassword = RunnerEnvironmentUtils.GetEnvironmentVariable(EnvironmentConsts.O365_Password);

            AssertService = new VSAssertService();

            ModelGeneratorService = new ModelGeneratorService();

            ModelGeneratorService.RegisterDefinitionGenerators(typeof(FieldDefinition).Assembly);
            ModelGeneratorService.RegisterDefinitionGenerators(typeof(TaxonomyTermDefinition).Assembly);

            Rnd = new DefaultRandomService();
        }

        #endregion

        #region properties

        public ModelGeneratorService ModelGeneratorService { get; set; }

        public RandomService Rnd { get; set; }

        public string SiteUrl { get; set; }

        public string UserName { get; set; }
        public string UserPassword { get; set; }


        public AssertServiceBase AssertService { get; set; }

        #endregion

        #region methods

        public void DeployReverseAndTestModel(ModelNode model)
        {
            DeployReverseAndTestModel(model, null);
        }

        public void DeployReverseAndTestModel(ModelNode model, IEnumerable<Type> reverseHandlers)
        {
            DeployReverseAndTestModel(new ModelNode[] { model }, reverseHandlers);
        }

        public void DeployReverseAndTestModel(IEnumerable<ModelNode> models)
        {
            DeployReverseAndTestModel(models, null);
        }

        public void DeployReverseAndTestModel(IEnumerable<ModelNode> models,
            IEnumerable<Type> reverseHandlers)
        {
            foreach (var deployedModel in models)
            {
                // deploy model
                DeployModel(deployedModel);

                // reverse model
                var reversedModel = ReverseModel(deployedModel, reverseHandlers);

                // validate model
                var reverseRegressionService = new ReverseValidationService();

                reverseRegressionService.DeployModel(new ReverseValidationModeHost
                {
                    OriginalModel = deployedModel,
                    ReversedModel = reversedModel,
                }, null);

                // assert model
                var hasMissedOrInvalidProps = ReverseRegressionAssertService.ResolveModelValidation(deployedModel);
                AssertService.IsFalse(hasMissedOrInvalidProps);
            }
        }

        private ModelNode ReverseModel(ModelNode deployedModel,
            IEnumerable<Type> reverseHandlers)
        {
            ReverseResult reverseResut = null;

            WithCSOMContext(context =>
            {
                var reverseService = new StandardCSOMReverseService();

                if (reverseHandlers != null)
                {
                    reverseService.Handlers.Clear();

                    foreach (var reverseHandler in reverseHandlers)
                    {
                        var reverseHandlerInstance = Activator.CreateInstance(reverseHandler)
                            as CSOMReverseHandlerBase;

                        reverseService.Handlers.Add(reverseHandlerInstance);
                    }
                }

                if (deployedModel.Value.GetType() == typeof(FarmDefinition))
                {
                    throw new SPMeta2NotImplementedException(
                        string.Format("Runner does not support model of type: [{0}]", deployedModel.Value.GetType()));
                }
                else if (deployedModel.Value.GetType() == typeof(WebApplicationDefinition))
                {
                    throw new SPMeta2NotImplementedException(
                        string.Format("Runner does not support model of type: [{0}]", deployedModel.Value.GetType()));
                }
                else if (deployedModel.Value.GetType() == typeof(SiteDefinition))
                {
                    reverseResut = reverseService.ReverseSiteModel(context, ReverseOptions.Default);
                }
                else if (deployedModel.Value.GetType() == typeof(WebDefinition))
                {
                    reverseResut = reverseService.ReverseWebModel(context, ReverseOptions.Default);
                }
                else if (deployedModel.Value.GetType() == typeof(ListDefinition))
                {
                    throw new SPMeta2NotImplementedException(
                        string.Format("Runner does not support model of type: [{0}]", deployedModel.Value.GetType()));
                }
                else
                {
                    throw new SPMeta2NotImplementedException(
                        string.Format("Runner does not support model of type: [{0}]", deployedModel.Value.GetType()));

                }

            });

            return reverseResut.Model;
        }

        private void DeployModel(ModelNode model)
        {
            WithCSOMContext(context =>
            {
                var provisionService = new StandardCSOMProvisionService();

                if (model.Value.GetType() == typeof(FarmDefinition))
                {
                    throw new SPMeta2NotImplementedException(
                        string.Format("Runner does not support model of type: [{0}]", model.Value.GetType()));
                }
                else if (model.Value.GetType() == typeof(WebApplicationDefinition))
                {
                    throw new SPMeta2NotImplementedException(
                     string.Format("Runner does not support model of type: [{0}]", model.Value.GetType()));
                }
                else if (model.Value.GetType() == typeof(SiteDefinition))
                {
                    provisionService.DeployModel(SiteModelHost.FromClientContext(context), model);
                }
                else if (model.Value.GetType() == typeof(WebDefinition))
                {
                    provisionService.DeployModel(WebModelHost.FromClientContext(context), model);
                }
                else if (model.Value.GetType() == typeof(ListDefinition))
                {
                    throw new SPMeta2NotImplementedException(
                     string.Format("Runner does not support model of type: [{0}]", model.Value.GetType()));
                }
                else
                {
                    throw new SPMeta2NotImplementedException(
                        string.Format("Runner does not support model of type: [{0}]", model.Value.GetType()));

                }
            });
        }

        #endregion

        #region utils

        private static SecureString GetSecurePasswordString(string password)
        {
            var securePassword = new SecureString();

            foreach (var s in password)
                securePassword.AppendChar(s);

            return securePassword;
        }


        public void WithCSOMContext(Action<ClientContext> action)
        {
            WithCSOMContext(SiteUrl, action);
        }

        public void WithCSOMContext(string siteUrl, Action<ClientContext> action)
        {
            WithCSOMContext(siteUrl, UserName, UserPassword, action);
        }

        /// <summary>
        /// Invokes given action under CSOM client context.
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <param name="action"></param>
        private void WithCSOMContext(string siteUrl, string userName, string userPassword, Action<ClientContext> action)
        {
            using (var context = new ClientContext(siteUrl))
            {
                context.Credentials = new SharePointOnlineCredentials(userName, GetSecurePasswordString(userPassword));
                action(context);
            }
        }


        #endregion
    }
}

﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPMeta2.Containers;
using SPMeta2.Definitions;
using SPMeta2.Definitions.Fields;
using SPMeta2.Reverse.Services;
using SPMeta2.Reverse.Tests.Base;
using SPMeta2.Syntax.Default;

namespace SPMeta2.Reverse.Tests.Impl.Definitions.Fields
{
    [TestClass]
    public class OutcomeChoiceFieldDefinitionTests : ReverseTestBase
    {
        #region tests

        [TestMethod]
        [TestCategory("Fields.OutcomeChoice")]
        public void Can_Reverse_Site_OutcomeChoiceFields()
        {
            var model = SPMeta2Model.NewSiteModel(site =>
            {
                site.AddOutcomeChoiceField(Def<OutcomeChoiceFieldDefinition>());
                site.AddOutcomeChoiceField(Def<OutcomeChoiceFieldDefinition>());
            });

            DeployReverseAndTestModel(model);
        }

        [TestMethod]
        [TestCategory("Fields.OutcomeChoice")]
        public void Can_Reverse_Web_OutcomeChoiceFields()
        {
            var options = ReverseOptions.Default
                            .AddDepthOption<WebDefinition>(0);

            var model = SPMeta2Model.NewWebModel(web =>
            {
                web.AddOutcomeChoiceField(Def<OutcomeChoiceFieldDefinition>());
                web.AddOutcomeChoiceField(Def<OutcomeChoiceFieldDefinition>());
            });

            DeployReverseAndTestModel(model,options);
        }

        #endregion
    }
}

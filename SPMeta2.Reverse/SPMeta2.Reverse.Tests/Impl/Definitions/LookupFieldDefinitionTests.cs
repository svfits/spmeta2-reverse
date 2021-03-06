using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPMeta2.Containers;
using SPMeta2.Definitions;
using SPMeta2.Definitions.Fields;
using SPMeta2.Reverse.CSOM.ReverseHandlers;
using SPMeta2.Reverse.CSOM.Services;
using SPMeta2.Reverse.Services;
using SPMeta2.Reverse.Tests.Base;
using SPMeta2.Syntax.Default;
using SPMeta2.Enumerations;

namespace SPMeta2.Reverse.Tests.Impl.Definitions
{
    [TestClass]
    public class LookupFieldDefinitionTests : ReverseTestBase
    {
        #region tests

        [TestMethod]
        [TestCategory("LookupFields")]
        public void Can_Reverse_LookupFields()
        {
            // only root web
            var options = ReverseOptions.Default
                            .AddDepthOption<WebDefinition>(0);

            // TODO
            // var title = Rnd.String();
            // options.AddFilterOption<LookupFieldDefinition>(d => d.Title == title);

            var model = SPMeta2Model.NewSiteModel(site =>
            {
                site.AddField(Def<LookupFieldDefinition>(def =>
                {
                    
                }));
            });

            DeployReverseAndTestModel(model, options);
        }

        #endregion
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPMeta2.Definitions;
using SPMeta2.Reverse.Services;
using SPMeta2.Reverse.Tests.Base;
using SPMeta2.Standard.Definitions.Fields;
using SPMeta2.Standard.Syntax;
using SPMeta2.Syntax.Default;

namespace SPMeta2.Reverse.Tests.Impl.Definitions.Fields
{
    [TestClass]
    public class LinkFieldDefinitionTests : ReverseTestBase
    {
        #region tests

        [TestMethod]
        [TestCategory("Fields.Link")]
        public void Can_Reverse_Site_LinkFields()
        {
            var model = SPMeta2Model.NewSiteModel(site =>
            {
                site.AddLinkField(Def<LinkFieldDefinition>());
                site.AddLinkField(Def<LinkFieldDefinition>());
            });

            DeployReverseAndTestModel(model);
        }

        [TestMethod]
        [TestCategory("Fields.Link")]
        public void Can_Reverse_Web_LinkFields()
        {
            var options = ReverseOptions.Default
                            .AddDepthOption<WebDefinition>(0);

            var model = SPMeta2Model.NewWebModel(web =>
            {
                web.AddLinkField(Def<LinkFieldDefinition>());
                web.AddLinkField(Def<LinkFieldDefinition>());
            });

            DeployReverseAndTestModel(model, options);
        }

        #endregion
    }
}

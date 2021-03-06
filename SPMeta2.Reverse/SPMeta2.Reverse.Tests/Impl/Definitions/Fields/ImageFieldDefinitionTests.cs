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
    public class ImageFieldDefinitionTests : ReverseTestBase
    {
        #region tests

        [TestMethod]
        [TestCategory("Fields.Image")]
        public void Can_Reverse_Site_ImageFields()
        {
            var model = SPMeta2Model.NewSiteModel(site =>
            {
                site.AddImageField(Def<ImageFieldDefinition>());
                site.AddImageField(Def<ImageFieldDefinition>());
            });

            DeployReverseAndTestModel(model);
        }

        [TestMethod]
        [TestCategory("Fields.Image")]
        public void Can_Reverse_Web_ImageFields()
        {
            var options = ReverseOptions.Default
                            .AddDepthOption<WebDefinition>(0);

            var model = SPMeta2Model.NewWebModel(web =>
            {
                web.AddImageField(Def<ImageFieldDefinition>());
                web.AddImageField(Def<ImageFieldDefinition>());
            });

            DeployReverseAndTestModel(model, options);
        }

        #endregion
    }
}

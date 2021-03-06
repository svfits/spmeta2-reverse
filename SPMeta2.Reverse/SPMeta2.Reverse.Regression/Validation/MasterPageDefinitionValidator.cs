using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SPMeta2.Definitions;
using SPMeta2.Reverse.Regression.Base;
using SPMeta2.Utils;
using SPMeta2.Reverse.Regression.Consts;
using SPMeta2.Containers.Assertion;

namespace SPMeta2.Reverse.Regression.Validation
{
    public class MasterPageDefinitionValidator : TypedReverseDefinitionValidatorBase<MasterPageDefinition>
    {
        public override void DeployModel(object modelHost, DefinitionBase model)
        {
            var typedModelHost = modelHost.WithAssertAndCast<ReverseValidationModeHost>("modelHost", m => m.RequireNotNull());

            var originalDefinition = typedModelHost.OriginalModel.Value.WithAssertAndCast<MasterPageDefinition>("value", m => m.RequireNotNull());
            var reversedDefinition = typedModelHost.ReversedModel.Value.WithAssertAndCast<MasterPageDefinition>("value", m => m.RequireNotNull());

            var assert = ServiceFactory.AssertService.NewAssert(originalDefinition, reversedDefinition);

            assert
                .ShouldBeEqual(s => s.FileName, r => r.FileName)
                .ShouldBeEqual(s => s.Title, r => r.Title)

                .SkipProperty(s => s.Description, SkipMessages.NotImplemented)
                .SkipProperty(s => s.DefaultCSSFile, SkipMessages.NotImplemented)
                .SkipProperty(s => s.UIVersion, SkipMessages.NotImplemented)

                .SkipProperty(s => s.ContentTypeId, SkipMessages.NotImplemented)
                .SkipProperty(s => s.ContentTypeName, SkipMessages.NotImplemented)
                .SkipProperty(s => s.Values, SkipMessages.NotImplemented)
                .SkipProperty(s => s.DefaultValues, SkipMessages.NotImplemented)

                .ShouldBeEqual((p, s, d) =>
                {
                    var isValid = true;

                    var srcProp = s.GetExpressionValue(o => o.Content);
                    var dstProp = d.GetExpressionValue(o => o.Content);

                    if (d.Content == null)
                    {
                        isValid = false;
                    }
                    else if (s.Content.Length != d.Content.Length)
                    {
                        isValid = false;
                    }
                    else
                    {
                        isValid = s.Content.SequenceEqual(d.Content);
                    }

                    return new PropertyValidationResult
                    {
                        Tag = p.Tag,
                        Src = srcProp,
                        Dst = dstProp,
                        IsValid = isValid
                    };
                });
            ;

        }
    }
}

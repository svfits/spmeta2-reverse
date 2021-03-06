# SPMeta2.Reverse
A library to provide reverse engineering of the existing SharePoint sites into SPMeta2 models.

### Build status
| Branch  | Status |
| ------------- | ------------- |
| dev   | [![Build status](https://ci.appveyor.com/api/projects/status/73pbufcanckaxnqi/branch/dev?svg=true)](https://ci.appveyor.com/project/SubPointSupport/spmeta2-reverse/branch/dev)  |
| beta  | [![Build status](https://ci.appveyor.com/api/projects/status/73pbufcanckaxnqi/branch/beta?svg=true)](https://ci.appveyor.com/project/SubPointSupport/spmeta2-reverse/branch/beta)  |
| master| [![Build status](https://ci.appveyor.com/api/projects/status/73pbufcanckaxnqi/branch/master?svg=true)](https://ci.appveyor.com/project/SubPointSupport/spmeta2-reverse/branch/master) |

### SPMeta2.Reverse in details

SPMeta2.Reverse provides a simple API to generate SPMeta2 models from the existing SharePoint sites. 
As easy as that. We are aiming to reverse engineer O365, SharePoint 2016/2013 web sites and generate a valid, redeployable SPMeta2 model.
As always, full regression testing, nice coverage for the SharePoint atrifact and friendly support is included.

As for the API, that's how we see it happening:
```cs
// SharePoint CSOM context - O365/SP2016/SP2013
var context = ..; 

// create the magic reverse service
var service = new CSOMReverseService();

// reverse the SharePoint site and web into M2 model
var siteModelResult = service.ReverseSiteModel(context, ReverseOptions.Default);
var webModelResult = service.ReverseWebModel(context, ReverseOptions.Default);

// here we go, your M2 models backed for you
// deploy later to other SharePoint site, farm or serialize and keep it for the future
var siteModel = siteModelResult.Model;
var webModel = webModelResult.Model;

```
Stay tuned, releasing fiest versions Feb, 2016. 

#### Feature requests, support and contributions
In case you have unexpected issues or keen to see new features please contact support on SPMeta2 Yammer or here at github:

* [https://www.yammer.com/spmeta2feedback](https://www.yammer.com/spmeta2feedback)
* [https://twitter.com/spmeta2](https://twitter.com/spmeta2)


### Current coverage support per definition
[Detailed coverage report](https://github.com/SubPointSolutions/spmeta2-reverse/blob/master/M2.Reverse.Coverage.Status.md)

* BooleanFieldDefinition
* ChoiceFieldDefinition
* ContentTypeDefinition
* ContentTypeFieldLinkDefinition
* ContentTypeLinkDefinition
* CurrencyFieldDefinition
* DateTimeFieldDefinition
* FeatureDefinition
* FieldDefinition
* FolderDefinition
* GeolocationFieldDefinition
* GuidFieldDefinition
* HTMLFieldDefinition
* ImageFieldDefinition
* LinkFieldDefinition
* ListDefinition
* ListViewDefinition
* LookupFieldDefinition
* MasterPageDefinition
* MediaFieldDefinition
* ModuleFileDefinition
* MultiChoiceFieldDefinition
* NoteFieldDefinition
* NumberFieldDefinition
* OutcomeChoiceFieldDefinition
* PropertyDefinition
* QuickLaunchNavigationNodeDefinition
* SandboxSolutionDefinition
* SecurityGroupDefinition
* SecurityRoleDefinition
* SummaryLinkFieldDefinition
* TaxonomyFieldDefinition
* TaxonomyTermDefinition
* TaxonomyTermGroupDefinition
* TaxonomyTermSetDefinition
* TaxonomyTermStoreDefinition
* TextFieldDefinition
* TopNavigationNodeDefinition
* UniqueContentTypeFieldsOrderDefinition
* URLFieldDefinition
* UserCustomActionDefinition
* UserFieldDefinition
* WebDefinition
* WebPartPageDefinition
* WelcomePageDefinition
* WikiPageDefinition

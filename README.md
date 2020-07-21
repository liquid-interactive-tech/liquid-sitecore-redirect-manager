# Liquid Sitecore URL Redirect Manager

The URL Redirect Manager is a Sitecore plug-in that reduces wasted time and resources spent managing and troubleshooting URL redirects. Editors can now gain a stronger sense of control and ownership of their website development and management.

Additionally, this plug-in can be an enabling tool for editors wanting valuable SEO control – it’s designed to be easily accessible and helps minimize the need for additional development resources.

## Features

The URL Redirect Manager plug-in provides the following features:

* Rapid creation of server-wide and site-specific redirects, including vanity URLs
* Dynamic Sitecore webpage redirecting
* Customization for all redirects include:
* Permanent and Temporary status codes
* Query string preservation support
* Ability to match incoming requests based on patterns
* Site specific request formatting to redirect and reformat incoming URLs for improved SEO

## Requirements
* Sitecore 9+

## How to install:

Download and install the package via the Sitecore Installation Wizard.

*This module has not been tested for compatibility with SXA and it's own implementation of item-based redirects*.

*This module makes use of a custom implementation of the Name Value List field type in order to allow for special characters in the "Name" column for Redirect Maps.*

## How To Get Started

1. Navigate to /sitecore/system/Modules/Redirects in the Content Editor and right click and insert a "Site Settings" item
    * This will be the item that contains all the redirects for one of your Sitecore Sites as declared in your \<sites> configuration
    * Consider naming these items the equivalent of the name attribute of the \<site> you wish to associate with this set of redirects
    * Make note of this item's ID, and patch in an added attribute to the Sitecore <site> node you want the redirects to apply to, equal to ID. 
    * Ex: \<site name="website" redirectSettingsId="{66CCDA46-DD79-47BB-BBF8-B6F376E9321C}"/>
2. Right click on your Site Settings item (or the Global folder), insert any of the redirect types, and fill in the necessary details.
3. Test in live mode and/or publish if necessary and visit the path you designated as your source (if using an Item Redirect be sure to visit the item's URL as it would be resolved by Sitecore) and get redirected to your desired destination!
   
## Redirect types

The Redirect Manager offers 3 different types of redirect templates that can be implemented and used by backend contributors:

* **Item Redirect** - Point requests made to any Sitecore item with a URL to a new destination (a different Sitecore item, an external URL, or a media item). Target link inherits your link manager settings and customizations.
* **Path Redirect** - Point any plain text path relative to the current domain to a new destination (a different Sitecore item, an external URL, or a media item). Source can also support relative (must start with ^) regex pattern matching. Target link inherits your link manager settings and customizations.
* **Redirect Map** - Quickly generate the IIS equivalent of a rewrite map, using Source/Target plain text pairs relative to the current domain. Source can also support relative (must start with ^) regex pattern matching. Target is a plain text path.

All redirects support the choice of return a 301 (Permanent) or 302 (Temporary) and whether or not to preserve the incoming query string. 

Redirects can be applied server-wide (agnostic of domain) or on a per site basis. This is determined by the placement of the redirect items.

Redirects created as descendants of a Site Settings item will only be evaluated when the site connected to that item.

Redirects can also be created in the "/sitecore/system/Modules/Global Redirects" folder. These redirects will be handled as "Global" redirects that are agnostic of the current request domain.

## Site Redirect Settings

Using the optional Site Redirect Settings template as your site's redirect item container, you can set site-wide settings that alter incoming URLs to meet a consistent SEO friendly format of your choosing:

  * Force www - force the incoming request's domain to be prepended with "www."
  * Force trailing slash - force the incoming path part of the request to be appended with a "/"
  * Force lowercase urls - force the incoming request to be all lowercase
  * Force https - redirect incoming requests to https 
    * *this is an application level redirect and is not a replacement for the server level handshake that must occur, therefore you must have your certificate and related binding set on your IIS server to use this. 
    * If your application offloads SSL through devices such as load balancers, do not use this setting as they may send HTTP requests 

## How it works:

On incoming request, the Redirect Manager will find the related Site Settings item (using the \<site>'s redirectSettingsId attribute) to load in all of the known redirects (which must be descendants of the Site Settings item) associated with that site.

The Redirect Manager will then loop through all of the Site's redirects and determine if it's Source matches the incoming path requested (regex pattern when applicable). On match, the URL will be redirected as determined by the Target.

**For Item and Path redirect types**, the target URL will be generated using the GetItemUrl method, which means it will inherit all settings/customization you have made to your current link provider. *Note that if your target is an internal media link, the link will use the current context site's domain (as media stored in the media library is agnostic of domain)*. 

**For redirect map redirect types**, the Source to match and Target to redirect to are determined by each entry row. Neither of these entries utilize Internal Links to Sitecore items, so make sure the target path is representative of what your final (relative) URL should evaluate to in order to reduce chained redirects.

Redirect Targets that are internal Sitecore items will always inherit the \<linkProvider> settings in the construction of the final URL to redirect to (by virtue of the GetItemURl function), with the exception of the AlwaysIncludeServerUrl and SiteResolving settings, which are required to be true for this module to work. Make sure your link provider settings are aligned with your server side rewrites (such as lower case URLs, .aspx trims, etc.) in order to avoid chained redirects.

## What can't it do?

The Redirect Manager is not intended as a complete replacement for web.config \<rewrite> nodes, and is instead intended to give Content Editors and Developers a limited toolset for rapidly solving simple redirect use cases typically associated with content changes and marketing campaigns, without the need for a code change. 

There are a few common use cases that this module intentionally does not support:

* Domain redirects
* Language version based redirects for \<site>s that feature multiple languages (all redirect fields are processed as "shared")
* Multiple conditions for single redirects AKA "MatchAll" 
* Conflict/contradict the \<linkProvider> settings for internal target destinations

## Configuration options

Each redirect type is enabled via config and is processed in the order in which they appear on each incoming request by way of the \<httpRequestBegin> pipeline. Should you wish to not make certain redirect types inaccessible to editors, you can do any combination of the following:

* Restrict users/roles from using the related redirect type's template using the Sitecore Security Editor
* Remove the insert options from the related folder templates
* Comment out the related redirect type's \<processor> via LiquidSC.Foundation.RedirectManager.config

### Cache
A cache containing the source and resolved target of each redirect/redirect map entry on a per site basis is stored for fast lookups after the first request. 

You may control at a database level when cache is used. Modify the RedirectManagerCacheDatabases value in LiquidSC.Foundation.RedirectManager.config with a comma delimited list of database names. You can adjust this setting further based on your environment needs using :require statements. 

The cache is by default cleared on item saved (in the context of live mode) and item publish events. These events can be disabled via the related \<event> nodes found in LiquidSC.Foundation.RedirectManager.config

By default this cache is enabled for the "master" and "web" database and has a configurable expiration timer on per redirect type basis.

This expiration timer can be adjusted via the \<CacheExpiration> node corresponding to each redirect type's process. 

If you wish to disable the cache (not recommend for production) entirely, you can do so by setting the related resolver's \<CacheExpiration> value to 0.

## Safeguards and Warnings

With great Redirect power comes great Redirect responsibility! There are a few built in safeguards to prevent unintentional problems:

* Item Redirects are naturally "global" by relation of how Sitecore items natively works, but nevertheless, they will not work unless placed within the site settings folder bound to the site they would normally resolve to, to prevent potential cross site "contamination".
* Only redirects of a path based source (Path Redirects and Redirect Maps) can be created on a global level (by virtue of what's stated above)
* Redirects will not be processed if in Preview or Experience Editor mode
* If a global redirect's Source path conflicts with a Site specific Source Path, the Global redirect's destination will be evaluated only
* There are a series of common known Sitecore associated relative paths that are ignored and will not process redirects if the incoming path matches them. These can be found in the LiquidSC.Foundation.RedirectManager.Pipelines.Base.RedirectManagerBase class's Process function.

\*Note that while nested redirects are supported, redirect loops are not prevented! Admins and Editors should take care to ensure that existing redirects are properly reviewed before introducing new ones and always test via live mode before publishing!

## Using the source code:

This project was built to be integrated into the Helix Base V9.3 solution scaffold (using "website" folders instead of "code" seen in 9.2 and below), with Unicorn for item serialization and source control tracking. You can add the project to your Helix Base solution by doing the following:

1. Install the package
2. Create a physical folder under src/Foundation to host the project (Ex: "RedirectManager")
3. Copy the project root to the newly created folder so that "website" and "serialization" sit directly under your created folder
4. Construct a same-named solution folder under "Foundation" within your solution
5. Right click on the newly created solution folder and add "Existing Project", selecting this project's csproj file
6. Build, then perform a Unicorn Sync of the Foundation.RedirectManager project to establish the connection between this project and the related yml files/Sitecore items it tracks

### Helix base compatibility

If on 9 through 9.2, it is trivial to either rename "website" to "code" prior to adding the project to solution, or incorporate both Helix base "website" and "code" folder projects by modifying your Directory.Build.props folder to check for both types on build:

`<ItemGroup>
    <ProjectReference Include="..\..\Foundation\*\website\*.csproj">
      <Name>%(Filename)</Name>
    </ProjectReference>
    <ProjectReference Include="..\..\Foundation\*\code\*.csproj">
      <Name>%(Filename)</Name>
    </ProjectReference>
    <ProjectReference Include="..\..\Feature\*\website\*.csproj">
      <Name>%(Filename)</Name>
    </ProjectReference>
    <ProjectReference Include="..\..\Feature\*\code\*.csproj">
      <Name>%(Filename)</Name>
    </ProjectReference>
    <ProjectReference Include="..\..\Project\*\website\*.csproj">
      <Name>%(Filename)</Name>
    </ProjectReference>
    <ProjectReference Include="..\..\Project\*\code\*.csproj">
      <Name>%(Filename)</Name>
    </ProjectReference>
  </ItemGroup>
`

If not on a Helix Base solution, install the package in order to get the necessary Sitecore items into your database, then integrate the source project as needed into your existing solution so that builds deploy the related DLLs and Configuration files to your Sitecore website's folder.

### External dependencies

The Custom Name Value List implementation is a locally created dependency, but the source code and related Sitecore items for the Custom Name Value List is not currently included here. These are included in the package only in the form of a dll and Sitecore items. A generic reference for the implementation can be found [here](https://trnktms.com/2016/10/15/sitecore-name-value-list-field-with-special-characters/) if needed.

### 9.2- vs 9.3+ compatibility strategy

We make use of preprocessor directives to resolve the deprecation of the "UrlOptions" and "MediaUrlOptions" classes in 9.3 and avoid having to maintain multiple branches. 

There are configurations dedicated to 9.3(+) and 9.2(-) each of which have a special symbol that gets generated on build to determine which direction the directives should take. The \<PackageReference> is wrapped in a conditional that determines which version of the Sitecore.Kernel package to use *on build* (due to the constraint that VS doesn't support conditions on the reference themselves). If you load the project (which defaults to 9.3+), switching your configuration will not cause intellisense errors to go away, you must build. 

*Take note that the base Debug and Release configurations are constructed to be future-forward, and generate the 9.3 symbol.* 

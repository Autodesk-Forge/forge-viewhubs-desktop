# data.management-csharp-desktop.sample

This sample demonstra a **Desktop** application that shows BIM 360 Team, BIM 360 Docs and Fusion Team hubs, which respective Projects, Folders, Items and Versions. Of each version it is possible to view it using Viewer.

## Demonstration

![](main_screen.png)

See [this quick video](https://twitter.com/augustomaia/status/905178317686439937).

## Architecture

When the user opens the desktop application, it redirects to Autodesk Account sign in page using the Forge Client ID stored on the Cloud module. After authenticate, the **access_token** and **refresh_token** are stored on an MongoDB database, creating a session ID. This ID is encrypted and sent to the Desktop client. 

On the next request, the Desktop client provides the Session ID, the Cloud module uses that to get the respective **access_token** and connects to Autodesk. Once the data is aquired, it is adjusted and sent to the Desktop client. 

The Desktop client will not have access to the **access_token**. 

![](architecture.png)

### Security concerns

This sample uses [CefSharp.WinForms](https://www.nuget.org/packages/CefSharp.WinForms), based on [Chromium Embedded Framework](https://bitbucket.org/chromiumembedded/cef), the open source version of [Google Chrome](https://www.google.com/chrome). It's is also available for [WPF](https://cefsharp.github.io/) applications. The build-in .NET WebView is based on Internet Explorer (version 11) and it's have several limitations. 

The Cloud module acts as a proxy between the Desktop client and Autodesk data. Only authorized information is returned, including the Viewer that implements a [Proxy](https://forge.autodesk.com/blog/securing-your-forge-viewer-token-behind-proxy-net).

## Setup

For using this sample, you need an Autodesk developer credentials. Visit the [Forge Developer Portal](https://developer.autodesk.com), sign up for an account, then [create an app](https://developer.autodesk.com/myapps/create). For this new app, use `https://localhost:3000/api/forge/callback/oauth` as Callback URL. Finally take note of the **Client ID** and **Client Secret**. 

### Cloud server

The cloud server module will store the credentials on a Mongo database. You can setup an development instance for free at [mLab](https://mlab.com) (or any other provider). Make sure to include a collection named: **users**

Open the `web.config` and edit the Forge Client ID, Secret and Callback URL. The **OAUTH_DATABASE** should point to your MongoDB instance, like: `mongodb://user:pwd@ds1234.mlab.com:56789/databasename`
 
```xml
  <appSettings>    <add key="FORGE_CLIENT_ID" value="" />    <add key="FORGE_CLIENT_SECRET" value="" />    <add key="FORGE_CALLBACK_URL" value="" />    <add key="OAUTH_DATABASE" value="" />  </appSettings>
```

### Desktop executable

Open the `app.config` file and specify the webserver address under **appSettings**:

```xml
  <appSettings>    <add key="serverAddress" value="http://localhost:3000" />  </appSettings>
```

For local testing, the desktop module should work "as is". 

## Deployment

For Appharbor deployment, following [this steps to configure your Forge Client ID & Secret](http://adndevblog.typepad.com/cloud_and_mobile/2017/01/deploying-forge-aspnet-samples-to-appharbor.html).

For AWS Elastic Beanstalk, setup keys from `web.config`. More details soon. 

### Tips & Tricks

The CEF Sharp library 

To keep keys locally, the sample defines a `web.keys.config` file that is added to **.gitignore**. On local development environment, this files stores the keys. When commit to Github, this file is not included. 

The AWS Elastic Beanstalk cannot override `web.config` variables on deployment, so this sample includes a **transform** on the `web.Release.config` to remove it. With that it should work.

```xml
  <appSettings>    <add key="FORGE_CLIENT_ID" xdt:Transform="Remove" xdt:Locator="Match(key)" />    <add key="FORGE_CLIENT_SECRET" xdt:Transform="Remove" xdt:Locator="Match(key)" />    <add key="FORGE_CALLBACK_URL" xdt:Transform="Remove" xdt:Locator="Match(key)" />    <add key="OAUTH_DATABASE" xdt:Transform="Remove" xdt:Locator="Match(key)" />  </appSettings>
```
## License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT). Please see the [LICENSE](LICENSE) file for full details.

## Author

Forge Partner Development Team

- Augusto Goncalves [@augustomaia](https://twitter.com/augustomaia)

See more at [Forge blog](https://forge.autodesk.com/blog).
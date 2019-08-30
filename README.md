# Monetizr Unity SDK

This SDK enables Monetizr Partners to use Monetizr in their Unity games easily, the SDK handles direct communication with our API and provides abstractions for easier usage.

You can visit https://docs.themonetizr.com/unity/index.html to find more useful information about Monetizr and integrations

## Requirements
This SDK requires at least Unity 2017.2. This version was chosen for its support for screen cutouts. 5.6 and earlier are not supported, as the SDK uses UnityWebRequest features introduced in 2017.1.

## Using the SDK

### Initial setup
Monetizr is implemented as a singleton prefab. You can find this prefab at **Monetizr/Prefabs/Monetizr Prefab**. You should place this in the first scene of your game, our code makes sure that there is only 1 prefab instantiated. Out-of-the-box it comes preconfigured with our test access token, which can be used to retrieve the "monetizr-sample-t-shirt" product.

#picture here

#### Base prefab configuration:

 * **Access Token** - this is your oAuth Access token, provided by Monetizr
 * **UI Prefab and Web View Prefab** - should be left as is, however power users are free to customize our UIs to fit their needs.

#### Additional settings:

 * **Logo** - add your own logo to product pages
 * **Portrait/Landscape background** - add a background image to product pages. These should be 9:16 and 16:9 images - they are cropped automatically should the aspect ratio not match.
 * **Portrait/Landscape video** - this will override the background image with a video instead, played back with a Unity VideoPlayer component.
 * **Show Fullscreen Alerts** - if something goes wrong, this will show an in-game error message. Disable to only output errors to the console.
 * **Never Use Web View** - on Android and iOS devices, our SDK provides an in-game web browser for checkout. If this is enabled, all platforms will use Unity's `Application.OpenURL(string url)` instead.

#picture here

### Showing the product
All Monetizr code is contained within the `Monetizr` namespace. All further examples are provided with the assumption that you are `using Monetizr;`. 

The SDK is written using the singleton pattern in order to simplify the workflow. At any time, you can access the current `MonetizrMonoBehaviour` with `MonetizrClient.Instance`. All functions relevant to game developers have been made as simple as possible.

You can show a product with a single line of code.
```csharp
   MonetizrClient.Instance.ShowProductForTag("monetizr-sample-t-shirt");
```

### Advanced features

#### Custom error handling

If you wish to show alerts to your players, but not use our provided alert screen, you can subscribe to the `MonetizrMonoBehaviour.MonetizrErrorOccurred` delegate.

```csharp
using Monetizr;

//Example subscription to delegate
public void HandleError(string error_msg) {
    //Do something with error_msg
    Debug.LogWarning(error_msg);
}

//At some point in your script you need to subscribe to the delegate
private void Start() {
    MonetizrClient.Instance.MonetizrErrorOccurred += HandleError;
}

//And you need to unsubscribe if this object is being destroyed, otherwise error-ception will occur
//OnDestroy is automatically called by Unity, similar to Start or Update
private void OnDestroy() {
    MonetizrClient.Instance.MonetizrErrorOccurred -= HandleError;
}
```
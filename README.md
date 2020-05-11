## What is Monetizr?
Monetizr is a turn-key platform for game developers enabling to sell or give-away game gear right inside the game's UI. You can use this SDK in your game to let players purchase products or claim gifts within the game.  All orders made with Monetizr automatically initiates fulfillment and shipping. More info: https://docs.themonetizr.com/docs/get-started.
 
## Monetizr Unity SDK
Monetizr Unity SDK is a plugin with the built-in functionality of:
- showing image carousel and fullscreen pictures in offers to end-users;
- HTML texts for descriptions;
- allowing end-users to select product variant options;
- displaying price in real or in-game currency (including discounts);
- checkout and payment support.

Monetizr uses oAuth 2 authentication behind the scenes for all payment and order related processes. The SDK takes care of managing oAuth 2. To use SDK and connect to Monetizr servers, all you need is a single API key. It can be retrieved via Monetizr web [Console][1]. API is a public two-way, it does not expose any useful information, but you should be aware of this.

Read the Monetizr's [Unity documentation][2] to find out more.

## Installation

**Requirements**. This plugin requires at least Unity 2017.2 to support screen cutouts. 5.6 and earlier are not supported, as the plugin uses UnityWebRequest features introduced in 2017.1.

Import the provided .unitypackage in your project, or paste the Monetizr folder from the Assets folder in your project.

## Using the library in your app

All Monetizr code is contained within the `Monetizr` namespace. Examples below assume that you are `using Monetizr;`. At any time, you can access the current `MonetizrMonoBehaviour` with `MonetizrClient.Instance`.

Monetizr is implemented as a singleton prefab at **Monetizr/Prefabs/Monetizr Prefab**. To instantiate it, place it in the first scene of your game. Prefab is configured with the public test [API key][3]: `4D2E54389EB489966658DDD83E2D1`.

To show a product in an [Offer View][4], you need to call a specific product_tag. Product tags represent a specific product, and they are managed in the web Console. For testing purposes, you can use public test product `T-shirt`.

Show an Offer View:

```csharp
   MonetizrClient.Instance.ShowProductForTag("T-shirt");
```

## Optional settings

By default UI Prefab and Web View Prefab are set and should be left as is.   However, users are free to customize Monetizr default UI to fit their game designs. Learn more about [customization][5].

[1]: https://app.themonetizr.com/
[2]: https://docs.themonetizr.com/docs/unity
[3]: https://docs.themonetizr.com/docs/creating-account#section-your-unique-access-token
[4]: https://docs.themonetizr.com/docs/offer-view
[5]: https://docs.themonetizr.com/docs/customization

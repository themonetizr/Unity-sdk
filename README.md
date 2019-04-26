# Monetizr UnitySDK!

This Manual describes how to import Monetizr UnitySDK into your game.

Prerequisite for this SDK to work:
Your Unity Version should support the new prefab system, introduced in Unity 2018.3


# What this SDK does

This SDK Enables Monetizr Partners to use Monetizr API in a Unity style without the need to implement direct communication with Monetizr API.

## Initializing the SDK

### 1. Prefabs Initialization
From the whole SDK you're interested in only one Prefab, called (can you imagine it) **"MonetizrPrefab"**
This Prefab exposes 3 properties:

1) Access Token - this is your oAuth Access token, provided from Monetizr
2) Root Canvas - As the Monetizr uses Unity UI, the product panel should be placed inside the UI Canvas. 99% of mobile games have the UI so it's pretty common that your game will also have one. This should be the Root Canvas inside your scene.
You can view at the example in the DemoScene and on the screenshot below:

![Screenshot1](http://i63.tinypic.com/oh0iva.png)

3) Product Prefab - A product prefab is an another prefab created by us for you in order to show the product to the end user.
If this field is empty for you, the prefab that should be attached there, is located at the following path:
**Monetizr -> Prefabs -> MonetizrProductPrefab**

### 2. Showing the product
The SDK is written using the singleton pattern in order to simplify the workflow.
There are **two ways to show the product** to the user:

1) Access the function MonetizrMonoBehaviour.ShowProductForTag(string) from the unity editor on ButtonClick or any other relevant event:

![enter image description here](http://i67.tinypic.com/x0wksl.png)

And as a parameter for this function use your **product tag** (e.g. *monetizr-sample-t-shirt*)

2) You can also call the function from code like this:
```csharp
   MonetizrClient.Instance.ShowProductForTag("monetizr-sample-t-shirt");
```
    

# That's it!

This is all you need to start using MonetizrSDK in your Unity project. 

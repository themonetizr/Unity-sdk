# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.1] - 2020-07-23

### Added

 * Basic test for order callback in testing scene, enable testing alerts to have it visible on real devices

### Changed

 * Support for Monetizr iOS SDK 3.5.5

### Fixed
 
 * Objective-C preprocessor directives not being set correctly by postprocessor script

## [2.0.0] - 2020-06-17

### Added

 * Support for iOS native Monetizr SDK
 * Much optimized and needed method of setting up Monetizr in projects - settings will now persist between updates and there is no need to worry about adding a prefab anymore!
 * Scaling option for big screen layout
 * Access token override
 * Delegate method which is called upon successful purchase (supported for Big Screen, Android with UGUI and iOS with native) - subscribe to `MonetizrClient.Instance.MonetizrOrderConfirmed`

### Changed
 
 * Deprecation of UGUI views for mobile
 * 3rd party WebView dependency is now Android only
 * The settings are much nicer to look at now!
 * Included testing and demo scene is now much cleaner and easier to understand

### Fixed

 * Loading spinner quietly taking a considerable chunk of CPU time
 * In-game testing alerts will now work when native plugins are enabled
 * Unhandled exception when trying to load an image from a blank URL

### Known issues

 * Unity 2017.2 will crash upon creating the settings file. Does not happen on later Unity versions.

## [1.4.0] - 2020-04-09

### Added

 * Support for _offer/product list_ API, which returns a list of all available offers
 * Grid and vertical offer list UI widgets to quickly add an offer list to your app
 * **EXPERIMENTAL** ability to use Monetizr Android SDK to display offer view instead of UGUI. In the future this will supersede the current method
 * A streamlined and complete checkout experienece for Big Screen
 * Support for claim orders for Big Screen (for mobile use the platform native SDKs)
 * Ability to show _locked_ products (lock is set locally)
 
### Fixed

 * Duplicate images displaying in situations where different SKUs are viewed
 * An issue with safe area not applying introduced in 1.3.0
 * Keyboard navigation for variant selection does not work properly when product has less than 3 variant dropdowns

### Known issues

 * The country selection dropdown in Big Screen acts odd when using keyboard input only

## [1.3.0] - 2020-01-22

### Added

 * **Replaced** scale setting with Big Screen view, which is a special view designed for use on PCs, taking into account potential for navigation with mouse, keyboard and controller.
 * In-depth theming options for Big Screen view to give developers options to make Monetizr product view look native to their game
 * Ability to implement custom checkout procedure using `MonetizrPaymentStarted`. Currently only available for big screen mode. See API documentation for implementation details.
 * Ability to use device language (from Unity's `Application.systemLanguage` API) to retrieve localized product information.

### Changed

 * Various behind-the-scene changes for how the product view is handled
 * Price formatting now correctly positions Alpha3 currency codes (was `USD4.95`, now `4.95 USD`)
 * Optimized API call timeout to accommodate infrastructure changes

### Fixed

 * Various timescale dependencies
 * iOS export bug related to WebView when using Unity 2019.3

### Known issues

 * The country selection dropdown acts odd when using keyboard input only
 * Keyboard navigation for variant selection does not work properly when product has less than 3 variant dropdowns

## [1.2.2] - 2019-10-26

### Fixed

 * Images didn't display inline in projects using Unity 2017.3 to 2018.2

### Changed

 * If a product only has single variant, then the variant selection won't be displayed.

## [1.2.1] - 2019-10-26

### Fixed

 * Removed unnecessary `using` from UIUtility.cs which broke compatibility with newer Unity versions

## [1.2.0] - 2019-10-24

### Added

 * Ability to scale UI to an overlaid window for platforms where a large fullscreen UI is not optimal, such as desktop  

### Changed

 * Finer controls have been added to the testing scene
 * Updated the light and dark themes to match other platforms
 * Dark theme from 1.1.0 is now called black theme
 * Theme colors are now applied to loading screen
 * Images now crop to fit in inline image viewers (main horizontal and verical view)

### Fixed  

 * Transparency issues in product view
 * Crashes on WebGL for certain Unity versions
 * Various cases where requests would not fail on connection loss (infinite loading, infinite 'Please wait...', etc.)

### Known issues

 * Checkout button does not work with touch input on WebGL builds

## [1.1.0] - 2019-10-11

### Added

 * Color scheme customization with dark and light presets (presets available via Unity component context menu)
 * Properties to read Monetizr UI state to help implement UI blocking (`MonetizrClient.Instance.AnyUiIsOpened` and `MonetizrClient.Instance.BackButtonHasAction`)
 * Discounted products display the original price as well
 * Breadcrumbs to variant selector
 * Prices to variant selector
 * Loading spinner while new images in main product view are loaded after variant change

### Changed

 * Reworked Monetizr UI in accordance with new guidelines which improve accessibility and UX
 * Removed swipe down gesture from image and variant selector views
 * Variant selector no longer permits selecting unavailable variants
 * Image viewer images now fit in the entire screen
 * Loading screen is now blander
 * Background image and logo customization is removed
 * .gif images are now ignored, as they do not display properly

### Fixed

 * All prices divisible by 0.10 will now display properly
 * Screen safe area related issues

## [1.0.0] - 2019-09-19

 * This is the initial release of the refreshed Monetizr Unity SDK. All further changes will be documented in this changelog.
 * The SDK is finally available as a unitypackage.

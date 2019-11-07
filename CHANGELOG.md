# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.2] - 2019-10-26

### Fixed

 * Images didn't display inline in projects using Unity 2017.3 to 2018.2

### Changed

 * 

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
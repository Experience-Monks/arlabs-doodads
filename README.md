# Jam3 Arlabs - AR Doodads Demo

[<img alt="Get This demo on Google Play" height="50px" src="https://play.google.com/intl/en_us/badges/images/apps/en-play-badge-border.png" />](https://play.google.com/store/apps/details?id=com.jam3.arlabs)

Some of these features have been used in this [Depth API overview](https://www.youtube.com/watch?v=VOVhCTb-1io) video.

[**ARCore Depth API**](https://developers.google.com/ar/develop/unity/depth/overview)
is enabled on a subset of ARCore-certified Android devices. **iOS devices (iPhone, iPad) are not supported**. Find the list of devices with Depth API support (marked with **Supports Depth API**) here:
[https://developers.google.com/ar/discover/supported-devices](https://developers.google.com/ar/discover/supported-devices).

See the [ARCore developer documentation](https://developers.google.com/ar) for
more information.

Download the this demo app on
[Google Play Store](https://play.google.com/store/apps/details?id=com.jam3.arlabs)
today.

# Table of Contents

-   [Setup](#setup)
-   [Playing](#playing)
-   [Building](#building)
-   [Helper Classes](#helper-classes)
-   [References](#references)
-   [Contributing](#contributing)
-   [Authors](#authors)
-   [License](#license)

## Setup

### Requirements

This sample demos requires:

-   [**Unity 2020.2.7f1**](https://unity3d.com)
-   [**AR Foundation v4.1.5**](https://developers.google.com/ar/develop/unity-arf)
-   [**AR Core Extensions v1.23.0**](https://github.com/google-ar/arcore-unity-extensions)

If you have trouble playing it, close and reopen the project and reimport all demo shaders to resolve any dependency issues in the Unity editor. This project only builds with the Build Platform **Android**. **Instant Preview** is not enabled for Depth API yet. To test with Depth support, build the project to an Android device instead of using the **Play** button in the Unity editor.

## Playing

You can play and test the app in editor but the Scan/ARCamera won't be disabled and will not work, use the **WASD** keys to move and **Shit + Mouse** to rotate the camera as a FPS game. the aplication will work as AR but with no background texture and no Depth features.

## Building

Individual scenes can be built and run by just enabling a particular scene, e.g. `Main`. Please make sure to set the **Build Platform** to **Android** and verify that the main `DemoCarousel` scene is the first enabled scene in the **Scenes In Build** list under **Build Settings**. Enable all scenes that are part of the demo user interface.

The project is set up to use the `IL2CPP` scripting backend instead of `Mono` to build an `ARM64` app. You may be prompted to locate the Android NDK folder. You can download the `NDK` by navigating to `Unity > Preferences > External Tools > NDK` and clicking the `Download` button.

## Helper Classes

The AR code retated are inside the folder `Assets/Scripts/AR` the structure is divided in:

-   Common (Common files used for all AR features)
-   Controllers (AR controllers)
-   Managers (AR managers)
-   Occlusion (Occlusiuon reletated AR scripts)
-   RawDepth (RawDepth reletated AR scripts)

### `ARCameraInfo`

A static class that contains references to all AR camera infromation,
texture of the depth map, camera intrinsics, and many other depth look up and
coordinate transformation utilities.

### `ARCameraEffect`

A class that enables the scan efect, it's attached tot he AR camera and will overlap the commnad buffer to add the background and also add the Depth AR Effect.

### `ARDepthMeshPreview`

This class is used to preview the mesh creation in realtime, it uses a custom shader to build the mesh fromt he raw depth.

### `ARMeshGenerationController`

This class is used to build the mesh from the raw depth data.

## References

[Google DepthLab](https://github.com/googlesamples/arcore-depth-lab)

## Contributing

Please read [CONTRIBUTING.md](https://github.com/Jam3/arlabs-doodads/blob/master/CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests.

## Authors

-   Fabio Toste - Creative Developer - @tostegroo - fabio.toste@jam3.com
-   Higor Bimonti - Unity Developer - @hgbimonti - higor@finemug.com
-   Nicolás Ezcurra - Unity Developer - @Nicolas-Ezcurra-Mediamonks - nicolas.ezcurra@mediamonks.com

## License

[Apache 2.0](https://github.com/Jam3/arlabs-doodads/blob/master/LICENSE).

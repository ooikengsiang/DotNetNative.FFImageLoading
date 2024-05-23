# DotNetNative.FFImageLoading - Fast & Furious Image Loading 

FFImageLoading Library for .NET 7 / .NET 8 native iOS / Android / Mac.

*This is a quick / lazy port from Xamarin.FFImageLoading to support .NET 7 and .NET 8 since Xamarin is going away. If you are looking for MAUI support, this is not the one you are looking for.*

## NuGet
FFImageLoading: https://www.nuget.org/packages/DotNetNative.FFImageLoading

FFImageLoading.Transformations: https://www.nuget.org/packages/DotNetNative.FFImageLoading.Transformations

FFImageLoading.Svg: https://www.nuget.org/packages/DotNetNative.FFImageLoading.Svg

## Features
- Support .NET 7 / .NET 8 native iOS / Android / Mac
- Configurable disk and memory caching
- Multiple image views using the same image source (url, path, resource) will use only one bitmap which is cached in memory (less memory usage)
- Deduplication of similar download/load requests. *(If 100 similar requests arrive at same time then one real loading will be performed while 99 others will wait).*
- Error and loading placeholders support
- Images can be automatically downsampled to specified size (less memory usage)
- Fluent API which is inspired by Picasso naming
- SVG / GIF support
- Image loading Fade-In animations support
- Can retry image downloads (RetryCount, RetryDelay)
- Android bitmap optimization. Saves 50% of memory by trying not to use transparency channel when possible.
- Transformations support
  - BlurredTransformation
  - CircleTransformation, RoundedTransformation, CornersTransformation, CropTransformation
  - ColorSpaceTransformation, GrayscaleTransformation, SepiaTransformation, TintTransformation
  - FlipTransformation, RotateTransformation
  - Supports custom transformations (native platform `ITransformation` implementations)

## Breaking Changes from Xamarin.FFImageLoading
- WebP is not supported.
- Xamarin.Form is not supported.
- Tizan and Windows platform is not supported.

https://github.com/luberda-molinet/FFImageLoading/wiki




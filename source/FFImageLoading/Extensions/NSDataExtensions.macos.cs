#if __MACOS__
using System;
using Foundation;
using CoreGraphics;
using ImageIO;
using FFImageLoading.Helpers;
using FFImageLoading.Work;
using FFImageLoading.Config;
using System.Threading;
using FFImageLoading.Decoders;
using System.Linq;
using System.Threading.Tasks;
using AppKit;
using PImage = AppKit.NSImage;

namespace FFImageLoading.Extensions
{
    public static class NSDataExtensions
    {
        public static async Task<PImage> ToImageAsync(this NSData data, CGSize destSize, nfloat destScale, Configuration config, TaskParameter parameters, GifDecoder.RCTResizeMode resizeMode = GifDecoder.RCTResizeMode.ScaleAspectFit, ImageInformation imageinformation = null, bool allowUpscale = false)
        {
            var decoded = await GifDecoder.SourceRegfToDecodedImageAsync(
				data, destSize, destScale, config, parameters, resizeMode, imageinformation, allowUpscale).ConfigureAwait(false);

            PImage result;

            if (decoded.IsAnimated)
            {
                result = new PImage();
                var repr = decoded.AnimatedImages
                                  .Select(v => v.Image.Representations().First())
                                  .ToArray();
                result.AddRepresentations(repr);
            }
            else
            {
                result = decoded.Image;
            }

            return result;
        }
    }
}
#endif
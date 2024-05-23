#if __IOS__
using System;
using CoreGraphics;
using FFImageLoading.Work;
using UIKit;
using PImage = UIKit.UIImage;

namespace FFImageLoading.Extensions
{
    public static class PImageExtensions
    {
        public static nuint GetMemorySize(this PImage image)
        {
            return (nuint)(image.CGImage.BytesPerRow * image.CGImage.Height);
        }

        public static PImage ResizeUIImage(this PImage image, double desiredWidth, double desiredHeight, InterpolationMode interpolationMode)
        {
            var widthRatio = desiredWidth / image.Size.Width;
            var heightRatio = desiredHeight / image.Size.Height;
            var scaleRatio = Math.Min(widthRatio, heightRatio);

            if (Math.Abs(desiredWidth) < double.Epsilon )
                scaleRatio = heightRatio;

            if (Math.Abs(desiredHeight) < double.Epsilon)
                scaleRatio = widthRatio;

            var aspectWidth = image.Size.Width * scaleRatio;
            var aspectHeight = image.Size.Height * scaleRatio;

            var newSize = new CGSize(aspectWidth, aspectHeight);

            UIGraphics.BeginImageContextWithOptions(newSize, false, 0);

            try
            {
                image.Draw(new CGRect((nfloat)0.0, (nfloat)0.0, newSize.Width, newSize.Height));

                using (var context = UIGraphics.GetCurrentContext())
                {
                    if (interpolationMode == InterpolationMode.None)
                        context.InterpolationQuality = CGInterpolationQuality.None;
                    else if (interpolationMode == InterpolationMode.Low)
                        context.InterpolationQuality = CGInterpolationQuality.Low;
                    else if (interpolationMode == InterpolationMode.Medium)
                        context.InterpolationQuality = CGInterpolationQuality.Medium;
                    else if (interpolationMode == InterpolationMode.High)
                        context.InterpolationQuality = CGInterpolationQuality.High;
                    else
                        context.InterpolationQuality = CGInterpolationQuality.Low;

                    var resizedImage = UIGraphics.GetImageFromCurrentImageContext();

                    return resizedImage;
                }
            }
            finally
            {
                UIGraphics.EndImageContext();
                image.TryDispose();
            }
        }

        public static System.IO.Stream AsPngStream(this PImage image)
        {
            return image.AsPNG()?.AsStream();
        }

        public static System.IO.Stream AsJpegStream(this PImage image, int quality = 80)
        {
            return image.AsJPEG(quality / 100f).AsStream();
        }
    }
}
#endif
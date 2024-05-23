#if __MACOS__
using System;
using CoreGraphics;
using FFImageLoading.Work;
using AppKit;
using PImage = AppKit.NSImage;

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
            var resizedImage = new PImage(newSize);
            resizedImage.LockFocus();
            image.Draw(new CGRect(CGPoint.Empty, newSize), CGRect.Empty, NSCompositingOperation.SourceOver, 1.0f);
            resizedImage.UnlockFocus();
            return resizedImage;
        }

        public static System.IO.Stream AsPngStream(this PImage image)
        {
            var imageRep = new NSBitmapImageRep(image.AsTiff());
            return imageRep.RepresentationUsingTypeProperties(NSBitmapImageFileType.Png)
                                                             .AsStream();
        }

        public static System.IO.Stream AsJpegStream(this PImage image, int quality = 80)
        {
            // todo: jpeg quality?
            var imageRep = new NSBitmapImageRep(image.AsTiff());
            return imageRep.RepresentationUsingTypeProperties(NSBitmapImageFileType.Jpeg)
                           .AsStream();
        }
    }
}
#endif
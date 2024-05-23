#if __IOS__
using System;
using FFImageLoading.Work;
using UIKit;
using PImage = UIKit.UIImage;

namespace FFImageLoading.Transformations
{
    public abstract class TransformationBase : ITransformation
    {
        public abstract string Key { get; }

        public IBitmap Transform(IBitmap bitmapHolder, string path, ImageSource source, bool isPlaceholder, string key)
        {
            var sourceBitmap = bitmapHolder.ToNative();
            return new BitmapHolder(Transform(sourceBitmap, path, source, isPlaceholder, key));
        }

        protected virtual PImage Transform(PImage sourceBitmap, string path, ImageSource source, bool isPlaceholder, string key)
        {
            return sourceBitmap;
        }
    }
}
#endif
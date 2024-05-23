#if __IOS__
using System;
using FFImageLoading.Helpers;
using System.Threading.Tasks;
using Foundation;
using System.Linq;
using System.IO;
using FFImageLoading.Extensions;
using System.Threading;
using FFImageLoading.Config;
using FFImageLoading.Cache;
using ImageIO;
using System.Collections.Generic;
using FFImageLoading.Decoders;
using CoreGraphics;
using UIKit;
using PImage = UIKit.UIImage;

namespace FFImageLoading.Work
{
    public class PlatformImageLoaderTask<TImageView> : ImageLoaderTask<PImage, PImage, TImageView> where TImageView : class
    {
        public PlatformImageLoaderTask(ITarget<PImage, TImageView> target, TaskParameter parameters, IImageService imageService) : base(ImageCache.Instance, target, parameters, imageService)
        {
        }

        public async override Task Init()
        {
            await ScaleHelper.InitAsync().ConfigureAwait(false);
            await base.Init().ConfigureAwait(false);
        }

        protected override async Task SetTargetAsync(PImage image, bool animated)
        {
			if (Target == null)
				return;

            await MainThreadDispatcher.PostAsync(() =>
            {
                ThrowIfCancellationRequested();
                PlatformTarget.Set(this, image, animated);
            }).ConfigureAwait(false);
        }

        protected override int DpiToPixels(int size)
        {
            return size.DpToPixels();
        }

        protected override IDecoder<PImage> ResolveDecoder(ImageInformation.ImageType type)
        {
            switch (type)
            {
                case ImageInformation.ImageType.GIF:
                    return new GifDecoder();

                default:
                    return new BaseDecoder();
            }
        }

        protected override async Task<PImage> TransformAsync(PImage bitmap, IList<ITransformation> transformations, string path, ImageSource source, bool isPlaceholder)
        {
            await StaticLocks.DecodingLock.WaitAsync(CancellationTokenSource.Token).ConfigureAwait(false); // Applying transformations is both CPU and memory intensive
            ThrowIfCancellationRequested();

            try
            {
                foreach (var transformation in transformations)
                {
                    ThrowIfCancellationRequested();

                    var old = bitmap;

                    try
                    {
                        var bitmapHolder = transformation.Transform(new BitmapHolder(bitmap), path, source, isPlaceholder, Key);
                        bitmap = bitmapHolder.ToNative();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(string.Format("Transformation failed: {0}", transformation.Key), ex);
                        throw;
                    }
                    finally
                    {
                        // Transformation succeeded, so garbage the source
                        if (old != null && old.Handle != IntPtr.Zero && old != bitmap && old.Handle != bitmap.Handle)
                        {
                            old.TryDispose();
                        }
                    }
                }
            }
            finally
            {
				StaticLocks.DecodingLock.Release();
            }

            return bitmap;
        }

        protected override Task<PImage> GenerateImageFromDecoderContainerAsync(IDecodedImage<PImage> decoded, ImageInformation imageInformation, bool isPlaceholder)
        {
            PImage result;

            if (decoded.IsAnimated)
            {
                result = PImage.CreateAnimatedImage(decoded.AnimatedImages
                    .Select(v => v.Image)
                    .Where(v => v?.CGImage != null).ToArray(), decoded.AnimatedImages.Sum(v => v.Delay) / 1000f);              
            }
            else
            {
                result = decoded.Image;
            }

            return Task.FromResult(result);
        }
    }
}
#endif
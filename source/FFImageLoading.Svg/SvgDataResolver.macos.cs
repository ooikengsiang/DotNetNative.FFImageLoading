#if __MACOS__
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FFImageLoading.Work;
using FFImageLoading.Config;
using SkiaSharp;
using FFImageLoading.DataResolvers;
using FFImageLoading.Extensions;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using FFImageLoading.Helpers;
using Foundation;
using AppKit;
using CoreGraphics;

namespace FFImageLoading.Svg.Platform
{
    /// <summary>
    /// Svg data resolver.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class SvgDataResolver : IVectorDataResolver
    {
#pragma warning disable RECS0108 // Warns about static fields in generic types
		private static readonly object _encodingLock = new object();
#pragma warning restore RECS0108 // Warns about static fields in generic types

		private static readonly CGColorSpace _colorSpace = CGColorSpace.CreateDeviceRGB();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:FFImageLoading.Svg.Platform.SvgDataResolver"/> class.
		/// Default SVG size is read from SVG file width / height attributes
		/// You can override it by specyfing vectorWidth / vectorHeight params
		/// </summary>
		/// <param name="vectorWidth">Vector width.</param>
		/// <param name="vectorHeight">Vector height.</param>
		/// <param name="useDipUnits">If set to <c>true</c> use dip units.</param>
		/// <param name="replaceStringMap">Replace string map.</param>
		public SvgDataResolver(int vectorWidth = 0, int vectorHeight = 0, bool useDipUnits = true, Dictionary<string, string> replaceStringMap = null)
        {
            VectorWidth = vectorWidth;
            VectorHeight = vectorHeight;
            UseDipUnits = useDipUnits;
            ReplaceStringMap = replaceStringMap;
        }

        public Configuration Configuration => ImageService.Instance.Config;

        public bool UseDipUnits { get; private set; }

        public int VectorHeight { get; private set; }

        public int VectorWidth { get; private set; }

        public Dictionary<string, string> ReplaceStringMap { get; set; }

		private async Task<DataResolverResult> Decode(SKPicture picture, SKBitmap bitmap, DataResolverResult resolvedData)
		{
			await StaticLocks.DecodingLock.WaitAsync().ConfigureAwait(false);

			try
			{
                var info = bitmap.Info;
				using (var provider = new CGDataProvider(bitmap.GetPixels(out var size), size.ToInt32()))
				using (var cgImage = new CGImage(info.Width, info.Height, 8, info.BitsPerPixel, info.RowBytes,
						_colorSpace, CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big,
						provider, null, false, CGColorRenderingIntent.Default))
				{
					IDecodedImage<object> container = new DecodedImage<object>()
					{
						Image = new NSImage(cgImage, CGSize.Empty),
					};
					return new DataResolverResult(container, resolvedData.LoadingResult, resolvedData.ImageInformation);
				}
			}
			finally
			{
				StaticLocks.DecodingLock.Release();
			}
		}

		public async Task<DataResolverResult> Resolve(string identifier, TaskParameter parameters, CancellationToken token)
        {
            var source = parameters.Source;

            if (!string.IsNullOrWhiteSpace(parameters.LoadingPlaceholderPath) && parameters.LoadingPlaceholderPath == identifier)
                source = parameters.LoadingPlaceholderSource;
            else if (!string.IsNullOrWhiteSpace(parameters.ErrorPlaceholderPath) && parameters.ErrorPlaceholderPath == identifier)
                source = parameters.ErrorPlaceholderSource;

            var resolvedData = await (Configuration.DataResolverFactory ?? new DataResolverFactory())
                                            .GetResolver(identifier, source, parameters, Configuration)
                                            .Resolve(identifier, parameters, token).ConfigureAwait(false);

            if (resolvedData?.Stream == null)
                throw new FileNotFoundException(identifier);

            var svg = new SKSvg()
            {
                ThrowOnUnsupportedElement = false,
            };
            SKPicture picture;

            if (ReplaceStringMap == null || ReplaceStringMap.Count == 0)
            {
                using (var svgStream = resolvedData.Stream)
                {
                    picture = svg.Load(svgStream, token);
                }
            }
            else
            {
                using (var svgStream = resolvedData.Stream)
                using (var reader = new StreamReader(svgStream))
                {
                    var inputString = await reader.ReadToEndAsync().ConfigureAwait(false);

                    foreach (var map in ReplaceStringMap
                             .Where(v => v.Key.StartsWith("regex:", StringComparison.OrdinalIgnoreCase)))
                    {
                        inputString = Regex.Replace(inputString, map.Key.Substring(6), map.Value);
                    }

                    var builder = new StringBuilder(inputString);

                    foreach (var map in ReplaceStringMap
                             .Where(v => !v.Key.StartsWith("regex:", StringComparison.OrdinalIgnoreCase)))
                    {
                        builder.Replace(map.Key, map.Value);
                    }

					token.ThrowIfCancellationRequested();

					using (var svgFinalStream = new MemoryStream(Encoding.UTF8.GetBytes(builder.ToString())))
                    {
                        picture = svg.Load(svgFinalStream);
                    }
                }
            }

			token.ThrowIfCancellationRequested();

            double sizeX = VectorWidth;
            double sizeY = VectorHeight;

			if (UseDipUnits)
			{
				sizeX = VectorWidth.DpToPixels();
				sizeY = VectorHeight.DpToPixels();
			}

			if (sizeX <= 0 && sizeY <= 0)
            {
                if (picture.CullRect.Width > 0)
                    sizeX = picture.CullRect.Width;
                else
                    sizeX = 400;

                if (picture.CullRect.Height > 0)
                    sizeY = picture.CullRect.Height;
                else
                    sizeY = 400;
            }
            else if (sizeX > 0 && sizeY <= 0)
            {
                sizeY = (int)(sizeX / picture.CullRect.Width * picture.CullRect.Height);
            }
            else if (sizeX <= 0 && sizeY > 0)
			{
                sizeX = (int)(sizeY / picture.CullRect.Height * picture.CullRect.Width);
            }

            resolvedData.ImageInformation.SetType(ImageInformation.ImageType.SVG);

            using (var bitmap = new SKBitmap(new SKImageInfo((int)sizeX, (int)sizeY)))
            using (var canvas = new SKCanvas(bitmap))
            using (var paint = new SKPaint())
            {
                canvas.Clear(SKColors.Transparent);
                var scaleX = (float)sizeX / picture.CullRect.Width;
                var scaleY = (float)sizeY / picture.CullRect.Height;
                var matrix = SKMatrix.CreateScale(scaleX, scaleY);
                canvas.DrawPicture(picture, ref matrix, paint);
                canvas.Flush();

				token.ThrowIfCancellationRequested();

				return await Decode(picture, bitmap, resolvedData).ConfigureAwait(false);
			}
        }
    }
}
#endif
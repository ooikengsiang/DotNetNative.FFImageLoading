﻿#if __IOS__
using System;
using CoreFoundation;
using FFImageLoading.Helpers;

namespace FFImageLoading.Extensions
{
    public static class UnitsExtensions
    {
        public static int DpToPixels(this int dp) => ImageService.Instance.DpToPixels(dp);

        public static int DpToPixels(this double dp) => ImageService.Instance.DpToPixels(dp);

        public static double PixelsToDp(this int px) => ImageService.Instance.PixelsToDp(px);

        public static double PixelsToDp(this double px) => ImageService.Instance.PixelsToDp(px);
    }
}
#endif
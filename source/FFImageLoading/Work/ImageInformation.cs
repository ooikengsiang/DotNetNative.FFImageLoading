﻿using System;
using System.Collections.Generic;
using FFImageLoading.Helpers.Exif;

namespace FFImageLoading.Work
{
    public class ImageInformation
    {
        public ImageInformation()
        {
        }

        public void SetCurrentSize(int width, int height)
        {
            CurrentWidth = width;
            CurrentHeight = height;
        }

        public void SetOriginalSize(int width, int height)
        {
            OriginalWidth = width;
            OriginalHeight = height;
        }

        public void SetPath(string path)
        {
            Path = path;
        }

        public void SetFilePath(string filePath)
        {
            FilePath = filePath;
        }

        public void SetKey(string cacheKey, string userCustomCacheKey)
        {
            CacheKey = cacheKey;
            UserCustomCacheKey = userCustomCacheKey;
        }

        public void SetType(ImageType type)
        {
            Type = type;
        }

        public void SetExif(IList<Helpers.Exif.Directory> exif)
        {
            Exif = exif;
        }

        public IList<Helpers.Exif.Directory> Exif { get; private set;  }

        public int CurrentWidth { get; private set; }

        public int CurrentHeight { get; private set; }

        public int OriginalWidth { get; private set; }

        public int OriginalHeight { get; private set; }

        public string Path { get; private set; }

        public string FilePath { get; private set; }

        public string CacheKey { get; private set; }

        public string UserCustomCacheKey { get; private set; }

        public ImageType Type { get; private set; }

        public string BaseKey
        {
            get
            {
                if (!string.IsNullOrEmpty(UserCustomCacheKey))
                    return UserCustomCacheKey;

                return Path ?? FilePath ?? CacheKey;
            }
        }

        public enum ImageType
        {
            Unknown,
            BMP,
            JPEG,
            GIF,
            TIFF,
            PNG,
            WEBP,
            SVG,
            ICO
        }
    }
}


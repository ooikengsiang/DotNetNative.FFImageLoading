#if __MACOS__
using System;
using System.Threading.Tasks;

namespace FFImageLoading.Helpers
{
    public static class ScaleHelper
    {
        static nfloat? _scale;
        public static nfloat Scale
        {
            get
            {
                if (!_scale.HasValue)
                {
                    InitAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                }

                return _scale.Value;
            }
        }

        public static async Task InitAsync()
        {
            if (_scale.HasValue)
                return;

            var dispatcher = ImageService.Instance.Config.MainThreadDispatcher;
            await dispatcher.PostAsync(() =>
            {
                _scale = AppKit.NSScreen.MainScreen.BackingScaleFactor;
            }).ConfigureAwait(false);
        }
    }
}
#endif
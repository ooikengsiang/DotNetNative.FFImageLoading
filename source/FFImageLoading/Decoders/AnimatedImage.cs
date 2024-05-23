using System;

namespace FFImageLoading
{
    public class AnimatedImage<TNativeImageContainer> : IAnimatedImage<TNativeImageContainer>
    {
        public TNativeImageContainer Image { get; set; }

        public int Delay { get; set; }
    }
}

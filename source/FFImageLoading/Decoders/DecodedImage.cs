using System;

namespace FFImageLoading
{
    public class DecodedImage<TNativeImageContainer> : IDecodedImage<TNativeImageContainer>
    {
        public bool IsAnimated { get; set; }

        public TNativeImageContainer Image { get; set; }

        public IAnimatedImage<TNativeImageContainer>[] AnimatedImages { get; set; }
    }
}

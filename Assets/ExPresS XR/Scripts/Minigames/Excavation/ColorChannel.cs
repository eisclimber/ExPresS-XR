using UnityEngine;

namespace ExPresSXR.Minigames.Excavation
{
    /// <summary>
    /// Represents a utility to work with individual RGBA color channels.
    /// All values work are normalize (i.e. in the range of 0.0f to 1.0f).
    /// </summary>
    public static class ColorChannel
    {
        /// <summary>
        /// Returns a color with only the activated color channel with the desired value and optional opacity.
        /// All other channels will be 0.0f with exception of the alpha channel which will be 1.0f to represent full opacity.
        /// </summary>
        /// <param name="channel">Channel to be manipulated.</param>
        /// <param name="channelValue">Value of the channel.</param>
        /// <param name="opacity">Opacity of the color. Will be ignored if the alpha channel is chosen.</param>
        /// <returns></returns>
        public static Color GetColorWithChannelValue(Channels channel, float channelValue, float opacity = 1.0f)
        {
            return new(
                channel == Channels.R ? channelValue : 0.0f,
                channel == Channels.G ? channelValue : 0.0f,
                channel == Channels.B ? channelValue : 0.0f,
                channel == Channels.A ? channelValue : opacity
            );
        }

        /// <summary>
        /// Retrieves the value of the color channel of the color.
        /// </summary>
        /// <param name="channel">Channel to be extracted.</param>
        /// <param name="color">Color to extract from.</param>
        /// <returns>Value of the color channel.</returns>
        public static float GetColorChannelValue(Channels channel, Color color)
        {
            return channel switch
            {
                Channels.R => color.r,
                Channels.G => color.g,
                Channels.B => color.b,
                _ => color.a
            };
        }

        /// <summary>
        /// Represents the for channels in a RGBA color.
        /// </summary>
        public enum Channels
        {
            R,
            G,
            B,
            A
        }
    }
}
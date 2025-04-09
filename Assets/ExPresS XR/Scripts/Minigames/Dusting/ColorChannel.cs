using UnityEngine;

namespace ExPResSXR.Minigames.ExcavationGame
{
    public static class ColorChannel
    {
        public static Color GetColorWithChannelValue(Channels channel, float channelValue, float opacity = 1.0f)
        {
            return new(
                channel == Channels.R ? channelValue : 0.0f,
                channel == Channels.G ? channelValue : 0.0f,
                channel == Channels.B ? channelValue : 0.0f,
                channel == Channels.A ? channelValue : opacity
            );
        }

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



        public enum Channels
        {
            R,
            G,
            B,
            A
        }
    }
}
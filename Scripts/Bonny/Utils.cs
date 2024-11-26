#region References
using System;
#endregion

namespace Bonny
{
    public static class Utils
    {
        public static int Clamp(this int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        public static int GetValueInRange(this int value, int leftMargin, int rightMargin)
        {
            return Clamp(value - leftMargin, 0, rightMargin - leftMargin);
        }
    }
}

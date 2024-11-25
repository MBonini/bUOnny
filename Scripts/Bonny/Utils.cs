#region References
using System;
#endregion

namespace Bonny
{
    public static class Utils
    {
        public static int GetValueInRange(this int value, int leftMargin, int rightMargin)
        {
            return Math.Max(Math.Min(value - leftMargin, rightMargin - leftMargin), 0);
        }
    }
}

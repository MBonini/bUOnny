#region References

using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

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

        public static bool PayWithGold(this PlayerMobile player, int goldAmount)
        {
            return (player.Backpack != null && player.Backpack.ConsumeTotal(typeof(Gold), goldAmount)) ||
                   Banker.Withdraw(player, goldAmount);
        }

        public static List<object> ParentHierarchy(this Item item)
        {
            return RecursiveParent(item);
        }

        private static List<object> RecursiveParent(object obj)
        {
            switch (obj)
            {
                case null:
                    return new List<object>();
                case Item item:
                {
                    List<object> result = RecursiveParent(item.Parent);
                    result.Add(obj);
                    return result;
                }
                default:
                    return new List<object> { obj };
            }
        }
    }
}

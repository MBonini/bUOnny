#region References
using Server.Targeting;
#endregion

namespace Server.Items
{
        public class DeedOfRepair : Item
    {
        const int repairAmount = 1024; //Amount of Hitpoints restored on object.

        [Constructable]
        public DeedOfRepair() : base(0x14F0)
        {
            Name = "Deed of repair";
            Hue = 0x44E;
            Weight = 0.1;
        }

        public DeedOfRepair(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.SendMessage("Select the item you want to repair.");
                from.Target = new RepairTarget(this);
            }
            else
            {
                from.SendMessage("This item must be in your backpack to use.");
            }
        }

        private class RepairTarget : Target
        {
            private readonly DeedOfRepair _deed;

            public RepairTarget(DeedOfRepair deed) : base(1, false, TargetFlags.None)
            {
                _deed = deed;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                //Check if item is of Type Armor
                if (targeted is BaseArmor && ((BaseArmor)targeted).CanRepair)
                {
                    BaseArmor item = (BaseArmor)targeted;

                    from.SendMessage("You use the Item Repair Deed to repair the object {0}.", item.Name);
                    item.HitPoints += repairAmount;

                    // Delete the deed after usage
                    _deed.Delete();
                }


                //Check if item is of Type Weapon
                else if (targeted is BaseWeapon && ((BaseWeapon)targeted).CanRepair)
                {
                    BaseWeapon item = (BaseWeapon)targeted;

                    from.SendMessage("You use the Item Repair Deed to repair the object {0}.", item.Name);
                    item.HitPoints += repairAmount;

                    // Delete the deed after usage
                    _deed.Delete();
                }

                //Check if item is of Type Jewel
                else if (targeted is BaseJewel && ((BaseJewel)targeted).CanRepair)
                {
                    BaseJewel item = (BaseJewel)targeted;

                    from.SendMessage("You use the Item Repair Deed to repair the object {0}.", item.Name);
                    item.HitPoints += repairAmount;

                    // Delete the deed after usage
                    _deed.Delete();
                }

                //Check if item is of Type Clothing
                else if (targeted is BaseClothing && ((BaseClothing)targeted).CanRepair)
                {
                    BaseClothing item = (BaseClothing)targeted;

                    from.SendMessage("You use the Item Repair Deed to repair the object {0}.", item.Name);
                    item.HitPoints += repairAmount;

                    // Delete the deed after usage
                    _deed.Delete();
                }

                //Check if item is of Type Talisman
                else if (targeted is BaseTalisman && ((BaseTalisman)targeted).CanRepair)
                {
                    BaseTalisman item = (BaseTalisman)targeted;

                    from.SendMessage("You use the Item Repair Deed to repair the object {0}.", item.Name);
                    item.HitPoints += repairAmount;

                    // Delete the deed after usage
                    _deed.Delete();
                }
                //if item not of defined type
                else { from.SendMessage("You can only repair specific items with this deed."); }
            }
        }
    }

}

#region References

using Server.Targeting;

#endregion

namespace Server.Items
{
    public sealed class DeedOfRepair : Item
    {
        private const int _RepairAmount = 1024; // Amount of Hitpoints restored on object.

        [Constructable]
        public DeedOfRepair() : base(0x14F0)
        {
            Name = "Deed of repair";
            Hue = 0x44E;
            Weight = 0.1;
            LootType = LootType.Blessed;
        }

        public DeedOfRepair(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0); // version
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
            private readonly DeedOfRepair _Deed;

            public RepairTarget(DeedOfRepair deed) : base(1, false, TargetFlags.None)
            {
                _Deed = deed;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                // Check if item is of Type Armor
                if (targeted is BaseArmor armor && armor.CanRepair)
                {
                    armor.HitPoints += _RepairAmount;
                    from.SendMessage(70, "You have repaired the armor {0}.", armor.Name);

                    // Delete the deed after usage
                    _Deed.Delete();
                }

                // Check if item is of Type Weapon
                else if (targeted is BaseWeapon weapon && weapon.CanRepair)
                {
                    weapon.HitPoints += _RepairAmount;
                    from.SendMessage(70, "You have repaired the weapon {0}.", weapon.Name);

                    // Delete the deed after usage
                    _Deed.Delete();
                }

                // Check if item is of Type Jewel
                else if (targeted is BaseJewel jewel && jewel.CanRepair)
                {
                    jewel.HitPoints += _RepairAmount;
                    from.SendMessage(70, "You have repaired the jewel {0}.", jewel.Name);

                    // Delete the deed after usage
                    _Deed.Delete();
                }

                // Check if item is of Type Clothing
                else if (targeted is BaseClothing clothing && clothing.CanRepair)
                {
                    clothing.HitPoints += _RepairAmount;
                    from.SendMessage(70, "You have repaired the clothing {0}.", clothing.Name);

                    // Delete the deed after usage
                    _Deed.Delete();
                }

                // Check if item is of Type Talisman
                else if (targeted is BaseTalisman talisman && talisman.CanRepair)
                {
                    talisman.HitPoints += _RepairAmount;
                    from.SendMessage(70, "You have repaired the talisman {0}.", talisman.Name);

                    // Delete the deed after usage
                    _Deed.Delete();
                }
                // if item not of defined type
                else
                {
                    from.SendMessage(38, "You can only repair items with durability.");
                }
            }
        }
    }
}

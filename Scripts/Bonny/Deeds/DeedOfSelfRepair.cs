#region References

using Server.Targeting;

#endregion

namespace Server.Items
{
    public sealed class DeedOfSelfRepair : Item
    {
        private const int _SelfRepairAmount = 5;

        [Constructable]
        public DeedOfSelfRepair() : base(0x14F0)
        {
            Name = "Deed of self repair";
            Hue = 0x0455;
            Weight = 0.1;
            LootType = LootType.Blessed;
        }

        public DeedOfSelfRepair(Serial serial) : base(serial)
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
                from.SendMessage("Select the weapon or armor you want to add self repair.");
                from.Target = new SelfRepairTarget(this);
            }
            else
            {
                from.SendMessage("This item must be in your backpack to use.");
            }
        }

        private class SelfRepairTarget : Target
        {
            private readonly DeedOfSelfRepair _Deed;

            public SelfRepairTarget(DeedOfSelfRepair deed) : base(1, false, TargetFlags.None)
            {
                _Deed = deed;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                // Check if item is of Type Armor
                if (targeted is BaseArmor armor && armor.CanRepair)
                {
                    armor.ArmorAttributes.SelfRepair += _SelfRepairAmount;
                    from.SendMessage(70, "You have added self repair to the armor.");

                    // Delete the deed after usage
                    _Deed.Delete();
                }

                // Check if item is of Type Weapon
                else if (targeted is BaseWeapon weapon && weapon.CanRepair)
                {
                    weapon.WeaponAttributes.SelfRepair += _SelfRepairAmount;
                    from.SendMessage(70, "You have added self repair to the weapon.");

                    // Delete the deed after usage
                    _Deed.Delete();
                }

                // Check if item is of Type Clothing
                else if (targeted is BaseClothing clothing && clothing.CanRepair)
                {
                    clothing.ClothingAttributes.SelfRepair += _SelfRepairAmount;
                    from.SendMessage(70, "You have added self repair to the clothing.");

                    // Delete the deed after usage
                    _Deed.Delete();
                }

                // if item not of defined type
                else
                {
                    from.SendMessage(38, "You can only add self repair to weapons and armors.");
                }
            }
        }
    }
}

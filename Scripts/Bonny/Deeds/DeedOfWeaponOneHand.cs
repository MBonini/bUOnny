#region References
using Server.Targeting;
#endregion

namespace Server.Items
{
    public sealed class DeedOfWeaponOneHand : Item
    {
        [Constructable]
        public DeedOfWeaponOneHand() : base(0x14F0)
        {
            Name = "Deed of weapon one hand";
            Hue = 1161;
            Weight = 0.1;
            LootType = LootType.Blessed;
        }

        public DeedOfWeaponOneHand(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0);
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
                from.SendMessage("Select the weapon you want to be one handed.");
                from.Target = new WeaponOneHandTarget(this);
            }
            else
            {
                from.SendLocalizedMessage(1042001);
            }
        }

        private class WeaponOneHandTarget : Target
        {
            private readonly DeedOfWeaponOneHand _Deed;

            public WeaponOneHandTarget(DeedOfWeaponOneHand deed) : base(1, false, TargetFlags.None)
            {
                _Deed = deed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                // Check if target is of Type Weapon
                if(target is BaseWeapon weapon)
                {
                    if(weapon.Layer != Layer.OneHanded)
                    {
                        weapon.Layer = Layer.OneHanded;
                        from.SendMessage( 70, "The weapon is now one handed!");

                        // Delete the deed after usage
                        _Deed.Delete();
                    }
                    else
                    {
                        from.SendMessage(38, "This weapon is already one handed.");
                    }
                }
                else
                {
                    from.SendMessage(38, "Can only enhance Weapons.");
                }
            }
        }
    }
}

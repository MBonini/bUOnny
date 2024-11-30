#region References
using Server.Targeting;
#endregion

namespace Server.Items
{
    public sealed class DeedOfSpellChanneling : Item
    {
        [Constructable]
        public DeedOfSpellChanneling() : base(0x14F0)
        {
            Name = "Deed of spell channeling";
            Hue = 0x0A1F;
            Weight = 0.1;
            LootType = LootType.Blessed;
        }

        public DeedOfSpellChanneling(Serial serial) : base(serial)
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
                from.SendMessage("Select the weapon or shield you want to have spell channeling.");
                from.Target = new SpellChannelingTarget(this);
            }
            else
            {
                from.SendLocalizedMessage(1042001);
            }
        }

        private class SpellChannelingTarget : Target
        {
            private readonly DeedOfSpellChanneling _Deed;

            public SpellChannelingTarget(DeedOfSpellChanneling deed) : base(1, false, TargetFlags.None)
            {
                _Deed = deed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                switch (target)
                {
                    // Check if target is of Type Weapon
                    case BaseWeapon weapon when weapon.Attributes.SpellChanneling != 1:
                        weapon.Attributes.SpellChanneling = 1;
                        from.SendMessage( 70, "The weapon is now spell channeling!");

                        // Delete the deed after usage
                        _Deed.Delete();
                        break;
                    case BaseWeapon weapon:
                        from.SendMessage(38, "This weapon is already spell channeling.");
                        break;
                    // Check if target is of Type Shield
                    case BaseShield shield when shield.Attributes.SpellChanneling != 1:
                        shield.Attributes.SpellChanneling = 1;
                        from.SendMessage( 70, "The shield is now spell channeling!");

                        // Delete the deed after usage
                        _Deed.Delete();
                        break;
                    case BaseShield shield:
                        from.SendMessage(38, "This shield is already spell channeling.");
                        break;
                    default:
                        from.SendMessage(38, "Can only enhance Weapons or Shields.");
                        break;
                }
            }
        }
    }
}

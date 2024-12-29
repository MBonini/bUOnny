#region References
using Server.Targeting;
#endregion

namespace Server.Items
{
    public sealed class DeedOfArmorMeditable : Item
    {
        [Constructable]
        public DeedOfArmorMeditable() : base(0x14F0)
        {
            Name = "Deed of armor meditable";
            Hue = 0x04F2;
            Weight = 0.1;
            LootType = LootType.Blessed;
        }

        public DeedOfArmorMeditable(Serial serial) : base(serial)
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
                from.SendMessage("Select the piece of armor you want to be meditable.");
                from.Target = new ArmorMeditableTarget(this);
            }
            else
            {
                from.SendLocalizedMessage(1042001);
            }
        }

        private class ArmorMeditableTarget : Target
        {
            private readonly DeedOfArmorMeditable _Deed;

            public ArmorMeditableTarget(DeedOfArmorMeditable deed) : base(1, false, TargetFlags.None)
            {
                _Deed = deed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                switch (target)
                {
                    // Check if target is of Type Armor
                    case BaseArmor armor when armor.ArmorAttributes.MageArmor != 1:
                        armor.ArmorAttributes.MageArmor = 1;
                        from.SendMessage( 70, "The piece of armor is now meditable!");

                        // Delete the deed after usage
                        _Deed.Delete();
                        break;
                    case BaseArmor armor:
                        from.SendMessage(38, "This piece of armor is already meditable.");
                        break;
                    // TODO BONNY: Add BaseClothing type -> ClothingAttributes.MageArmor = 1
                    default:
                        from.SendMessage(38, "Can only enhance pieces of Armor.");
                        break;
                }
            }
        }

    }
}

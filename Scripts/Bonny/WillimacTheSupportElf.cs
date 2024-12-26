#region References

using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.ContextMenus;
using Server.Gumps;
using System.Collections;
using Server.Network;
using Server.Engines.Points;

#endregion

namespace Server.Engines.Bonny
{
    public class WillimacTheSupportElf : BaseVendor
    {
        public override bool IsActiveVendor => false;
        public override bool IsInvulnerable => true;
        public override bool DisallowAllMoves => true;
        public override bool ClickTitle => true;
        public override bool CanTeach => false;

        private List<SBInfo> _SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => _SBInfos;
        public override void InitSBInfo()
        {
        }

        [Constructable]
        public WillimacTheSupportElf() : base("the Support Elf")
        {
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = "Willimac";

            Race = Race.Elf;
            Female = true;
            Body = 606;
            Hue = 0x361;

            FacialHairItemID = 0;
            FacialHairHue = 0;

            HairItemID = 0x2FCF;
            HairHue = 0x038;
        }

        public override void InitOutfit()
        {
            SetWearable(new FemaleLeatherChest(), 0x04AB);
            SetWearable(new LeatherSkirt(), 0x04AB);
            SetWearable(new LeatherGloves(), 0x04AB);
            SetWearable(new Boots(), 0x04AB);
            SetWearable(new Circlet(), 0x04AB);
            SetWearable(new ElvenQuiver());

            if (Backpack == null)
            {
                Item backpack = new Backpack();
                backpack.Movable = false;
                AddItem(backpack);
            }
        }

        public WillimacTheSupportElf(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}

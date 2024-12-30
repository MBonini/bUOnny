#region References

using System.Collections.Generic;
using Server.Engines.Bonny.BeltOfStrength;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

#endregion

namespace Server.Engines.Bonny
{
    public sealed class WillimacTheSupportElf : BaseVendor
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

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile player && player.InRange(this.Location, 5))
                player.SendGump(new WillimacGump(this, player));
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

    public sealed class WillimacGump : Gump
    {
        private readonly WillimacTheSupportElf _Willimac;
        private readonly PlayerMobile _Player;

        public WillimacGump(WillimacTheSupportElf willimac, PlayerMobile player) : base(25, 25)
        {
            _Willimac = willimac;
            _Player = player;
            _Player.CloseGump(typeof(WillimacGump));

            AddPage(0);

            const int gumpWidth = 50 + 200 + 50;
            const int gumpHeight = 50 + 75 + 50;
            AddBackground(0, 0, gumpWidth, gumpHeight, 5054);
            AddImageTiled(10, 10, gumpWidth - 10 * 2, gumpHeight - 10 * 2, 2624);

            AddLabel(50, 50, 88, "Hi, how can I help you?");

            AddButton(50, 100, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(100, 100, 88, "Belt of Strength");
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            int buttonId = info.ButtonID;
            _Player.CloseGump(typeof(WillimacGump));

            switch (buttonId)
            {
                case 1:
                    _Player.SendGump(new BeltOfStrengthGump(_Player));
                    break;
            }
        }
    }
}

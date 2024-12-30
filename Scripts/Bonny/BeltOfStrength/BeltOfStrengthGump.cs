#region References

using System.Collections.Generic;
using System.Linq;
using Bonny;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

#endregion

namespace Server.Engines.Bonny.BeltOfStrength
{
    public class BeltOfStrengthGump : Gump
    {
        private readonly PlayerMobile _Player;

        public BeltOfStrengthGump(PlayerMobile player) : base(25, 25)
        {
            _Player = player;
            _Player.CloseGump(typeof(BeltOfStrengthGump));

            AddPage(0);

            const int gumpWidth = 50 + 300 + 50;
            const int gumpHeight = 50 + 125 + 50;
            AddBackground(0, 0, gumpWidth, gumpHeight, 5054);
            AddImageTiled(10, 10, gumpWidth - 10 * 2, gumpHeight - 10 * 2, 2624);

            AddLabel(50, 50, 88, "Belt of Strength");

            AddButton(50, 100, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(100, 100, 88, "Buy a Belt of Orc Strength (1000 gp)");

            AddButton(50, 150, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddLabel(100, 150, 88, "Upgrade Belts of Strength");
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            int buttonId = info.ButtonID;
            _Player.CloseGump(typeof(BeltOfStrengthGump));

            switch (buttonId)
            {
                case 1:
                    if (_Player.PayWithGold(1000))
                    {
                        _Player.PlaceInBackpack(new BeltOfOrcStrength());
                        _Player.SendMessage(70, "You have acquired a new Belt of Orc Strength.");
                    }
                    else
                    {
                        _Player.SendMessage(38, "You have not enough gold to buy a Belt of Orc Strength.");
                    }

                    break;
                case 2:
                    bool findOne = false;

                    List<BeltOfStrength> ownedBelts = _Player.Backpack?.FindItemsByType<BeltOfStrength>(true) ?? new List<BeltOfStrength>();
                    List<BeltOfStrength> wornBelts = _Player.Items.Where(item => item is BeltOfStrength).Cast<BeltOfStrength>().ToList();
                    List<BeltOfStrength> belts = new List<BeltOfStrength>();
                    belts.AddRange(ownedBelts);
                    belts.AddRange(wornBelts);
                    belts.ForEach(belt =>
                    {
                        if (belt != null && belt.Upgrade(_Player))
                        {
                            findOne = true;
                            _Player.SendMessage(70, "A Belt of Strength has been upgraded.");
                        }
                    });

                    if (!findOne)
                    {
                        _Player.SendMessage(38, "No Belt of Strength is ready to be upgraded.");
                    }

                    break;
            }
        }
    }
}

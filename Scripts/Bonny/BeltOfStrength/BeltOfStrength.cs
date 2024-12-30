#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Bonny;
using Server.ContextMenus;
using Server.Engines.Bonny.BeltOfStrength;
using Server.Items;
using Server.Mobiles;

#endregion

namespace Server.Engines.Bonny.BeltOfStrength
{
    public abstract class BeltOfStrength : BaseWaist
    {
        private static readonly HashSet<Type> _TrackedCreatures = new HashSet<Type>
            {typeof(Troll), typeof(FrostTroll), typeof(Ettin), typeof(Ogre), typeof(Cyclops), typeof(Titan), typeof(OgreLord)};

        private int _Points;

        public BeltOfStrength(int hue, int points = 0) : base(0x2B68, hue)
        {
            Weight = 4.0;

            _Points = points;
        }

        public override void OnDelete()
        {
            base.OnDelete();

            EventSink.CreatureDeath -= OnCreatureDeath;
        }

        public override bool OnEquip(Mobile from)
        {
            if (CanUpgrade()) EventSink.CreatureDeath += OnCreatureDeath;

            return base.OnEquip(from);
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            EventSink.CreatureDeath -= OnCreatureDeath;
        }

        private void OnCreatureDeath(CreatureDeathEventArgs e)
        {
            if (CanUpgrade() &&
                e.Creature is BaseCreature creature && _TrackedCreatures.Contains(creature.GetType()) &&
                e.Killer is PlayerMobile killer && killer.FindItemOnLayer(Layer.Waist) == this)
            {
                _Points += creature.Fame / 1000;
                killer.SendMessage(88, "BeltOfStrength: added points: +{0} (total: {1})", creature.Fame / 1000, _Points);
            }
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
        }

        public override bool Scissor(Mobile from, Scissors scissors)
        {
            from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
            return false;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            list.Add(new PointDetails(from, this));
        }

        private sealed class PointDetails : ContextMenuEntry
        {
            private readonly PlayerMobile _Player;
            private readonly BeltOfStrength _Belt;

            public PointDetails(Mobile mobile, BeltOfStrength belt) : base(484) // Info
            {
                _Player = mobile as PlayerMobile;
                _Belt = belt;
            }

            public override void OnClick()
            {
                if (_Belt.CanUpgrade())
                {
                    _Player.SendMessage(88, "This belt has {0} points and can be upgraded at {1} points", _Belt._Points, _Belt.UpgradeThreshold());
                    if (_Belt._Points >= _Belt.UpgradeThreshold()) _Player.SendMessage(88, "This belt is ready to be upgraded!!!");
                    _Player.SendMessage(88, "Gain points killing: Troll, FrostTroll, Ettin, Ogre, Cyclop, Titan and OgreLord while you wear this belt.");

                    return;
                }

                _Player.SendMessage(88, "This belt has reached the maximum level, congratulations!");
            }
        }

        public bool Upgrade(PlayerMobile requester)
        {
            if (!CanUpgrade()) return false;

            if (_Points >= UpgradeThreshold() && this.ParentHierarchy().First() == requester)
            {
                int points = _Points;

                this.Delete();

                requester.PlaceInBackpack(UpgradedBelt(points));

                return true;
            }

            return false;
        }

        protected abstract bool CanUpgrade();

        protected abstract int UpgradeThreshold();

        protected abstract BeltOfStrength UpgradedBelt(int points);

        public BeltOfStrength(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // Version

            writer.Write(_Points);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _Points = reader.ReadInt();

            List<object> parentHierarchy = this.ParentHierarchy();
            if (parentHierarchy.Count == 2 && parentHierarchy.First() is PlayerMobile player) this.OnEquip(player);
        }
    }
}

namespace Server.Items
{
    public sealed class BeltOfOrcStrength : BeltOfStrength
    {
        public BeltOfOrcStrength() : base(0x0029)
        {
            Name = "Belt of Orc Strength";

            Attributes.BonusStr = 20;
            Attributes.BonusHits = 20;
            Attributes.WeaponDamage = 10;

            Resistances.Physical = 10;

            ClothingAttributes.MageArmor = 1;

            LootType = LootType.Blessed;
        }

        protected override bool CanUpgrade()
        {
            return true;
        }

        protected override int UpgradeThreshold()
        {
            return 100;
        }

        protected override BeltOfStrength UpgradedBelt(int points)
        {
            return new BeltOfTrollStrength(points);
        }

        public BeltOfOrcStrength(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
}

namespace Server.Items
{
    public sealed class BeltOfTrollStrength : BeltOfStrength
    {
        public BeltOfTrollStrength(int points) : base(0x0028, points)
        {
            Name = "Belt of Troll Strength";

            Attributes.BonusStr = 50;
            Attributes.BonusHits = 50;
            Attributes.WeaponDamage = 25;
            Attributes.RegenHits = 2;

            Resistances.Physical = 25;

            ClothingAttributes.MageArmor = 1;

            LootType = LootType.Blessed;
        }

        protected override bool CanUpgrade()
        {
            return true;
        }

        protected override int UpgradeThreshold()
        {
            return 250;
        }

        protected override BeltOfStrength UpgradedBelt(int points)
        {
            return new BeltOfEttinStrength(points);
        }

        public BeltOfTrollStrength(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
}

namespace Server.Items
{
    public sealed class BeltOfEttinStrength : BeltOfStrength
    {
        public BeltOfEttinStrength(int points) : base(0x0027, points)
        {
            Name = "Belt of Ettin Strength";

            Attributes.BonusStr = 100;
            Attributes.BonusHits = 100;
            Attributes.WeaponDamage = 50;
            Attributes.RegenHits = 5;

            Resistances.Physical = 50;

            ClothingAttributes.MageArmor = 1;

            LootType = LootType.Blessed;
        }

        protected override bool CanUpgrade()
        {
            return true;
        }

        protected override int UpgradeThreshold()
        {
            return 500;
        }

        protected override BeltOfStrength UpgradedBelt(int points)
        {
            return new BeltOfCyclopStrength(points);
        }

        public BeltOfEttinStrength(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
}

namespace Server.Items
{
    public sealed class BeltOfCyclopStrength : BeltOfStrength
    {
        public BeltOfCyclopStrength(int points) : base(0x0026, points)
        {
            Name = "Belt of Cyclop Strength";

            Attributes.BonusStr = 200;
            Attributes.BonusHits = 200;
            Attributes.WeaponDamage = 80;
            Attributes.RegenHits = 10;

            Resistances.Physical = 70;

            ClothingAttributes.MageArmor = 1;

            LootType = LootType.Blessed;
        }

        protected override bool CanUpgrade()
        {
            return true;
        }

        protected override int UpgradeThreshold()
        {
            return 1000;
        }

        protected override BeltOfStrength UpgradedBelt(int points)
        {
            return new BeltOfTitanStrength();
        }

        public BeltOfCyclopStrength(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
}

namespace Server.Items
{
    public sealed class BeltOfTitanStrength : BeltOfStrength
    {
        public BeltOfTitanStrength() : base(0x0025)
        {
            Name = "Belt of Titan Strength";

            Attributes.BonusStr = 350;
            Attributes.BonusHits = 350;
            Attributes.WeaponDamage = 150;
            Attributes.RegenHits = 20;

            Resistances.Physical = 95;

            ClothingAttributes.MageArmor = 1;

            LootType = LootType.Blessed;
        }

        protected override bool CanUpgrade()
        {
            return false;
        }

        protected override int UpgradeThreshold()
        {
            return 0;
        }

        protected override BeltOfStrength UpgradedBelt(int points)
        {
            return new BeltOfTitanStrength();
        }

        public BeltOfTitanStrength(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
}

/*
 * Inspired by Ashlar, beloved of Morrigan
 */

#region References

using System;
using Server.Gumps;
using Server.Network;

#endregion

namespace Server.Items
{
    public sealed class BookOfSkillTraining : Item
    {
        private int _StudyTime;
        private int _Ouch;

        [CommandProperty(AccessLevel.GameMaster)]
        public int StudyTime
        {
            get => _StudyTime;
            set
            {
                _StudyTime = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Ouch
        {
            get => _Ouch;
            set
            {
                _Ouch = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double MinSkill { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public double MaxSkill { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Studying { get; set; }

        [Constructable]
        public BookOfSkillTraining() : this(3, 5, -50.0, 1000.0, false)
        {
        }

        [Constructable]
        public BookOfSkillTraining(int studyTime, int ouch, double minSkill, double maxSkill, bool studying) :
            base(0x1E25)
        {
            Name = "Books of skill training";
            Movable = true;
            LootType = LootType.Blessed;
            Weight = 5.0;
            Hue = 0x03E9;

            StudyTime = studyTime;
            Ouch = ouch;
            MinSkill = minSkill;
            MaxSkill = maxSkill;
            Studying = studying;
        }

        public BookOfSkillTraining(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((int)_StudyTime);
            writer.Write((int)_Ouch);

            writer.Write(MinSkill);
            writer.Write(MaxSkill);

            writer.Write((bool)Studying);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _StudyTime = reader.ReadInt();
            _Ouch = reader.ReadInt();

            MinSkill = reader.ReadDouble();
            MaxSkill = reader.ReadDouble();

            Studying = reader.ReadBool();
        }

        public override void OnDoubleClick(Mobile from)
        {
            UseBook(from);
        }

        public void UseBook(Mobile from)
        {
            if (!Studying)
                from.SendGump(new SkillTrainingGump(from, this));
            else
                from.SendMessage(38, "You must wait until your current studies are completed.");
        }
    }

    public sealed class SkillTrainingGump : Gump
    {
        private readonly Mobile _From;
        private readonly BookOfSkillTraining _Book;

        private readonly SkillName[] _Skills = Enum.GetValues(typeof(SkillName)) as SkillName[];

        private readonly int _NRows = 15;

        public SkillTrainingGump(Mobile from, BookOfSkillTraining book) : base(25, 25)
        {
            _From = from;
            _Book = book;
            _From.CloseGump(typeof(SkillTrainingGump));

            AddPage(0);

            int gumpWidth = 50 + 200 * 4;
            int gumpHeight = 50 + 25 * (_NRows - 1) + 50;
            AddBackground(0, 0, gumpWidth, gumpHeight, 5054);
            AddImageTiled(10, 10, gumpWidth - 10 * 2, gumpHeight - 10 * 2, 2624);
            AddAlphaRegion(10, 10, gumpWidth - 10 * 2, gumpHeight - 10 * 2);

            AddLabel(75, 25, 88, "What do you want to study?");

            for (int skillId = 0; skillId < _Skills.Length; skillId++)
            {
                int xDelta = 200 * (skillId / _NRows);
                int yDelta = 25 * (skillId % _NRows);
                AddButton(50 + xDelta, 50 + yDelta, 4005, 4007, skillId + 1, GumpButtonType.Reply, 0);
                AddLabel(100 + xDelta, 50 + yDelta, 88, _Skills[skillId].ToString());
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (_Book.Deleted)
                return;

            if (info.ButtonID < 1 || info.ButtonID > _Skills.Length)
            {
                return;
            }
            SkillName requestedSkillName = _Skills[info.ButtonID - 1];

            if (!_From.InRange(_Book.GetWorldLocation(), 2))
            {
                _From.SendMessage(38, "Your eyes are not quite up to the challenge, get a little closer.");
                _Book.UseBook(_From);
                return;
            }

            if (_From.Hits <= _Book.Ouch)
            {
                _From.SendMessage(38, "You are too weak to study...");
                _Book.UseBook(_From);
                return;
            }

            if (_From.Skills[requestedSkillName].Base >= _Book.MaxSkill)
            {
                _From.SendMessage( 38, "You have mastered all that these books have to teach regarding {0}.", requestedSkillName.ToString());
                _Book.UseBook(_From);
                return;
            }

            if (_From.Skills[requestedSkillName].Base >= _From.Skills[requestedSkillName].Cap)
            {
                _From.SendMessage( 38, "You turn to the {0} section of the books and study for a while, but you have reached already your limits in this skill.", requestedSkillName.ToString());
                _Book.UseBook(_From);
                return;
            }

            _From.SendMessage( 70, "You turn to the {0} section of the books and study for a while.", requestedSkillName.ToString());
            _From.CheckSkill(requestedSkillName, _Book.MinSkill, _Book.MaxSkill);
            _From.Hits -= _Book.Ouch;
            _From.Stam -= _Book.Ouch;
            _From.Mana -= _Book.Ouch;
            _Book.Studying = true;
            new StudyTimer(_From, _Book).Start();
        }
    }

    public sealed class StudyTimer : Timer
    {
        private readonly Mobile _From;
        private readonly BookOfSkillTraining _Book;

        public StudyTimer(Mobile from, BookOfSkillTraining book) : base(TimeSpan.FromSeconds(book.StudyTime))
        {
            _From = from;
            _Book = book;
        }

        protected override void OnTick()
        {
            _Book.Studying = false;
            _Book.UseBook(_From);
        }
    }
}

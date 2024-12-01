#region References

using System;
using Server.Gumps;
using Server.Network;

#endregion

namespace Server.Items
{
    public sealed class BookOfSkillTraining : Item
    {
        private StudyTimer _StudyTimer;

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

        private void UseBook(Mobile from)
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

        private readonly SkillName[] _Skills = {
            SkillName.Lockpicking,
            SkillName.RemoveTrap,
            SkillName.DetectHidden,
            SkillName.Fishing,
            SkillName.Hiding,
            SkillName.Stealing,
            SkillName.Stealth,
            SkillName.AnimalTaming,
            SkillName.AnimalLore,
            SkillName.Veterinary
        };

        public SkillTrainingGump(Mobile from, BookOfSkillTraining book) : base(25, 25)
        {
            _From = from;
            _Book = book;
            _From.CloseGump(typeof(SkillTrainingGump));

            AddPage(0);

            AddBackground(50, 10, 425, 174, 5054);
            AddImageTiled(58, 20, 408, 155, 2624);
            AddAlphaRegion(58, 20, 408, 155);

            AddLabel(75, 25, 88, "What do you want to study?");

            for (int skillId = 0; skillId < _Skills.Length; skillId++)
            {
                int xDelta = 200 * (skillId / 5);
                int yDelta = 25 * (skillId % 5);
                AddButton(75 + xDelta, 50 + yDelta, 4005, 4007, skillId, GumpButtonType.Reply, 0);
                AddLabel(125 + xDelta, 50 + yDelta, 88, _Skills[skillId].ToString());
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (_Book.Deleted)
                return;

            if (info.ButtonID == 0)
            {
                if (!_From.InRange(_Book.GetWorldLocation(), 2))
                {
                    _From.SendMessage("Your eyes are not quite up to the challenge, get a little closer.");
                }
                else
                {
                    if (_From.Hits <= _Book.Ouch)
                    {
                        _From.SendMessage("You are too weak!");
                    }
                    else
                    {
                        new StudyTimer(_From, _Book).Start();
                        if (_From.Skills.Lockpicking.Base >= _Book.MaxSkill)
                            _From.SendMessage(
                                "You have mastered all that these books have to teach regarding Lockpicking");
                        else
                            _From.SendMessage(
                                "You turn to the Lockpicking section of the books and study for a while.");
                        _From.CheckSkill(SkillName.Lockpicking, _Book.MinSkill, _Book.MaxSkill);
                        _From.Hits = (_From.Hits - _Book.Ouch);
                        _From.Stam = (_From.Stam - _Book.Ouch);
                        _From.Mana = (_From.Mana - _Book.Ouch);
                        _Book.Studying = true;
                    }
                }
            }
            else if (info.ButtonID == 1)
            {
                if (!_From.InRange(_Book.GetWorldLocation(), 2))
                {
                    _From.SendMessage("Your eyes are not quite up to the challenge, get a little closer.");
                }
                else
                {
                    if (_From.Hits <= _Book.Ouch)
                    {
                        _From.SendMessage("You are too weak!");
                    }
                    else
                    {
                        new StudyTimer(_From, _Book).Start();
                        if (_From.Skills.RemoveTrap.Base >= _Book.MaxSkill)
                            _From.SendMessage(
                                "You have mastered all that these books have to teach regarding Remove Trap.");
                        else
                            _From.SendMessage(
                                "You turn to the Remove Trap section of the books and study for a while.");
                        _From.CheckSkill(SkillName.RemoveTrap, _Book.MinSkill, _Book.MaxSkill);
                        _From.Hits = (_From.Hits - _Book.Ouch);
                        _From.Stam = (_From.Stam - _Book.Ouch);
                        _From.Mana = (_From.Mana - _Book.Ouch);
                        _Book.Studying = true;
                    }
                }
            }
            else if (info.ButtonID == 2)
            {
                if (!_From.InRange(_Book.GetWorldLocation(), 2))
                {
                    _From.SendMessage("Your eyes are not quite up to the challenge, get a little closer.");
                }
                else
                {
                    if (_From.Hits <= _Book.Ouch)
                    {
                        _From.SendMessage("You are too weak!");
                    }
                    else
                    {
                        new StudyTimer(_From, _Book).Start();
                        if (_From.Skills.DetectHidden.Base >= _Book.MaxSkill)
                            _From.SendMessage(
                                "You have mastered all that these books have to teach regarding Detect Hidden.");
                        else
                            _From.SendMessage(
                                "You turn to the Detect Hidden section of the books and study for a while.");
                        _From.CheckSkill(SkillName.DetectHidden, _Book.MinSkill, _Book.MaxSkill);
                        _From.Hits = (_From.Hits - _Book.Ouch);
                        _From.Stam = (_From.Stam - _Book.Ouch);
                        _From.Mana = (_From.Mana - _Book.Ouch);
                        _Book.Studying = true;
                    }
                }
            }
            else if (info.ButtonID == 3)
            {
                if (!_From.InRange(_Book.GetWorldLocation(), 2))
                {
                    _From.SendMessage("Your eyes are not quite up to the challenge, get a little closer.");
                }
                else
                {
                    if (_From.Hits <= _Book.Ouch)
                    {
                        _From.SendMessage("You are too weak!");
                    }
                    else
                    {
                        new StudyTimer(_From, _Book).Start();
                        if (_From.Skills.Fishing.Base >= _Book.MaxSkill)
                            _From.SendMessage(
                                "You have mastered all that these books have to teach regarding Fishing.");
                        else
                            _From.SendMessage("You turn to the Fishing section of the books and study for a while.");
                        _From.CheckSkill(SkillName.Fishing, _Book.MinSkill, _Book.MaxSkill);
                        _From.Hits = (_From.Hits - _Book.Ouch);
                        _From.Stam = (_From.Stam - _Book.Ouch);
                        _From.Mana = (_From.Mana - _Book.Ouch);
                        _Book.Studying = true;
                    }
                }
            }
            else if (info.ButtonID == 4)
            {
                if (!_From.InRange(_Book.GetWorldLocation(), 2))
                {
                    _From.SendMessage("Your eyes are not quite up to the challenge, get a little closer.");
                }
                else
                {
                    if (_From.Hits <= _Book.Ouch)
                    {
                        _From.SendMessage("You are too weak!");
                    }
                    else
                    {
                        new StudyTimer(_From, _Book).Start();
                        if (_From.Skills.Hiding.Base >= _Book.MaxSkill)
                            _From.SendMessage("You have mastered all that these books have to teach regarding Hiding.");
                        else
                            _From.SendMessage("You turn to the Hiding section of the books and study for a while.");
                        _From.CheckSkill(SkillName.Hiding, _Book.MinSkill, _Book.MaxSkill);
                        _From.Hits = (_From.Hits - _Book.Ouch);
                        _From.Stam = (_From.Stam - _Book.Ouch);
                        _From.Mana = (_From.Mana - _Book.Ouch);
                        _Book.Studying = true;
                    }
                }
            }
            else if (info.ButtonID == 5)
            {
                if (!_From.InRange(_Book.GetWorldLocation(), 2))
                {
                    _From.SendMessage("Your eyes are not quite up to the challenge, get a little closer.");
                }
                else
                {
                    if (_From.Hits <= _Book.Ouch)
                    {
                        _From.SendMessage("You are too weak!");
                    }
                    else
                    {
                        new StudyTimer(_From, _Book).Start();
                        if (_From.Skills.Stealing.Base >= _Book.MaxSkill)
                            _From.SendMessage(
                                "You have mastered all that these books have to teach regarding Stealing.");
                        else
                            _From.SendMessage("You turn to the Stealing section of the books and study for a while.");
                        _From.CheckSkill(SkillName.Stealing, _Book.MinSkill, _Book.MaxSkill);
                        _From.Hits = (_From.Hits - _Book.Ouch);
                        _From.Stam = (_From.Stam - _Book.Ouch);
                        _From.Mana = (_From.Mana - _Book.Ouch);
                        _Book.Studying = true;
                    }
                }
            }
            else if (info.ButtonID == 6)
            {
                if (!_From.InRange(_Book.GetWorldLocation(), 2))
                {
                    _From.SendMessage("Your eyes are not quite up to the challenge, get a little closer.");
                }
                else
                {
                    if (_From.Hits <= _Book.Ouch)
                    {
                        _From.SendMessage("You are too weak!");
                    }
                    else
                    {
                        new StudyTimer(_From, _Book).Start();
                        if (_From.Skills.Stealth.Base >= _Book.MaxSkill)
                            _From.SendMessage(
                                "You have mastered all that these books have to teach regarding Stealth.");
                        else
                            _From.SendMessage("You turn to the Stealth section of the books and study for a while.");
                        _From.CheckSkill(SkillName.Stealth, _Book.MinSkill, _Book.MaxSkill);
                        _From.Hits = (_From.Hits - _Book.Ouch);
                        _From.Stam = (_From.Stam - _Book.Ouch);
                        _From.Mana = (_From.Mana - _Book.Ouch);
                        _Book.Studying = true;
                    }
                }
            }
            else if (info.ButtonID == 7)
            {
                if (!_From.InRange(_Book.GetWorldLocation(), 2))
                {
                    _From.SendMessage("Your eyes are not quite up to the challenge, get a little closer.");
                }
                else
                {
                    if (_From.Hits <= _Book.Ouch)
                    {
                        _From.SendMessage("You are too weak!");
                    }
                    else
                    {
                        new StudyTimer(_From, _Book).Start();
                        if (_From.Skills.AnimalTaming.Base >= _Book.MaxSkill)
                            _From.SendMessage(
                                "You have mastered all that these books have to teach regarding Animal Taming.");
                        else
                            _From.SendMessage(
                                "You turn to the Animal Taming section of the books and study for a while.");
                        _From.CheckSkill(SkillName.AnimalTaming, _Book.MinSkill, _Book.MaxSkill);
                        _From.Hits = (_From.Hits - _Book.Ouch);
                        _From.Stam = (_From.Stam - _Book.Ouch);
                        _From.Mana = (_From.Mana - _Book.Ouch);
                        _Book.Studying = true;
                    }
                }
            }
            else if (info.ButtonID == 8)
            {
                if (!_From.InRange(_Book.GetWorldLocation(), 2))
                {
                    _From.SendMessage("Your eyes are not quite up to the challenge, get a little closer.");
                }
                else
                {
                    if (_From.Hits <= _Book.Ouch)
                    {
                        _From.SendMessage("You are too weak!");
                    }
                    else
                    {
                        new StudyTimer(_From, _Book).Start();
                        if (_From.Skills.AnimalLore.Base >= _Book.MaxSkill)
                            _From.SendMessage(
                                "You have mastered all that these books have to teach regarding Animal Lore.");
                        else
                            _From.SendMessage(
                                "You turn to the Animal Lore section of the books and study for a while.");
                        _From.CheckSkill(SkillName.AnimalLore, _Book.MinSkill, _Book.MaxSkill);
                        _From.Hits = (_From.Hits - _Book.Ouch);
                        _From.Stam = (_From.Stam - _Book.Ouch);
                        _From.Mana = (_From.Mana - _Book.Ouch);
                        _Book.Studying = true;
                    }
                }
            }
            else if (info.ButtonID == 9)
            {
                if (!_From.InRange(_Book.GetWorldLocation(), 2))
                {
                    _From.SendMessage("Your eyes are not quite up to the challenge, get a little closer.");
                }
                else
                {
                    if (_From.Hits <= _Book.Ouch)
                    {
                        _From.SendMessage("You are too weak!");
                    }
                    else
                    {
                        new StudyTimer(_From, _Book).Start();
                        if (_From.Skills.Veterinary.Base >= _Book.MaxSkill)
                            _From.SendMessage(
                                "You have mastered all that these books have to teach regarding Veterinary.");
                        else
                            _From.SendMessage("You turn to the Veterinary section of the books and study for a while.");
                        _From.CheckSkill(SkillName.Veterinary, _Book.MinSkill, _Book.MaxSkill);
                        _From.Hits = (_From.Hits - _Book.Ouch);
                        _From.Stam = (_From.Stam - _Book.Ouch);
                        _From.Mana = (_From.Mana - _Book.Ouch);
                        _Book.Studying = true;
                    }
                }
            }
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
            _From.SendGump(new SkillTrainingGump(_From, _Book));
        }
    }
}

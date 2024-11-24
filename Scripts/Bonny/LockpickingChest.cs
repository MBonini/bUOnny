namespace Server.Items
{
    public class LockpickingChest : LockableContainer
    {
        private bool m_Locked;

        [Constructable]
        public LockpickingChest(): base(0x9AA)
        {
            Name = "Lockpicking Chest";
            Locked = true;
            LockLevel = 1;
            MaxLockLevel = 1000; // edit here to go as high as you want for your shard. 95 is osi style, 100 is max, higher with power scrolls or however you set it up in your shard.
            RequiredSkill = 1;
            Movable = true;  // set to true if you make available to players
            Weight = 4.0;
            Hue = 0x0497;
        }


        public override void LockPick(Mobile from)
        {
            Locked = true;
            from.SendMessage("The container magically relocks it self.");
        }
        public LockpickingChest(Serial serial)
            : base(serial)
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
    }
}

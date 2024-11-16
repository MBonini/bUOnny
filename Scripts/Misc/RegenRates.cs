using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.SkillMasteries;

namespace Server.Misc
{
    public delegate Int32 RegenBonusHandler(Mobile from);

    public class RegenRates
    {
        public static List<RegenBonusHandler> HitsBonusHandlers = new List<RegenBonusHandler>();
        public static List<RegenBonusHandler> StamBonusHandlers = new List<RegenBonusHandler>();
        public static List<RegenBonusHandler> ManaBonusHandlers = new List<RegenBonusHandler>();

        [CallPriority(10)]
        public static void Configure()
        {
            Mobile.DefaultHitsRate = TimeSpan.FromSeconds(11.0);
            Mobile.DefaultStamRate = TimeSpan.FromSeconds(7.0);
            Mobile.DefaultManaRate = TimeSpan.FromSeconds(7.0);

            Mobile.ManaRegenRateHandler = new RegenRateHandler(Mobile_ManaRegenRate);

            if (Core.AOS)
            {
                Mobile.StamRegenRateHandler = new RegenRateHandler(Mobile_StamRegenRate);
                Mobile.HitsRegenRateHandler = new RegenRateHandler(Mobile_HitsRegenRate);
            }
        }

        public static double GetArmorOffset(Mobile from)
        {
            double rating = 0.0;

            if (!Core.AOS)
                rating += GetArmorMeditationValue(from.ShieldArmor as BaseArmor);

            rating += GetArmorMeditationValue(from.NeckArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.HandArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.HeadArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.ArmsArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.LegsArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.ChestArmor as BaseArmor);

            return rating / 4;
        }

        private static void CheckBonusSkill(Mobile m, int cur, int max, SkillName skill)
        {
            if (!m.Alive)
                return;

            double n = (double)cur / max;
            double v = Math.Sqrt(m.Skills[skill].Value * 0.005);

            n *= (1.0 - v);
            n += v;

            m.CheckSkill(skill, n);
        }

        public static bool CheckTransform(Mobile m, Type type)
        {
            return TransformationSpellHelper.UnderTransformation(m, type);
        }

        public static bool CheckAnimal(Mobile m, Type type)
        {
            return AnimalForm.UnderTransformation(m, type);
        }

        private static TimeSpan Mobile_HitsRegenRate(Mobile from)
        {
            return TimeSpan.FromSeconds(1.0 / (0.1 * (1 + HitPointRegen(from))));
        }

        private static TimeSpan Mobile_StamRegenRate(Mobile from)
        {
            if (from.Skills == null)
                return Mobile.DefaultStamRate;

            CheckBonusSkill(from, from.Stam, from.StamMax, SkillName.Focus);

            double bonus = from.Skills[SkillName.Focus].Value * 0.1;

            bonus += StamRegen(from);

            if (Core.SA)
            {
                double rate = 1.0 / (1.42 + (bonus / 100));

                if (from is BaseCreature && ((BaseCreature)from).IsMonster)
                {
                    rate *= 1.95;
                }

                return TimeSpan.FromSeconds(rate);
            }
            else
            {
                return TimeSpan.FromSeconds(1.0 / (0.1 * (2 + bonus)));
            }
        }

        private static TimeSpan Mobile_ManaRegenRate(Mobile from)
        {
            if (from.Skills == null)
                return Mobile.DefaultManaRate;

            if (!from.Meditating)
                CheckBonusSkill(from, from.Mana, from.ManaMax, SkillName.Meditation);

            double rate;
            double armorPenalty = GetArmorOffset(from);

            {
				{
					// E.g.: Int 250 + Meditation 100 + ManaRegen 0 = 350
					// E.g.: Int 350 + Meditation 120 + ManaRegen 6 = 530
					double medPoints = from.Int + from.Skills[SkillName.Meditation].Value + (ManaRegen(from) * 10);
					
					// Formula obtained by: https://www.dcode.fr/function-equation-finder
					// Formula plot by: https://www.desmos.com/calculator
					// (0, 7.0), (350, 1.0) => 7 - 3x/175
					// (350, 1.0), (1000, 0.0) => 20/13 - x/650
					if (medPoints <= 350)
						rate = 7.0 - (3 * medPoints / 175);
					else if (medPoints < 1000)
						rate = (20.0/13.0) - (medPoints / 650);
					else
						rate = 0.0;
				}

                rate += armorPenalty;

                if (from.Meditating)
                    rate *= 0.25;

                if (rate < 0.0)
                    rate = 0.0;
                else if (rate > 7.0)
                    rate = 7.0;
            }

            if (double.IsNaN(rate))
            {
                return Mobile.DefaultManaRate;
            }

            return TimeSpan.FromSeconds(rate);
        }

        public static double HitPointRegen(Mobile from)
        {
            double points = AosAttributes.GetValue(from, AosAttribute.RegenHits);

            if (from is BaseCreature)
                points += ((BaseCreature)from).DefaultHitsRegen;

            if (Core.ML && from is PlayerMobile && from.Race == Race.Human)	//Is this affected by the cap?
                points += 2;

            if (points < 0)
                points = 0;

            if (Core.ML && from is PlayerMobile)	//does racial bonus go before/after?
                points = Math.Min(points, 18);

            if (CheckTransform(from, typeof(HorrificBeastSpell)))
                points += 20;

            if (CheckAnimal(from, typeof(Dog)) || CheckAnimal(from, typeof(Cat)))
                points += from.Skills[SkillName.Ninjitsu].Fixed / 30;

            // Skill Masteries - goes after cap
            points += RampageSpell.GetBonus(from, RampageSpell.BonusType.HitPointRegen);
            points += CombatTrainingSpell.RegenBonus(from);
            points += BarrabHemolymphConcentrate.HPRegenBonus(from);

            if (Core.AOS)
                foreach (RegenBonusHandler handler in HitsBonusHandlers)
                    points += handler(from);

            return points;
        }

        public static double StamRegen(Mobile from)
        {
            double points = AosAttributes.GetValue(from, AosAttribute.RegenStam);

            if (from is BaseCreature)
                points += ((BaseCreature)from).DefaultStamRegen;

            if (CheckTransform(from, typeof(VampiricEmbraceSpell)))
                points += 15;

            if (CheckAnimal(from, typeof(Kirin)))
                points += 20;

            if (Core.ML && from is PlayerMobile)
                points = Math.Min(points, 24);

            // Skill Masteries - goes after cap
            points += RampageSpell.GetBonus(from, RampageSpell.BonusType.StamRegen);

            if (points < -1)
                points = -1;

            if (Core.AOS)
                foreach (RegenBonusHandler handler in StamBonusHandlers)
                    points += handler(from);

            return points;
        }

        public static double ManaRegen(Mobile from)
        {
            double points = AosAttributes.GetValue(from, AosAttribute.RegenMana);

            if (from is BaseCreature)
                points += ((BaseCreature)from).DefaultManaRegen;

            if (CheckTransform(from, typeof(VampiricEmbraceSpell)))
                points += 3;
            else if (CheckTransform(from, typeof(LichFormSpell)))
                points += 13;

            if (from is PlayerMobile && from.Race == Race.Gargoyle)
                points += 2;

            foreach (RegenBonusHandler handler in ManaBonusHandlers)
                points += handler(from);

            return points;
        }

        public static double GetArmorMeditationValue(BaseArmor ar)
        {
            if (ar == null || ar.ArmorAttributes.MageArmor != 0 || ar.Attributes.SpellChanneling != 0)
                return 0.0;

            switch ( ar.MeditationAllowance )
            {
                default:
                case ArmorMeditationAllowance.None:
                    return ar.BaseArmorRatingScaled;
                case ArmorMeditationAllowance.Half:
                    return ar.BaseArmorRatingScaled / 2.0;
                case ArmorMeditationAllowance.All:
                    return 0.0;
            }
        }
    }
}

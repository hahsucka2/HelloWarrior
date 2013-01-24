using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.WoWInternals;
using Color = System.Drawing.Color;
using Styx.Common;
using Styx.WoWInternals.WoWObjects;

using ExtensionMethods;

using System.Linq;

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        public static bool HasAuraWithMechanic(this WoWUnit unit, params WoWSpellMechanic[] mechanics)
        {
            return unit.GetAllAuras().Any(a => mechanics.Contains(a.Spell.Mechanic));
        }
    }
}



namespace HelloWarrior
{
    public class HelloWarriorRoutine : CombatRoutine
    {

        public static WoWClass[] ClassesToDisarm = { WoWClass.Warrior, WoWClass.DeathKnight, WoWClass.Rogue };

        public override string Name { get { return "HelloWarrior"; } }
        public override WoWClass Class { get { return WoWClass.Warrior; } }
        private static LocalPlayer Me { get { return StyxWoW.Me; } }
        private static WoWUnit Target { get { return StyxWoW.Me.CurrentTarget; } }

        public override void Initialize()
        {

        }

        public override void Pulse()
        {
            //\\//\\ TODO //\\//\\//\\//\\//
            //Healthstone
            //Spell Reflect
            //Shield Wall
            //Die by the sword
            //Rallying Cry
            //Skull Banner
            //Avatar
            //Recklessness
            //Disarm
            //\\//\\//\\//\\//\\//\\//\\//\\


            //Break CC
            if (Me.HasAuraWithMechanic(WoWSpellMechanic.Fleeing, WoWSpellMechanic.Sapped, WoWSpellMechanic.Incapacitated, WoWSpellMechanic.Horrified))
            {
                Cast("Berserker Rage");
            }
            else if (Me.HasAuraWithMechanic(WoWSpellMechanic.Rooted, /*WoWSpellMechanic.Slowed, */WoWSpellMechanic.Snared))
            {
                Cast("Escape Artist");
            }

            //pvp trinket
            //else if (Me.HasAuraWithMechanic(WoWSpellMechanic.Rooted, WoWSpellMechanic.Slowed, WoWSpellMechanic.Snared))
            //{
            //    
            //}



            //Defensive cooldowns






            if (Target != null && Target.IsAlive && Target.Attackable)
            {

                if (Target.IsCasting && Target.CurrentCastTimeLeft.TotalSeconds <= 1)
                {
                    if (Me.IsSafelyFacing(Target))
                    {
                        if (Target.Distance <= 10 && !Target.HasAura("Shockwave") && !SpellManager.GlobalCooldown && SpellManager.CanCast("Shockwave"))
                        {
                            Cast("Shockwave");
                            return;
                        }
                        if (Target.IsWithinMeleeRange && !SpellManager.GlobalCooldown && SpellManager.CanCast("Pummel"))
                        {
                            Cast("Pummel");
                            return;
                        }
                    }
                    if (Target.Distance <= 10 && !SpellManager.GlobalCooldown && SpellManager.CanCast("Intimidating Shout"))
                    {
                        Cast("Intimidating Shout");
                        return;
                    }

                }

                if (Target.IsPlayer && !Target.HasAura("Hamstring") && Target.IsWithinMeleeRange && !SpellManager.GlobalCooldown && SpellManager.CanCast("Hamstring"))
                {
                    Cast("Hamstring");
                }
                else if (Target.IsPlayer && !Target.HasAura("Hamstring") && Target.Distance <= 10 && !Target.HasAura("Piercing Howl") && !SpellManager.GlobalCooldown && SpellManager.CanCast("Piercing Howl"))
                {
                    Cast("Piercing Howl");
                }

                //Disarm
                else if (Target.IsPlayer && Target.IsWithinMeleeRange && !SpellManager.GlobalCooldown && SpellManager.CanCast("Disarm") && ClassesToDisarm.Any(a => Target.Class.Equals(a)))
                {
                    Cast("Disarm");
                }

                else if (Target.IsWithinMeleeRange && !SpellManager.GlobalCooldown && SpellManager.CanCast("Mortal Strike"))
                {
                    Cast("Mortal Strike");
                }
                else if (Target.IsWithinMeleeRange && !SpellManager.GlobalCooldown && SpellManager.CanCast("Colossus Smash") && !Target.HasAura("Colossus Smash"))
                {
                    Cast("Colossus Smash");
                }
                else if (Target.IsWithinMeleeRange && !SpellManager.GlobalCooldown && SpellManager.CanCast("Execute"))
                {
                    Cast("Execute");
                }
                else if (Target.IsWithinMeleeRange && !SpellManager.GlobalCooldown && SpellManager.CanCast("Overpower"))
                {
                    Cast("Overpower");
                }
                else if (!SpellManager.GlobalCooldown && SpellManager.CanCast("Deadly Calm") && GetAuraStackCount("Taste for Blood") >= 3)
                {
                    Cast("Deadly Calm");
                }
                else if (Target.IsWithinMeleeRange && SpellManager.CanCast("Heroic Strike") && Me.HasAura("Taste for Blood") && GetAuraTimeLeft("Taste for Blood") <= 2)
                {
                    Cast("Heroic Strike");
                }
                else if (Target.IsWithinMeleeRange && SpellManager.CanCast("Heroic Strike") && Me.HasAura("Deadly Calm") && Me.CurrentRage >= GetAuraStackCount("Deadly Calm") * 20)
                {
                    Cast("Heroic Strike");
                }
                else if (Target.IsWithinMeleeRange && SpellManager.CanCast("Slam") && Target.HealthPercent > 20 && Me.CurrentRage >= 70)
                {
                    Cast("Slam");
                }

            }
            c++;
        }

        public void Cast(string spellName)
        {
            Lua.DoString("RunMacroText(\"/cast " + spellName + "\")");
            Log("Casting " + spellName);
        }

        public uint GetAuraStackCount(string auraName)
        {
            WoWAura wa = StyxWoW.Me.GetAuraByName(auraName);
            return wa != null ? wa.StackCount : 0;
        }

        public static double GetAuraTimeLeft(string aura, WoWUnit u = null)
        {
            if (u == null) { u = Me; }
            WoWAura wa = u.GetAuraByName(aura);
            return wa != null ? wa.TimeLeft.TotalSeconds : 0;
        }

        public void Log(string msg)
        {
            Log(msg, Color.White);
        }

        public void Log(string msg, Color color)
        {

            Logging.Write(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B), msg);

        }

        /*

        private Composite _combat, _buffs;
        public override string Name { get { return "HelloWarrior"; } }
        public override WoWClass Class { get { return WoWClass.Warrior; } }
        public override Composite CombatBehavior { get { return _combat; } }
        public override Composite PreCombatBuffBehavior { get { return _buffs; } }
        public override Composite CombatBuffBehavior { get { return _buffs; } }
        private static LocalPlayer Me { get { return StyxWoW.Me; } }
        private static WoWUnit Target { get { return StyxWoW.Me.CurrentTarget; } }

        public override void Initialize() {
            _combat = CreateCombat();
            _buffs = CreateBuffs();
        }

        Composite CreateBuffs() {
            return new PrioritySelector(

                
                Cast("Battle Shout")

            );
        }

        Composite CreateCombat() {


            return new PrioritySelector(


                
                //Cast("Shockwave"),
                //Cast("Pummel"),

                //new Decorator(ret => Me.CurrentTarget.IsWithinMeleeRange,
                //    new PrioritySelector(

                Cast("Hamstring", ret => Target.IsWithinMeleeRange && ( !Target.HasAura("Hamstring") || getAuraTimeLeft("Hamstring", Target) <= 3 ) ),

                //Cast("Piercing Howl", ret => Target.Distance < 10 && !Target.HasAura("Piercing Howl") && !Target.HasAura("Hamstring")),

                Cast("Mortal Strike", ret => Target.IsWithinMeleeRange),

                Cast("Colossus Smash", ret => Target.IsWithinMeleeRange && getAuraTimeLeft("Colossus Smash", Target) <= 2),

                Cast("Execute", ret => Target.IsWithinMeleeRange),
                Cast("Overpower", ret => Target.IsWithinMeleeRange),

                Cast("Deadly Calm", ret => Target.IsWithinMeleeRange && getAuraStackCount("Taste for Blood") >= 3),

                Cast("Heroic Strike", ret => Target.IsWithinMeleeRange && hasAura("Taste for Blood") && getAuraTimeLeft("Taste for Blood") <= 2, false),
                Cast("Heroic Strike", ret => Target.IsWithinMeleeRange && hasAura("Deadly Calm") && Me.CurrentRage >= getAuraStackCount("Deadly Calm") * 20, false),

                Cast("Slam", ret => Target.IsWithinMeleeRange && Target.HealthPercent > 20 && Me.CurrentRage >= 70)


            );

        }

        public static TimeSpan GetSpellCooldown(string spell) {
            SpellFindResults sfr;
            if (SpellManager.FindSpell(spell, out sfr))
                return (sfr.Override ?? sfr.Original).CooldownTimeLeft;

            return TimeSpan.MaxValue;
        }

        public static double getAuraTimeLeft(string aura, WoWUnit u = null)
        {
            if (u == null)
            {
                u = StyxWoW.Me;
            }

            WoWAura wa = u.GetAuraByName(aura);
            return wa != null ? wa.TimeLeft.TotalSeconds : 0;

        }

        public static bool hasAura(string aura, WoWUnit u = null)
        {
            if (u == null)
            {
                u = StyxWoW.Me;
            }

            WoWAura wa = u.GetAuraByName(aura);
            return wa != null;

        }

        public static uint getAuraStackCount(string aura)
        {

            WoWAura wa = StyxWoW.Me.GetAuraByName(aura);
            return wa != null ? wa.StackCount : 0;

        }

        private delegate T Selection<out T>(object context);
        Composite Cast(string spell, Selection<bool> reqs = null, bool useGCD = true)
        {

            //Logging.Write(LogLevel.Normal, spell + ":" + GetSpellCooldown(spell));

            return
                new Decorator(
                    ret => ((reqs != null && reqs(ret)) || (reqs == null)) && SpellManager.CanCast(spell) && (!useGCD || !SpellManager.GlobalCooldown) && !StyxWoW.Me.IsCasting,
                    new Sequence(
                        //new Action(ret => Logging.Write(LogLevel.Normal, spell + " distance=" + Target.Distance)),
                        new Action(ret => SpellManager.Cast(spell))
                    )
                    
               );

        }

        */


    }
}

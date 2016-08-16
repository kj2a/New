namespace CarrySharp.Settings
{

    using CarrySharp;
    using Ensage;
    using SharpDX;
    using System.Collections.Generic;
    using System.Linq;

    class Variables
    {
        public class DictionarySleep
        {
            #region Properties

            public float Period { get; set; }
            public string Text { get; set; }

            public long Time { get; set; }

            #endregion Properties
        }

        public class DictionaryUnit
        {
            #region Properties

            public List<Ht> Ht { get; set; }
            public Unit Unit { get; set; }

            #endregion Properties

            #region Methods

            public bool AHealth(Entity unit)
            {
                if (unit.Handle != Unit.Handle) return false;
                if (Ht.Any(x => x.Health - unit.Health < 10)) return true;
                Ht.Add(new Ht { Health = unit.Health, Time = Game.GameTime, ACreeps = Lasthit.Attack(unit) });
                return true;
            }

            #endregion Methods
        }

        public class Ht
        {
            #region Properties

            public int ACreeps { get; set; }
            public float Health { get; set; }
            public float Time { get; set; }

            #endregion Properties
        }

            internal class Var
    {
        #region Fields

        public static int AutoAttackMode;
        public static bool AutoAttackTypeDef;
        public static List<Unit> Creeps;
        public static List<DictionaryUnit> CreepsDic = new List<DictionaryUnit>();
        public static Unit CreeptargetH;
        public static Unit CreeptargetS;
        public static bool DisableAaKeyPressed;
        public static double HeroAPoint;
        public static bool Loaded;
        public static Hero Me;
        public static Ability Q, W, E, R;
        public static Dictionary<string, ParticleEffect> RadiusHeroParticleEffect;
        public static int Seconds;
        public static List<DictionarySleep> SleepDic = new List<DictionarySleep>();
        public static List<Unit> StackableSummons = new List<Unit>();
        public static Dictionary<Unit, List<Unit>> Summons;
        public static bool SummonsAutoAttackTypeDef;
        public static bool SummonsDisableAaKeyPressed;
        public static Hero Target;

        #endregion Fields
    }
  }
}
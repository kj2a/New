using System.Security.Permissions;

namespace CarrySharp.Settings
{

    using System;
    using Ensage;
    using Ensage.Common.Menu;


    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    class HeroSelect
    {
        public static bool IsSelected { get; set; }
        public static string HeroName { get; set; }	
        public static Type HeroClass { get; set; }
        public static object HeroInstance { get; set; }

        public static void Load()
        {
            var me = ObjectManager.LocalHero;

            HeroName = Utilities.FirstUpper(Utilities.GetHeroName(me.Name)).Replace("_", "");

            HeroClass = Type.GetType("CarrySharp.Heroes." + HeroName);

            if (HeroClass == null)
            {
                Common.PrintError("Your hero is not supported by CarrySharp :(");
                IsSelected = false;
                return;
            }

            HeroInstance = Activator.CreateInstance(HeroClass);

            HeroClass.GetField("me").SetValue(HeroInstance, me);

            HeroClass.GetField("Menu").SetValue(HeroInstance, new Menu(HeroName, "options", false, me.Name, HeroName != null));
            IsSelected = true;
        }

        public static void Combo()
        {
            if (!IsSelected) return;

            HeroClass.GetMethod("Combo").Invoke(HeroInstance, null);
        }

        public static void HeroesLoadEvent()
        {
            if (!IsSelected) return;

            HeroClass.GetMethod("OnLoadEvent").Invoke(HeroInstance, null);
        }

        public static void ControllerCloseEvent()
        {
            if (!IsSelected) return;

            HeroClass.GetMethod("OnCloseEvent").Invoke(HeroInstance, null);
        }

        public static void Unload()
        {
            IsSelected = false;
            HeroName = null;
            HeroInstance = null;
            HeroClass = null;
        }

    }
}

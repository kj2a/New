using System.Security.Permissions;
namespace CarrySharp.Heroes
{

    using Ensage;
    using Ensage.Common.Menu;

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    class Vars
    {
        protected Hero e;
        public Hero me;
        public Menu Menu;
        public bool Activated, CastQ, CastW, CastE, CastR;

    }

    interface IHeroEvents
    {
        void Combo();
        void OnLoadEvent();
        void OnCloseEvent();
    }

}

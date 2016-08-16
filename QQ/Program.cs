using System.Security.Permissions;

namespace CarrySharp
{
    using System;
    using System.Threading;

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    class Program
    {

        static void Main(string[] args)
        {
            Bootstrap.Initialize();
        }
    }
}

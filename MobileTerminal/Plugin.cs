using LSPD_First_Response.Mod.API;
using System.Linq;

namespace MobileTerminal
{
    internal static class LoadedPlugin
    {
        internal static bool Exists(string name)
        {
            return Functions
                .GetAllUserPlugins()
                .Any(assembly => assembly.GetName().Name == name);
        }
    }
}

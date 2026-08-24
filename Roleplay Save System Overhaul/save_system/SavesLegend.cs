using HarmonyLib;
using Qud.UI;
using System.Collections.Generic;
using XRL.UI.Framework;


namespace zbrek_RoleplaySaveSystemOverhaul.HarmonyPatches
{
    [HarmonyPatch(typeof(SaveManagement), "UpdateMenuBars")]
    internal class SaveManagementAddKeyLegend
    {
        private static void Postfix(SaveManagement __instance)
        {
            __instance.hotkeyBar.GetNavigationContext().disabled = true;
            __instance.hotkeyBar.BeforeShow(null, new List<MenuOption>
            {
                new MenuOption { InputCommand = "NavigationXYAxis", Description = "navigate" },
                new MenuOption { KeyDescription = ControlManager.getCommandInputDescription("Accept"), Description = "select" },
                new MenuOption { InputCommand = "CmdDelete", Description = "delete" },
                new MenuOption { InputCommand = "zbrek_RoleplaySaveSystemOverhaul_SavesList", Description = "saves list" }
            });
        }
    }
}
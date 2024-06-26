﻿using HarmonyLib;
using TransportLinesManager.UI;
using TransportLinesManager.WorldInfoPanels;

namespace TransportLinesManager.Overrides 
{
    [HarmonyPatch(typeof(WorldInfoPanel))]
	public static class WorldInfoPanelOverrides 
	{
		[HarmonyPatch(typeof(WorldInfoPanel), "IsValidTarget")]
        [HarmonyPrefix]
        public static bool IsValidTarget(WorldInfoPanel __instance, ref bool __result)
        {
            if (__instance is PublicTransportWorldInfoPanel && UVMPublicTransportWorldInfoPanel.GetLineID(out ushort lineId, out bool fromBuilding) && (lineId == 0 || fromBuilding))
            {
                __result = true;
                return false;
            }
            return true;
        }

	}
}

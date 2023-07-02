﻿using Klyte.TransportLinesManager.CommonsWindow;
using Klyte.TransportLinesManager.UI;
using HarmonyLib;
using Klyte.TransportLinesManager.Data.Base;
using Klyte.TransportLinesManager.Data.TsdImplementations;

namespace Klyte.TransportLinesManager.Overrides
{
    [HarmonyPatch(typeof(PublicTransportInfoViewPanel))]
    internal static class PublicTransportInfoViewPanelOverrides
    {

        public static TLMLineCreationToolbox Toolbox { get; private set; }

        [HarmonyPatch(typeof(PublicTransportInfoViewPanel), "OpenDetailPanel")]
        [HarmonyPrefix]
        public static bool OpenDetailPanel(int idx)
        {
            TransportSystemDefinition def;
            switch (idx)
            {
                case 0:
                    def = TransportSystemDefinitionType.BUS;
                    break;
                case 1:
                    def = TransportSystemDefinitionType.TROLLEY;
                    break;
                case 2:
                    def = TransportSystemDefinitionType.TRAM;
                    break;
                case 3:
                    def = TransportSystemDefinitionType.METRO;
                    break;
                case 4:
                    def = TransportSystemDefinitionType.TRAIN;
                    break;
                case 5:
                    def = TransportSystemDefinitionType.FERRY;
                    break;
                case 6:
                    def = TransportSystemDefinitionType.BLIMP;
                    break;
                case 7:
                    def = TransportSystemDefinitionType.MONORAIL;
                    break;
                case 9:
                    def = TransportSystemDefinitionType.TOUR_PED;
                    break;
                case 10:
                    def = TransportSystemDefinitionType.TOUR_BUS;
                    break;
                default:
                    def = TransportSystemDefinitionType.BUS;
                    break;
            }

            TLMPanel.Instance?.OpenAt(def);
            return false;
        }

        [HarmonyPatch(typeof(PublicTransportInfoViewPanel), "OpenDetailPanelDefaultTab")]
        [HarmonyPrefix]
        public static bool OpenDetailPanelDefaultTab()
        {
            OpenDetailPanel(0);
            return false;
        }

        [HarmonyPatch(typeof(PublicTransportInfoViewPanel), "Start")]
        [HarmonyPrefix]
        public static void AfterAwake(PublicTransportInfoViewPanel __instance) => Toolbox = __instance.gameObject.AddComponent<TLMLineCreationToolbox>();

    }

}

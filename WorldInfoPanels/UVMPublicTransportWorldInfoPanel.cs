﻿using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using Klyte.Commons.Extensions;
using Klyte.Commons.Extensions.UI;
using Klyte.Commons.Utils;
using Klyte.TransportLinesManager.Data.Tsd;
using Klyte.TransportLinesManager.Data.DataContainers;
using Klyte.TransportLinesManager.Utils;
using Klyte.TransportLinesManager.WorldInfoPanels.Tabs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Klyte.Commons.Extensions.Patcher;

namespace Klyte.TransportLinesManager.WorldInfoPanels
{
    public class UVMPublicTransportWorldInfoPanel : Patcher, IPatcher
    {
        internal static UVMPublicTransportWorldInfoPanelObject m_obj;
        private static bool m_dirty;
        private static Type m_dirtySource;

        public void Awake()
        {
            m_obj = new UVMPublicTransportWorldInfoPanelObject();

            TransportManager.instance.eventLineColorChanged += (x) =>
            {
                if (GetLineID(out ushort lineId, out bool fromBuilding) && x == lineId && !fromBuilding)
                {
                    MarkDirty(null);
                }
            };
            TransportManager.instance.eventLineNameChanged += (x) =>
            {
                if (GetLineID(out ushort lineId, out bool fromBuilding) && x == lineId && !fromBuilding)
                {
                    m_obj.m_nameField.text = Singleton<TransportManager>.instance.GetLineName(x);
                }
            };
        }

        public static bool CheckEnabled() => PluginManager.instance.FindPluginInfo(typeof(TransportLinesManagerMod).Assembly)?.isEnabled ?? false;

        public static void OverrideStart(PublicTransportWorldInfoPanel __instance)
        {
            m_obj.origInstance = __instance;
            __instance.component.width = 800;

            BindComponents(__instance);

            SetNameFieldProperties();

            KlyteMonoUtils.CreateTabsComponent(out m_obj.m_lineConfigTabs, out _, __instance.transform, "LineConfig", new Vector4(15, 45, 365, 30), new Vector4(15, 80, 380, 445));

            m_obj.m_childControls.Add("Default", TabCommons.CreateTabLocalized<UVMMainWIPTab>(m_obj.m_lineConfigTabs, "ThumbStatistics", "K45_TLM_WIP_STATS_TAB", "Default", false));
            m_obj.m_childControls.Add("DefaultRegional", TabCommons.CreateTabLocalized<TLMRegionalMainTab>(m_obj.m_lineConfigTabs, "ThumbStatistics", "K45_TLM_WIP_REGIONAL_TAB", "DefaultRegional", false));
            m_obj.m_childControls.Add("Reports", TabCommons.CreateTabLocalized<TLMReportsTab>(m_obj.m_lineConfigTabs, "IconMessage", "K45_TLM_WIP_REPORT_TAB", "Reports", false));
            m_obj.m_childControls.Add("Budget", TabCommons.CreateTabLocalized<UVMBudgetConfigTab>(m_obj.m_lineConfigTabs, "InfoPanelIconCurrency", "K45_TLM_WIP_BUDGET_CONFIGURATION_TAB", "Budget", false));
            m_obj.m_childControls.Add("Ticket", TabCommons.CreateTabLocalized<TLMTicketConfigTab>(m_obj.m_lineConfigTabs, "FootballTicketIcon", "K45_TLM_WIP_TICKET_CONFIGURATION_TAB", "Ticket", false));
            m_obj.m_childControls.Add("AssetSelection", TabCommons.CreateTabLocalized<TLMAssetSelectorTab>(m_obj.m_lineConfigTabs, "IconPolicyFreePublicTransport", "K45_TLM_WIP_ASSET_SELECTION_TAB", "AssetSelection", false));
            m_obj.m_childControls.Add("DepotSelection", TabCommons.CreateTabLocalized<TLMDepotSelectorTab>(m_obj.m_lineConfigTabs, "UIFilterBigBuildings", "K45_TLM_WIP_DEPOT_SELECTION_TAB", "DepotSelection", false));
            m_obj.m_childControls.Add("PrefixConfig", TabCommons.CreateTabLocalized<TLMPrefixOptionsTab>(m_obj.m_lineConfigTabs, "InfoIconLevel", "K45_TLM_WIP_PREFIX_CONFIG_TAB", "PrefixConfig", false));

            m_obj.m_childControls.Add("StopsPanel", __instance.Find<UIPanel>("StopsPanel").parent.gameObject.AddComponent<UVMTransportLineLinearMap>());
            DestroyNotUsed(__instance);

            m_obj.m_specificConfig = UIHelperExtension.AddCheckboxLocale(__instance.component, "K45_TLM_USE_SPECIFIC_CONFIG", false, (x) =>
            {
                if (GetLineID(out ushort lineId, out bool fromBuilding))
                {
                    if (!fromBuilding)
                    {
                        TLMTransportLineExtension.Instance.SetUseCustomConfig(lineId, x);
                        MarkDirty(typeof(UVMPublicTransportWorldInfoPanel));
                    }
                }
            });
            m_obj.m_specificConfig.relativePosition = new Vector3(10, 530);
            m_obj.m_specificConfig.isInteractive = false;
            KlyteMonoUtils.LimitWidthAndBox(m_obj.m_specificConfig.label, 400);
        }

        private static void BindComponents(PublicTransportWorldInfoPanel __instance)
        {
            //PARENT
            m_obj.m_nameField = __instance.Find<UITextField>("LineName");
            m_obj.m_vehicleType = __instance.Find<UISprite>("VehicleType");
            m_obj.m_vehicleType.size = new Vector2(32, 22);
            m_obj.m_deleteButton = __instance.Find<UIButton>("DeleteLine");
        }

        private static void DestroyNotUsed(PublicTransportWorldInfoPanel __instance)
        {
            FakeDestroy<UIComponent>(__instance, ("ActivityPanel"));
            FakeDestroy<UIButton>(__instance, ("VehicleSelector"));
            FakeDestroy<UILabel>(__instance, ("LabelPassengers"));

            FakeDestroy<UISlider>(__instance, ("SliderModifyVehicleCount"));
            FakeDestroy<UILabel>(__instance, ("VehicleCountPercent"));
            FakeDestroy<UILabel>(__instance, ("VehicleAmount"));
            FakeDestroy<UIPanel>(__instance, ("PanelVehicleCount"));

            FakeDestroy<UISlider>(__instance, ("SliderTicketPrice"));
            FakeDestroy<UILabel>(__instance, ("LabelTicketPrice"));
            FakeDestroy<UIPanel>(__instance, ("TicketPriceSection"));
        }
        
        public static void FakeDestroy<T>(PublicTransportWorldInfoPanel parent, string name) where T : UIComponent
        {
            var comp = parent.Find<T>(name);
            if (comp)
            {
                comp.isVisible = false;
                comp.isEnabled = false;
                comp.isInteractive = false;
            }
            else
            {
                LogUtils.DoWarnLog($"The component {name} doesn't exists in the PublicTransportWorldInfoPanel!");
            }
        }

        public static void FakeDestroy(UIComponent comp)
        {
            if (comp)
            {
                comp.isVisible = false;
                comp.isEnabled = false;
                comp.isInteractive = false;
            }
            else
            {
                LogUtils.DoWarnLog($"The component sent doesn't exists in the PublicTransportWorldInfoPanel! Stacktrace: {Environment.StackTrace}");
            }
        }

        private static void SetNameFieldProperties()
        {
            if (m_obj.m_nameField != null)
            {
                m_obj.m_nameField.maxLength = 100;
                m_obj.m_nameField.eventTextSubmitted += OnRename;
            }
        }

        public static bool OnSetTarget()
        {
            GetLineID(out ushort lineID, out bool fromBuilding);
            if (fromBuilding)
            {
                var lineObj = TransportLinesManagerMod.Controller.BuildingLines[lineID];
                if (lineObj == null)
                {
                    return false;
                }
                m_obj.m_nameField.text = TLMLineUtils.GetLineName(lineID, fromBuilding);
                m_obj.m_nameField.Disable();
                m_obj.m_specificConfig.isVisible = false;
                m_obj.m_deleteButton.isVisible = false;
            }
            else
            {
                if (lineID >= TransportManager.MAX_LINE_COUNT)
                {
                    throw new Exception($"INVALID LINE SET AS TARGET: {lineID}");
                }
                if (lineID != 0)
                {
                    m_obj.m_nameField.text = TLMLineUtils.GetLineName(lineID, fromBuilding);
                    m_obj.m_nameField.Enable();
                    m_obj.m_specificConfig.isVisible = TransportSystemDefinition.FromLineId(lineID, fromBuilding).HasVehicles();
                    m_obj.m_specificConfig.isChecked = TLMTransportLineExtension.Instance.IsUsingCustomConfig(lineID);
                    m_obj.m_cachedLength = Singleton<TransportManager>.instance.m_lines.m_buffer[lineID].m_totalLength;
                    m_obj.m_deleteButton.isVisible = true;
                }
                else
                {
                    m_obj.m_nameField.text = string.Format(Locale.Get("K45_TLM_OUTSIDECONNECTION_LISTNAMETEMPLATE"), GetCurrentTSD().GetTransportName());
                    m_obj.m_nameField.Disable();
                    m_obj.m_specificConfig.isVisible = false;
                    m_obj.m_deleteButton.isVisible = false;
                }
            }

            foreach (KeyValuePair<string, IUVMPTWIPChild> tab in m_obj.m_childControls)
            {
                if (tab.Value.MayBeVisible())
                {
                    m_obj.m_lineConfigTabs.ShowTab(tab.Key);
                    tab.Value.OnSetTarget(m_dirtySource);
                }
                else
                {
                    m_obj.m_lineConfigTabs.HideTab(tab.Key);
                    tab.Value.Hide();
                }
            }
            m_dirty = false;
            m_dirtySource = null;

            if (m_obj.m_lineConfigTabs.selectedIndex == -1 || !(m_obj.m_lineConfigTabs.tabPages.components[m_obj.m_lineConfigTabs.selectedIndex].GetComponent<IUVMPTWIPChild>()?.MayBeVisible() ?? false))
            {
                for (int i = 0; i < m_obj.m_lineConfigTabs.tabCount; i++)
                {
                    if (m_obj.m_lineConfigTabs.tabPages.components[i].GetComponent<IUVMPTWIPChild>()?.MayBeVisible() ?? false)
                    {
                        m_obj.m_lineConfigTabs.selectedIndex = i;
                        break;
                    }
                }
            }

            return false;
        }

        public static void UpdateBindings()
        {
            if (GetLineID(out ushort lineID, out bool fromBuilding))
            {
                if (!fromBuilding)
                {
                    if (lineID < TransportManager.MAX_LINE_COUNT)
                    {
                        if (m_obj.m_cachedLength != Singleton<TransportManager>.instance.m_lines.m_buffer[lineID].m_totalLength || m_dirty)
                        {
                            OnSetTarget();
                        }
                        m_obj.m_vehicleType.spriteName = GetVehicleTypeIcon();

                        foreach (KeyValuePair<string, IUVMPTWIPChild> tab in m_obj.m_childControls)
                        {
                            if (tab.Value.MayBeVisible())
                            {
                                tab.Value.UpdateBindings();
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("INVALID LINE TO UPDATE: " + lineID);
                    }
                }
                else
                {
                    if (m_dirty)
                    {
                        OnSetTarget();
                    }
                    m_obj.m_vehicleType.spriteName = GetVehicleTypeIcon();
                    foreach (KeyValuePair<string, IUVMPTWIPChild> tab in m_obj.m_childControls)
                    {
                        if (tab.Value.MayBeVisible())
                        {
                            tab.Value.UpdateBindings();
                        }
                    }
                }
            }
        }

        public static void MarkDirty(Type source) => SimulationManager.instance.StartCoroutine(MarkDirtyAsync(source));

        private static IEnumerator MarkDirtyAsync(Type source)
        {
            yield return 0;
            m_dirty = true;
            m_dirtySource = source;
            yield break;
        }

        private static void OnRename(UIComponent comp, string text)
        {
            GetLineID(out ushort lineId, out bool fromBuilding);
            if (fromBuilding)
            {
                return;
            }
            if (lineId > 0)
            {
                m_obj.origInstance.StartCoroutine(TLMController.Instance.RenameCoroutine(lineId, text));
            }
        }

        internal static UVMPublicTransportWorldInfoPanelObject.LineType GetLineType(ushort lineID, bool fromBuilding)
        {
            if (!fromBuilding)
            {
                string name = Singleton<TransportManager>.instance.m_lines.m_buffer[lineID].Info.name;
                if (name != null)
                {
                    if (name == "Sightseeing Bus")
                    {
                        return UVMPublicTransportWorldInfoPanelObject.LineType.TouristBus;
                    }
                    if (name == "Pedestrian")
                    {
                        return UVMPublicTransportWorldInfoPanelObject.LineType.WalkingTour;
                    }
                }
            }
            return UVMPublicTransportWorldInfoPanelObject.LineType.Default;
        }

        public static void OnBudgetClicked()
        {
            if (ToolsModifierControl.IsUnlocked(UnlockManager.Feature.Economy))
            {
                ToolsModifierControl.mainToolbar.ShowEconomyPanel(1);
                WorldInfoPanel.Hide<PublicTransportWorldInfoPanel>();
            }
        }

        private static void OnLineNameChanged(ushort id)
        {
            GetLineID(out ushort lineId, out bool fromBuilding);
            if (fromBuilding)
            {
                return;
            }
            if (lineId > 0 && id == lineId)
            {
                m_obj.m_nameField.text = Singleton<TransportManager>.instance.GetLineName(id);
            }
        }

        internal static bool GetLineID(out ushort lineId, out bool fromBuilding)
        {
            if (m_obj.CurrentInstanceID.Type == (InstanceType)TLMInstanceType.TransportSystemDefinition)
            {
                fromBuilding = false;
                lineId = 0;
                return true;
            }
            if (m_obj.CurrentInstanceID.Type == (InstanceType)TLMInstanceType.BuildingLines)
            {
                fromBuilding = true;
                lineId = (ushort)(m_obj.CurrentInstanceID.Index);
                return true;
            }
            fromBuilding = false;
            if (m_obj.CurrentInstanceID.Type == InstanceType.TransportLine)
            {
                lineId = m_obj.CurrentInstanceID.TransportLine;
                return true;
            }
            if (m_obj.CurrentInstanceID.Type == InstanceType.Vehicle)
            {
                ushort firstVehicle = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[m_obj.CurrentInstanceID.Vehicle].GetFirstVehicle(m_obj.CurrentInstanceID.Vehicle);
                if (firstVehicle != 0)
                {
                    lineId = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[firstVehicle].m_transportLine;
                    return true;
                }
            }
            fromBuilding = false;
            lineId = 0xFFFF;
            return false;
        }

        internal static TransportSystemDefinition GetCurrentTSD() => GetLineID(out ushort lineId, out bool fromBuilding) && (lineId > 0 || fromBuilding) ? TransportSystemDefinition.FromLineId(lineId, fromBuilding) : TransportSystemDefinition.FromIndex(m_obj.CurrentInstanceID.Index);

        internal static void ForceReload() => OnSetTarget();

        public static string GetVehicleTypeIcon() => GetCurrentTSD()?.GetTransportTypeIcon();

        public static void OnEnableOverride()
        {
            Singleton<TransportManager>.instance.eventLineNameChanged += OnLineNameChanged;
            foreach (KeyValuePair<string, IUVMPTWIPChild> tab in m_obj.m_childControls)
            {
                tab.Value.OnEnable();
            }
        }

        public static void OnDisableOverride()
        {
            Singleton<TransportManager>.instance.eventLineNameChanged -= OnLineNameChanged;
            foreach (KeyValuePair<string, IUVMPTWIPChild> tab in m_obj.m_childControls)
            {
                tab.Value.OnDisable();
            }
        }

        public static void PreOnGotFocus()
		{
            foreach (KeyValuePair<string, IUVMPTWIPChild> tab in m_obj.m_childControls)
            {
                tab.Value.OnGotFocus();
            }
		}

        public class UVMPublicTransportWorldInfoPanelObject
        {
            public readonly Dictionary<string, IUVMPTWIPChild> m_childControls = new Dictionary<string, IUVMPTWIPChild>();

            internal PublicTransportWorldInfoPanel origInstance = null;

            private Func<PublicTransportWorldInfoPanel, InstanceID> m_getterInstanceId = ReflectionUtils.GetGetFieldDelegate<PublicTransportWorldInfoPanel, InstanceID>("m_InstanceID", typeof(PublicTransportWorldInfoPanel));
            internal InstanceID CurrentInstanceID => origInstance is null ? (default) : m_getterInstanceId(origInstance);

            internal UITextField m_nameField;

            internal UISprite m_vehicleType;

            internal UICheckBox m_specificConfig;

            internal UIButton m_deleteButton;

            internal float m_cachedLength;

            internal enum LineType
            {
                Default,
                TouristBus,
                WalkingTour
            }

            internal UITabstrip m_lineConfigTabs;
        }


    }
}
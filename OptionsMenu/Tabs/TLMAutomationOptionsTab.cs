﻿using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Commons.Extensions.UI;
using TransportLinesManager.Data.DataContainers;
using static Commons.UI.DefaultEditorUILib;

namespace TransportLinesManager.OptionsMenu.Tabs
{
    internal class TLMAutomationOptionsTab : UICustomControl, ITLMConfigOptionsTab
    {
        private UIComponent parent;
        private UICheckBox m_autoColor;
        private UICheckBox m_autoName;
        private UICheckBox m_circular;
        private UICheckBox m_addLineCode;
        private UICheckBox m_expressTrams;
        private UICheckBox m_expressBuses;
        private UICheckBox m_expressTrolleys;
        private UICheckBox m_clockSwap;
        public void ReloadData()
        {
            if (TLMBaseConfigXML.Instance is null)
            {
                return;
            }
            m_autoColor.isChecked = TLMBaseConfigXML.CurrentContextConfig.UseAutoColor;
            m_autoName.isChecked = TLMBaseConfigXML.CurrentContextConfig.UseAutoName;
            m_circular.isChecked = TLMBaseConfigXML.CurrentContextConfig.CircularIfSingleDistrictLine;
            m_addLineCode.isChecked = TLMBaseConfigXML.CurrentContextConfig.AddLineCodeInAutoname;
            m_expressBuses.isChecked = TLMBaseConfigXML.CurrentContextConfig.ExpressBusesEnabled;
            m_expressTrams.isChecked = TLMBaseConfigXML.CurrentContextConfig.ExpressTramsEnabled;
            m_expressTrolleys.isChecked = TLMBaseConfigXML.CurrentContextConfig.ExpressTrolleybusesEnabled;
            m_clockSwap.isChecked = TransportLinesManagerMod.UseGameClockAsReferenceIfNoDayNight;
        }

        private void Awake()
        {
            parent = GetComponentInParent<UIComponent>();
            UIHelperExtension group7 = new UIHelperExtension(parent.GetComponentInChildren<UIScrollablePanel>());
            ((UIScrollablePanel)group7.Self).autoLayoutDirection = LayoutDirection.Horizontal;
            ((UIScrollablePanel)group7.Self).wrapLayout = true;
            ((UIScrollablePanel)group7.Self).width = 730;

            group7.AddLabel(Locale.Get("TLM_AUTOMATION_CONFIG"));
            group7.AddSpace(15);

            AddCheckboxLocale("TLM_AUTO_COLOR_ENABLED", out m_autoColor, group7, (x) => TLMBaseConfigXML.CurrentContextConfig.UseAutoColor = x);
            AddCheckboxLocale("TLM_AUTO_NAME_ENABLED", out m_autoName, group7, (x) => TLMBaseConfigXML.CurrentContextConfig.UseAutoName = x);
            AddCheckboxLocale("TLM_USE_CIRCULAR_AUTO_NAME", out m_circular, group7, (x) => TLMBaseConfigXML.CurrentContextConfig.CircularIfSingleDistrictLine = x);
            AddCheckboxLocale("TLM_ADD_LINE_NUMBER_AUTO_NAME", out m_addLineCode, group7, (x) => TLMBaseConfigXML.CurrentContextConfig.AddLineCodeInAutoname = x);
            group7.AddSpace(15);

            AddCheckboxLocale("TLM_ENABLE_EXPRESS_BUSES", out m_expressBuses, group7, (x) => TLMBaseConfigXML.CurrentContextConfig.ExpressBusesEnabled = x);
            m_expressBuses.tooltipLocaleID = "TLM_ENABLE_EXPRESS_BUSES_DESC";
            AddCheckboxLocale("TLM_ENABLE_EXPRESS_TRAMS", out m_expressTrams, group7, (x) => TLMBaseConfigXML.CurrentContextConfig.ExpressTramsEnabled = x);
            m_expressTrams.tooltipLocaleID = "TLM_ENABLE_EXPRESS_TRAMS_DESC";
            AddCheckboxLocale("TLM_ENABLE_EXPRESS_TROLLEYS", out m_expressTrolleys, group7, (x) => TLMBaseConfigXML.CurrentContextConfig.ExpressTrolleybusesEnabled = x);
            m_expressTrolleys.tooltipLocaleID = "TLM_ENABLE_EXPRESS_TROLLEYS_DESC";
            group7.AddSpace(15);

            AddCheckboxLocale("TLM_USEGAMECLOCKIFDAYNIGHTDISABLED", out m_clockSwap, group7, (x) => TransportLinesManagerMod.UseGameClockAsReferenceIfNoDayNight = x);


        }


    }
}

﻿using ColossalFramework.UI;
using Commons.Extensions.UI;
using Commons.Utils;
using TransportLinesManager.WorldInfoPanels.Components;
using System.Collections.Generic;
using UnityEngine;
using static TransportLinesManager.Data.Managers.TLMTransportLineStatusesManager;
using static TransportLinesManager.WorldInfoPanels.Tabs.TLMReportsTab;

namespace TransportLinesManager.WorldInfoPanels.ReportTabs
{

    internal abstract class BasicReportTab<L, D> : UICustomControl, ITLMReportChild where D : BasicReportData, new() where L : BaseReportLine<D>
    {

        private UIPanel m_bg;
        private readonly L[] m_reportLines = new L[17];
        private L m_aggregateLine;


        #region Overridable

        public void Awake()
        {
            m_bg = component as UIPanel;
            m_bg.autoLayout = true;
            m_bg.autoLayoutDirection = LayoutDirection.Vertical;
            m_bg.clipChildren = true;

            var uiHelper = new UIHelperExtension(m_bg);

            UILabel titleLabel = uiHelper.AddLabel("");
            titleLabel.autoSize = true;
            titleLabel.textAlignment = UIHorizontalAlignment.Center;
            titleLabel.minimumSize = new Vector2(m_bg.width, 0);
            titleLabel.localeID = TitleLocaleID;

            MonoUtils.CreateUIElement(out UIPanel listTitle, m_bg.transform, "LT");
            L titleList = listTitle.gameObject.AddComponent<L>();
            titleList.AsTitle();

            MonoUtils.CreateUIElement(out UIPanel reportLinesContainer, m_bg.transform, "listContainer", new Vector4(0, 0, m_bg.width, m_bg.height - titleLabel.height - listTitle.height - 35));
            reportLinesContainer.autoLayout = true;
            reportLinesContainer.autoLayoutDirection = LayoutDirection.Horizontal;
            MonoUtils.CreateScrollPanel(reportLinesContainer, out UIScrollablePanel reportLines, out _, reportLinesContainer.width - 10, reportLinesContainer.height, Vector3.zero);

            for (int i = 0; i < m_reportLines.Length; i++)
            {
                MonoUtils.CreateUIElement(out UIPanel line, reportLines.transform, $"L{i}");
                m_reportLines[i] = line.gameObject.AddComponent<L>();
            }
            MonoUtils.CreateUIElement(out UIPanel aggregateLine, m_bg.transform, $"L_AGG");
            m_aggregateLine = aggregateLine.gameObject.AddComponent<L>();
        }


        public void OnEnable()
        {
        }

        public void OnDisable()
        { }

        protected abstract List<D> GetReportData(ushort lineId);
        protected abstract void AddToTotalizer(ref D totalizer, D data);
        protected abstract string TitleLocaleID { get; }

        private uint m_lastUpdateFrame;

        public void UpdateBindings(bool showDayTime, bool force)
        {
            if (m_bg.isVisible && (force || m_lastUpdateFrame + 30 < SimulationManager.instance.m_referenceFrameIndex) && UVMPublicTransportWorldInfoPanel.GetLineID(out ushort lineId, out bool fromBuilding) && !fromBuilding && lineId > 0)
            {
                List<D> report = GetReportData(lineId);
                var totalizer = new D();
                for (int i = 0; i < m_reportLines.Length; i++)
                {
                    m_reportLines[i].SetData(report[16 - i], showDayTime, TLMController.IsRealTimeEnabled);
                    if (i > 0)
                    {
                        AddToTotalizer(ref totalizer, report[16 - i]);
                    }
                }
                m_aggregateLine.SetDataTotalizer(totalizer);
                m_lastUpdateFrame = SimulationManager.instance.m_referenceFrameIndex;
            }
        }

        public abstract bool MayBeVisible();
        public void Hide() => m_bg.isVisible = false;

        #endregion


    }
}
﻿using TransportLinesManager.Data.Managers;
using TransportLinesManager.WorldInfoPanels.Components;
using System.Collections.Generic;
using static TransportLinesManager.Data.Managers.TLMTransportLineStatusesManager;

namespace TransportLinesManager.WorldInfoPanels.ReportTabs
{

    internal class TLMLinePassengerStudentTouristsReportTab : BasicReportTab<TLMStudentTouristsReportLine, StudentsTouristsReport>
    {
        protected override string TitleLocaleID { get; } = "TLM_PASSENGERS_LINE_REPORT";
        public override bool MayBeVisible() => true;
        protected override List<StudentsTouristsReport> GetReportData(ushort lineId) => TLMTransportLineStatusesManager.instance.GetLineStudentTouristsTotalReport(lineId);
        protected override void AddToTotalizer(ref StudentsTouristsReport totalizer, StudentsTouristsReport data)
        {
            totalizer.Student += data.Student;
            totalizer.Tourists += data.Tourists;
            totalizer.Total += data.Total;
        }
    }
}
using ColossalFramework.Globalization;
using TransportLinesManager.Data.Base;
using TransportLinesManager.UI;

namespace TransportLinesManager.WorldInfoPanels.Components
{
    public class TLMTicketPriceEditorLine : TLMBaseSliderEditorLine<TLMTicketPriceEditorLine, TicketPriceEntryXml>
    {
        public const string TICKET_PRICE_LINE_TEMPLATE = "TLM_TicketPriceLineTemplate";


        public static void EnsureTemplate() => EnsureTemplate(TICKET_PRICE_LINE_TEMPLATE);
        public override uint GetValueAsInt(ref TransportLine t) => Entry.Value;
        public override string GetValueFormat(ref TransportLine t) => Entry.Value == 0 ? Locale.Get("TLM_DEFAULT_TICKET_VALUE") : (Entry.Value / 100f).ToString(Settings.moneyFormat, LocaleManager.cultureInfo);
        public override void SetValueFromTyping(ref TransportLine t, uint value) => SetValue(value);
    }

}


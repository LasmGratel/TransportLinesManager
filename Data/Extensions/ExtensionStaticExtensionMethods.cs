﻿using Commons.Utils.UtilitiesClasses;
using TransportLinesManager.Data.Base;
using TransportLinesManager.Data.Tsd;
using TransportLinesManager.Interfaces;
using TransportLinesManager.ModShared;
using TransportLinesManager.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TransportLinesManager.Data.Extensions
{
    public static class ExtensionStaticExtensionMethods
    {
        #region Assets List
        public static List<TransportAsset> GetAssetListForLine<T>(this T it, ushort lineId) where T : IAssetSelectorExtension => it.SafeGet(it.LineToIndex(lineId)).AssetTransportList;
        public static void SetAssetListForLine<T>(this T it, ushort lineId, List<string> list) where T : IAssetSelectorExtension => it.SafeGet(it.LineToIndex(lineId)).AssetTransportList = new SimpleXmlList<TransportAsset>();
        public static void AddAssetToLine<T>(this T it, ushort lineId, string assetId) where T : IAssetSelectorExtension
        {
            List<TransportAsset> list = it.GetAssetListForLine(lineId);
            if (list.Any(item => item.name == assetId))
            {
                return;
            }
            var item = new TransportAsset
            {
                name = assetId
            };
            list.Add(item);
        }
        public static void RemoveAssetFromLine<T>(this T it, ushort lineId, string assetId) where T : IAssetSelectorExtension
        {
            List<TransportAsset> list = it.GetAssetListForLine(lineId);
            if (!list.Any(item => item.name == assetId))
            {
                return;
            }
            list.RemoveAll(x => x.name == assetId);
        }
        public static void UseDefaultAssetsAtLine<T>(this T it, ushort lineId) where T : IAssetSelectorExtension => it.GetAssetListForLine(lineId).Clear();
        #endregion

        #region Name
        public static string GetName<T>(this T it, uint prefix) where T : INameableExtension => it.SafeGet(prefix).Name;
        public static void SetName<T>(this T it, uint prefix, string name) where T : INameableExtension => it.SafeGet(prefix).Name = name;
        #endregion

        #region Budget Multiplier
        public static TimeableList<BudgetEntryXml> GetBudgetsMultiplierForLine<T>(this T it, ushort lineId) where T : IBudgetableExtension => it.SafeGet(it.LineToIndex(lineId)).BudgetEntries;
        public static uint GetBudgetMultiplierForHourForLine<T>(this T it, ushort lineId, float hour) where T : IBudgetableExtension
        {
            TimeableList<BudgetEntryXml> budget = it.GetBudgetsMultiplierForLine(lineId);
            Tuple<Tuple<BudgetEntryXml, int>, Tuple<BudgetEntryXml, int>, float> currentBudget = budget.GetAtHour(hour);
            return (uint)Mathf.Lerp(currentBudget.First.First.Value, currentBudget.Second.First.Value, currentBudget.Third);
        }
        public static void SetBudgetMultiplierForLine<T>(this T it, ushort lineId, uint multiplier, int hour) where T : IBudgetableExtension => it.SafeGet(it.LineToIndex(lineId)).BudgetEntries.Add(new BudgetEntryXml()
        {
            Value = multiplier,
            HourOfDay = hour
        });
        public static void RemoveBudgetMultiplierForLine<T>(this T it, ushort lineId, int hour) where T : IBudgetableExtension => it.SafeGet(it.LineToIndex(lineId)).BudgetEntries.RemoveAtHour(hour);
        public static void RemoveAllBudgetMultipliersOfLine<T>(this T it, ushort lineId) where T : IBudgetableExtension => it.SafeGet(it.LineToIndex(lineId)).BudgetEntries = new TimeableList<BudgetEntryXml>()
        {
            new BudgetEntryXml(){HourOfDay=0,Value=100}
        };
        public static void SetAllBudgetMultipliersForLine<T>(this T it, ushort lineId, TimeableList<BudgetEntryXml> newValue) where T : IBudgetableExtension => it.SafeGet(it.LineToIndex(lineId)).BudgetEntries = newValue;
        #endregion
        #region Ticket Price
        public static TimeableList<TicketPriceEntryXml> GetTicketPricesForLine<T>(this T it, ushort lineId) where T : ITicketPriceExtension => it.SafeGet(it.LineToIndex(lineId)).TicketPriceEntries;
        public static void SetTicketPricesForLine<T>(this T it, ushort lineId, TimeableList<TicketPriceEntryXml> newPrices) where T : ITicketPriceExtension => it.SafeGet(it.LineToIndex(lineId)).TicketPriceEntries = newPrices;
        public static void ClearTicketPricesOfLine<T>(this T it, ushort lineId) where T : ITicketPriceExtension => it.SafeGet(it.LineToIndex(lineId)).TicketPriceEntries = new TimeableList<TicketPriceEntryXml>()
        {
            new TicketPriceEntryXml(){HourOfDay=0,Value=0}
        };
        public static Tuple<TicketPriceEntryXml, int> GetTicketPriceForHourForLine<T>(this T it, ushort lineId, float hour) where T : ITicketPriceExtension
        {
            TimeableList<TicketPriceEntryXml> ticketPrices = it.GetTicketPricesForLine(lineId);
            return ticketPrices.GetAtHourExact(hour);
        }
        public static void SetTicketPriceToLine<T>(this T it, ushort lineId, uint multiplier, int hour) where T : ITicketPriceExtension => it.SafeGet(it.LineToIndex(lineId)).TicketPriceEntries.Add(new TicketPriceEntryXml()
        {
            Value = multiplier,
            HourOfDay = hour
        });
        public static void RemoveTicketPriceEntryToLine<T>(this T it, ushort lineId, int hour) where T : ITicketPriceExtension => it.SafeGet(it.LineToIndex(lineId)).TicketPriceEntries.RemoveAtHour(hour);

        #endregion

        #region Color
        public static Color GetColor<T>(this T it, uint prefix) where T : IColorSelectableExtension => it.SafeGet(prefix).Color;

        public static void SetColor<T>(this T it, uint prefix, Color value) where T : IColorSelectableExtension
        {
            if (value.a < 1)
            {
                it.CleanColor(prefix);
            }
            else
            {
                it.SafeGet(prefix).Color = value;
            }
            TLMFacade.Instance?.OnLineSymbolParameterChanged();
        }

        public static void CleanColor<T>(this T it, uint prefix) where T : IColorSelectableExtension => it.SafeGet(prefix).Color = default;
        #endregion

        #region Depot


        private static IDepotSelectionStorage EnsureCreationDepotConfig<T>(T it, uint idx) where T : IDepotSelectableExtension
        {
            IDepotSelectionStorage config = it.SafeGet(idx);
            if (config.DepotsAllowed == null)
            {
                config.DepotsAllowed = new SimpleXmlHashSet<ushort>();
            }

            return config;
        }
        public static void AddDepotForLine<T>(this T it, ushort lineId, ushort buildingID) where T : IDepotSelectableExtension => EnsureCreationDepotConfig(it, it.LineToIndex(lineId)).DepotsAllowed.Add(buildingID);

        public static void RemoveDepotForLine<T>(this T it, ushort lineId, ushort buildingID) where T : IDepotSelectableExtension => EnsureCreationDepotConfig(it, it.LineToIndex(lineId)).DepotsAllowed.Remove(buildingID);

        public static void RemoveAllDepotsForLine<T>(this T it, ushort lineId) where T : IDepotSelectableExtension => EnsureCreationDepotConfig(it, it.LineToIndex(lineId)).DepotsAllowed.Clear();

        public static void AddAllDepots<T>(this T it, uint idx) where T : IDepotSelectableExtension => it.SafeGet(idx).DepotsAllowed = null;
        public static List<ushort> GetAllowedDepots<T>(this T it, TransportSystemDefinition tsd, ushort lineId) where T : IDepotSelectableExtension
        {
            IDepotSelectionStorage data = it.SafeGet(it.LineToIndex(lineId));
            List<ushort> saida = TLMDepotUtils.GetAllDepotsFromCity(tsd);
            if (data.DepotsAllowed == null)
            {
                return saida;
            }
            else
            {
                return data.DepotsAllowed.ToList();
            }
        }

        #endregion

    }
}

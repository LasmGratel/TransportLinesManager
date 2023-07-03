﻿using Commons.Interfaces;
using Commons.Utils.UtilitiesClasses;
using TransportLinesManager.Data.Base.ConfigurationContainers.OutsideConnections;
using TransportLinesManager.Overrides;
using System.Linq;
using System.Xml.Serialization;

namespace TransportLinesManager.Data.Base.ConfigurationContainers
{
    public class TLMBuildingsConfiguration : IIdentifiable
    {
        [XmlAttribute("buildingId")]
        public long? Id { get; set; }

        [XmlAttribute("tlmManagedRegionalLines")]
        public bool TlmManagedRegionalLines { get; set; }

        [XmlElement("platformMapping")]
        public NonSequentialList<PlatformConfig> PlatformMappings { get; set; } = new NonSequentialList<PlatformConfig>();

        public TLMBuildingsConfiguration() => NetManagerOverrides.EventSegmentReleased += OnSegmentReleased;
        ~TLMBuildingsConfiguration() => NetManagerOverrides.EventSegmentReleased -= OnSegmentReleased;

        internal void GetPlatformData(uint laneId, out PlatformConfig dataObj)
        {
            if (!PlatformMappings.TryGetValue(laneId, out dataObj))
            {
                dataObj = PlatformMappings[laneId] = new PlatformConfig();
                dataObj.VehicleLaneId = laneId;
            }
        }

        private void OnSegmentReleased(ushort segmentId)
        {
            if (SimulationManager.exists)
            {
                var laneSegment = PlatformMappings.Values.Where(x => NetManager.instance.m_lanes.m_buffer[x.PlatformLaneId].m_segment == segmentId);
                if (laneSegment.Count() > 0)
                {
                    SimulationManager.instance.AddAction(() =>
                        {
                            foreach (var lane in laneSegment)
                            {
                                lane.ReleaseNodes((ushort)Id);
                                PlatformMappings.Remove(lane.Id ?? 0);
                            }
                        });
                }
            }
        }

        internal void OnToggleTlmRegionalManagement(bool value)
        {
            ref Building building = ref BuildingManager.instance.m_buildings.m_buffer[Id ?? 0];
            if (value != TlmManagedRegionalLines && (building.Info.m_buildingAI is TransportStationAI stationAI))
            {
                TlmManagedRegionalLines = value;
                stationAI.SetEmptying((ushort)(Id ?? 0), ref building, value);
                var nextSubBuilding = building.m_subBuilding;
                while (nextSubBuilding != 0)
                {
                    ref Building subBuilding = ref BuildingManager.instance.m_buildings.m_buffer[nextSubBuilding];
                    stationAI.SetEmptying(nextSubBuilding, ref subBuilding, value);
                    nextSubBuilding = subBuilding.m_subBuilding;
                }
                if (value)
                {
                    foreach (var mapping in PlatformMappings.Values)
                    {
                        mapping.UpdateStationNodes((ushort)Id);
                    }
                }
                else
                {
                    foreach (var mapping in PlatformMappings.Values)
                    {
                        mapping.ReleaseNodes((ushort)Id);
                    }
                    PlatformMappings.Clear();
                }
            }
        }


    }
}

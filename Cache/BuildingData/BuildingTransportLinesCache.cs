﻿using Commons.Utils.UtilitiesClasses;
using TransportLinesManager.Data.Tsd;
using TransportLinesManager.Overrides;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TransportLinesManager.Cache.BuildingData
{
    public class BuildingTransportLinesCache : MonoBehaviour
    {


        private SimpleNonSequentialList<BuildingTransportDataCache> BuildingTransportDataCache;
        private Dictionary<ushort, InnerBuildingLine> InnerBuildingLinesIndex;

        public void InvalidateLinesCache() => InnerBuildingLinesIndex = null;

        private void Awake() => BuildingTransportDataCache = new SimpleNonSequentialList<BuildingTransportDataCache>();

        private void OnEnable()
        {
            BuildingManager.instance.EventBuildingReleased += ResetBuilding;
            BuildingManager.instance.EventBuildingRelocated += ResetBuilding;
            NetManagerOverrides.EventNodeChanged += ResetAllBuilding;
            NetManagerOverrides.EventSegmentChanged += ResetAllBuildingSegment;
        }
        private void OnDisable()
        {
            BuildingManager.instance.EventBuildingReleased -= ResetBuilding;
            BuildingManager.instance.EventBuildingRelocated -= ResetBuilding;
            NetManagerOverrides.EventNodeChanged -= ResetAllBuilding;
            NetManagerOverrides.EventNodeChanged -= ResetAllBuildingSegment;
        }

        internal void ResetBuilding(ushort buildingId)
        {
            if (TLMController.UpdateRegionalLinesFromBuilding(buildingId))
            {
                BuildingTransportDataCache.Remove(buildingId);
                InnerBuildingLinesIndex = null;
            }
        }

        private void ResetAllBuilding(ushort nodeId)
        {
            if (TLMController.UpdateRegionalLinesFromNode(nodeId))
            {
                BuildingTransportDataCache.Clear();
                InnerBuildingLinesIndex = null;
            }
        }
        private void ResetAllBuildingSegment(ushort segmentId)
        {
            var nm = NetManager.instance;
            ref NetSegment seg = ref nm.m_segments.m_buffer[segmentId];
            if (TransportSystemDefinition.FromNetInfo(seg.Info)?.LevelIntercity != null)
            {
                var building = BuildingManager.instance.FindBuilding(seg.m_middlePosition, 200f, ItemClass.Service.PublicTransport, 0, 0, 0);
                if (BuildingTransportDataCache.Remove(building))
                {
                    InnerBuildingLinesIndex = null;
                }
            }
        }

        public void RenderBuildingLines(RenderManager.CameraInfo cameraInfo, ushort buildingId)
        {
            var targetBuildingId = Building.FindParentBuilding(buildingId);
            if (targetBuildingId == 0)
            {
                targetBuildingId = buildingId;
            }

            ref Building b = ref BuildingManager.instance.m_buildings.m_buffer[targetBuildingId];
            var info = b.Info;
            if (info.m_buildingAI is TransportStationAI tsai)
            {
                if (!BuildingTransportDataCache.ContainsKey(targetBuildingId))
                {
                    BuildingTransportDataCache[targetBuildingId] = new BuildingTransportDataCache(targetBuildingId, ref b, tsai);
                }
                BuildingTransportDataCache[targetBuildingId].RenderLines(cameraInfo);
            }
        }

        public void RenderPlatformStops(RenderManager.CameraInfo cameraInfo, ushort buildingId) => SafeGet(buildingId).RenderStopPoints(cameraInfo);

        public BuildingTransportDataCache SafeGet(ushort buildingId)
        {
            ref Building b = ref BuildingManager.instance.m_buildings.m_buffer[buildingId];
            var info = b.Info;

            if (b.m_parentBuilding != 0)
            {
                return SafeGet(Building.FindParentBuilding(buildingId));
            }
            if (info.m_buildingAI is not TransportStationAI tsai)
            {
                return null;
            }
            if (BuildingTransportDataCache.ContainsKey(buildingId))
            {
                return BuildingTransportDataCache[buildingId];
            }
            BuildingTransportDataCache[buildingId] = new BuildingTransportDataCache(buildingId, ref b, tsai);
            InnerBuildingLinesIndex = null;
            return BuildingTransportDataCache[buildingId];
        }

        public InnerBuildingLine this[ushort nodeId]
        {
            get
            {
				if (InnerBuildingLinesIndex is null)
                {
                    InnerBuildingLinesIndex = BuildingTransportDataCache.SelectMany(x => x.Value.RegionalLines).ToDictionary(x => (ushort)x.Key, x => x.Value);
                }
                return InnerBuildingLinesIndex.TryGetValue(nodeId, out InnerBuildingLine result) ? result : null;
            }
        }
    }
}
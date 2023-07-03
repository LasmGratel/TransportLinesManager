﻿using Commons.Utils;
using UnityEngine;

namespace TransportLinesManager.ModShared
{
    internal class BridgeADRFallback : IBridgeADR
    {
        public override bool GetAddressStreetAndNumber(Vector3 sidewalk, Vector3 midPosBuilding, out int number, out string streetName) => SegmentUtils.GetBasicAddressStreetAndNumber(sidewalk, midPosBuilding, out number, out streetName);
        public override bool GetStreetSuffix(Vector3 sidewalk, Vector3 midPosBuilding, out string streetName) => SegmentUtils.GetBasicAddressStreetAndNumber(sidewalk, midPosBuilding, out _, out streetName);
    }
}
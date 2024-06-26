﻿using Commons.UI.SpriteNames;
using UnityEngine;

namespace TransportLinesManager.Data.Tsd
{
    public partial class TransportSystemDefinition
    {
        public static readonly TransportSystemDefinition EVAC_BUS = new TransportSystemDefinition(
                    ItemClass.SubService.None,
            VehicleInfo.VehicleType.Car,
            TransportInfo.TransportType.EvacuationBus,
            ItemClass.Level.Level4,
            new TransferManager.TransferReason[] {
                        TransferManager.TransferReason.EvacuateA,
                        TransferManager.TransferReason.EvacuateB,
                        TransferManager.TransferReason.EvacuateC,
                        TransferManager.TransferReason.EvacuateD,
                        TransferManager.TransferReason.EvacuateVipA,
                        TransferManager.TransferReason.EvacuateVipB,
                        TransferManager.TransferReason.EvacuateVipC,
                        TransferManager.TransferReason.EvacuateVipD
                    },
            new Color32(202, 162, 31, 255),
            50,
            LineIconSpriteNames.CrossIcon,
            false);
    }

}

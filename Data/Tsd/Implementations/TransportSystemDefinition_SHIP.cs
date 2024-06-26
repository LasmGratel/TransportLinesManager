﻿using Commons.UI.SpriteNames;
using UnityEngine;

namespace TransportLinesManager.Data.Tsd
{
    public partial class TransportSystemDefinition
    {
        public static readonly TransportSystemDefinition SHIP = new TransportSystemDefinition(
                    ItemClass.SubService.PublicTransportShip,
            VehicleInfo.VehicleType.Ship,
            TransportInfo.TransportType.Ship,
            ItemClass.Level.Level1,
            new TransferManager.TransferReason[] { TransferManager.TransferReason.PassengerShip },
            new Color32(0xa3, 0xb0, 0, 255),
            100,
            LineIconSpriteNames.DiamondIcon,
            true,
            ItemClass.Level.Level1);
    }

}

﻿using Commons.UI.SpriteNames;
using UnityEngine;

namespace TransportLinesManager.Data.Tsd
{
    public partial class TransportSystemDefinition
    {
        public static readonly TransportSystemDefinition MONORAIL = new TransportSystemDefinition(
                    ItemClass.SubService.PublicTransportMonorail,
            VehicleInfo.VehicleType.Monorail,
            TransportInfo.TransportType.Monorail,
            ItemClass.Level.Level1,
            new TransferManager.TransferReason[] { TransferManager.TransferReason.Monorail },
            new Color32(217, 51, 89, 255),
            180,
            LineIconSpriteNames.RoundedSquareIcon,
            true);
    }
}

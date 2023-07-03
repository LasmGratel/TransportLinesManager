﻿using ColossalFramework.Globalization;

namespace TransportLinesManager.Utils
{
    public enum NamingMode
    {
        Number = 0,
        LatinLower = 1,
        LatinUpper = 2,
        GreekLower = 3,
        GreekUpper = 4,
        CyrillicLower = 5,
        CyrillicUpper = 6,
        None = 7,
        LatinLowerNumber = 8,
        LatinUpperNumber = 9,
        GreekLowerNumber = 10,
        GreekUpperNumber = 11,
        CyrillicLowerNumber = 12,
        CyrillicUpperUpper = 13,
        Roman = 14
    }

    public static class NamingModeExtensions
    {
        public static string GetName(this NamingMode namingMode) => Locale.Get("K45_TLM_MODO_NOMENCLATURA", namingMode.ToString());
    }

}


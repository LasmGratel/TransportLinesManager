﻿using ColossalFramework.UI;
using Klyte.Commons.Extensors;

namespace Klyte.TransportLinesManager.OptionsMenu.Tabs
{
    internal class TLMModInfoTab : UICustomControl
    {

        UIComponent parent;

        private void Awake()
        {
            parent = GetComponentInParent<UIComponent>();
            UIHelperExtension group9 = new UIHelperExtension(parent);
            ((UIPanel)group9.self).wrapLayout = true;
            ((UIPanel)group9.self).width = 730;
            TransportLinesManagerMod.instance.PopulateGroup9(group9);
        }
        
    }
}
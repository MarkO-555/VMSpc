﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.XmlFileManagers;

namespace VMSpc.Panels
{
    class VSimpleGauge : VPanel
    {
        private MainWindow parent;

        public VSimpleGauge(MainWindow parent, PanelSettings panelSettings, PanelManager panelManager) 
            : base(parent, panelSettings, panelManager)
        {
            this.parent = parent;
        }

        protected override void GeneratePanel()
        {

        }
    }
}

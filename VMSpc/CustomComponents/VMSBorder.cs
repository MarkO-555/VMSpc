﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VMSpc.XmlFileManagers;
using VMSpc.DlgWindows;
using VMSpc.Panels;
using VMSpc.CustomComponents;
using VMSpc.DevHelpers;
using System.Timers;
using static VMSpc.Constants;
using static VMSpc.XmlFileManagers.ParamDataManager;
using System.Globalization;

namespace VMSpc.CustomComponents
{
    public class VMSBorder : Border
    {
        public VMSBorder() : base() { }
    }
}

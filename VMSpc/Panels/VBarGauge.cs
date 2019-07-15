﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VMSpc.DevHelpers;
using VMSpc.DlgWindows;
using VMSpc.XmlFileManagers;

namespace VMSpc.Panels
{
    /// <summary> Base class of VSimpleGauge, VScanGauge, and VRoundGauge </summary>
    abstract class VBarGauge : VPanel
    {
        protected Rectangle EmptyBar;
        protected Rectangle FillBar;
        protected TextBlock TitleText;
        protected TextBlock ValueText;

        private static readonly Random getrandom = new Random();

        public VBarGauge(MainWindow mainWindow, PanelSettings panelSettings)
        : base(mainWindow, panelSettings)
        {
            FillBar = new Rectangle
            {
                Stroke = new SolidColorBrush(Colors.Black),
                Fill = new SolidColorBrush(Colors.Green)
            };
            EmptyBar = new Rectangle
            {
                Stroke = new SolidColorBrush(Colors.Black),
                Fill = new SolidColorBrush(Colors.Black)
            };
            TitleText = new TextBlock();
            ValueText = new TextBlock();
            canvas.Children.Add(EmptyBar);
            canvas.Children.Add(FillBar);
            canvas.Children.Add(TitleText);
            canvas.Children.Add(ValueText);

            GeneratePanel();
        }

        protected override void Init()
        {
            base.Init();
        }

        public override void GeneratePanel()
        {
            DrawTitleText();
            DrawValueText();
            DrawBar();
            DrawFillBar();
        }

        protected override abstract VMSDialog GenerateDlg();

        //Move to VPanel?
        protected virtual void DrawTitleText()
        {
            TitleText.Text = "Turbo Boost Pressure - Extended";
            TitleText.Background = new SolidColorBrush(Colors.Blue);
            TitleText.Width = canvas.Width;
            TitleText.Height = canvas.Height / 4;
            TitleText.FontSize = MeasureFontSize(TitleText.Text, TitleText.Width, TitleText.Height); //TODO
            TitleText.VerticalAlignment = VerticalAlignment.Center;
            //canvas.Children.Add(TitleText);
            Canvas.SetTop(TitleText, 0);
            ApplyRightBottomCoords(TitleText);
        }

        /// <summary>
        /// Generates the rectangle for positioning the gauge's value text. This implementation is used by VSimpleGauge and VScanGauge, but is overridden by VRoundGauge
        /// </summary>
        protected virtual void DrawValueText()
        {
            ValueText.Text = "" + border.Width;
            ValueText.Background = new SolidColorBrush(Colors.Yellow);
            ValueText.Width = canvas.Width;
            ValueText.Height = canvas.Height / 4;
            ValueText.FontSize = MeasureFontSize(ValueText.Text, ValueText.Width, ValueText.Height);
            ValueText.FontWeight = FontWeights.Bold;
            //canvas.Children.Add(ValueText);
            Canvas.SetTop(ValueText, Canvas.GetBottom(TitleText));
            ApplyRightBottomCoords(ValueText);
        }

        /// <summary>
        /// Draws the initial empty bar. This implementation is used by VSimpleGauge and VScanGauge, but is overridden by VRoundGauge
        /// </summary>
        protected virtual void DrawBar()
        {
            Canvas.SetTop(EmptyBar, 3 * (canvas.Height / 4));   //Generates a bar that fills the bottom 1/4 of the panel
            EmptyBar.Height = canvas.Height / 4;
            EmptyBar.Width = canvas.Width;
            //canvas.Children.Add(EmptyBar);
        }

        /// <summary>
        /// Generates the bar used for filling the bar with color. This implementation is used by VSimpleGauge and VScanGauge, but is overridden by VRoundGauge
        /// </summary>
        protected virtual void DrawFillBar()
        {
            Canvas.SetTop(FillBar, 3 * (canvas.Height / 4));    //Generates a bar that fills the bottom 1/4 of the panel
            FillBar.Height = canvas.Height / 4;
            //canvas.Children.Add(FillBar);
        }

        /// <summary>
        /// Updates the fill bar with the specified value. This implementation is used by VSimpleGauge and VScanGauge, but is overridden by VRoundGauge
        /// </summary>
        protected virtual void UpdateFillBar(double value)
        {
            FillBar.Width = value;
        }

        /// <summary>
        /// Draws the gauge's value text
        /// </summary>
        protected void UpdateValueText(double value)
        {
            //ValueText.Text = "" + value;
        }

        //implemented in child classes
        public override void UpdatePanel() { }
    }
}

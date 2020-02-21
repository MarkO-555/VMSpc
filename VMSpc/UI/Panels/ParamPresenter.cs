﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using VMSpc.JsonFileManagers;
using static VMSpc.JsonFileManagers.ConfigurationManager;
using static VMSpc.Constants;

namespace VMSpc.Panels
{ 
    public class ParamPresenter
    {
        protected JParameter parameter;
        protected bool useMetric;
        public int textPosition;
        public double lastValue;
        public ushort pid;
        protected string unit, metricUnit;

        public virtual double CurrentValue => ((!useMetric) ? parameter.LastValue : parameter.LastMetricValue);

        public ParamPresenter(ushort pid, PanelSettings panelSettings, string alt_unit = "", string alt_mUnit = "")
        {
            parameter = ConfigManager.ParamData.GetParam(pid);
            unit = (alt_unit.Length > 0) ? parameter.Unit : alt_unit;
            metricUnit = (alt_mUnit.Length > 0) ? parameter.MetricUnit : alt_mUnit;
            this.pid = pid;
            useMetric = panelSettings.showInMetric;
            textPosition = (int)panelSettings.alignment;//TODO
            lastValue = DUB_NODATA;
        }

        /// <summary> Checks whether or not the currentValue is a fresh value. Note that calling this method will update lastValue, immediately rendering the data stale </summary>
        protected bool HasNewValue()
        {
            bool retval = (CurrentValue != lastValue);
            lastValue = CurrentValue;
            return retval;
        }

        /// <summary> Indicates whether or not the value should be used for updating the UI. A valid value is both fresh and sits between gaugeMin and gaugeMax </summary>
        public virtual bool IsValidForUpdate()
        {
            return (HasNewValue() && (CurrentValue >= parameter.GaugeMin));
        }

        protected double ValueToPercent(double value, double min, double max)
        {
            double span = max - min;
            return (value - min) / span;
        }

        protected string FormattedValue()
        {
            if (parameter != null)
                return String.Format(parameter.Format, CurrentValue);
            else
                return CurrentValue.ToString();
        }

        public virtual string ValueAsString => (CurrentValue == DUB_NODATA) ? "No Data" : FormattedValue();


    }
    public class GaugePresenter : ParamPresenter
    {
        
        public bool showValue;
        public bool showUnit;
        public bool showName;
        public bool showAbbreviation;
        public bool showGraph;

        public GaugePresenter(ushort pid, GaugeSettings panelSettings)
            : base(pid, panelSettings)
        {
            parameter = ConfigManager.ParamData.GetParam(pid);
            showName = panelSettings.showName;
            useMetric = panelSettings.showInMetric;
            showUnit = panelSettings.showUnit;
            showValue = panelSettings.showValue;
            showAbbreviation = panelSettings.showAbbreviation;
            textPosition = (int)panelSettings.alignment;
            showGraph = panelSettings.showGraph;
        }

        public string Title => (showAbbreviation) ? parameter.Abbreviation : parameter.ParamName;

        /// <summary> Returns the current value as a percentage of the difference between gaugeMin and gaugeMax. Useful for visual presentation of the value in gauges </summary>
        public double ValueAsPercent => ValueToPercent(CurrentValue, parameter.GaugeMin, parameter.GaugeMax);

        public double GreenMaxAsPercent  =>  ValueToPercent(parameter.HighYellow, parameter.GaugeMin, parameter.GaugeMax);
        public double YellowMaxAsPercent => ValueToPercent(parameter.HighRed, parameter.GaugeMin, parameter.GaugeMax);
        public double RedMaxAsPercent    => ValueToPercent(parameter.GaugeMax, parameter.GaugeMin, parameter.GaugeMax);

        /// <summary> Returns a stringified version of the current value, which conditionally renders the value text (if showValue is true) + the unit text (if showUnit is true) </summary>
        public override string ValueAsString => (
            (CurrentValue == DUB_NODATA && showValue)
            ? "No Data"
            : (
                ((showValue) ? String.Format(parameter.Format, CurrentValue) : "") +
                ((showUnit) ? ((!useMetric) ? parameter.Unit : parameter.MetricUnit) : "")
              )
        );
    }
    /*
    public class OdometerPresenter : ParamPresenter
    {
        private double startValue;
        public bool showCaptions, showUnit;
        OdometerManager manager;
        public OdometerPresenter(ushort pid, OdometerSettings panelSettings, OdometerManager manager, double startValue, string alt_unit = "", string alt_mUnit = "")
            :base(pid, panelSettings, alt_unit, alt_mUnit)
        {
            this.startValue = startValue;
            this.manager = manager;
            showCaptions = panelSettings.showCaptions;
            showUnit = panelSettings.showUnits;
        }

        private double GetOdometerValue()
        {
            if (pid == 511 || pid == 512)
                return parameter.LastValue;
            else
                return parameter.LastValue - startValue;
        }

        public override double CurrentValue => GetOdometerValue();

        public override string ValueAsString => (
            (CurrentValue == DUB_NODATA)
            ? "No Data"
            : (
                (String.Format(parameter.Format, CurrentValue)) +
                ((showUnit) ? ((!useMetric) ? " " + unit : " " + metricUnit) : "")
              )
        );

    }
    */
}
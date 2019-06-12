﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Parsers
{
    public static class SPNWrapper
    {
        /*
        public const ushort fanSpeed = 986;
        public const ushort exhaustTemp3241 = 3241;
        public const ushort exhaustTemp173 = 173;
        public const ushort egrTemp = 412;
        public const ushort egrDiffPressure = 411;
        public const ushort ecuTemp = 1136;
        public const ushort dpfOutletTemp = 3246;
        public const ushort dpfIntakeTemp = 3242;
        public const ushort defTankTemp = 3031;
        public const ushort defTankLevel = 1761;
        public const ushort crankCasePressure = 101;
        public const ushort coolantPressure = 109;
        public const ushort coolantLevel = 111;
        public const ushort clutchSlipPercent = 522;
        public const ushort baroPressure = 108;
        public const ushort ambientTemp = 171;
        public const ushort retarderSwitch = 47;
        public const ushort retarderOilPressure = 119;
        public const ushort retarderOilTemp = 120;
        public const ushort retarderStatus = 121;
        public const ushort percentAcceleratorPosition = 91;
        public const ushort voltage = 168;
        public const ushort cruiseSpeed = 86;
        public const ushort coolantTemp = 110;
        public const ushort engineLoad = 92;
        public const ushort fuelRate = 183;
        public const ushort fuelTemp = 174;
        public const ushort instantMPG = 184;
        public const ushort airInletTemp = 172;
        public const ushort intakeTemp = 105;
        public const ushort oilPSI = 100;
        public const ushort retarderPercent = 122;
        public const ushort oilTemp = 175;
        public const ushort roadSpeed = 84;
        public const ushort cruiseSetStatus = 85;
        public const ushort engineSpeed = 190;
        public const ushort transmissionTemp = 177;
        public const ushort transmissionSpeed = 191;
        public const ushort turboBoost = 102;
        public const ushort rangeSelected = 162;
        public const ushort rangeAttained = 163;
        public const ushort totalMilesCummins = 244;
        public const ushort totalMiles = 245;
        public const ushort engineHours = 247;
        public const ushort totalFuel = 250;
        public const ushort diagnosticsPID = 0xC2;
        public const ushort multipartMessage = 0xC0;
        public const ushort torquePercent = 92;
        public const ushort rpms = 190;
        */


        public static TSPNFlag spn_dpfActiveStatus = new TSPNFlag(1, 2);
        public static TSPNFlag spn_dpfActiveError = new TSPNFlag(1, 2);  // set to 1 when dpf_active is 10b
        public static TSPNFlag spn_dpfInhibitClutch = new TSPNFlag(2, 4);
        public static TSPNFlag spn_dpfInhibitSwitch = new TSPNFlag(2, 2);
        public static TSPNFlag spn_dpfInhibitStatus = new TSPNFlag(2, 0);
        public static TSPNFlag spn_dpfInhibitSpeed = new TSPNFlag(3, 6);
        public static TSPNFlag spn_dpfInhibitOffidle = new TSPNFlag(3, 2);
        public static TSPNFlag spn_dpfInhibitPTO = new TSPNFlag(3, 0);
        public static TSPNFlag spn_dpfInhibitParkbrake = new TSPNFlag(4, 0);
        public static TSPNFlag spn_waitToStart = new TSPNFlag(3, 0);
        public static TSPNFlag spn_engineShutdownApproaching = new TSPNFlag(4, 2);
        public static TSPNFlag spn_engineShutdown = new TSPNFlag(4, 0);
        public static TSPNFlag spn_waterInFuel = new TSPNFlag(0, 0);
        public static TSPNFlag spn_absActive = new TSPNFlag(0, 2);

        public static TSPNBits spn_fanState = new TSPNBits(1, 0, 4);
        public static TSPNBits spn_dpfHighTempLamp = new TSPNBits(6, 2, 3);
        public static TSPNBits spn_dpfLamp = new TSPNBits(0, 0, 3);

        public static TSPNByte spn_cruiseSetSpeed = new TSPNByte(5, 0.006215f * 100, 0.0f, 0.01f * 100, 0.0f);
        public static TSPNByte spn_inletTemp = new TSPNByte(5, 1.8f, -40.0f, 1.0f, -40.0f);
        public static TSPNByte spn_accelPos = new TSPNByte(1, 0.4f, 0.0f, 0.4f, 0.0f);
        public static TSPNByte spn_loadPercent = new TSPNByte(2, 1.0, 0.0, 1.0, 0.0);
        public static TSPNByte spn_fuelPressure = new TSPNByte(0, 0.580151f, 0.0f, 4.0f, 0.0f);
        public static TSPNByte spn_oilLevel = new TSPNByte(2, 0.4f, 0.0f, 0.4f, 0.0f);
        public static TSPNByte spn_oilPressure = new TSPNByte(3, 0.580151f, 0.0f, 4.0f, 0.0f);
        public static TSPNByte spn_defTankLevel = new TSPNByte(0, 0.4f, 0.0f, 0.4f, 0.0f);
        public static TSPNByte spn_defTankTemp = new TSPNByte(1, 1.8f, -40.0f, 1.0f, -40.0f);
        public static TSPNByte spn_1708_RtdrSwtich = new TSPNByte(0, 1.0, 0.0, 1.0, 0.0);
        public static TSPNByte spn_1708_RtdrStatus = new TSPNByte(0, 1.0, 0.0, 1.0, 0.0);
        public static TSPNByte spn_1708_RtdrPressure = new TSPNByte(0, 1.0, 0.0, 1.0, 0.0);
        public static TSPNByte spn_1708_RtdrOilTemp = new TSPNByte(0, 1.0, 0.0, 1.0, 0.0);
        public static TSPNByte spn_baroPressure = new TSPNByte(0, 0.147649901f, 0.0f, 0.5f, 0.0f);
        public static TSPNByte spn_interCoolerTemp = new TSPNByte(6, 1.8f, -40.0f, 1.0f, -40.0f);
        public static TSPNByte spn_fuelLevel = new TSPNByte(1, 0.4f, 0.0f, 0.4f, 0.0f);
        public static TSPNByte spn_torquePercent = new TSPNByte(2, 1.0, -125.0, 1.0, -125.0);
        public static TSPNByte spn_turboBoostPressure = new TSPNByte(1, 0.2900755f, 0.0f, 2.0f, 0.0f);
        public static TSPNByte spn_intakeManifoldTemp = new TSPNByte(2, 1.8f, -40.0f, 1.0f, -40.0f);
        public static TSPNByte spn_airInletPressure = new TSPNByte(3, 0.2900755f, 0.0f, 2.0f, 0.0f);
        public static TSPNByte spn_airFilterDiffPressure = new TSPNByte(4, 0.00725188689f, 0.0f, 0.05f, 0.0f);
        public static TSPNByte spn_retarderPct = new TSPNByte(1, 1.0, -125.0, 1.0, -125.0);
        public static TSPNByte spn_coolantTemp = new TSPNByte(0, 1.8f, -40.0f, 1.0f, -40.0f);
        public static TSPNByte spn_fuelTemp = new TSPNByte(1, 1.8f, -40.0f, 1.0f, -40.0f);
        public static TSPNByte spn_coolantPressure = new TSPNByte(6, 0.2900755f, 0.0f, 2.0f, 0.0f);
        public static TSPNByte spn_coolantLevel = new TSPNByte(7, 0.4f, 0.0f, 0.4f, 0.0f);
        public static TSPNByte spn_clutchSlipPercent = new TSPNByte(3, 0.4f, 0.0f, 0.4f, 0.0f);
        public static TSPNByte spn_fanSpeed = new TSPNByte(0, 0.4f, 0.0f, 0.4f, 0.0f);

        public static TSPNWord spn_exhaustTemp3241 = new TSPNWord(0, 0.05625f, -459.4f, 0.03125f, -273.0f);
        public static TSPNWord spn_dpfIntakeTemp = new TSPNWord(2, 0.05625f, -459.4f, 0.03125f, -273.0f);
        public static TSPNWord spn_dpfOutletTemp = new TSPNWord(2, 0.05625f, -459.4f, 0.03125f, -273.0f);
        public static TSPNWord spn_roadSpeed = new TSPNWord(1, 0.0000242775f * 100 /**cfg_speed_multiplier*/, 0.0f, 0.0000390625f * 100 /**cfg_speed_multiplier*/, 0.0f);  // note *speed_multiplier is x100 SAME THING
        public static TSPNWord spn_ambientTemp = new TSPNWord(3, 0.05625f, -459.4f, 0.03125f, -273.0f);
        public static TSPNWord spn_rpms = new TSPNWord(3, 0.125, 0.0, 0.125, 0.0);
        public static TSPNWord spn_crankCasePressure = new TSPNWord(4, 0.0011331073f, -36.2594344f, 0.0078125f, -250.0f);
        public static TSPNWord spn_injectorRailPressure = new TSPNWord(2, 0.001133107f, 0.0f, 0.0078125f, 0);
        public static TSPNWord spn_oilTemp = new TSPNWord(2, 0.05625f, -459.4f, 0.03125f, -273.0f);
        public static TSPNWord spn_ecuTemp = new TSPNWord(2, 0.05625f, -459.4f, 0.03125f, -273.0f);
        public static TSPNWord spn_egrDiffPressure = new TSPNWord(4, 0.0011331073f, -36.2594344f, 0.0078125f, -250.0f);
        public static TSPNWord spn_egrTemp = new TSPNWord(6, 0.05625f, -459.4f, 0.03125f, -273.0f);
        public static TSPNWord spn_outputShaftSpeed = new TSPNWord(1, 0.125f, 0.0f, 0.125f, 0.0f);
        public static TSPNWord spn_transTemp = new TSPNWord(4, 0.05625f, -459.4f, 0.03125f, -273.0f);
        public static TSPNWord spn_inputShaftSpeed = new TSPNWord(1, 0.125f, 0.0f, 0.125f, 0.0f);
        public static TSPNWord spn_exhaustTemp173 = new TSPNWord(5, 0.05625f, -459.4f, 0.03125f, -273.0f);
        public static TSPNWord spn_instantMPG = new TSPNWord(2, 0.00000046f * 100 * 100, 0.0f, 0.0000001953125f * 100 * 100, 0.0f);
        public static TSPNWord spn_turboSpeed = new TSPNWord(1, 4.0f, 0.0f, 4.0f, 0.0f);
        public static TSPNWord spn_fuelRate = new TSPNWord(0, 1.320860255f / 100, 0.0f, 5.0f / 100, 0.0f);
        public static TSPNWord spn_batteryVolts = new TSPNWord(4, 0.05f, 0.0f, 0.05f, 0.0f);
        public static TSPNWord spn_rollingMPG = new TSPNWord(0, 0.01f, 0.0f, 0.00425143707f, 0.0f);
        public static TSPNWord spn_recentMPG = new TSPNWord(0, 0.01f, 0.0f, 0.00425143707f, 0.0f);
        public static TSPNWord spn_torque = new TSPNWord(0, 1.0f, 0.0f, 1.35581795f, 0.0f);
        public static TSPNWord spn_horsepower = new TSPNWord(0, 1.0f, 0.0f, 0.745699872f, 0.0f);
        public static TSPNWord spn_reciprocalMPG = new TSPNWord(2, 0.00000046f * 100 * 100, 0.0f, 0.0000001953125f * 100 * 100, 0.0f, 235.215f); //235.215 is conversion per gallon
        public static TSPNWord spn_rollingLPK = new TSPNWord(0, 0.01f, 0.0f, 0.00425143707f, 0.0f, 235.215f);
        public static TSPNWord spn_recentLPK = new TSPNWord(0, 0.01f, 0.0f, 0.00425143707f, 0.0f, 235.215f);
        public static TSPNWord spn_acceleration = new TSPNWord(0, 1.0, 0.0, 1.0, 0.0);
        public static TSPNWord spn_braking = new TSPNWord(0, 1.0, 0.0, 1.0, 0.0);
        public static TSPNWord spn_peakAcceleration = new TSPNWord(0, 1.0, 0.0, 1.0, 0.0);
        public static TSPNWord spn_transMode = new TSPNWord(0, 1.0, 0.0, 1.0, 0.0);

        public static TSPNUint spn_hours = new TSPNUint(0, 0.05f, 0.0f, 0.05f, 0.0f);
        public static TSPNUint spn_idleFuel = new TSPNUint(0, 0.132086f, 0.0f, 0.5f, 0.0f);
        public static TSPNUint spn_idleHours = new TSPNUint(4, 0.05f, 0.0f, 0.05f, 0.0f);
        public static TSPNUint spn_fuel = new TSPNUint(4, 0.132086f, 0.0f, 0.5f, 0.0f);
        public static TSPNUint spn_odometer = new TSPNUint(0, 0.003106856f, 0.0f, 0.005f, 0);
        public static TSPNUint spn_odometerALT = new TSPNUint(0, 0.003106856f, 0.0f, 0.005f, 0);


        static SPNWrapper() { }
 //       public static SPNWrapper SPNManager { get; set; } = new SPNWrapper();
 //       public SPNWrapper() { }

        public static void InitializeSPNs()
        {

        }
    }
}
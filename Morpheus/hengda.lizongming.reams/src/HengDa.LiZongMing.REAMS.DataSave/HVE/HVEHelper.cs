using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.CtrlServices;
using HengDa.LiZongMing.REAMS.Devices;
using HengDa.LiZongMing.REAMS.HVE.Dtos;
using Microsoft.Extensions.Logging;
using Model.Commands;
using Model.Configurations;
using Model.Events;
using Model.Measurements;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HengDa.LiZongMing.REAMS
{
    public static class HVEHelper
    {


        public static HveRunStatusDto SetAllConfigurationsResponse(ref HveRunStatusDto m, GetAllConfigurationsResponse rsp)
        {
            return SetAllConfigurationsResponse(ref m, rsp.ConfigurationList);
        }
        public static HveRunStatusDto SetAllConfigurationsResponse(ref HveRunStatusDto m, List<Configuration> list)
        {
            if (m == null)
                m = new HveRunStatusDto();
            for (int i = 0; i < list.Count; i++)
            {
                var c = list[i];
                if (c is DoseRateMeasurementConfiguration dConf)
                {
                    CtrlHVEService.DoseRateUnit = dConf.UnitString;
                }
                //else if (c is ScalarMeasurementConfiguration sConf)
                //{
                //}
                //else if (c is CompositeMeasurementConfiguration cmConf)
                //{
                //}
                //else if (c is EventConfiguration eConf)
                //{
                //}
                else if (c is AlarmEventConfiguration aeConf)
                {
                    switch (c.Name)
                    {
                        case "DoseRateHighAlarm":
                            m.DoseRateHighAlarmValue = Convert.ToDecimal(aeConf.AlarmValue);
                            break;
                        case "DoseRateLowAlarm":
                            m.DoseRateLowAlarmValue = Convert.ToDecimal(aeConf.AlarmValue);
                            break;
                        case "HighVoltageHigh":
                            m.HighVoltageHighValue = Convert.ToDecimal(aeConf.AlarmValue);
                            break;
                        case "HighVoltageLow":
                            m.HighVoltageLowValue = Convert.ToDecimal(aeConf.AlarmValue);
                            break;
                        case "BatteryVoltageHigh":
                            m.BatteryVoltageHighValue = Convert.ToDecimal(aeConf.AlarmValue);
                            break;
                        case "BatteryVoltageLow":
                            m.BatteryVoltageLowValue = Convert.ToDecimal(aeConf.AlarmValue);
                            break;

                    }
                    //conf
                }
                //else if (c is ExternalDisplayConfiguration edConf)
                //{

                //    //conf
                //}
                //else if (c is WeatherStationConfiguration wsConf)
                //{

                //    //conf
                //}
                //else if (c is SerialPortConfiguration spConf)
                //{

                //    //conf
                //}
                else
                {
                    //Logger.LogDebug($"没有使用{c.Name}");
                }





            }
            return m;
        }
        private static readonly ILogger Logger = new Microsoft.Extensions.Logging.LoggerFactory().CreateLogger("Helper");

        internal static void SetScalarMeasurementsResponse(ref HveRecordDto r, ref HveExtRecordDto r2, List<ScalarMeasurement> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                SetScalarMeasurementsResponse(ref r, ref r2, list[i]);
            }
        }
        internal static void SetScalarMeasurementsResponse(ref HveRecordDto r, ref HveExtRecordDto r2, ScalarMeasurement c)
        {
            //list
            if (r == null)
                r = new HveRecordDto();
            if (r2 == null)
                r2 = new HveExtRecordDto();



            switch (c.Name)
            {
                case "DoseRate":
                    r.DoseRate = Convert.ToDecimal(c.Value);
                    //r.DoseRateStdDev = Convert.ToDecimal(c.StdDev);
                    break;
                case "ElectrometerTemperature":
                    r.ElectrometerTemperature = Convert.ToDecimal(c.Value);
                    break;
                case "BatteryVoltage":
                    r.BatteryVoltage = Convert.ToDecimal(c.Value);
                    break;
                case "HighVoltage":
                    r.HighVoltage = Convert.ToDecimal(c.Value);
                    break;


                case "BatteryCurrent":  //好象不到这
                    r2.BatteryCurrent = Convert.ToDecimal(c.Value);
                    break;
                case "BatteryTemperature":
                    r2.BatteryTemperature = Convert.ToDecimal(c.Value);
                    break;
                case "ElectrometerHumidity":
                    r2.ElectrometerHumidity = Convert.ToDecimal(c.Value);
                    break;
                case "FullChargeBatteryCapacity":
                    r2.FullChargeBatteryCapacity = Convert.ToDecimal(c.Value);
                    break;
                case "IntegratorTemperature":
                    r2.IntegratorTemperature = Convert.ToDecimal(c.Value);
                    break;
                case "PercentBatteryCapacity":
                    r2.PercentBatteryCapacity = Convert.ToDecimal(c.Value);
                    break;
                case "RemainingBatteryCapacity":
                    r2.RemainingBatteryCapacity = Convert.ToDecimal(c.Value);
                    break;
                case "Uptime":
                    r2.Uptime = Convert.ToDecimal(c.Value);
                    break;

                default:
                    Logger.LogDebug($"没有使用测量值{c.Name}");
                    break;
            }

            //Logger.LogDebug($"没有使用测量值{c.Name}");


        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="r2"></param>
        /// <param name="c"></param>
        internal static bool SetSentinelStateResponse(ref HveRunStatusDto r, SentinelState c)
        {
            //list
            if (r == null)
                r = new HveRunStatusDto();

            bool IsAlarm = false;

            switch (c.Name)
            {

                case "BatteryVoltageLow":
                    IsAlarm = (c.CurrentState != "NoAlarm");
                    r.BatteryVoltageLow = IsAlarm;
                    break;
                case "BatteryCapacityAlarm":
                    IsAlarm = (c.CurrentState != "NoAlarm");
                    r.BatteryCapacityAlarm = IsAlarm;
                    break;
                case "DatabaseSizeWarning":
                    IsAlarm = (c.CurrentState != "NoAlarm");
                    r.DatabaseSizeWarning = IsAlarm;
                    break;
                case "DatabaseStatus":
                    //IsAlarm = (c.CurrentState != "NoAlarm");
                    r.DatabaseStatus = c.CurrentState;
                    break;
                case "DoseRateHighAlarm":
                    IsAlarm = (c.CurrentState != "NoAlarm");
                    r.DoseRateHighAlarm = IsAlarm;
                    break;
                case "DoseRateLowAlarm":
                    IsAlarm = (c.CurrentState != "NoAlarm");
                    r.DoseRateLowAlarm = IsAlarm;
                    break;
                case "ExternalPower":
                    IsAlarm = (c.CurrentState != "ExternalPowerOn");
                    r.ExternalPower = IsAlarm;
                    //r.DoseRateStdDev = Convert.ToDecimal(c.StdDev);
                    break;
                case "HighBatteryDischarge":
                    IsAlarm = (c.CurrentState != "NoAlarm");
                    r.HighBatteryDischarge = IsAlarm;
                    break;
                case "HighBatteryTemperature":
                    IsAlarm = (c.CurrentState != "NoAlarm");
                    r.HighBatteryTemperature = IsAlarm;
                    break;
                case "HighChargingCurrent":
                    IsAlarm = (c.CurrentState != "NoAlarm");
                    r.HighChargingCurrent = IsAlarm;
                    break;
                case "HighVoltageHigh":
                    IsAlarm = (c.CurrentState != "NoAlarm");
                    r.HighVoltageHigh = IsAlarm;
                    break;
                case "HighVoltageLow":
                    IsAlarm = (c.CurrentState != "NoAlarm");
                    r.HighVoltageLow = IsAlarm;
                    break;
                case "SystemStatus":
                    //IsAlarm =;
                    r.SystemStatus = c.CurrentState;
                    break;
                default:
                    Logger.LogDebug($"没有使用事件名{c.Name}");
                    break;
            }

            //Logger.LogDebug($"没有使用测量值{c.Name}");
            if (IsAlarm)
            {
                r.AlarmUpdateTime = c.Time;
            }
            else
            {
                r.RunUpdateTime = c.Time;
            }
            return IsAlarm;
        }
    }
}

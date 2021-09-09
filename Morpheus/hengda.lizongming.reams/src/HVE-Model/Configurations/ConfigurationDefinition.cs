using System;
using System.Collections.Generic;

namespace Model.Configurations
{
	// Token: 0x02000055 RID: 85
	public class ConfigurationDefinition
	{
		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060001EB RID: 491 RVA: 0x000043D2 File Offset: 0x000025D2
		// (set) Token: 0x060001EC RID: 492 RVA: 0x000043DA File Offset: 0x000025DA
		public string Name { get; set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060001ED RID: 493 RVA: 0x000043E3 File Offset: 0x000025E3
		// (set) Token: 0x060001EE RID: 494 RVA: 0x000043EB File Offset: 0x000025EB
		public string ConfigType { get; set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060001EF RID: 495 RVA: 0x000043F4 File Offset: 0x000025F4
		// (set) Token: 0x060001F0 RID: 496 RVA: 0x000043FC File Offset: 0x000025FC
		public int ConfigId { get; set; }

		// Token: 0x060001F1 RID: 497 RVA: 0x00004408 File Offset: 0x00002608
		public ConfigurationDefinition(string name, string type, int configId)
		{
			this.Name = name;
			this.ConfigType = type;
			this.ConfigId = configId;
			if (!ConfigurationDefinition.IdToNameDictionary.ContainsKey(configId))
			{
				ConfigurationDefinition.IdToNameDictionary.Add(configId, name);
			}
			if (!ConfigurationDefinition.NameToIdDictionary.ContainsKey(name))
			{
				ConfigurationDefinition.NameToIdDictionary.Add(name, configId);
			}
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00004462 File Offset: 0x00002662
		public static string GetNameFromId(int configId)
		{
			if (ConfigurationDefinition.IdToNameDictionary.ContainsKey(configId))
			{
				return ConfigurationDefinition.IdToNameDictionary[configId];
			}
			return "";
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x00004482 File Offset: 0x00002682
		public static int GetIdFromName(string name)
		{
			if (ConfigurationDefinition.NameToIdDictionary.ContainsKey(name))
			{
				return ConfigurationDefinition.NameToIdDictionary[name];
			}
			return -1;
		}

		// Token: 0x0400010D RID: 269
		public const string ScalarMeasurementType = "ScalarMeasurement";

		// Token: 0x0400010E RID: 270
		public const string CompositeMeasurementType = "CompositeMeasurement";

		// Token: 0x0400010F RID: 271
		public const string EventType = "Event";

		// Token: 0x04000110 RID: 272
		public const string AlarmEventType = "AlarmEvent";

		// Token: 0x04000111 RID: 273
		public const string SystemType = "System";

		// Token: 0x04000112 RID: 274
		public const string SerialType = "Serial";

		// Token: 0x04000113 RID: 275
		public const string WaveformCompositeType = "WaveformMeasurement";

		// Token: 0x04000114 RID: 276
		public const string BatteryCompositeType = "BatteryMeasurement";

		// Token: 0x04000115 RID: 277
		public const string PerformanceCompositeType = "PerformanceMeasurement";

		// Token: 0x04000116 RID: 278
		public const string MeteorologyCompositeType = "MeteorologyMeasurement";

		// Token: 0x04000117 RID: 279
		private static readonly Dictionary<int, string> IdToNameDictionary = new Dictionary<int, string>();

		// Token: 0x04000118 RID: 280
		private static readonly Dictionary<string, int> NameToIdDictionary = new Dictionary<string, int>();

		// Token: 0x04000119 RID: 281
		public static ConfigurationDefinition Uptime = new ConfigurationDefinition("Uptime", "ScalarMeasurement", 1);

		// Token: 0x0400011A RID: 282
		public static ConfigurationDefinition DoseRate = new ConfigurationDefinition("DoseRate", "ScalarMeasurement", 2);

		// Token: 0x0400011B RID: 283
		public static ConfigurationDefinition DoseRateStdDevPercent = new ConfigurationDefinition("DoseRateStdDevPercent", "ScalarMeasurement", 3);

		// Token: 0x0400011C RID: 284
		public static ConfigurationDefinition BatteryVoltage = new ConfigurationDefinition("BatteryVoltage", "ScalarMeasurement", 4);

		// Token: 0x0400011D RID: 285
		public static ConfigurationDefinition BatteryCurrent = new ConfigurationDefinition("BatteryCurrent", "ScalarMeasurement", 5);

		// Token: 0x0400011E RID: 286
		public static ConfigurationDefinition BatteryTemperature = new ConfigurationDefinition("BatteryTemperature", "ScalarMeasurement", 6);

		// Token: 0x0400011F RID: 287
		public static ConfigurationDefinition ElectrometerX1Slope = new ConfigurationDefinition("ElectrometerX1Slope", "ScalarMeasurement", 7);

		// Token: 0x04000120 RID: 288
		public static ConfigurationDefinition ElectrometerX10Slope = new ConfigurationDefinition("ElectrometerX10Slope", "ScalarMeasurement", 8);

		// Token: 0x04000121 RID: 289
		public static ConfigurationDefinition DaqTemperature = new ConfigurationDefinition("DaqTemperature", "ScalarMeasurement", 9);

		// Token: 0x04000122 RID: 290
		public static ConfigurationDefinition DaqHumidity = new ConfigurationDefinition("DaqHumidity", "ScalarMeasurement", 10);

		// Token: 0x04000123 RID: 291
		public static ConfigurationDefinition DAQAirPressure = new ConfigurationDefinition("DAQAirPressure", "ScalarMeasurement", 11);

		// Token: 0x04000124 RID: 292
		public static ConfigurationDefinition HighVoltage = new ConfigurationDefinition("HighVoltage", "ScalarMeasurement", 12);

		// Token: 0x04000125 RID: 293
		public static ConfigurationDefinition HighVoltageStdDev = new ConfigurationDefinition("HighVoltageStdDev", "ScalarMeasurement", 13);

		// Token: 0x04000126 RID: 294
		public static ConfigurationDefinition ElectrometerCurrentUnfiltered = new ConfigurationDefinition("ElectrometerCurrentUnfiltered", "ScalarMeasurement", 14);

		// Token: 0x04000127 RID: 295
		public static ConfigurationDefinition ElectrometerCurrentPercentChange = new ConfigurationDefinition("ElectrometerCurrentPercentChange", "ScalarMeasurement", 15);

		// Token: 0x04000128 RID: 296
		public static ConfigurationDefinition ElectrometerCurrent = new ConfigurationDefinition("ElectrometerCurrent", "ScalarMeasurement", 16);

		// Token: 0x04000129 RID: 297
		public static ConfigurationDefinition GammaCurrent = new ConfigurationDefinition("GammaCurrent", "ScalarMeasurement", 17);

		// Token: 0x0400012A RID: 298
		public static ConfigurationDefinition GammaCurrentCorrected = new ConfigurationDefinition("GammaCurrentCorrected", "ScalarMeasurement", 18);

		// Token: 0x0400012B RID: 299
		public static ConfigurationDefinition IntegratorTemperature = new ConfigurationDefinition("IntegratorTemperature", "ScalarMeasurement", 19);

		// Token: 0x0400012C RID: 300
		public static ConfigurationDefinition ElectrometerTemperature = new ConfigurationDefinition("ElectrometerTemperature", "ScalarMeasurement", 20);

		// Token: 0x0400012D RID: 301
		public static ConfigurationDefinition ElectrometerHumidity = new ConfigurationDefinition("ElectrometerHumidity", "ScalarMeasurement", 21);

		// Token: 0x0400012E RID: 302
		public static ConfigurationDefinition WindSpeed = new ConfigurationDefinition("WindSpeed", "ScalarMeasurement", 22);

		// Token: 0x0400012F RID: 303
		public static ConfigurationDefinition WindDirection = new ConfigurationDefinition("WindDirection", "ScalarMeasurement", 23);

		// Token: 0x04000130 RID: 304
		public static ConfigurationDefinition RainAccumulator = new ConfigurationDefinition("RainAccumulation", "ScalarMeasurement", 24);

		// Token: 0x04000131 RID: 305
		public static ConfigurationDefinition AirPressure = new ConfigurationDefinition("AirPressure", "ScalarMeasurement", 25);

		// Token: 0x04000132 RID: 306
		public static ConfigurationDefinition AirTemperature = new ConfigurationDefinition("AirTemperature", "ScalarMeasurement", 26);

		// Token: 0x04000133 RID: 307
		public static ConfigurationDefinition Humidity = new ConfigurationDefinition("Humidity", "ScalarMeasurement", 27);

		// Token: 0x04000134 RID: 308
		public static ConfigurationDefinition CycleCount = new ConfigurationDefinition("PowerCycleCount", "ScalarMeasurement", 28);

		// Token: 0x04000135 RID: 309
		public static ConfigurationDefinition MinutesToFullCharge = new ConfigurationDefinition("MinutesTillFullCharge", "ScalarMeasurement", 29);

		// Token: 0x04000136 RID: 310
		public static ConfigurationDefinition MinutesToEmpty = new ConfigurationDefinition("MinutesTillEmpty", "ScalarMeasurement", 30);

		// Token: 0x04000137 RID: 311
		public static ConfigurationDefinition BatteryRemainingCapacity = new ConfigurationDefinition("RemainingBatteryCapacity", "ScalarMeasurement", 31);

		// Token: 0x04000138 RID: 312
		public static ConfigurationDefinition BatteryFullChargeCapacity = new ConfigurationDefinition("FullChargeBatteryCapacity", "ScalarMeasurement", 32);

		// Token: 0x04000139 RID: 313
		public static ConfigurationDefinition PercentBatteryCapacity = new ConfigurationDefinition("PercentBatteryCapacity", "ScalarMeasurement", 33);

		// Token: 0x0400013A RID: 314
		public static ConfigurationDefinition DatabaseFileSize = new ConfigurationDefinition("DatabaseFileSize", "ScalarMeasurement", 34);

		// Token: 0x0400013B RID: 315
		public static ConfigurationDefinition DatabaseRecordCount = new ConfigurationDefinition("DatabaseRecordCount", "ScalarMeasurement", 35);

		// Token: 0x0400013C RID: 316
		public static ConfigurationDefinition WaveformCorrectionCount = new ConfigurationDefinition("WaveformCorrectionCount", "ScalarMeasurement", 36);

		// Token: 0x0400013D RID: 317
		public static ConfigurationDefinition X1Waveform = new ConfigurationDefinition("X1Waveform", "CompositeMeasurement", 100);

		// Token: 0x0400013E RID: 318
		public static ConfigurationDefinition X10Waveform = new ConfigurationDefinition("X10Waveform", "CompositeMeasurement", 101);

		// Token: 0x0400013F RID: 319
		public static ConfigurationDefinition BatteryStatus = new ConfigurationDefinition("BatteryStatus", "CompositeMeasurement", 102);

		// Token: 0x04000140 RID: 320
		public static ConfigurationDefinition MetStation = new ConfigurationDefinition("MetStation", "CompositeMeasurement", 103);

		// Token: 0x04000141 RID: 321
		public static ConfigurationDefinition ElectrometerUpdateInterval = new ConfigurationDefinition("ElectrometerUpdateInterval", "CompositeMeasurement", 104);

		// Token: 0x04000142 RID: 322
		public static ConfigurationDefinition NotifyThreadExecutionTime = new ConfigurationDefinition("NotifyThreadExecutionTime", "CompositeMeasurement", 105);

		// Token: 0x04000143 RID: 323
		public static ConfigurationDefinition IntegrationTime = new ConfigurationDefinition("ElectrometerIntegrationTime", "CompositeMeasurement", 106);

		// Token: 0x04000144 RID: 324
		public static ConfigurationDefinition AcquisitionThreadTickInterval = new ConfigurationDefinition("AcquisitionThreadTick", "CompositeMeasurement", 107);

		// Token: 0x04000145 RID: 325
		public static ConfigurationDefinition AcquisitionThreadExecutionTime = new ConfigurationDefinition("AcquisitionThreadExecutionTime", "CompositeMeasurement", 108);

		// Token: 0x04000146 RID: 326
		public static ConfigurationDefinition LEDThreadExecutionTime = new ConfigurationDefinition("LEDThreadExecutionTime", "CompositeMeasurement", 109);

		// Token: 0x04000147 RID: 327
		public static ConfigurationDefinition DataLogInsertQueueCount = new ConfigurationDefinition("DataLogInsertQueueCount", "CompositeMeasurement", 110);

		// Token: 0x04000148 RID: 328
		public static ConfigurationDefinition PingReturnTime = new ConfigurationDefinition("PingReturnTime", "CompositeMeasurement", 111);

		// Token: 0x04000149 RID: 329
		public static ConfigurationDefinition DeleteRecordsTime = new ConfigurationDefinition("DeleteRecordsTime", "CompositeMeasurement", 112);

		// Token: 0x0400014A RID: 330
		public static ConfigurationDefinition ElectrometerRange = new ConfigurationDefinition("Range", "Event", 200);

		// Token: 0x0400014B RID: 331
		public static ConfigurationDefinition ElectrometerGain = new ConfigurationDefinition("ElectrometerGain", "Event", 201);

		// Token: 0x0400014C RID: 332
		public static ConfigurationDefinition ElectrometerRangeMode = new ConfigurationDefinition("ElectrometerRangeMode", "Event", 202);

		// Token: 0x0400014D RID: 333
		public static ConfigurationDefinition ElectrometerUpdate = new ConfigurationDefinition("ElectrometerUpdate", "Event", 203);

		// Token: 0x0400014E RID: 334
		public static ConfigurationDefinition ElectrometerEepromStatus = new ConfigurationDefinition("ElectrometerEepromStatus", "Event", 204);

		// Token: 0x0400014F RID: 335
		public static ConfigurationDefinition SaveConfiguration = new ConfigurationDefinition("SaveConfiguration", "Event", 206);

		// Token: 0x04000150 RID: 336
		public static ConfigurationDefinition ExternalPower = new ConfigurationDefinition("ExternalPower", "Event", 207);

		// Token: 0x04000151 RID: 337
		public static ConfigurationDefinition BatteryPresent = new ConfigurationDefinition("BatteryPresent", "Event", 208);

		// Token: 0x04000152 RID: 338
		public static ConfigurationDefinition SystemStatus = new ConfigurationDefinition("SystemStatus", "Event", 209);

		// Token: 0x04000153 RID: 339
		public static ConfigurationDefinition DatabaseStatus = new ConfigurationDefinition("DatabaseStatus", "Event", 210);

		// Token: 0x04000154 RID: 340
		public static ConfigurationDefinition OperatingMode = new ConfigurationDefinition("OperatingMode", "Event", 211);

		// Token: 0x04000155 RID: 341
		public static ConfigurationDefinition SystemStartup = new ConfigurationDefinition("SystemStartup", "Event", 212);

		// Token: 0x04000156 RID: 342
		public static ConfigurationDefinition NetworkStatus = new ConfigurationDefinition("NetworkStatus", "Event", 213);

		// Token: 0x04000157 RID: 343
		public static ConfigurationDefinition DateTimeSet = new ConfigurationDefinition("Date/Time Set", "Event", 214);

		// Token: 0x04000158 RID: 344
		public static ConfigurationDefinition ConfigurationChange = new ConfigurationDefinition("ConfigurationChange", "Event", 215);

		// Token: 0x04000159 RID: 345
		public static ConfigurationDefinition HPICConfigurationChange = new ConfigurationDefinition("HPICConfigurationChange", "Event", 216);

		// Token: 0x0400015A RID: 346
		public static ConfigurationDefinition ElectrometerConfigurationChange = new ConfigurationDefinition("ElectrometerConfigurationChange", "Event", 217);

		// Token: 0x0400015B RID: 347
		public static ConfigurationDefinition MetStationStatus = new ConfigurationDefinition("MetStationStatus", "Event", 218);

		// Token: 0x0400015C RID: 348
		public static ConfigurationDefinition DoseRateHighAlarm = new ConfigurationDefinition("DoseRateHighAlarm", "AlarmEvent", 300);

		// Token: 0x0400015D RID: 349
		public static ConfigurationDefinition DoseRateLowAlarm = new ConfigurationDefinition("DoseRateLowAlarm", "AlarmEvent", 301);

		// Token: 0x0400015E RID: 350
		public static ConfigurationDefinition ElectrometerCurrentChangeAlarm = new ConfigurationDefinition("ElectrometerChangeAlarm", "AlarmEvent", 303);

		// Token: 0x0400015F RID: 351
		public static ConfigurationDefinition DoseRateVariationAlarm = new ConfigurationDefinition("DoseRateVariationAlarm", "AlarmEvent", 304);

		// Token: 0x04000160 RID: 352
		public static ConfigurationDefinition HighVoltageHigh = new ConfigurationDefinition("HighVoltageHigh", "AlarmEvent", 305);

		// Token: 0x04000161 RID: 353
		public static ConfigurationDefinition HighVoltageLow = new ConfigurationDefinition("HighVoltageLow", "AlarmEvent", 306);

		// Token: 0x04000162 RID: 354
		public static ConfigurationDefinition HighBattery = new ConfigurationDefinition("BatteryVoltageHigh", "AlarmEvent", 307);

		// Token: 0x04000163 RID: 355
		public static ConfigurationDefinition LowBattery = new ConfigurationDefinition("BatteryVoltageLow", "AlarmEvent", 308);

		// Token: 0x04000164 RID: 356
		public static ConfigurationDefinition HighDischargeCurrent = new ConfigurationDefinition("HighBatteryDischarge", "AlarmEvent", 309);

		// Token: 0x04000165 RID: 357
		public static ConfigurationDefinition HighChargeCurrent = new ConfigurationDefinition("HighChargingCurrent", "AlarmEvent", 310);

		// Token: 0x04000166 RID: 358
		public static ConfigurationDefinition BatteryTemperatureAlarm = new ConfigurationDefinition("HighBatteryTemperature", "AlarmEvent", 311);

		// Token: 0x04000167 RID: 359
		public static ConfigurationDefinition BatteryCapacityAlarm = new ConfigurationDefinition("BatteryCapacityAlarm", "AlarmEvent", 312);

		// Token: 0x04000168 RID: 360
		public static ConfigurationDefinition DatabaseSizeWarning = new ConfigurationDefinition("DatabaseSizeWarning", "AlarmEvent", 313);

		// Token: 0x04000169 RID: 361
		public static ConfigurationDefinition RSDetectionConfiguration = new ConfigurationDefinition("RSDetectionConfiguration", "System", 400);

		// Token: 0x0400016A RID: 362
		public static ConfigurationDefinition DisplayConfiguration = new ConfigurationDefinition("DisplayConfiguration", "System", 401);

		// Token: 0x0400016B RID: 363
		public static ConfigurationDefinition WeatherStationConfiguration = new ConfigurationDefinition("WeatherStationConfiguration", "System", 402);

		// Token: 0x0400016C RID: 364
		public static ConfigurationDefinition COM2Configuration = new ConfigurationDefinition("COM2:", "Serial", 500);

		// Token: 0x0400016D RID: 365
		public static ConfigurationDefinition COM3Configuration = new ConfigurationDefinition("COM3:", "Serial", 501);

		// Token: 0x0400016E RID: 366
		public static ConfigurationDefinition COM1Configuration = new ConfigurationDefinition("COM1:", "Serial", 502);

		// Token: 0x0400016F RID: 367
		public static ConfigurationDefinition COM4Configuration = new ConfigurationDefinition("COM4:", "Serial", 503);
	}
}

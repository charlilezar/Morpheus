using System;

namespace Model.Events
{
	// Token: 0x02000082 RID: 130
	public enum EventStates : ushort
	{
		// Token: 0x04000280 RID: 640
		Undefined,
		// Token: 0x04000281 RID: 641
		RangeUnknown = 10,
		// Token: 0x04000282 RID: 642
		RangeLow,
		// Token: 0x04000283 RID: 643
		RangeHigh,
		// Token: 0x04000284 RID: 644
		Overranged,
		// Token: 0x04000285 RID: 645
		GainUnknown = 40,
		// Token: 0x04000286 RID: 646
		GainX1,
		// Token: 0x04000287 RID: 647
		GainX10,
		// Token: 0x04000288 RID: 648
		Autorange = 50,
		// Token: 0x04000289 RID: 649
		Zero,
		// Token: 0x0400028A RID: 650
		ExternalPowerOn = 60,
		// Token: 0x0400028B RID: 651
		ExternalPowerOff,
		// Token: 0x0400028C RID: 652
		BatteryPresent = 70,
		// Token: 0x0400028D RID: 653
		BatteryNotPresent,
		// Token: 0x0400028E RID: 654
		BatteryError,
		// Token: 0x0400028F RID: 655
		BatteryTemperatureOK = 80,
		// Token: 0x04000290 RID: 656
		BatteryHot,
		// Token: 0x04000291 RID: 657
		BatteryCold,
		// Token: 0x04000292 RID: 658
		StateUnknown = 100,
		// Token: 0x04000293 RID: 659
		NoBatteryConnected,
		// Token: 0x04000294 RID: 660
		Charging,
		// Token: 0x04000295 RID: 661
		FullyCharged,
		// Token: 0x04000296 RID: 662
		Discharging,
		// Token: 0x04000297 RID: 663
		FullyDischarged,
		// Token: 0x04000298 RID: 664
		SystemStateNormal = 110,
		// Token: 0x04000299 RID: 665
		SystemAlarm,
		// Token: 0x0400029A RID: 666
		OperatingModeUnknown = 120,
		// Token: 0x0400029B RID: 667
		Normal,
		// Token: 0x0400029C RID: 668
		Offline,
		// Token: 0x0400029D RID: 669
		CheckSource,
		// Token: 0x0400029E RID: 670
		ElectrometerUpdate = 140,
		// Token: 0x0400029F RID: 671
		EEPROMIdle = 150,
		// Token: 0x040002A0 RID: 672
		EEPROMPass,
		// Token: 0x040002A1 RID: 673
		EEPROMFail,
		// Token: 0x040002A2 RID: 674
		EEPROMInvalidConfiguration,
		// Token: 0x040002A3 RID: 675
		NoAlarm = 180,
		// Token: 0x040002A4 RID: 676
		AlarmActive,
		// Token: 0x040002A5 RID: 677
		DatabaseInitializing = 190,
		// Token: 0x040002A6 RID: 678
		DatabaseOK,
		// Token: 0x040002A7 RID: 679
		DatabaseError,
		// Token: 0x040002A8 RID: 680
		ManualDeletion,
		// Token: 0x040002A9 RID: 681
		AutomaticDeletion,
		// Token: 0x040002AA RID: 682
		CreatedNewDatabase,
		// Token: 0x040002AB RID: 683
		SystemInitializing = 200,
		// Token: 0x040002AC RID: 684
		SystemStarted,
		// Token: 0x040002AD RID: 685
		SystemPowerdown,
		// Token: 0x040002AE RID: 686
		Connected = 210,
		// Token: 0x040002AF RID: 687
		NoIPAddress,
		// Token: 0x040002B0 RID: 688
		HasIPAddress,
		// Token: 0x040002B1 RID: 689
		DateTimeSet = 220,
		// Token: 0x040002B2 RID: 690
		ConfigurationChange = 230,
		// Token: 0x040002B3 RID: 691
		ElectrometerConfigurationChange = 240,
		// Token: 0x040002B4 RID: 692
		ElectrometerConfigurationInvalid,
		// Token: 0x040002B5 RID: 693
		HPICConfigurationChange = 250,
		// Token: 0x040002B6 RID: 694
		HPICConfigurationInvalid,
		// Token: 0x040002B7 RID: 695
		MetStationOffline = 260,
		// Token: 0x040002B8 RID: 696
		MetStationOK,
		// Token: 0x040002B9 RID: 697
		MetStationError
	}
}

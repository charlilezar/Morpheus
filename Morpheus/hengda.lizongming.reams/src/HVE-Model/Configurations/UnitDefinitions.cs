using System;
using System.Collections.Generic;

namespace Model.Configurations
{
	// Token: 0x0200006F RID: 111
	public static class UnitDefinitions
	{
		// Token: 0x060002A9 RID: 681 RVA: 0x0000681B File Offset: 0x00004A1B
		static UnitDefinitions()
		{
			UnitDefinitions.SetupUnits();
			UnitDefinitions.SetupCategories();
			UnitDefinitions.SetupToUnitConverters();
			UnitDefinitions.SetupFromUnitConverters();
		}

		// Token: 0x060002AA RID: 682 RVA: 0x0000685C File Offset: 0x00004A5C
		private static void SetupUnits()
		{
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.RoentgenPerHour, "R/h");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.SievertPerHour, "Sv/h");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.GrayPerHour, "Gy/h");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.DegreesC, "C");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.DegreesF, "F");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.Volt, "V");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.Amp, "A");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.RH, "RH");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.Degrees, "deg");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.MetersPerSecond, "m/s");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.KilometersPerHour, "k/h");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.MilesPerHour, "mph");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.bar, "bar");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.kiloPascals, "kPa");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.Psi, "psi");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.mmHg, "mmHg");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.inHg, "inHg");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.Minutes, "min");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.Hours, "hours");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.Days, "days");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.Count, "");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.MM, "mm");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.CM, "cm");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.Inches, "In");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.VoltsPerSecond, "V/s");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.AmpHours, "AH");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.Percent, "%");
			UnitDefinitions.UnitIdNameDictionary.Add(UnitType.Bytes, "Bytes");
		}

		// Token: 0x060002AB RID: 683 RVA: 0x00006A48 File Offset: 0x00004C48
		private static void SetupToUnitConverters()
		{
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.RoentgenPerHour, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.SievertPerHour, new Func<double, double>(UnitDefinitions.ToSievertConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.GrayPerHour, new Func<double, double>(UnitDefinitions.ToSievertConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.DegreesC, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.DegreesF, new Func<double, double>(UnitDefinitions.ToDegreeFConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.Volt, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.Amp, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.RH, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.Degrees, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.MetersPerSecond, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.KilometersPerHour, new Func<double, double>(UnitDefinitions.ToKilometersPerHourConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.MilesPerHour, new Func<double, double>(UnitDefinitions.ToMPHConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.bar, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.kiloPascals, new Func<double, double>(UnitDefinitions.TokPaConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.Psi, new Func<double, double>(UnitDefinitions.ToPsiConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.mmHg, new Func<double, double>(UnitDefinitions.TommHgConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.inHg, new Func<double, double>(UnitDefinitions.ToinHgConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.Minutes, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.Hours, new Func<double, double>(UnitDefinitions.ToHoursConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.Days, new Func<double, double>(UnitDefinitions.ToDaysConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.Count, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.MM, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.CM, new Func<double, double>(UnitDefinitions.ToCmConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.Inches, new Func<double, double>(UnitDefinitions.ToInchesConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.VoltsPerSecond, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.AmpHours, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.Percent, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.FromBaseUnitConversionDictionary.Add(UnitType.Bytes, new Func<double, double>(UnitDefinitions.UnityConverter));
		}

		// Token: 0x060002AC RID: 684 RVA: 0x00006CF8 File Offset: 0x00004EF8
		private static void SetupFromUnitConverters()
		{
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.RoentgenPerHour, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.SievertPerHour, new Func<double, double>(UnitDefinitions.FromSievertConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.GrayPerHour, new Func<double, double>(UnitDefinitions.FromSievertConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.DegreesC, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.DegreesF, new Func<double, double>(UnitDefinitions.FromDegreeFConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.Volt, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.Amp, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.RH, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.Degrees, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.MetersPerSecond, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.KilometersPerHour, new Func<double, double>(UnitDefinitions.FromKilometersPerHourConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.MilesPerHour, new Func<double, double>(UnitDefinitions.FromMPHConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.bar, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.kiloPascals, new Func<double, double>(UnitDefinitions.FromkPaConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.Psi, new Func<double, double>(UnitDefinitions.FromPsiConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.mmHg, new Func<double, double>(UnitDefinitions.FrommmHgConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.inHg, new Func<double, double>(UnitDefinitions.FrominHgConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.Minutes, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.Hours, new Func<double, double>(UnitDefinitions.FromHoursConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.Days, new Func<double, double>(UnitDefinitions.FromDaysConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.Count, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.MM, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.CM, new Func<double, double>(UnitDefinitions.FromCmConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.Inches, new Func<double, double>(UnitDefinitions.FromInchesConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.VoltsPerSecond, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.AmpHours, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.Percent, new Func<double, double>(UnitDefinitions.UnityConverter));
			UnitDefinitions.ToBaseUnitConversionDictionary.Add(UnitType.Bytes, new Func<double, double>(UnitDefinitions.UnityConverter));
		}

		// Token: 0x060002AD RID: 685 RVA: 0x00006FA8 File Offset: 0x000051A8
		private static void SetupCategories()
		{
			List<UnitType> value = new List<UnitType>
			{
				UnitType.RoentgenPerHour,
				UnitType.SievertPerHour,
				UnitType.GrayPerHour
			};
			UnitDefinitions.UnitCategoryDictionary.Add(UnitCategory.DoseRate, value);
			value = new List<UnitType>
			{
				UnitType.DegreesC,
				UnitType.DegreesF
			};
			UnitDefinitions.UnitCategoryDictionary.Add(UnitCategory.Temperature, value);
			value = new List<UnitType>
			{
				UnitType.Volt
			};
			UnitDefinitions.UnitCategoryDictionary.Add(UnitCategory.Voltage, value);
			value = new List<UnitType>
			{
				UnitType.Amp
			};
			UnitDefinitions.UnitCategoryDictionary.Add(UnitCategory.Current, value);
			value = new List<UnitType>
			{
				UnitType.RH
			};
			UnitDefinitions.UnitCategoryDictionary.Add(UnitCategory.Humidity, value);
			value = new List<UnitType>
			{
				UnitType.MetersPerSecond,
				UnitType.KilometersPerHour,
				UnitType.MilesPerHour
			};
			UnitDefinitions.UnitCategoryDictionary.Add(UnitCategory.Speed, value);
			value = new List<UnitType>
			{
				UnitType.Degrees
			};
			UnitDefinitions.UnitCategoryDictionary.Add(UnitCategory.Direction, value);
			value = new List<UnitType>
			{
				UnitType.bar,
				UnitType.kiloPascals,
				UnitType.Psi,
				UnitType.mmHg,
				UnitType.inHg
			};
			UnitDefinitions.UnitCategoryDictionary.Add(UnitCategory.Pressure, value);
			value = new List<UnitType>
			{
				UnitType.Minutes,
				UnitType.Hours,
				UnitType.Days
			};
			UnitDefinitions.UnitCategoryDictionary.Add(UnitCategory.Timespan, value);
			value = new List<UnitType>
			{
				UnitType.Count
			};
			UnitDefinitions.UnitCategoryDictionary.Add(UnitCategory.Counter, value);
			value = new List<UnitType>
			{
				UnitType.MM,
				UnitType.CM,
				UnitType.Inches
			};
			UnitDefinitions.UnitCategoryDictionary.Add(UnitCategory.Length, value);
			value = new List<UnitType>
			{
				UnitType.VoltsPerSecond
			};
			UnitDefinitions.UnitCategoryDictionary.Add(UnitCategory.Slope, value);
			value = new List<UnitType>
			{
				UnitType.AmpHours
			};
			UnitDefinitions.UnitCategoryDictionary.Add(UnitCategory.BatteryCapacity, value);
			value = new List<UnitType>
			{
				UnitType.Percent
			};
			UnitDefinitions.UnitCategoryDictionary.Add(UnitCategory.Percent, value);
			value = new List<UnitType>
			{
				UnitType.Bytes
			};
			UnitDefinitions.UnitCategoryDictionary.Add(UnitCategory.FileSize, value);
		}

		// Token: 0x060002AE RID: 686 RVA: 0x00007206 File Offset: 0x00005406
		public static List<UnitType> GetAvailableUnits(UnitCategory cat)
		{
			return UnitDefinitions.UnitCategoryDictionary[cat];
		}

		// Token: 0x060002AF RID: 687 RVA: 0x00007213 File Offset: 0x00005413
		private static double UnityConverter(double value)
		{
			return value;
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x00007216 File Offset: 0x00005416
		private static double ToSievertConverter(double value)
		{
			return value * DoseRateMeasurementConfiguration.Instance.RoentgenToSievert;
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x00007224 File Offset: 0x00005424
		private static double FromSievertConverter(double value)
		{
			return value / DoseRateMeasurementConfiguration.Instance.RoentgenToSievert;
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x00007232 File Offset: 0x00005432
		private static double ToDegreeFConverter(double value)
		{
			return value * 1.8 + 32.0;
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x00007249 File Offset: 0x00005449
		private static double FromDegreeFConverter(double value)
		{
			return value * 0.5555 + 17.777;
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x00007260 File Offset: 0x00005460
		private static double ToKilometersPerHourConverter(double value)
		{
			return value * 3.6;
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x0000726D File Offset: 0x0000546D
		private static double FromKilometersPerHourConverter(double value)
		{
			return value * 0.27778;
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x0000727A File Offset: 0x0000547A
		private static double ToMPHConverter(double value)
		{
			return value * 2.237;
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x00007287 File Offset: 0x00005487
		private static double FromMPHConverter(double value)
		{
			return value * 0.44703;
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x00007294 File Offset: 0x00005494
		private static double TokPaConverter(double value)
		{
			return value * 100.0;
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x000072A1 File Offset: 0x000054A1
		private static double FromkPaConverter(double value)
		{
			return value * 0.01;
		}

		// Token: 0x060002BA RID: 698 RVA: 0x000072AE File Offset: 0x000054AE
		private static double ToPsiConverter(double value)
		{
			return value * 14.5;
		}

		// Token: 0x060002BB RID: 699 RVA: 0x000072BB File Offset: 0x000054BB
		private static double FromPsiConverter(double value)
		{
			return value * 0.06897;
		}

		// Token: 0x060002BC RID: 700 RVA: 0x000072C8 File Offset: 0x000054C8
		private static double TommHgConverter(double value)
		{
			return value * 750.06;
		}

		// Token: 0x060002BD RID: 701 RVA: 0x000072D5 File Offset: 0x000054D5
		public static double FrommmHgConverter(double value)
		{
			return value * 0.0013332;
		}

		// Token: 0x060002BE RID: 702 RVA: 0x000072E2 File Offset: 0x000054E2
		private static double ToinHgConverter(double value)
		{
			return value * 29.53;
		}

		// Token: 0x060002BF RID: 703 RVA: 0x000072EF File Offset: 0x000054EF
		private static double FrominHgConverter(double value)
		{
			return value * 0.003386;
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x000072FC File Offset: 0x000054FC
		private static double ToHoursConverter(double value)
		{
			return value / 60.0;
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x00007309 File Offset: 0x00005509
		private static double FromHoursConverter(double value)
		{
			return value * 60.0;
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x00007316 File Offset: 0x00005516
		private static double ToDaysConverter(double value)
		{
			return value / 1440.0;
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x00007323 File Offset: 0x00005523
		private static double FromDaysConverter(double value)
		{
			return value * 1440.0;
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x00007330 File Offset: 0x00005530
		private static double ToCmConverter(double value)
		{
			return value * 0.1;
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0000733D File Offset: 0x0000553D
		private static double FromCmConverter(double value)
		{
			return value * 10.0;
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0000734A File Offset: 0x0000554A
		private static double ToInchesConverter(double value)
		{
			return value * 0.0394;
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x00007357 File Offset: 0x00005557
		private static double FromInchesConverter(double value)
		{
			return value * 25.4;
		}

		// Token: 0x0400021B RID: 539
		public static Dictionary<UnitType, string> UnitIdNameDictionary = new Dictionary<UnitType, string>();

		// Token: 0x0400021C RID: 540
		public static Dictionary<UnitType, Func<double, double>> ToBaseUnitConversionDictionary = new Dictionary<UnitType, Func<double, double>>();

		// Token: 0x0400021D RID: 541
		public static Dictionary<UnitType, Func<double, double>> FromBaseUnitConversionDictionary = new Dictionary<UnitType, Func<double, double>>();

		// Token: 0x0400021E RID: 542
		public static Dictionary<UnitCategory, List<UnitType>> UnitCategoryDictionary = new Dictionary<UnitCategory, List<UnitType>>();
	}
}

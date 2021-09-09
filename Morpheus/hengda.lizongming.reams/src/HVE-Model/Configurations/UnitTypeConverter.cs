using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Model.Configurations
{
	// Token: 0x02000070 RID: 112
	public class UnitTypeConverter : EnumTypeConverter
	{
		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x00007364 File Offset: 0x00005564
		// (set) Token: 0x060002C9 RID: 713 RVA: 0x0000736B File Offset: 0x0000556B
		public static ScalarMeasurementConfiguration SelectedConfiguration { get; set; }

		// Token: 0x060002CA RID: 714 RVA: 0x00007373 File Offset: 0x00005573
		public UnitTypeConverter() : base(typeof(UnitType))
		{
		}

		// Token: 0x060002CB RID: 715 RVA: 0x00007388 File Offset: 0x00005588
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			List<UnitType> availableUnits = UnitDefinitions.GetAvailableUnits(UnitTypeConverter.SelectedConfiguration.UnitCategory);
			return new TypeConverter.StandardValuesCollection(availableUnits.ToArray());
		}

		// Token: 0x060002CC RID: 716 RVA: 0x000073B0 File Offset: 0x000055B0
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				return (UnitType)Enum.Parse(typeof(UnitType), value.ToString(), true);
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}

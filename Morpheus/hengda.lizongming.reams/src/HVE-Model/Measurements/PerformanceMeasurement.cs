using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CommonLibrary;

namespace Model.Measurements
{
	// Token: 0x0200008C RID: 140
	public class PerformanceMeasurement : CompositeMeasurement
	{
		// Token: 0x17000135 RID: 309
		// (get) Token: 0x060003F3 RID: 1011 RVA: 0x0000A10A File Offset: 0x0000830A
		// (set) Token: 0x060003F4 RID: 1012 RVA: 0x0000A112 File Offset: 0x00008312
		[XmlIgnore]
		public PerformanceHistogram Histogram { get; set; }

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x060003F5 RID: 1013 RVA: 0x0000A11B File Offset: 0x0000831B
		// (set) Token: 0x060003F6 RID: 1014 RVA: 0x0000A128 File Offset: 0x00008328
		public override string CompositeValue
		{
			get
			{
				return Serializers.Serializer<PerformanceHistogram>(this.Histogram);
			}
			set
			{
				this.Histogram = Serializers.Deserialize<PerformanceHistogram>(value);
			}
		}

		// Token: 0x040002E9 RID: 745
		private static readonly Dictionary<string, PerformanceMeasurement> MeasurementDictionary = new Dictionary<string, PerformanceMeasurement>();
	}
}

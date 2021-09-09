using System;
using System.Collections.Generic;
using Model.Measurements;

namespace Model.Commands
{
	// Token: 0x02000040 RID: 64
	public class GetAllScalarMeasurementsResponse
	{
		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000171 RID: 369 RVA: 0x000035DF File Offset: 0x000017DF
		// (set) Token: 0x06000172 RID: 370 RVA: 0x000035E7 File Offset: 0x000017E7
		public List<ScalarMeasurement> MeasurementList
		{
			get
			{
				return this._measurementList;
			}
			set
			{
				this._measurementList = value;
			}
		}

		// Token: 0x040000B2 RID: 178
		public const string XmlElementName = "GetAllScalarMeasurementsResponse";

		// Token: 0x040000B3 RID: 179
		private List<ScalarMeasurement> _measurementList = new List<ScalarMeasurement>();
	}
}

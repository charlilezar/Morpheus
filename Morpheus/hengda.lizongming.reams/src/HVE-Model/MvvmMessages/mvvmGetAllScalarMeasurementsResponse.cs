using System;
using System.Collections.Generic;
using Model.Commands;
using Model.DataService;
using Model.Measurements;

namespace Model.MvvmMessages
{
	// Token: 0x0200009F RID: 159
	public class mvvmGetAllScalarMeasurementsResponse : mvvmMessage
	{
		// Token: 0x1700019A RID: 410
		// (get) Token: 0x060004AE RID: 1198 RVA: 0x0000B464 File Offset: 0x00009664
		// (set) Token: 0x060004AF RID: 1199 RVA: 0x0000B46C File Offset: 0x0000966C
		public List<ScalarMeasurement> MeasurementList { get; private set; }

		// Token: 0x060004B0 RID: 1200 RVA: 0x0000B475 File Offset: 0x00009675
		public mvvmGetAllScalarMeasurementsResponse(SentryDataService dataService, GetAllScalarMeasurementsResponse resp) : base(dataService)
		{
			this.MeasurementList = resp.MeasurementList;
		}
	}
}

using System;
using Model.Commands;
using Model.DataService;

namespace Model.MvvmMessages
{
	// Token: 0x020000A6 RID: 166
	public class mvvmMeasurementQueryResponse : mvvmMessage
	{
		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x060004C9 RID: 1225 RVA: 0x0000B5C5 File Offset: 0x000097C5
		// (set) Token: 0x060004CA RID: 1226 RVA: 0x0000B5CD File Offset: 0x000097CD
		public SqlMeasurementQueryResponse Response { get; private set; }

		// Token: 0x060004CB RID: 1227 RVA: 0x0000B5D6 File Offset: 0x000097D6
		public mvvmMeasurementQueryResponse(SentryDataService dataService, SqlMeasurementQueryResponse resp) : base(dataService)
		{
			this.Response = resp;
		}
	}
}

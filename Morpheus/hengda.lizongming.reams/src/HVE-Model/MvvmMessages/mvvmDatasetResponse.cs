using System;
using System.Data;
using Model.Commands;
using Model.DataService;

namespace Model.MvvmMessages
{
	// Token: 0x020000A5 RID: 165
	public class mvvmDatasetResponse : mvvmMessage
	{
		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x060004C2 RID: 1218 RVA: 0x0000B565 File Offset: 0x00009765
		// (set) Token: 0x060004C3 RID: 1219 RVA: 0x0000B56D File Offset: 0x0000976D
		public DataSet Dataset { get; private set; }

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x060004C5 RID: 1221 RVA: 0x0000B57F File Offset: 0x0000977F
		// (set) Token: 0x060004C4 RID: 1220 RVA: 0x0000B576 File Offset: 0x00009776
		public int QueryTimeMilliseconds { get; set; }

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x060004C7 RID: 1223 RVA: 0x0000B590 File Offset: 0x00009790
		// (set) Token: 0x060004C6 RID: 1222 RVA: 0x0000B587 File Offset: 0x00009787
		public int SerializationTimeMilliseconds { get; set; }

		// Token: 0x060004C8 RID: 1224 RVA: 0x0000B598 File Offset: 0x00009798
		public mvvmDatasetResponse(SentryDataService dataService, SqlQueryResponse resp) : base(dataService)
		{
			this.Dataset = resp.Dataset;
			this.QueryTimeMilliseconds = resp.QueryTimeMilliseconds;
			this.SerializationTimeMilliseconds = resp.SerializationTimeMilliseconds;
		}
	}
}

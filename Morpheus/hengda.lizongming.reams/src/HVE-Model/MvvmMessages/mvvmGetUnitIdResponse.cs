using System;
using Model.Commands;
using Model.DataService;

namespace Model.MvvmMessages
{
	// Token: 0x020000A2 RID: 162
	public class mvvmGetUnitIdResponse : mvvmMessage
	{
		// Token: 0x1700019E RID: 414
		// (get) Token: 0x060004B9 RID: 1209 RVA: 0x0000B4F3 File Offset: 0x000096F3
		// (set) Token: 0x060004BA RID: 1210 RVA: 0x0000B4FB File Offset: 0x000096FB
		public UnitInfo UnitInformation { get; set; }

		// Token: 0x060004BB RID: 1211 RVA: 0x0000B504 File Offset: 0x00009704
		public mvvmGetUnitIdResponse(SentryDataService dataService, GetUnitIdResponse resp) : base(dataService)
		{
			this.UnitInformation = resp.UnitInformation;
		}
	}
}

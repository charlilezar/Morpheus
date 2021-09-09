using System;
using System.Collections.Generic;
using Model.Commands;
using Model.DataService;
using Model.Events;

namespace Model.MvvmMessages
{
	// Token: 0x0200009C RID: 156
	public class mvvmGetAllEventsResponse : mvvmMessage
	{
		// Token: 0x17000197 RID: 407
		// (get) Token: 0x060004A5 RID: 1189 RVA: 0x0000B3F2 File Offset: 0x000095F2
		// (set) Token: 0x060004A6 RID: 1190 RVA: 0x0000B3FA File Offset: 0x000095FA
		public List<SentinelState> EventList { get; private set; }

		// Token: 0x060004A7 RID: 1191 RVA: 0x0000B403 File Offset: 0x00009603
		public mvvmGetAllEventsResponse(SentryDataService dataService, GetAllEventsResponse resp) : base(dataService)
		{
			this.EventList = resp.EventList;
		}
	}
}

using System;
using System.Collections.Generic;
using Model.Events;

namespace Model.Commands
{
	// Token: 0x02000036 RID: 54
	public class GetAllEventsResponse
	{
		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000153 RID: 339 RVA: 0x000034A3 File Offset: 0x000016A3
		// (set) Token: 0x06000154 RID: 340 RVA: 0x000034AB File Offset: 0x000016AB
		public List<SentinelState> EventList
		{
			get
			{
				return this._eventList;
			}
			set
			{
				this._eventList = value;
			}
		}

		// Token: 0x0400009F RID: 159
		public const string XmlElementName = "GetAllEventsResponse";

		// Token: 0x040000A0 RID: 160
		private List<SentinelState> _eventList = new List<SentinelState>();
	}
}

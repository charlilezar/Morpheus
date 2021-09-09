using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Model.Events;

namespace Model.Commands
{
	// Token: 0x02000022 RID: 34
	public class SqlEventQueryResponse
	{
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000EA RID: 234 RVA: 0x00002F70 File Offset: 0x00001170
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x00002F67 File Offset: 0x00001167
		public string Name { get; set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000EB RID: 235 RVA: 0x00002F78 File Offset: 0x00001178
		[XmlIgnore]
		public int Count
		{
			get
			{
				return this.EventList.Count;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000ED RID: 237 RVA: 0x00002F8E File Offset: 0x0000118E
		// (set) Token: 0x060000EC RID: 236 RVA: 0x00002F85 File Offset: 0x00001185
		public bool Success { get; set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000EF RID: 239 RVA: 0x00002F9F File Offset: 0x0000119F
		// (set) Token: 0x060000EE RID: 238 RVA: 0x00002F96 File Offset: 0x00001196
		public string Status { get; set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000F1 RID: 241 RVA: 0x00002FB0 File Offset: 0x000011B0
		// (set) Token: 0x060000F0 RID: 240 RVA: 0x00002FA7 File Offset: 0x000011A7
		public int QueryTimeMilliseconds { get; set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x00002FB8 File Offset: 0x000011B8
		// (set) Token: 0x060000F3 RID: 243 RVA: 0x00002FC0 File Offset: 0x000011C0
		public List<SentinelState> EventList { get; set; }

		// Token: 0x060000F4 RID: 244 RVA: 0x00002FC9 File Offset: 0x000011C9
		public SqlEventQueryResponse()
		{
			this.Name = "";
			this.EventList = new List<SentinelState>();
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00002FE7 File Offset: 0x000011E7
		public SqlEventQueryResponse(string name)
		{
			this.Name = name;
			this.EventList = new List<SentinelState>();
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00003001 File Offset: 0x00001201
		public void AddEvent(SentinelState evnt)
		{
			this.EventList.Add(evnt);
		}

		// Token: 0x0400006B RID: 107
		public const string XmlElementName = "SqlEventQueryResponse";
	}
}

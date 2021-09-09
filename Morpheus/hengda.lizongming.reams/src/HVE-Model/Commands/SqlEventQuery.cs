using System;

namespace Model.Commands
{
	// Token: 0x02000021 RID: 33
	public class SqlEventQuery
	{
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x00002F02 File Offset: 0x00001102
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x00002EF9 File Offset: 0x000010F9
		public string Name { get; set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x00002F19 File Offset: 0x00001119
		// (set) Token: 0x060000E3 RID: 227 RVA: 0x00002F0A File Offset: 0x0000110A
		public DateTime Start
		{
			get
			{
				return this._start.ToLocalTime();
			}
			set
			{
				this._start = value.ToUniversalTime();
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x00002F35 File Offset: 0x00001135
		// (set) Token: 0x060000E5 RID: 229 RVA: 0x00002F26 File Offset: 0x00001126
		public DateTime End
		{
			get
			{
				return this._end.ToLocalTime();
			}
			set
			{
				this._end = value.ToUniversalTime();
			}
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00002F42 File Offset: 0x00001142
		public SqlEventQuery()
		{
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00002F4A File Offset: 0x0000114A
		public SqlEventQuery(string eventName, DateTime start, DateTime end)
		{
			this.Name = eventName;
			this.Start = start;
			this.End = end;
		}

		// Token: 0x04000067 RID: 103
		public const string XmlElementName = "SqlEventQuery";

		// Token: 0x04000068 RID: 104
		private DateTime _start;

		// Token: 0x04000069 RID: 105
		private DateTime _end;
	}
}

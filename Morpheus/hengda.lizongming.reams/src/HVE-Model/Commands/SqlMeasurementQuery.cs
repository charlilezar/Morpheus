using System;

namespace Model.Commands
{
	// Token: 0x0200001F RID: 31
	public class SqlMeasurementQuery
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x0000293B File Offset: 0x00000B3B
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x00002932 File Offset: 0x00000B32
		public string Name { get; set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000BA RID: 186 RVA: 0x0000294C File Offset: 0x00000B4C
		// (set) Token: 0x060000B9 RID: 185 RVA: 0x00002943 File Offset: 0x00000B43
		public bool CancelQuery { get; set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00002963 File Offset: 0x00000B63
		// (set) Token: 0x060000BB RID: 187 RVA: 0x00002954 File Offset: 0x00000B54
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

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000BE RID: 190 RVA: 0x0000297F File Offset: 0x00000B7F
		// (set) Token: 0x060000BD RID: 189 RVA: 0x00002970 File Offset: 0x00000B70
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

		// Token: 0x060000BF RID: 191 RVA: 0x0000298C File Offset: 0x00000B8C
		public SqlMeasurementQuery()
		{
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00002994 File Offset: 0x00000B94
		public SqlMeasurementQuery(bool cancelQuery)
		{
			this.Name = "";
			this.CancelQuery = cancelQuery;
			this.Start = DateTime.Now;
			this.End = DateTime.Now;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x000029C4 File Offset: 0x00000BC4
		public SqlMeasurementQuery(string objectName, DateTime start, DateTime end)
		{
			this.Name = objectName;
			this.Start = start;
			this.End = end;
			this.CancelQuery = false;
		}

		// Token: 0x04000057 RID: 87
		public const string XmlElementName = "SqlMeasurementQuery";

		// Token: 0x04000058 RID: 88
		private DateTime _start;

		// Token: 0x04000059 RID: 89
		private DateTime _end;
	}
}

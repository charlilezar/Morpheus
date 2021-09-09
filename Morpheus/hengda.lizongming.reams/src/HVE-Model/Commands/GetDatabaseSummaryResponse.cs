using System;

namespace Model.Commands
{
	// Token: 0x0200001B RID: 27
	public class GetDatabaseSummaryResponse
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600008E RID: 142 RVA: 0x0000277D File Offset: 0x0000097D
		// (set) Token: 0x0600008F RID: 143 RVA: 0x00002785 File Offset: 0x00000985
		public bool IncludeRecordCount { get; set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000090 RID: 144 RVA: 0x0000278E File Offset: 0x0000098E
		// (set) Token: 0x06000091 RID: 145 RVA: 0x00002796 File Offset: 0x00000996
		public DateTime OldestScalarMeasurement
		{
			get
			{
				return this._oldestScalarMeasurement;
			}
			set
			{
				this._oldestScalarMeasurement = value.ToLocalTime();
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000092 RID: 146 RVA: 0x000027A5 File Offset: 0x000009A5
		// (set) Token: 0x06000093 RID: 147 RVA: 0x000027AD File Offset: 0x000009AD
		public DateTime NewestScalarMeasurement
		{
			get
			{
				return this._newestScalarMeasurement;
			}
			set
			{
				this._newestScalarMeasurement = value.ToLocalTime();
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000094 RID: 148 RVA: 0x000027BC File Offset: 0x000009BC
		// (set) Token: 0x06000095 RID: 149 RVA: 0x000027C4 File Offset: 0x000009C4
		public DateTime OldestCompositeMeasurement
		{
			get
			{
				return this._oldestCompositeMeasurement;
			}
			set
			{
				this._oldestCompositeMeasurement = value.ToLocalTime();
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000096 RID: 150 RVA: 0x000027D3 File Offset: 0x000009D3
		// (set) Token: 0x06000097 RID: 151 RVA: 0x000027DB File Offset: 0x000009DB
		public DateTime NewestCompositeMeasurement
		{
			get
			{
				return this._newestCompositeMeasurement;
			}
			set
			{
				this._newestCompositeMeasurement = value.ToLocalTime();
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000098 RID: 152 RVA: 0x000027EA File Offset: 0x000009EA
		// (set) Token: 0x06000099 RID: 153 RVA: 0x000027F2 File Offset: 0x000009F2
		public DateTime OldestEvent
		{
			get
			{
				return this._oldestEvent;
			}
			set
			{
				this._oldestEvent = value.ToLocalTime();
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600009A RID: 154 RVA: 0x00002801 File Offset: 0x00000A01
		// (set) Token: 0x0600009B RID: 155 RVA: 0x00002809 File Offset: 0x00000A09
		public DateTime NewestEvent
		{
			get
			{
				return this._newestEvent;
			}
			set
			{
				this._newestEvent = value.ToLocalTime();
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600009C RID: 156 RVA: 0x00002818 File Offset: 0x00000A18
		// (set) Token: 0x0600009D RID: 157 RVA: 0x00002820 File Offset: 0x00000A20
		public int MeasurementCount
		{
			get
			{
				return this._measurementCount;
			}
			set
			{
				this._measurementCount = value;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00002829 File Offset: 0x00000A29
		// (set) Token: 0x0600009F RID: 159 RVA: 0x00002831 File Offset: 0x00000A31
		public int EventCount
		{
			get
			{
				return this._eventCount;
			}
			set
			{
				this._eventCount = value;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x0000283A File Offset: 0x00000A3A
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x00002842 File Offset: 0x00000A42
		public double MaxRecords
		{
			get
			{
				return this._maxRecords;
			}
			set
			{
				this._maxRecords = value;
			}
		}

		// Token: 0x04000042 RID: 66
		public const string XmlElementName = "GetDatabaseSummaryResponse";

		// Token: 0x04000043 RID: 67
		private DateTime _oldestScalarMeasurement;

		// Token: 0x04000044 RID: 68
		private DateTime _newestScalarMeasurement;

		// Token: 0x04000045 RID: 69
		private DateTime _oldestCompositeMeasurement;

		// Token: 0x04000046 RID: 70
		private DateTime _newestCompositeMeasurement;

		// Token: 0x04000047 RID: 71
		private DateTime _oldestEvent;

		// Token: 0x04000048 RID: 72
		private DateTime _newestEvent;

		// Token: 0x04000049 RID: 73
		private int _measurementCount;

		// Token: 0x0400004A RID: 74
		private int _eventCount;

		// Token: 0x0400004B RID: 75
		private double _maxRecords;
	}
}

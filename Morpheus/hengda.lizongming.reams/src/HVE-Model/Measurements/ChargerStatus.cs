using System;

namespace Model.Measurements
{
	// Token: 0x0200008E RID: 142
	public class ChargerStatus
	{
		// Token: 0x0600042B RID: 1067 RVA: 0x0000A520 File Offset: 0x00008720
		public ChargerStatus(ushort value)
		{
			this._value = value;
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x0600042C RID: 1068 RVA: 0x0000A52F File Offset: 0x0000872F
		public bool AC_PRESENT
		{
			get
			{
				return (this._value & 128) != 0;
			}
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x0600042D RID: 1069 RVA: 0x0000A543 File Offset: 0x00008743
		public bool BATTERY_PRESENT
		{
			get
			{
				return (this._value & 64) != 0;
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x0600042E RID: 1070 RVA: 0x0000A554 File Offset: 0x00008754
		public bool POWER_FAIL
		{
			get
			{
				return (this._value & 64) != 0;
			}
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x0600042F RID: 1071 RVA: 0x0000A565 File Offset: 0x00008765
		public bool ALARM_INHIBITED
		{
			get
			{
				return (this._value & 64) != 0;
			}
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06000430 RID: 1072 RVA: 0x0000A576 File Offset: 0x00008776
		public bool RES_UR
		{
			get
			{
				return (this._value & 64) != 0;
			}
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000431 RID: 1073 RVA: 0x0000A587 File Offset: 0x00008787
		public bool RES_HOT
		{
			get
			{
				return (this._value & 64) != 0;
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06000432 RID: 1074 RVA: 0x0000A598 File Offset: 0x00008798
		public bool RES_COLD
		{
			get
			{
				return (this._value & 64) != 0;
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x06000433 RID: 1075 RVA: 0x0000A5A9 File Offset: 0x000087A9
		public bool RES_OR
		{
			get
			{
				return (this._value & 64) != 0;
			}
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06000434 RID: 1076 RVA: 0x0000A5BA File Offset: 0x000087BA
		public bool VOLTAGE_OR
		{
			get
			{
				return (this._value & 64) != 0;
			}
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x06000435 RID: 1077 RVA: 0x0000A5CB File Offset: 0x000087CB
		public bool CURRENT_OR
		{
			get
			{
				return (this._value & 64) != 0;
			}
		}

		// Token: 0x040002F0 RID: 752
		private readonly ushort _value;
	}
}

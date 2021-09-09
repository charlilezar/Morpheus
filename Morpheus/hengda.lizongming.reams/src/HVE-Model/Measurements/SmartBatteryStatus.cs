using System;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Model.Measurements
{
	// Token: 0x0200008D RID: 141
	public class SmartBatteryStatus
	{
		// Token: 0x060003F8 RID: 1016 RVA: 0x0000A142 File Offset: 0x00008342
		public void SetBatteryRegister(int address, ushort value)
		{
			this._registers[address] = value;
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x0000A150 File Offset: 0x00008350
		public SmartBatteryStatus()
		{
			this._chargerStatusValue = 0;
			for (int i = 0; i < 36; i++)
			{
				this._registers[i] = 0;
			}
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x060003FA RID: 1018 RVA: 0x0000A198 File Offset: 0x00008398
		// (set) Token: 0x060003FB RID: 1019 RVA: 0x0000A1A0 File Offset: 0x000083A0
		[XmlIgnore]
		public ushort[] Registers
		{
			get
			{
				return this._registers;
			}
			set
			{
				this._registers = value;
			}
		}

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x060003FC RID: 1020 RVA: 0x0000A1A9 File Offset: 0x000083A9
		// (set) Token: 0x060003FD RID: 1021 RVA: 0x0000A1B1 File Offset: 0x000083B1
		public ushort ChargerStatusValue
		{
			get
			{
				return this._chargerStatusValue;
			}
			set
			{
				this._chargerStatusValue = value;
			}
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x060003FE RID: 1022 RVA: 0x0000A1BA File Offset: 0x000083BA
		[XmlIgnore]
		public int RemainingCapacityAlarm
		{
			get
			{
				return (int)this._registers[1];
			}
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x060003FF RID: 1023 RVA: 0x0000A1C4 File Offset: 0x000083C4
		[XmlIgnore]
		public int RemainingTimeAlarm
		{
			get
			{
				return (int)this._registers[2];
			}
		}

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06000400 RID: 1024 RVA: 0x0000A1CE File Offset: 0x000083CE
		[XmlIgnore]
		public int BatteryMode
		{
			get
			{
				return (int)this._registers[3];
			}
		}

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06000401 RID: 1025 RVA: 0x0000A1D8 File Offset: 0x000083D8
		[XmlIgnore]
		public int AtRate
		{
			get
			{
				return (int)this._registers[4];
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000402 RID: 1026 RVA: 0x0000A1E2 File Offset: 0x000083E2
		[XmlIgnore]
		public int AtRateTimeToFull
		{
			get
			{
				return (int)((short)this._registers[5]);
			}
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06000403 RID: 1027 RVA: 0x0000A1ED File Offset: 0x000083ED
		[XmlIgnore]
		public int AtRateTimeToEmpty
		{
			get
			{
				return (int)((short)this._registers[6]);
			}
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06000404 RID: 1028 RVA: 0x0000A1F8 File Offset: 0x000083F8
		[XmlIgnore]
		public bool AtRateOK
		{
			get
			{
				return this._registers[7] == 1;
			}
		}

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06000405 RID: 1029 RVA: 0x0000A205 File Offset: 0x00008405
		[XmlIgnore]
		public int Temperature
		{
			get
			{
				if (this._registers[8] == 0)
				{
					return 0;
				}
				return (int)(this._registers[8] / 10 - 273);
			}
		}

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000406 RID: 1030 RVA: 0x0000A224 File Offset: 0x00008424
		[XmlIgnore]
		public int Voltage
		{
			get
			{
				return (int)this._registers[9];
			}
		}

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000407 RID: 1031 RVA: 0x0000A22F File Offset: 0x0000842F
		[XmlIgnore]
		public int Current
		{
			get
			{
				return (int)((short)this._registers[10]);
			}
		}

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06000408 RID: 1032 RVA: 0x0000A23B File Offset: 0x0000843B
		[XmlIgnore]
		public int AverageCurrent
		{
			get
			{
				return (int)((short)this._registers[11]);
			}
		}

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x06000409 RID: 1033 RVA: 0x0000A247 File Offset: 0x00008447
		[XmlIgnore]
		public int MaxError
		{
			get
			{
				return (int)this._registers[12];
			}
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x0600040A RID: 1034 RVA: 0x0000A252 File Offset: 0x00008452
		[XmlIgnore]
		public int RelativeStateOfCharge
		{
			get
			{
				return (int)this._registers[13];
			}
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x0600040B RID: 1035 RVA: 0x0000A25D File Offset: 0x0000845D
		[XmlIgnore]
		public int AbsoluteStateOfCharge
		{
			get
			{
				return (int)this._registers[14];
			}
		}

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x0600040C RID: 1036 RVA: 0x0000A268 File Offset: 0x00008468
		[XmlIgnore]
		public int RemainingCapacity
		{
			get
			{
				return (int)this._registers[15];
			}
		}

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x0600040D RID: 1037 RVA: 0x0000A273 File Offset: 0x00008473
		[XmlIgnore]
		public int FullChargeCapacity
		{
			get
			{
				return (int)this._registers[16];
			}
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x0600040E RID: 1038 RVA: 0x0000A27E File Offset: 0x0000847E
		[XmlIgnore]
		public int RunTimeToEmpty
		{
			get
			{
				return (int)((short)this._registers[17]);
			}
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x0600040F RID: 1039 RVA: 0x0000A28A File Offset: 0x0000848A
		[XmlIgnore]
		public int AverageTimeToEmpty
		{
			get
			{
				return (int)((short)this._registers[18]);
			}
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x06000410 RID: 1040 RVA: 0x0000A296 File Offset: 0x00008496
		[XmlIgnore]
		public int AverageTimeToFull
		{
			get
			{
				return (int)((short)this._registers[19]);
			}
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000411 RID: 1041 RVA: 0x0000A2A2 File Offset: 0x000084A2
		[XmlIgnore]
		public int BatteryChargingCurrent
		{
			get
			{
				return (int)((short)this._registers[20]);
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000412 RID: 1042 RVA: 0x0000A2AE File Offset: 0x000084AE
		[XmlIgnore]
		public int BatteryChargingVoltage
		{
			get
			{
				return (int)this._registers[21];
			}
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06000413 RID: 1043 RVA: 0x0000A2B9 File Offset: 0x000084B9
		[XmlIgnore]
		public int BatteryStatus
		{
			get
			{
				return (int)this._registers[22];
			}
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06000414 RID: 1044 RVA: 0x0000A2C4 File Offset: 0x000084C4
		[XmlIgnore]
		public int CycleCount
		{
			get
			{
				return (int)this._registers[23];
			}
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06000415 RID: 1045 RVA: 0x0000A2CF File Offset: 0x000084CF
		[XmlIgnore]
		public int DesignCapacity
		{
			get
			{
				return (int)this._registers[24];
			}
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06000416 RID: 1046 RVA: 0x0000A2DA File Offset: 0x000084DA
		[XmlIgnore]
		public int DesignVoltage
		{
			get
			{
				return (int)this._registers[25];
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06000417 RID: 1047 RVA: 0x0000A2E5 File Offset: 0x000084E5
		[XmlIgnore]
		public int SpecificationInfo
		{
			get
			{
				return (int)this._registers[26];
			}
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x06000418 RID: 1048 RVA: 0x0000A2F0 File Offset: 0x000084F0
		[XmlIgnore]
		public int ManufacturerDate
		{
			get
			{
				return (int)this._registers[27];
			}
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x06000419 RID: 1049 RVA: 0x0000A2FB File Offset: 0x000084FB
		[XmlIgnore]
		public int SerialNumber
		{
			get
			{
				return (int)this._registers[28];
			}
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x0000A308 File Offset: 0x00008508
		public void UpdateStatus(SmartBatteryStatus batteryStatus)
		{
			this._chargerStatusValue = batteryStatus.ChargerStatusValue;
			for (int i = 0; i < 36; i++)
			{
				this._registers[i] = batteryStatus.Registers[i];
			}
		}

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x0600041B RID: 1051 RVA: 0x0000A33E File Offset: 0x0000853E
		[XmlIgnore]
		public bool AcPresent
		{
			get
			{
				return (this._chargerStatusValue & 32768) != 0;
			}
		}

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x0600041C RID: 1052 RVA: 0x0000A352 File Offset: 0x00008552
		[XmlIgnore]
		public bool BatteryPresent
		{
			get
			{
				return (this._chargerStatusValue & 16384) != 0;
			}
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x0600041D RID: 1053 RVA: 0x0000A366 File Offset: 0x00008566
		[XmlIgnore]
		public bool PowerFail
		{
			get
			{
				return (this._chargerStatusValue & 8192) != 0;
			}
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x0600041E RID: 1054 RVA: 0x0000A37A File Offset: 0x0000857A
		[XmlIgnore]
		public bool AlarmInhibited
		{
			get
			{
				return (this._chargerStatusValue & 4096) != 0;
			}
		}

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x0600041F RID: 1055 RVA: 0x0000A38E File Offset: 0x0000858E
		[XmlIgnore]
		public bool RES_UR
		{
			get
			{
				return (this._chargerStatusValue & 2048) != 0;
			}
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x06000420 RID: 1056 RVA: 0x0000A3A2 File Offset: 0x000085A2
		[XmlIgnore]
		public bool HotBattery
		{
			get
			{
				return (this._chargerStatusValue & 1024) != 0;
			}
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06000421 RID: 1057 RVA: 0x0000A3B6 File Offset: 0x000085B6
		[XmlIgnore]
		public bool ColdBattery
		{
			get
			{
				return (this._chargerStatusValue & 512) != 0;
			}
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000422 RID: 1058 RVA: 0x0000A3CA File Offset: 0x000085CA
		[XmlIgnore]
		public bool RES_OR
		{
			get
			{
				return (this._chargerStatusValue & 256) != 0;
			}
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000423 RID: 1059 RVA: 0x0000A3DE File Offset: 0x000085DE
		[XmlIgnore]
		public bool VoltageOverRange
		{
			get
			{
				return (this._chargerStatusValue & 128) != 0;
			}
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000424 RID: 1060 RVA: 0x0000A3F2 File Offset: 0x000085F2
		[XmlIgnore]
		public bool CurrentOverRange
		{
			get
			{
				return (this._chargerStatusValue & 64) != 0;
			}
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06000425 RID: 1061 RVA: 0x0000A403 File Offset: 0x00008603
		[XmlIgnore]
		public bool ChargerInhibited
		{
			get
			{
				return (this._chargerStatusValue & 1) != 0;
			}
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x06000426 RID: 1062 RVA: 0x0000A413 File Offset: 0x00008613
		[XmlIgnore]
		public int ChargerLevel
		{
			get
			{
				return (this._chargerStatusValue & 48) >> 4;
			}
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x0000A420 File Offset: 0x00008620
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			reader.ReadStartElement();
			string text = reader.ReadElementString("Registers");
			string[] array = text.Split(new char[]
			{
				',',
				'<'
			});
			int num = array.Length - 1;
			this._registers = new ushort[num];
			this._chargerStatusValue = ushort.Parse(array[0]);
			for (int i = 0; i < num; i++)
			{
				this._registers[i] = ushort.Parse(array[i + 1]);
			}
			reader.ReadEndElement();
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x0000A4A8 File Offset: 0x000086A8
		public void WriteXml(XmlWriter writer)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0},", this._chargerStatusValue);
			for (int i = 0; i < 36; i++)
			{
				stringBuilder.AppendFormat((i == 35) ? "{0}" : "{0},", this._registers[i]);
			}
			writer.WriteElementString("Registers", stringBuilder.ToString());
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x0000A515 File Offset: 0x00008715
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x040002EB RID: 747
		private const int NumStatusRegisters = 36;

		// Token: 0x040002EC RID: 748
		private ushort[] _registers = new ushort[36];

		// Token: 0x040002ED RID: 749
		private ushort _chargerStatusValue;

		// Token: 0x040002EE RID: 750
		private ushort _lastRegisterValue = 254;

		// Token: 0x040002EF RID: 751
		private static bool _initialSample = true;
	}
}

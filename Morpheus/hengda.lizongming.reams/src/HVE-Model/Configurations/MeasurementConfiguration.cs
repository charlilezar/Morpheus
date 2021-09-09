using System;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Model.Configurations
{
	// Token: 0x02000053 RID: 83
	public abstract class MeasurementConfiguration : Configuration
	{
		// Token: 0x060001C7 RID: 455 RVA: 0x00003F67 File Offset: 0x00002167
		protected MeasurementConfiguration()
		{
			this.InitializeCustomDescripters();
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x00003F75 File Offset: 0x00002175
		protected MeasurementConfiguration(ConfigurationDefinition configDef, bool factoryUseOnly) : base(configDef, factoryUseOnly)
		{
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x00003F7F File Offset: 0x0000217F
		protected MeasurementConfiguration(ConfigurationDefinition configDef, bool notifyEnable, TimeSpan updateInterval, TimeSpan minUpdateInterval, TimeSpan loggingInterval, MeasurementSource source, bool factoryUseOnly) : base(configDef, factoryUseOnly)
		{
			this.NotifyEnable = notifyEnable;
			this.MinUpdateIntervalSpan = minUpdateInterval;
			this.LoggingIntervalSpan = loggingInterval;
			this.UpdateIntervalSpan = updateInterval;
			this.Source = source;
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00003FB0 File Offset: 0x000021B0
		private void InitializeCustomDescripters()
		{
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060001CB RID: 459 RVA: 0x00003FB4 File Offset: 0x000021B4
		// (set) Token: 0x060001CC RID: 460 RVA: 0x00003FCF File Offset: 0x000021CF
		[Browsable(false)]
		public double UpdateInterval
		{
			get
			{
				return this.UpdateIntervalSpan.TotalSeconds;
			}
			set
			{
				this.UpdateIntervalSpan = TimeSpan.FromSeconds(value);
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060001CD RID: 461 RVA: 0x00003FE0 File Offset: 0x000021E0
		// (set) Token: 0x060001CE RID: 462 RVA: 0x00003FFB File Offset: 0x000021FB
		[Browsable(false)]
		public double MinUpdateInterval
		{
			get
			{
				return this.MinUpdateIntervalSpan.TotalSeconds;
			}
			set
			{
				this.MinUpdateIntervalSpan = TimeSpan.FromSeconds(value);
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060001CF RID: 463 RVA: 0x0000400C File Offset: 0x0000220C
		// (set) Token: 0x060001D0 RID: 464 RVA: 0x00004027 File Offset: 0x00002227
		[Browsable(false)]
		public double LoggingInterval
		{
			get
			{
				return this.LoggingIntervalSpan.TotalSeconds;
			}
			set
			{
				this.LoggingIntervalSpan = TimeSpan.FromSeconds(value);
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060001D1 RID: 465 RVA: 0x00004035 File Offset: 0x00002235
		// (set) Token: 0x060001D2 RID: 466 RVA: 0x0000403D File Offset: 0x0000223D
		[ReadOnly(true)]
		public MeasurementSource Source { get; set; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060001D3 RID: 467 RVA: 0x00004046 File Offset: 0x00002246
		// (set) Token: 0x060001D4 RID: 468 RVA: 0x0000404E File Offset: 0x0000224E
		[Category("Measurement")]
		public bool NotifyEnable { get; set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060001D5 RID: 469 RVA: 0x00004057 File Offset: 0x00002257
		// (set) Token: 0x060001D6 RID: 470 RVA: 0x00004060 File Offset: 0x00002260
		[XmlIgnore]
		[Category("Measurement")]
		public TimeSpan UpdateIntervalSpan
		{
			get
			{
				return this._updateIntervalSpan;
			}
			set
			{
				if (this.Source == MeasurementSource.Self)
				{
					if (value > MeasurementConfiguration._maxUpdateInterval)
					{
						return;
					}
					if (value < this.MinUpdateIntervalSpan && value.TotalSeconds > 0.0)
					{
						this._updateIntervalSpan = this.MinUpdateIntervalSpan;
					}
					else
					{
						long value2 = value.Ticks / 50000000L * 50000000L;
						this._updateIntervalSpan = TimeSpan.FromTicks(value2);
					}
					this.UpdateIntervalSeconds = this._updateIntervalSpan.TotalSeconds;
				}
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060001D7 RID: 471 RVA: 0x000040E4 File Offset: 0x000022E4
		// (set) Token: 0x060001D8 RID: 472 RVA: 0x000040EC File Offset: 0x000022EC
		[Browsable(false)]
		[Category("Measurement")]
		[XmlIgnore]
		public TimeSpan MinUpdateIntervalSpan { get; set; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060001D9 RID: 473 RVA: 0x000040F5 File Offset: 0x000022F5
		// (set) Token: 0x060001DA RID: 474 RVA: 0x00004100 File Offset: 0x00002300
		[Category("Measurement")]
		[XmlIgnore]
		public TimeSpan LoggingIntervalSpan
		{
			get
			{
				return this._loggingIntervalSpan;
			}
			set
			{
				if (value > MeasurementConfiguration._maxLoggingInterval)
				{
					return;
				}
				if (value < this.MinUpdateIntervalSpan && value.TotalSeconds > 0.0)
				{
					this._loggingIntervalSpan = this.MinUpdateIntervalSpan;
				}
				else
				{
					long value2 = value.Ticks / 50000000L * 50000000L;
					this._loggingIntervalSpan = TimeSpan.FromTicks(value2);
				}
				this.LoggingIntervalSeconds = this._loggingIntervalSpan.TotalSeconds;
				this.LogEnable = (this.LoggingIntervalSeconds != 0.0);
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060001DB RID: 475 RVA: 0x00004196 File Offset: 0x00002396
		// (set) Token: 0x060001DC RID: 476 RVA: 0x0000419E File Offset: 0x0000239E
		[Browsable(false)]
		[XmlIgnore]
		public double LoggingIntervalSeconds { get; private set; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060001DD RID: 477 RVA: 0x000041A7 File Offset: 0x000023A7
		// (set) Token: 0x060001DE RID: 478 RVA: 0x000041AF File Offset: 0x000023AF
		[Category("Measurement")]
		public bool LogEnable { get; set; }

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060001DF RID: 479 RVA: 0x000041B8 File Offset: 0x000023B8
		// (set) Token: 0x060001E0 RID: 480 RVA: 0x000041C0 File Offset: 0x000023C0
		[XmlIgnore]
		[Browsable(false)]
		public double UpdateIntervalSeconds { get; private set; }

		// Token: 0x060001E1 RID: 481 RVA: 0x000041CC File Offset: 0x000023CC
		public void ReadMeasurementBaseXml(XmlReader reader)
		{
			base.ReadBaseXml(reader);
			this.UpdateInterval = reader.ReadElementDouble("UpdateInterval");
			this.MinUpdateInterval = reader.ReadElementDouble("MinUpdateInterval");
			this.LoggingInterval = reader.ReadElementDouble("LoggingInterval");
			this.NotifyEnable = reader.ReadElementBoolean("NotifyEnable");
			this.LogEnable = reader.ReadElementBoolean("LogEnable");
			this.Source = reader.ReadElementEnum<MeasurementSource>("Source");
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x00004248 File Offset: 0x00002448
		public void WriteMeasurementBaseXml(XmlWriter writer)
		{
			base.WriteBaseXml(writer);
			writer.WriteElementDouble("UpdateInterval", this.UpdateInterval, "0");
			writer.WriteElementDouble("MinUpdateInterval", this.MinUpdateInterval, "0");
			writer.WriteElementDouble("LoggingInterval", this.LoggingInterval, "0");
			writer.WriteElementBoolean("NotifyEnable", this.NotifyEnable);
			writer.WriteElementBoolean("LogEnable", this.LogEnable);
			writer.WriteElementEnum("Source", this.Source);
		}

		// Token: 0x04000101 RID: 257
		private TimeSpan _loggingIntervalSpan;

		// Token: 0x04000102 RID: 258
		private TimeSpan _updateIntervalSpan;

		// Token: 0x04000103 RID: 259
		private static TimeSpan _maxUpdateInterval = TimeSpan.FromDays(1.0);

		// Token: 0x04000104 RID: 260
		private static TimeSpan _maxLoggingInterval = TimeSpan.FromDays(1.0);
	}
}

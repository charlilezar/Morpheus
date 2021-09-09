using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Model.Measurements;

namespace Model.Configurations
{
	// Token: 0x02000051 RID: 81
	public class AlarmEventConfiguration : EventConfiguration, IXmlSerializable
	{
		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060001B9 RID: 441 RVA: 0x00003C89 File Offset: 0x00001E89
		// (set) Token: 0x060001BA RID: 442 RVA: 0x00003C91 File Offset: 0x00001E91
		[Category("Alarm")]
		public double AlarmValue
		{
			get
			{
				return this._alarmValue;
			}
			set
			{
				if (value <= this._maxAlarmValue && value >= this._minAlarmValue)
				{
					this._alarmValue = value;
				}
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060001BB RID: 443 RVA: 0x00003CAC File Offset: 0x00001EAC
		// (set) Token: 0x060001BC RID: 444 RVA: 0x00003CB4 File Offset: 0x00001EB4
		[XmlElement(ElementName = "Measurement")]
		[ReadOnly(true)]
		[Category("Alarm")]
		public string MeasurementName { get; set; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060001BD RID: 445 RVA: 0x00003CBD File Offset: 0x00001EBD
		// (set) Token: 0x060001BE RID: 446 RVA: 0x00003CC5 File Offset: 0x00001EC5
		[ReadOnly(true)]
		[Category("Alarm")]
		public AlarmEventConfiguration.AlarmType TypeOfAlarm { get; set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060001BF RID: 447 RVA: 0x00003CCE File Offset: 0x00001ECE
		// (set) Token: 0x060001C0 RID: 448 RVA: 0x00003CD6 File Offset: 0x00001ED6
		[Category("Alarm")]
		public bool AlarmEnabled { get; set; }

		// Token: 0x060001C1 RID: 449 RVA: 0x00003CDF File Offset: 0x00001EDF
		public AlarmEventConfiguration()
		{
			this._alarmCallback = null;
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x00003D00 File Offset: 0x00001F00
		public AlarmEventConfiguration(ConfigurationDefinition configDef, ScalarMeasurement measurement, AlarmEventConfiguration.AlarmType alarmType, double alarmValue, double maxAlarmValue, double minAlarmValue, bool logOnChange, EventConfiguration.NotifyCriteria notifyCriteria, bool autoReset, bool readOnly, bool factoryOnly) : base(configDef, logOnChange, notifyCriteria, readOnly, factoryOnly)
		{
			this._alarmValue = alarmValue;
			this._maxAlarmValue = maxAlarmValue;
			this._minAlarmValue = minAlarmValue;
			this.TypeOfAlarm = alarmType;
			this._measurement = measurement;
			this.MeasurementName = measurement.Name;
			this.AlarmEnabled = true;
			this._autoReset = autoReset;
			this._alarmCallback = null;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x00003D74 File Offset: 0x00001F74
		public new static AlarmEventConfiguration Deserialize(string xmlString)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AlarmEventConfiguration));
			AlarmEventConfiguration result;
			using (StringReader stringReader = new StringReader(xmlString))
			{
				result = (AlarmEventConfiguration)xmlSerializer.Deserialize(stringReader);
			}
			return result;
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00003DC4 File Offset: 0x00001FC4
		public new void ReadXml(XmlReader reader)
		{
			try
			{
				reader.MoveToContent();
				reader.ReadStartElement();
				base.ReadBaseXml(reader);
				this._alarmValue = reader.ReadElementDouble("AlarmValue");
				this._maxAlarmValue = reader.ReadElementDouble("MaxAlarmValue");
				this._minAlarmValue = reader.ReadElementDouble("MinAlarmValue");
				base.LogOnChange = reader.ReadElementBoolean("LogOnChange");
				base.Notify = reader.ReadElementEnum<EventConfiguration.NotifyCriteria>("Notify");
				base.ReadOnly = reader.ReadElementBoolean("ReadOnly");
				this.TypeOfAlarm = reader.ReadElementEnum<AlarmEventConfiguration.AlarmType>("TypeOfAlarm");
				this.MeasurementName = reader.ReadElementString("MeasurementName");
				this.AlarmEnabled = reader.ReadElementBoolean("AlarmEnabled");
				reader.ReadEndElement();
				base.DeserializeSuccess = true;
			}
			catch (Exception)
			{
				base.DeserializeSuccess = false;
			}
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x00003EA8 File Offset: 0x000020A8
		public new void WriteXml(XmlWriter writer)
		{
			base.WriteBaseXml(writer);
			writer.WriteElementDouble("AlarmValue", this._alarmValue, "0.000e0");
			writer.WriteElementDouble("MaxAlarmValue", this._maxAlarmValue, "0.000e0");
			writer.WriteElementDouble("MinAlarmValue", this._minAlarmValue, "0.000e0");
			writer.WriteElementBoolean("LogOnChange", base.LogOnChange);
			writer.WriteElementEnum("Notify", base.Notify);
			writer.WriteElementBoolean("ReadOnly", base.ReadOnly);
			writer.WriteElementEnum("TypeOfAlarm", this.TypeOfAlarm);
			writer.WriteElementString("MeasurementName", this.MeasurementName);
			writer.WriteElementBoolean("AlarmEnabled", this.AlarmEnabled);
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x00003F64 File Offset: 0x00002164
		public new XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x040000F1 RID: 241
		private double _alarmValue;

		// Token: 0x040000F2 RID: 242
		private double _maxAlarmValue;

		// Token: 0x040000F3 RID: 243
		private double _minAlarmValue;

		// Token: 0x040000F4 RID: 244
		private bool _autoReset;

		// Token: 0x040000F5 RID: 245
		private ScalarMeasurement _measurement;

		// Token: 0x040000F6 RID: 246
		private Action<ScalarMeasurement, AlarmEventConfiguration> _alarmCallback;

		// Token: 0x040000F7 RID: 247
		private double _lastValue = double.MaxValue;

		// Token: 0x02000052 RID: 82
		public enum AlarmType
		{
			// Token: 0x040000FC RID: 252
			GreaterThan,
			// Token: 0x040000FD RID: 253
			LessThan,
			// Token: 0x040000FE RID: 254
			PercentChange,
			// Token: 0x040000FF RID: 255
			GreaterThanAbsolute,
			// Token: 0x04000100 RID: 256
			LessThanAbsolute
		}
	}
}

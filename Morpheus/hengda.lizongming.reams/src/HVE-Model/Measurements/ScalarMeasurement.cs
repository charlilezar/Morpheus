using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Model.Configurations;

namespace Model.Measurements
{
	// Token: 0x020000B2 RID: 178
	public class ScalarMeasurement : Measurement, IXmlSerializable
	{
		// Token: 0x170001AD RID: 429
		// (get) Token: 0x06000502 RID: 1282 RVA: 0x0000BE9A File Offset: 0x0000A09A
		// (set) Token: 0x06000503 RID: 1283 RVA: 0x0000BEA2 File Offset: 0x0000A0A2
		public ScalarMeasurementConfiguration Configuration { get; set; }

		// Token: 0x06000504 RID: 1284 RVA: 0x0000BEAB File Offset: 0x0000A0AB
		public ScalarMeasurement()
		{
			this.Configuration = new ScalarMeasurementConfiguration();
			this.ClearAccumulator();
		}

		// Token: 0x06000505 RID: 1285 RVA: 0x0000BEC4 File Offset: 0x0000A0C4
		public ScalarMeasurement(ScalarMeasurementConfiguration configuration) : base(configuration.Name)
		{
			this.Configuration = configuration;
			this.ClearAccumulator();
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06000506 RID: 1286 RVA: 0x0000BEDF File Offset: 0x0000A0DF
		[XmlIgnore]
		public string Format
		{
			get
			{
				return this.Configuration.FormatString;
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06000507 RID: 1287 RVA: 0x0000BEEC File Offset: 0x0000A0EC
		// (set) Token: 0x06000508 RID: 1288 RVA: 0x0000BEF4 File Offset: 0x0000A0F4
		public double Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
				this.m_Accum += value;
				this.m_Accum2 += value * value;
				this.m_Count += 1.0;
				if (value > this.Max)
				{
					this.Max = value;
				}
				if (value < this.Min)
				{
					this.Min = value;
				}
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06000509 RID: 1289 RVA: 0x0000BF5C File Offset: 0x0000A15C
		// (set) Token: 0x0600050A RID: 1290 RVA: 0x0000BF64 File Offset: 0x0000A164
		public bool AlarmActive { get; set; }

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x0600050B RID: 1291 RVA: 0x0000BF6D File Offset: 0x0000A16D
		[XmlIgnore]
		public double Average
		{
			get
			{
				if (this.m_Count > 0.0)
				{
					return this.m_Accum / this.m_Count;
				}
				return 0.0;
			}
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x0600050C RID: 1292 RVA: 0x0000BF97 File Offset: 0x0000A197
		[XmlIgnore]
		public double StdDev
		{
			get
			{
				return 0.0;
			}
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x0600050D RID: 1293 RVA: 0x0000BFA2 File Offset: 0x0000A1A2
		// (set) Token: 0x0600050E RID: 1294 RVA: 0x0000BFAA File Offset: 0x0000A1AA
		[XmlIgnore]
		public double Max { get; private set; }

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x0600050F RID: 1295 RVA: 0x0000BFB3 File Offset: 0x0000A1B3
		// (set) Token: 0x06000510 RID: 1296 RVA: 0x0000BFBB File Offset: 0x0000A1BB
		[XmlIgnore]
		public double Min { get; private set; }

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06000511 RID: 1297 RVA: 0x0000BFC4 File Offset: 0x0000A1C4
		// (set) Token: 0x06000512 RID: 1298 RVA: 0x0000BFCC File Offset: 0x0000A1CC
		[XmlIgnore]
		public double MinLimit { get; set; }

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06000513 RID: 1299 RVA: 0x0000BFD5 File Offset: 0x0000A1D5
		// (set) Token: 0x06000514 RID: 1300 RVA: 0x0000BFDD File Offset: 0x0000A1DD
		[XmlIgnore]
		public double MaxLimit { get; set; }

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000515 RID: 1301 RVA: 0x0000BFE6 File Offset: 0x0000A1E6
		// (set) Token: 0x06000516 RID: 1302 RVA: 0x0000BFEE File Offset: 0x0000A1EE
		[XmlIgnore]
		public bool LimitsEnabled { get; set; }

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06000517 RID: 1303 RVA: 0x0000BFF8 File Offset: 0x0000A1F8
		[XmlIgnore]
		public string FormattedValue
		{
			get
			{
				if (this.Configuration != null)
				{
					return this.Configuration.FormattedValue(this.Value);
				}
				return this.Value.ToString();
			}
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x0000C02D File Offset: 0x0000A22D
		public void ClearAccumulator()
		{
			this.m_Accum = 0.0;
			this.m_Count = 0.0;
			this.Max = double.MinValue;
			this.Min = double.MaxValue;
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x0000C06C File Offset: 0x0000A26C
		public static ScalarMeasurement DeserializeFromStream(MemoryStream stream)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.IgnoreComments = false;
			xmlReaderSettings.IgnoreWhitespace = true;
			XmlReader xmlReader = new XmlTextReader(stream);
			xmlReader = XmlReader.Create(xmlReader, xmlReaderSettings);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ScalarMeasurement));
			return (ScalarMeasurement)xmlSerializer.Deserialize(xmlReader);
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x0000C0BC File Offset: 0x0000A2BC
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			reader.ReadStartElement();
			base.Name = reader.ReadElementString("Name");
			base.Time = reader.ReadElementDateTime("Time");
			this.Value = reader.ReadElementDouble("Value");
			this.AlarmActive = reader.ReadElementBoolean("AlarmActive");
			reader.ReadEndElement();
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x0000C120 File Offset: 0x0000A320
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString("Name", base.Name);
			writer.WriteElementDateTime("Time", base.Time);
			writer.WriteElementDouble("Value", this.Value, this.Configuration.FormatString);
			writer.WriteElementBoolean("AlarmActive", this.AlarmActive);
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x0000C17C File Offset: 0x0000A37C
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x04000342 RID: 834
		public const string XmlElementName = "ScalarMeasurement";

		// Token: 0x04000343 RID: 835
		private const string ValuePropertyName = "Value";

		// Token: 0x04000344 RID: 836
		private double m_Accum;

		// Token: 0x04000345 RID: 837
		private double m_Accum2;

		// Token: 0x04000346 RID: 838
		private double m_Count;

		// Token: 0x04000347 RID: 839
		private double _value;
	}
}

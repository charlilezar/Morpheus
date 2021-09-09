using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Model.Configurations
{
	// Token: 0x02000061 RID: 97
	public class DoseRateMeasurementConfiguration : ScalarMeasurementConfiguration, IXmlSerializable
	{
		// Token: 0x06000213 RID: 531 RVA: 0x00005490 File Offset: 0x00003690
		public DoseRateMeasurementConfiguration()
		{
			DoseRateMeasurementConfiguration.Instance = this;
			this.RoentgenToSievert = 0.0087;
			this.DoseRateTimeConstant = 3;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x000054B4 File Offset: 0x000036B4
		public DoseRateMeasurementConfiguration(ConfigurationDefinition configDef, UnitCategory unitCategory, FormatType formatType, string formatString, bool notifyEnable, TimeSpan updateInterval, TimeSpan minUpdateInterval, TimeSpan loggingInterval, MeasurementSource source, double mean, double sd, bool factoryUse) : base(configDef, unitCategory, formatType, formatString, notifyEnable, updateInterval, minUpdateInterval, loggingInterval, source, mean, sd, factoryUse)
		{
			DoseRateMeasurementConfiguration.Instance = this;
			this._roentgenToSievert = 0.0087;
			this.DoseRateTimeConstant = 3;
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000215 RID: 533 RVA: 0x000054F8 File Offset: 0x000036F8
		// (set) Token: 0x06000216 RID: 534 RVA: 0x00005500 File Offset: 0x00003700
		[Category("DoseRate")]
		public int DoseRateTimeConstant
		{
			get
			{
				return this._doseRateTimeConstant;
			}
			set
			{
				if (value >= 0 && value <= 10)
				{
					this._doseRateTimeConstant = value;
					DoseRateMeasurementConfiguration._filterFactor = 1.0 / ((double)value + 1.0);
				}
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000217 RID: 535 RVA: 0x0000552D File Offset: 0x0000372D
		// (set) Token: 0x06000218 RID: 536 RVA: 0x00005535 File Offset: 0x00003735
		[Category("DoseRate")]
		public double RoentgenToSievert
		{
			get
			{
				return this._roentgenToSievert;
			}
			set
			{
				if (value > 0.001 && value < 0.02)
				{
					this._roentgenToSievert = value;
				}
			}
		}

		// Token: 0x06000219 RID: 537 RVA: 0x00005558 File Offset: 0x00003758
		public new static DoseRateMeasurementConfiguration Deserialize(string xmlString)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(DoseRateMeasurementConfiguration));
			DoseRateMeasurementConfiguration result;
			using (StringReader stringReader = new StringReader(xmlString))
			{
				result = (DoseRateMeasurementConfiguration)xmlSerializer.Deserialize(stringReader);
			}
			return result;
		}

		// Token: 0x0600021A RID: 538 RVA: 0x000055A8 File Offset: 0x000037A8
		public new void ReadXml(XmlReader reader)
		{
			try
			{
				reader.MoveToContent();
				reader.ReadStartElement();
				base.ReadMeasurementBaseXml(reader);
				base.Units = reader.ReadElementEnum<UnitType>("Units");
				base.UnitCategory = reader.ReadElementEnum<UnitCategory>("UnitCategory");
				base.Formatting = reader.ReadElementEnum<FormatType>("Formatting");
				base.FormatString = reader.ReadElementString("FormatString");
				this.DoseRateTimeConstant = reader.ReadElementInteger("DoseRateTimeConstant");
				this.RoentgenToSievert = reader.ReadElementDouble("RoentgenToSievert");
				reader.ReadEndElement();
				base.DeserializeSuccess = true;
			}
			catch (Exception)
			{
				base.DeserializeSuccess = false;
			}
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00005658 File Offset: 0x00003858
		public new void WriteXml(XmlWriter writer)
		{
			base.WriteMeasurementBaseXml(writer);
			writer.WriteElementEnum("Units", base.Units);
			writer.WriteElementEnum("UnitCategory", base.UnitCategory);
			writer.WriteElementEnum("Formatting", base.Formatting);
			writer.WriteElementString("FormatString", base.FormatString);
			writer.WriteElementInteger("DoseRateTimeConstant", this.DoseRateTimeConstant);
			writer.WriteElementDouble("RoentgenToSievert", this.RoentgenToSievert, "0.000e0");
		}

		// Token: 0x0600021C RID: 540 RVA: 0x000056D7 File Offset: 0x000038D7
		public new XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x040001E1 RID: 481
		[Browsable(false)]
		public static DoseRateMeasurementConfiguration Instance;

		// Token: 0x040001E2 RID: 482
		private int _doseRateTimeConstant;

		// Token: 0x040001E3 RID: 483
		private double _roentgenToSievert;

		// Token: 0x040001E4 RID: 484
		private static double _filterFactor;
	}
}

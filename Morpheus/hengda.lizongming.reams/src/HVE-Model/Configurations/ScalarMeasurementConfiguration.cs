using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Model.Configurations
{
	// Token: 0x02000060 RID: 96
	[XmlInclude(typeof(DoseRateMeasurementConfiguration))]
	public class ScalarMeasurementConfiguration : MeasurementConfiguration, IXmlSerializable
	{
		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060001F5 RID: 501 RVA: 0x00004CA5 File Offset: 0x00002EA5
		// (set) Token: 0x060001F6 RID: 502 RVA: 0x00004CAD File Offset: 0x00002EAD
		[Browsable(false)]
		public double SimulatedMean { get; private set; }

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060001F7 RID: 503 RVA: 0x00004CB6 File Offset: 0x00002EB6
		// (set) Token: 0x060001F8 RID: 504 RVA: 0x00004CBE File Offset: 0x00002EBE
		[Browsable(false)]
		public double SimulatedSD { get; private set; }

		// Token: 0x060001F9 RID: 505 RVA: 0x00004CC7 File Offset: 0x00002EC7
		public ScalarMeasurementConfiguration()
		{
		}

		// Token: 0x060001FA RID: 506 RVA: 0x00004CD0 File Offset: 0x00002ED0
		public ScalarMeasurementConfiguration(ConfigurationDefinition configDef, UnitCategory unitCategory, FormatType formatType, string formatString, bool notifyEnable, TimeSpan updateInterval, TimeSpan minUpdateInterval, TimeSpan loggingInterval, MeasurementSource source, double mean, double sd, bool factoryUse) : base(configDef, notifyEnable, updateInterval, minUpdateInterval, loggingInterval, source, factoryUse)
		{
			this.UnitCategory = unitCategory;
			List<UnitType> list = UnitDefinitions.UnitCategoryDictionary[this.UnitCategory];
			this.Units = list[0];
			this.Formatting = formatType;
			this.FormatString = formatString;
			this.SimulatedMean = mean;
			this.SimulatedSD = sd;
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060001FB RID: 507 RVA: 0x00004D34 File Offset: 0x00002F34
		// (set) Token: 0x060001FC RID: 508 RVA: 0x00004D3C File Offset: 0x00002F3C
		[TypeConverter(typeof(UnitTypeConverter))]
		[Category("Measurement")]
		public UnitType Units
		{
			get
			{
				return this._units;
			}
			set
			{
				if (UnitDefinitions.UnitIdNameDictionary.ContainsKey(value))
				{
					this._units = value;
					this._unitString = UnitDefinitions.UnitIdNameDictionary[value];
					this._toBaseUnitConversionCallback = UnitDefinitions.ToBaseUnitConversionDictionary[value];
					this._fromBaseUnitConversionCallback = UnitDefinitions.FromBaseUnitConversionDictionary[value];
				}
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060001FD RID: 509 RVA: 0x00004D90 File Offset: 0x00002F90
		// (set) Token: 0x060001FE RID: 510 RVA: 0x00004D98 File Offset: 0x00002F98
		[ReadOnly(true)]
		[Browsable(false)]
		public UnitCategory UnitCategory { get; set; }

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060001FF RID: 511 RVA: 0x00004DA1 File Offset: 0x00002FA1
		// (set) Token: 0x06000200 RID: 512 RVA: 0x00004DA9 File Offset: 0x00002FA9
		[Browsable(false)]
		public FormatType Formatting { get; set; }

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000201 RID: 513 RVA: 0x00004DB2 File Offset: 0x00002FB2
		// (set) Token: 0x06000202 RID: 514 RVA: 0x00004DBA File Offset: 0x00002FBA
		[Browsable(false)]
		public string FormatString { get; set; }

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000203 RID: 515 RVA: 0x00004DC3 File Offset: 0x00002FC3
		// (set) Token: 0x06000204 RID: 516 RVA: 0x00004DCB File Offset: 0x00002FCB
		[Browsable(false)]
		public string UnitString
		{
			get
			{
				return this._unitString;
			}
			set
			{
				this._unitString = value;
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000205 RID: 517 RVA: 0x00004DD4 File Offset: 0x00002FD4
		[Browsable(false)]
		public Func<double, double> ToBaseUnitConversionCallback
		{
			get
			{
				return this._toBaseUnitConversionCallback;
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000206 RID: 518 RVA: 0x00004DDC File Offset: 0x00002FDC
		[Browsable(false)]
		public Func<double, double> FromBaseUnitConversionCallback
		{
			get
			{
				return this._fromBaseUnitConversionCallback;
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000207 RID: 519 RVA: 0x00004DE4 File Offset: 0x00002FE4
		// (set) Token: 0x06000208 RID: 520 RVA: 0x00004DEC File Offset: 0x00002FEC
		[Browsable(false)]
		public bool Selected { get; set; }

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000209 RID: 521 RVA: 0x00004DF5 File Offset: 0x00002FF5
		// (set) Token: 0x0600020A RID: 522 RVA: 0x00004DFD File Offset: 0x00002FFD
		[Browsable(false)]
		public object Tag { get; set; }

		// Token: 0x0600020B RID: 523 RVA: 0x00004E08 File Offset: 0x00003008
		public int GetRoundedDecadeIndex(double value)
		{
			double num = (value == 0.0) ? 0.0 : Math.Log10(Math.Abs(value));
			double num2;
			if (num >= 0.0)
			{
				num2 = (double)((int)Math.Floor(num));
			}
			else
			{
				num2 = (double)((int)(-(int)Math.Ceiling(-num)));
			}
			if (num2 < -15.0)
			{
				num2 = -15.0;
			}
			else if (num2 > 11.0)
			{
				num2 = 26.0;
			}
			return (int)num2 + 15;
		}

		// Token: 0x0600020C RID: 524 RVA: 0x00004E90 File Offset: 0x00003090
		private Tuple<string, string> ToStringWithUnits(double value)
		{
			Tuple<string, string> result;
			try
			{
				if (this.Formatting == FormatType.UseFormatString)
				{
					result = new Tuple<string, string>(value.ToString(this.FormatString), this._unitString);
				}
				else if (this.Formatting == FormatType.UseUnitPrefix)
				{
					int roundedDecadeIndex = this.GetRoundedDecadeIndex(value);
					string item = string.Format(ScalarMeasurementConfiguration.PrefixValueFormatString[roundedDecadeIndex], value * ScalarMeasurementConfiguration.ScaleFactor[roundedDecadeIndex]);
					string item2 = string.Format(ScalarMeasurementConfiguration.PrefixUnitsFormatString[roundedDecadeIndex], this._unitString);
					result = new Tuple<string, string>(item, item2);
				}
				else if (this.Formatting == FormatType.FormatAsTimeSpan)
				{
					TimeSpan timeSpan = TimeSpan.Zero;
					if (this.Units == UnitType.Minutes)
					{
						timeSpan = TimeSpan.FromMinutes(value);
					}
					else if (this.Units == UnitType.Hours)
					{
						timeSpan = TimeSpan.FromHours(value);
					}
					else if (this.Units == UnitType.Days)
					{
						timeSpan = TimeSpan.FromDays(value);
					}
					string item3 = string.Format("{0}.{1:00}:{2:00}:{3:00}", new object[]
					{
						timeSpan.Days,
						timeSpan.Hours,
						timeSpan.Minutes,
						timeSpan.Seconds
					});
					result = new Tuple<string, string>(item3, "");
				}
				else
				{
					result = new Tuple<string, string>("", "");
				}
			}
			catch (Exception)
			{
				result = new Tuple<string, string>("", "");
			}
			return result;
		}

		// Token: 0x0600020D RID: 525 RVA: 0x00005008 File Offset: 0x00003208
		public string FormattedValue(double value)
		{
			Tuple<string, string> tuple = this.ToStringWithUnits(value);
			return tuple.Item1 + " " + tuple.Item2;
		}

		// Token: 0x0600020E RID: 526 RVA: 0x00005034 File Offset: 0x00003234
		public static ScalarMeasurementConfiguration Deserialize(string xmlString)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ScalarMeasurementConfiguration));
			ScalarMeasurementConfiguration result;
			using (StringReader stringReader = new StringReader(xmlString))
			{
				result = (ScalarMeasurementConfiguration)xmlSerializer.Deserialize(stringReader);
			}
			return result;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00005084 File Offset: 0x00003284
		public void ReadXml(XmlReader reader)
		{
			try
			{
				reader.MoveToContent();
				reader.ReadStartElement();
				base.ReadMeasurementBaseXml(reader);
				this.Units = reader.ReadElementEnum<UnitType>("Units");
				this.UnitCategory = reader.ReadElementEnum<UnitCategory>("UnitCategory");
				this.Formatting = reader.ReadElementEnum<FormatType>("Formatting");
				this.FormatString = reader.ReadElementString("FormatString");
				this.UnitString = reader.ReadElementString("UnitString");
				reader.ReadEndElement();
				base.DeserializeSuccess = true;
			}
			catch (Exception)
			{
				base.DeserializeSuccess = false;
			}
		}

		// Token: 0x06000210 RID: 528 RVA: 0x00005124 File Offset: 0x00003324
		public void WriteXml(XmlWriter writer)
		{
			base.WriteMeasurementBaseXml(writer);
			writer.WriteElementEnum("Units", this.Units);
			writer.WriteElementEnum("UnitCategory", this.UnitCategory);
			writer.WriteElementEnum("Formatting", this.Formatting);
			writer.WriteElementString("FormatString", this.FormatString);
			writer.WriteElementString("UnitString", this.UnitString);
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000518D File Offset: 0x0000338D
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x040001D1 RID: 465
		[Browsable(false)]
		public static List<ScalarMeasurementConfiguration> MeasurementConfigurationList = new List<ScalarMeasurementConfiguration>();

		// Token: 0x040001D2 RID: 466
		[Browsable(false)]
		public static Dictionary<string, ScalarMeasurementConfiguration> MeasurementConfigurationDictionary = new Dictionary<string, ScalarMeasurementConfiguration>();

		// Token: 0x040001D3 RID: 467
		private UnitType _units;

		// Token: 0x040001D4 RID: 468
		private string _unitString;

		// Token: 0x040001D5 RID: 469
		private Func<double, double> _toBaseUnitConversionCallback;

		// Token: 0x040001D6 RID: 470
		private Func<double, double> _fromBaseUnitConversionCallback;

		// Token: 0x040001D7 RID: 471
		private static readonly string[] PrefixValueFormatString = new string[]
		{
			"{0:0.00}",
			"{0:0.00}",
			"{0:0.0}",
			"{0:0.00}",
			"{0:0.00}",
			"{0:0.0}",
			"{0:0.00}",
			"{0:0.00}",
			"{0:0.0}",
			"{0:0.00}",
			"{0:0.00}",
			"{0:0.0}",
			"{0:0.00}",
			"{0:0.00}",
			"{0:0.0}",
			"{0:0.00}",
			"{0:0.00}",
			"{0:0.0}",
			"{0:0.00}",
			"{0:0.00}",
			"{0:0.0}",
			"{0:0.00}",
			"{0:0.00}",
			"{0:0.0}",
			"{0:0.00}",
			"{0:0.00}",
			"{0:0.0}"
		};

		// Token: 0x040001D8 RID: 472
		private static readonly string[] PrefixUnitsFormatString = new string[]
		{
			"f{0}",
			"f{0}",
			"f{0}",
			"p{0}",
			"p{0}",
			"p{0}",
			"n{0}",
			"n{0}",
			"n{0}",
			"u{0}",
			"u{0}",
			"u{0}",
			"m{0}",
			"m{0}",
			"m{0}",
			"{0}",
			"{0}",
			"{0}",
			"K{0}",
			"K{0}",
			"K{0}",
			"M{0}",
			"M{0}",
			"M{0}",
			"G{0}",
			"G{0}",
			"G{0}"
		};

		// Token: 0x040001D9 RID: 473
		private static readonly double[] ScaleFactor = new double[]
		{
			1000000000000000.0,
			1000000000000000.0,
			1000000000000000.0,
			1000000000000.0,
			1000000000000.0,
			1000000000000.0,
			1000000000.0,
			1000000000.0,
			1000000000.0,
			1000000.0,
			1000000.0,
			1000000.0,
			1000.0,
			1000.0,
			1000.0,
			1.0,
			1.0,
			1.0,
			0.001,
			0.001,
			0.001,
			1E-06,
			1E-06,
			1E-06,
			1E-09,
			1E-09,
			1E-09
		};
	}
}

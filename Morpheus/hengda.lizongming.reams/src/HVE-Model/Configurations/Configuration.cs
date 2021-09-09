using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace Model.Configurations
{
	// Token: 0x0200004E RID: 78
	public abstract class Configuration
	{
		// Token: 0x06000196 RID: 406 RVA: 0x00003792 File Offset: 0x00001992
		protected Configuration()
		{
			this.ConfigurationId = 0;
			this.Name = "";
			this.Type = "";
			this.DisplayEnabled = true;
			this.DateModified = DateTime.Now;
		}

		// Token: 0x06000197 RID: 407 RVA: 0x000037CC File Offset: 0x000019CC
		protected Configuration(ConfigurationDefinition configDef, bool factoryUseOnly)
		{
			this.Name = configDef.Name;
			this.Type = configDef.ConfigType;
			this.ConfigurationId = configDef.ConfigId;
			this.FactoryUseOnly = factoryUseOnly;
			this.DisplayEnabled = true;
			this.DateModified = DateTime.Now;
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000198 RID: 408 RVA: 0x0000381C File Offset: 0x00001A1C
		// (set) Token: 0x06000199 RID: 409 RVA: 0x00003824 File Offset: 0x00001A24
		[Browsable(false)]
		public int ConfigurationId { get; set; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x0600019A RID: 410 RVA: 0x0000382D File Offset: 0x00001A2D
		// (set) Token: 0x0600019B RID: 411 RVA: 0x00003835 File Offset: 0x00001A35
		[Category("General")]
		[ReadOnly(true)]
		public string Name { get; set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x0600019C RID: 412 RVA: 0x0000383E File Offset: 0x00001A3E
		// (set) Token: 0x0600019D RID: 413 RVA: 0x00003846 File Offset: 0x00001A46
		[Category("General")]
		[ReadOnly(true)]
		public string Type { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x0600019E RID: 414 RVA: 0x0000384F File Offset: 0x00001A4F
		// (set) Token: 0x0600019F RID: 415 RVA: 0x00003857 File Offset: 0x00001A57
		[ReadOnly(true)]
		[Category("General")]
		public DateTime DateModified { get; set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060001A0 RID: 416 RVA: 0x00003860 File Offset: 0x00001A60
		// (set) Token: 0x060001A1 RID: 417 RVA: 0x00003868 File Offset: 0x00001A68
		[Browsable(false)]
		public bool FactoryUseOnly { get; set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060001A2 RID: 418 RVA: 0x00003871 File Offset: 0x00001A71
		// (set) Token: 0x060001A3 RID: 419 RVA: 0x00003879 File Offset: 0x00001A79
		[Browsable(false)]
		public bool DisplayEnabled { get; set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060001A4 RID: 420 RVA: 0x00003882 File Offset: 0x00001A82
		[Browsable(false)]
		protected static int ConfigurationCount
		{
			get
			{
				return Configuration.ConfigurationList.Count;
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060001A5 RID: 421 RVA: 0x0000388E File Offset: 0x00001A8E
		// (set) Token: 0x060001A6 RID: 422 RVA: 0x00003896 File Offset: 0x00001A96
		[Browsable(false)]
		public bool DeserializeSuccess { get; protected set; }

		// Token: 0x060001A7 RID: 423 RVA: 0x0000389F File Offset: 0x00001A9F
		public override string ToString()
		{
			return "Configuration [" + this.Name + "]";
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x000038B8 File Offset: 0x00001AB8
		protected void ReadBaseXml(XmlReader reader)
		{
			this.ConfigurationId = reader.ReadElementInteger("ConfigurationId");
			this.Name = reader.ReadElementString("Name");
			this.Type = reader.ReadElementString("Type");
			this.DateModified = reader.ReadElementDateTime("DateModified");
			this.FactoryUseOnly = reader.ReadElementBoolean("FactoryUseOnly");
			this.DisplayEnabled = reader.ReadElementBoolean("DisplayEnabled");
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000392C File Offset: 0x00001B2C
		protected void WriteBaseXml(XmlWriter writer)
		{
			try
			{
				writer.WriteElementInteger("ConfigurationId", this.ConfigurationId);
				writer.WriteElementString("Name", this.Name);
				writer.WriteElementString("Type", this.Type);
				writer.WriteElementDateTime("DateModified", this.DateModified);
				writer.WriteElementBoolean("FactoryUseOnly", this.FactoryUseOnly);
				writer.WriteElementBoolean("DisplayEnabled", this.DisplayEnabled);
			}
			catch
			{
			}
		}

		// Token: 0x060001AA RID: 426 RVA: 0x000039B4 File Offset: 0x00001BB4
		public static Configuration CreateConfiguration(string xmlString)
		{
			Configuration result = null;
			string stringBetween = xmlString.GetStringBetween('<', '>');
			xmlString.ParseForElement("Name");
			string key;
			switch (key = stringBetween)
			{
			case "ScalarMeasurementConfiguration":
				result = ScalarMeasurementConfiguration.Deserialize(xmlString);
				break;
			case "DoseRateMeasurementConfiguration":
				result = DoseRateMeasurementConfiguration.Deserialize(xmlString);
				break;
			case "CompositeMeasurementConfiguration":
				result = CompositeMeasurementConfiguration.Deserialize(xmlString);
				break;
			case "EventConfiguration":
				result = EventConfiguration.Deserialize(xmlString);
				break;
			case "AlarmEventConfiguration":
				result = AlarmEventConfiguration.Deserialize(xmlString);
				break;
			case "RSDetectionConfiguration":
				result = RSDetectionConfiguration.Deserialize(xmlString);
				break;
			case "ExternalDisplayConfiguration":
				result = ExternalDisplayConfiguration.Deserialize(xmlString);
				break;
			case "WeatherStationConfiguration":
				result = WeatherStationConfiguration.Deserialize(xmlString);
				break;
			case "SerialPortConfiguration":
				result = SerialPortConfiguration.Deserialize(xmlString);
				break;
			}
			return result;
		}

		// Token: 0x040000DF RID: 223
		public static List<Configuration> ConfigurationList = new List<Configuration>();

		// Token: 0x040000E0 RID: 224
		private static readonly Dictionary<string, Configuration> ConfigurationNameDictionary = new Dictionary<string, Configuration>();
	}
}

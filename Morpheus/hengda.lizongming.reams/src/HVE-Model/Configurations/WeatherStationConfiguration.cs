using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Model.Configurations
{
	// Token: 0x02000071 RID: 113
	public class WeatherStationConfiguration : Configuration, IXmlSerializable
	{
		// Token: 0x060002CD RID: 717 RVA: 0x000073E4 File Offset: 0x000055E4
		public WeatherStationConfiguration() : base(ConfigurationDefinition.WeatherStationConfiguration, false)
		{
			this.Enabled = false;
			this.CommPort = AviliableCommPorts.COM1;
			this.Model = WeatherStations.WXT520;
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x060002CE RID: 718 RVA: 0x00007407 File Offset: 0x00005607
		public static WeatherStationConfiguration Instance
		{
			get
			{
				return WeatherStationConfiguration._instance;
			}
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x060002CF RID: 719 RVA: 0x0000740E File Offset: 0x0000560E
		// (set) Token: 0x060002D0 RID: 720 RVA: 0x00007416 File Offset: 0x00005616
		[Category("WeatherStation")]
		public AviliableCommPorts CommPort { get; set; }

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x060002D1 RID: 721 RVA: 0x0000741F File Offset: 0x0000561F
		// (set) Token: 0x060002D2 RID: 722 RVA: 0x00007427 File Offset: 0x00005627
		[Category("WeatherStation")]
		public WeatherStations Model { get; set; }

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x060002D3 RID: 723 RVA: 0x00007430 File Offset: 0x00005630
		// (set) Token: 0x060002D4 RID: 724 RVA: 0x00007438 File Offset: 0x00005638
		[Category("WeatherStation")]
		public bool Enabled { get; set; }

		// Token: 0x060002D5 RID: 725 RVA: 0x00007444 File Offset: 0x00005644
		public static WeatherStationConfiguration Deserialize(string xmlString)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(WeatherStationConfiguration));
			WeatherStationConfiguration result;
			using (StringReader stringReader = new StringReader(xmlString))
			{
				result = (WeatherStationConfiguration)xmlSerializer.Deserialize(stringReader);
			}
			return result;
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x00007494 File Offset: 0x00005694
		public void ReadXml(XmlReader reader)
		{
			try
			{
				reader.MoveToContent();
				reader.ReadStartElement();
				base.ReadBaseXml(reader);
				this.CommPort = reader.ReadElementEnum<AviliableCommPorts>("CommPort");
				this.Model = reader.ReadElementEnum<WeatherStations>("Model");
				this.Enabled = reader.ReadElementBoolean("Enabled");
				reader.ReadEndElement();
				base.DeserializeSuccess = true;
			}
			catch (Exception)
			{
				base.DeserializeSuccess = false;
			}
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x00007514 File Offset: 0x00005714
		public void WriteXml(XmlWriter writer)
		{
			base.WriteBaseXml(writer);
			writer.WriteElementEnum("CommPort", this.CommPort);
			writer.WriteElementEnum("Model", this.Model);
			writer.WriteElementBoolean("Enabled", this.Enabled);
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x00007550 File Offset: 0x00005750
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x04000220 RID: 544
		private static WeatherStationConfiguration _instance;
	}
}

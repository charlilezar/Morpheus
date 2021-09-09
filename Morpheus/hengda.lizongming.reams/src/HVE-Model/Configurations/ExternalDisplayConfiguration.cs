using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Model.Configurations
{
	// Token: 0x02000063 RID: 99
	public class ExternalDisplayConfiguration : Configuration, IXmlSerializable
	{
		// Token: 0x06000222 RID: 546 RVA: 0x000057E3 File Offset: 0x000039E3
		public ExternalDisplayConfiguration() : base(ConfigurationDefinition.DisplayConfiguration, false)
		{
			this.Enabled = false;
			this.CommPort = AviliableCommPorts.COM3;
			this.UpdateWithLoggingInterval = false;
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000223 RID: 547 RVA: 0x00005806 File Offset: 0x00003A06
		// (set) Token: 0x06000224 RID: 548 RVA: 0x0000580E File Offset: 0x00003A0E
		[Category("Display")]
		public AviliableCommPorts CommPort { get; set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000225 RID: 549 RVA: 0x00005817 File Offset: 0x00003A17
		// (set) Token: 0x06000226 RID: 550 RVA: 0x0000581F File Offset: 0x00003A1F
		[Category("Display")]
		public bool Enabled { get; set; }

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000227 RID: 551 RVA: 0x00005828 File Offset: 0x00003A28
		// (set) Token: 0x06000228 RID: 552 RVA: 0x00005830 File Offset: 0x00003A30
		[Category("Display")]
		[Description("True - Update the external display at the same rate interval as the dose rate logging interval.\nFalse - Update the external display at the same rate interval as the dose rate update interval.\nNote: The dose rate value logged to the database is an average of the update values obtained during that logging interval.")]
		public bool UpdateWithLoggingInterval { get; set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000229 RID: 553 RVA: 0x00005839 File Offset: 0x00003A39
		public static ExternalDisplayConfiguration Instance
		{
			get
			{
				return ExternalDisplayConfiguration._instance;
			}
		}

		// Token: 0x0600022A RID: 554 RVA: 0x00005840 File Offset: 0x00003A40
		public static ExternalDisplayConfiguration Deserialize(string xmlString)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExternalDisplayConfiguration));
			ExternalDisplayConfiguration result;
			using (StringReader stringReader = new StringReader(xmlString))
			{
				result = (ExternalDisplayConfiguration)xmlSerializer.Deserialize(stringReader);
			}
			return result;
		}

		// Token: 0x0600022B RID: 555 RVA: 0x00005890 File Offset: 0x00003A90
		public void ReadXml(XmlReader reader)
		{
			try
			{
				reader.MoveToContent();
				reader.ReadStartElement();
				base.ReadBaseXml(reader);
				this.CommPort = reader.ReadElementEnum<AviliableCommPorts>("CommPort");
				this.Enabled = reader.ReadElementBoolean("Enabled");
				this.UpdateWithLoggingInterval = reader.ReadElementBoolean("UpdateWithLoggingInterval");
				reader.ReadEndElement();
				base.DeserializeSuccess = true;
			}
			catch (Exception)
			{
				base.DeserializeSuccess = false;
			}
		}

		// Token: 0x0600022C RID: 556 RVA: 0x00005910 File Offset: 0x00003B10
		public void WriteXml(XmlWriter writer)
		{
			base.WriteBaseXml(writer);
			writer.WriteElementEnum("CommPort", this.CommPort);
			writer.WriteElementBoolean("Enabled", this.Enabled);
			writer.WriteElementBoolean("UpdateWithLoggingInterval", this.UpdateWithLoggingInterval);
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000594C File Offset: 0x00003B4C
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x040001E6 RID: 486
		private static ExternalDisplayConfiguration _instance;
	}
}

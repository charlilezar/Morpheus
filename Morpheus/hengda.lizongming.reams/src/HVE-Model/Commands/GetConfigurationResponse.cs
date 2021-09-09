using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Model.Configurations;

namespace Model.Commands
{
	// Token: 0x02000013 RID: 19
	public class GetConfigurationResponse : IXmlSerializable
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000050 RID: 80 RVA: 0x000023EF File Offset: 0x000005EF
		// (set) Token: 0x0600004F RID: 79 RVA: 0x000023E6 File Offset: 0x000005E6
		public Configuration Config { get; set; }

		// Token: 0x06000051 RID: 81 RVA: 0x000023F7 File Offset: 0x000005F7
		public GetConfigurationResponse()
		{
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000023FF File Offset: 0x000005FF
		public GetConfigurationResponse(Configuration config)
		{
			this.Config = config;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x0000240E File Offset: 0x0000060E
		public override string ToString()
		{
			if (this.Config != null)
			{
				return this.Config.Name;
			}
			return "";
		}

		// Token: 0x06000054 RID: 84 RVA: 0x0000242C File Offset: 0x0000062C
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			reader.ReadStartElement();
			string xmlString = reader.ReadOuterXml();
			this.Config = Configuration.CreateConfiguration(xmlString);
			reader.ReadEndElement();
		}

		// Token: 0x06000055 RID: 85 RVA: 0x0000245F File Offset: 0x0000065F
		public void WriteXml(XmlWriter writer)
		{
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002461 File Offset: 0x00000661
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x04000025 RID: 37
		public const string XmlElementName = "GetConfigurationResponse";
	}
}

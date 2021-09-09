using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using CommonLibrary;
using Model.Configurations;

namespace Model.Commands
{
	// Token: 0x02000014 RID: 20
	public class SetConfiguration : IXmlSerializable
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000058 RID: 88 RVA: 0x0000246D File Offset: 0x0000066D
		// (set) Token: 0x06000057 RID: 87 RVA: 0x00002464 File Offset: 0x00000664
		public Configuration Config { get; set; }

		// Token: 0x06000059 RID: 89 RVA: 0x00002475 File Offset: 0x00000675
		public SetConfiguration()
		{
		}

		// Token: 0x0600005A RID: 90 RVA: 0x0000247D File Offset: 0x0000067D
		public SetConfiguration(Configuration config)
		{
			this.Config = config;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x0000248C File Offset: 0x0000068C
		public void ReadXml(XmlReader reader)
		{
		}

		// Token: 0x0600005C RID: 92 RVA: 0x0000248E File Offset: 0x0000068E
		public void WriteXml(XmlWriter writer)
		{
			Serializers.Serializer(this.Config, writer);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000249C File Offset: 0x0000069C
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x04000027 RID: 39
		public const string XmlElementName = "SetConfiguration";
	}
}

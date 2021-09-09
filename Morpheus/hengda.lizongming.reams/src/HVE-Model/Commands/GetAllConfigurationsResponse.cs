using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Model.Configurations;

namespace Model.Commands
{
	// Token: 0x02000012 RID: 18
	public class GetAllConfigurationsResponse : IXmlSerializable
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000048 RID: 72 RVA: 0x0000235C File Offset: 0x0000055C
		// (set) Token: 0x06000049 RID: 73 RVA: 0x00002364 File Offset: 0x00000564
		public List<Configuration> ConfigurationList { get; set; }

		// Token: 0x0600004A RID: 74 RVA: 0x0000236D File Offset: 0x0000056D
		public GetAllConfigurationsResponse()
		{
			this.ConfigurationList = new List<Configuration>();
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00002380 File Offset: 0x00000580
		public GetAllConfigurationsResponse(List<Configuration> configList)
		{
			this.ConfigurationList = configList;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002390 File Offset: 0x00000590
		public void ReadXml(XmlReader reader)
		{
			this.ConfigurationList = new List<Configuration>();
			reader.MoveToContent();
			reader.ReadStartElement();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				string xmlString = reader.ReadOuterXml();
				Configuration item = Configuration.CreateConfiguration(xmlString);
				this.ConfigurationList.Add(item);
			}
			reader.ReadEndElement();
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000023E1 File Offset: 0x000005E1
		public void WriteXml(XmlWriter writer)
		{
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000023E3 File Offset: 0x000005E3
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x04000023 RID: 35
		public const string XmlElementName = "GetAllConfigurationsResponse";
	}
}

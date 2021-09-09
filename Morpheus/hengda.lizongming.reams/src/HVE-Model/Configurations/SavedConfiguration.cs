using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using CommonLibrary;

namespace Model.Configurations
{
	// Token: 0x02000068 RID: 104
	public class SavedConfiguration : IXmlSerializable
	{
		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000242 RID: 578 RVA: 0x00005C2C File Offset: 0x00003E2C
		// (set) Token: 0x06000243 RID: 579 RVA: 0x00005C34 File Offset: 0x00003E34
		public string UnitSerialNumber { get; set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000244 RID: 580 RVA: 0x00005C3D File Offset: 0x00003E3D
		// (set) Token: 0x06000245 RID: 581 RVA: 0x00005C45 File Offset: 0x00003E45
		public string DaqSerialNumber { get; set; }

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000246 RID: 582 RVA: 0x00005C4E File Offset: 0x00003E4E
		// (set) Token: 0x06000247 RID: 583 RVA: 0x00005C56 File Offset: 0x00003E56
		public string ElectrometerSerialNumber { get; set; }

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000248 RID: 584 RVA: 0x00005C5F File Offset: 0x00003E5F
		// (set) Token: 0x06000249 RID: 585 RVA: 0x00005C67 File Offset: 0x00003E67
		public string FirmwareRevision { get; set; }

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x0600024A RID: 586 RVA: 0x00005C70 File Offset: 0x00003E70
		// (set) Token: 0x0600024B RID: 587 RVA: 0x00005C78 File Offset: 0x00003E78
		public List<Configuration> ConfigurationList { get; set; }

		// Token: 0x0600024C RID: 588 RVA: 0x00005C81 File Offset: 0x00003E81
		public SavedConfiguration()
		{
			this.ConfigurationList = new List<Configuration>();
		}

		// Token: 0x0600024D RID: 589 RVA: 0x00005C94 File Offset: 0x00003E94
		public SavedConfiguration(List<Configuration> configList)
		{
			this.ConfigurationList = configList;
		}

		// Token: 0x0600024E RID: 590 RVA: 0x00005CA4 File Offset: 0x00003EA4
		public void ReadXml(XmlReader reader)
		{
			this.ConfigurationList = new List<Configuration>();
			reader.MoveToContent();
			reader.ReadStartElement();
			this.UnitSerialNumber = reader.ReadElementString("UnitSerialNumber");
			this.DaqSerialNumber = reader.ReadElementString("DaqSerialNumber");
			this.ElectrometerSerialNumber = reader.ReadElementString("ElectrometerSerialNumber");
			this.FirmwareRevision = reader.ReadElementString("FirmwareRevision");
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				string xmlString = reader.ReadOuterXml();
				Configuration item = Configuration.CreateConfiguration(xmlString);
				this.ConfigurationList.Add(item);
			}
			reader.ReadEndElement();
		}

		// Token: 0x0600024F RID: 591 RVA: 0x00005D3C File Offset: 0x00003F3C
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString("UnitSerialNumber", this.UnitSerialNumber);
			writer.WriteElementString("DaqSerialNumber", this.DaqSerialNumber);
			writer.WriteElementString("ElectrometerSerialNumber", this.ElectrometerSerialNumber);
			writer.WriteElementString("FirmwareRevision", this.FirmwareRevision);
			foreach (Configuration obj in this.ConfigurationList)
			{
				Serializers.Serializer(obj, writer);
			}
		}

		// Token: 0x06000250 RID: 592 RVA: 0x00005DD4 File Offset: 0x00003FD4
		public XmlSchema GetSchema()
		{
			return null;
		}
	}
}

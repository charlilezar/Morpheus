using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using CommonLibrary;

namespace Model.Measurements
{
	// Token: 0x02000085 RID: 133
	public class BatteryMeasurement : CompositeMeasurement, IXmlSerializable
	{
		// Token: 0x060003AB RID: 939 RVA: 0x00009BCD File Offset: 0x00007DCD
		public BatteryMeasurement()
		{
			BatteryMeasurement.Instance = this;
			base.CompositeType = "BatteryMeasurement";
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x060003AC RID: 940 RVA: 0x00009BE6 File Offset: 0x00007DE6
		// (set) Token: 0x060003AD RID: 941 RVA: 0x00009BF3 File Offset: 0x00007DF3
		public override string CompositeValue
		{
			get
			{
				return Serializers.Serializer<SmartBatteryStatus>(this.BatteryStatus);
			}
			set
			{
				this.BatteryStatus = Serializers.Deserialize<SmartBatteryStatus>(value);
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x060003AE RID: 942 RVA: 0x00009C01 File Offset: 0x00007E01
		// (set) Token: 0x060003AF RID: 943 RVA: 0x00009C09 File Offset: 0x00007E09
		[XmlIgnore]
		public SmartBatteryStatus BatteryStatus { get; set; }

		// Token: 0x060003B0 RID: 944 RVA: 0x00009C14 File Offset: 0x00007E14
		public static BatteryMeasurement DeserializeFromStream(MemoryStream stream)
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				IgnoreComments = false,
				IgnoreWhitespace = true
			};
			XmlReader xmlReader = new XmlTextReader(stream);
			xmlReader = XmlReader.Create(xmlReader, settings);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(BatteryMeasurement));
			return (BatteryMeasurement)xmlSerializer.Deserialize(xmlReader);
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x00009C68 File Offset: 0x00007E68
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			reader.ReadStartElement();
			base.Name = reader.ReadElementString("Name");
			base.Time = reader.ReadElementDateTime("Time");
			base.CompositeType = reader.ReadElementString("CompositeType");
			this.BatteryStatus = new SmartBatteryStatus();
			this.BatteryStatus.ReadXml(reader);
			reader.ReadEndElement();
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x00009CD4 File Offset: 0x00007ED4
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString("Name", base.Name);
			writer.WriteElementDateTime("Time", base.Time);
			writer.WriteElementString("CompositeType", base.CompositeType);
			writer.WriteStartElement("CompositeValue");
			this.BatteryStatus.WriteXml(writer);
			writer.WriteEndElement();
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x00009D31 File Offset: 0x00007F31
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x040002C3 RID: 707
		public static BatteryMeasurement Instance;
	}
}

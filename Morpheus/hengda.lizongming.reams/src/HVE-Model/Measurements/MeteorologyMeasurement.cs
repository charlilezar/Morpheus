using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using CommonLibrary;

namespace Model.Measurements
{
	// Token: 0x02000087 RID: 135
	public class MeteorologyMeasurement : CompositeMeasurement, IXmlSerializable
	{
		// Token: 0x060003CF RID: 975 RVA: 0x00009E20 File Offset: 0x00008020
		public MeteorologyMeasurement()
		{
			MeteorologyMeasurement.Instance = this;
			base.CompositeType = "MeteorologyMeasurement";
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x060003D0 RID: 976 RVA: 0x00009E39 File Offset: 0x00008039
		// (set) Token: 0x060003D1 RID: 977 RVA: 0x00009E46 File Offset: 0x00008046
		public override string CompositeValue
		{
			get
			{
				return Serializers.Serializer<MeteorologicalData>(this.MetData);
			}
			set
			{
				this.MetData = Serializers.Deserialize<MeteorologicalData>(value);
			}
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x00009E54 File Offset: 0x00008054
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			reader.ReadStartElement();
			base.Name = reader.ReadElementString("Name");
			base.Time = reader.ReadElementDateTime("Time");
			base.CompositeType = reader.ReadElementString("CompositeType");
			reader.ReadEndElement();
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x00009EA8 File Offset: 0x000080A8
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString("Name", base.Name);
			writer.WriteElementDateTime("Time", base.Time);
			writer.WriteElementString("CompositeType", base.CompositeType);
			writer.WriteStartElement("CompositeValue");
			writer.WriteEndElement();
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x00009EF9 File Offset: 0x000080F9
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x040002D3 RID: 723
		public static MeteorologyMeasurement Instance;

		// Token: 0x040002D4 RID: 724
		public MeteorologicalData MetData;
	}
}

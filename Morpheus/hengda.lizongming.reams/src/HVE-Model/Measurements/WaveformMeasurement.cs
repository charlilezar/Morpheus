using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using CommonLibrary;

namespace Model.Measurements
{
	// Token: 0x0200008F RID: 143
	public class WaveformMeasurement : CompositeMeasurement, IXmlSerializable
	{
		// Token: 0x06000436 RID: 1078 RVA: 0x0000A5DC File Offset: 0x000087DC
		public WaveformMeasurement()
		{
			base.CompositeType = "WaveformMeasurement";
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000437 RID: 1079 RVA: 0x0000A5EF File Offset: 0x000087EF
		// (set) Token: 0x06000438 RID: 1080 RVA: 0x0000A5F6 File Offset: 0x000087F6
		public static WaveformMeasurement CurrentWaveformMeasurement { get; set; }

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06000439 RID: 1081 RVA: 0x0000A5FE File Offset: 0x000087FE
		// (set) Token: 0x0600043A RID: 1082 RVA: 0x0000A606 File Offset: 0x00008806
		[XmlIgnore]
		public ElectrometerWaveform Waveform { get; set; }

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x0600043B RID: 1083 RVA: 0x0000A60F File Offset: 0x0000880F
		// (set) Token: 0x0600043C RID: 1084 RVA: 0x0000A61C File Offset: 0x0000881C
		public override string CompositeValue
		{
			get
			{
				return Serializers.Serializer<ElectrometerWaveform>(this.Waveform);
			}
			set
			{
				this.Waveform = Serializers.Deserialize<ElectrometerWaveform>(value);
			}
		}

		// Token: 0x0600043D RID: 1085 RVA: 0x0000A62C File Offset: 0x0000882C
		public static WaveformMeasurement DeserializeFromStream(MemoryStream stream)
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				IgnoreComments = false,
				IgnoreWhitespace = true
			};
			XmlReader xmlReader = new XmlTextReader(stream);
			xmlReader = XmlReader.Create(xmlReader, settings);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(WaveformMeasurement));
			return (WaveformMeasurement)xmlSerializer.Deserialize(xmlReader);
		}

		// Token: 0x0600043E RID: 1086 RVA: 0x0000A680 File Offset: 0x00008880
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			reader.ReadStartElement();
			base.Name = reader.ReadElementString("Name");
			base.Time = reader.ReadElementDateTime("Time");
			base.CompositeType = reader.ReadElementString("CompositeType");
			this.Waveform = new ElectrometerWaveform();
			this.Waveform.ReadXml(reader);
			reader.ReadEndElement();
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x0000A6EC File Offset: 0x000088EC
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString("Name", base.Name);
			writer.WriteElementDateTime("Time", base.Time);
			writer.WriteElementString("CompositeType", base.CompositeType);
			writer.WriteStartElement("CompositeValue");
			this.Waveform.WriteXml(writer);
			writer.WriteEndElement();
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x0000A749 File Offset: 0x00008949
		public XmlSchema GetSchema()
		{
			return null;
		}
	}
}

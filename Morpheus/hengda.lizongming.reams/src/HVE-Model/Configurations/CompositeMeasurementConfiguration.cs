using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Model.Configurations
{
	// Token: 0x02000054 RID: 84
	public class CompositeMeasurementConfiguration : MeasurementConfiguration, IXmlSerializable
	{
		// Token: 0x060001E4 RID: 484 RVA: 0x000042F9 File Offset: 0x000024F9
		public CompositeMeasurementConfiguration()
		{
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x00004301 File Offset: 0x00002501
		public CompositeMeasurementConfiguration(ConfigurationDefinition configDef, bool notifyEnable, TimeSpan updateInterval, TimeSpan minUpdateInterval, TimeSpan loggingInterval, bool factoryUse) : base(configDef, notifyEnable, updateInterval, minUpdateInterval, loggingInterval, MeasurementSource.Self, factoryUse)
		{
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x00004314 File Offset: 0x00002514
		public static CompositeMeasurementConfiguration Deserialize(string xmlString)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(CompositeMeasurementConfiguration));
			CompositeMeasurementConfiguration result;
			using (StringReader stringReader = new StringReader(xmlString))
			{
				result = (CompositeMeasurementConfiguration)xmlSerializer.Deserialize(stringReader);
			}
			return result;
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00004364 File Offset: 0x00002564
		public void ReadXml(XmlReader reader)
		{
			try
			{
				reader.MoveToContent();
				reader.ReadStartElement();
				base.ReadMeasurementBaseXml(reader);
				reader.ReadEndElement();
				base.DeserializeSuccess = true;
			}
			catch (Exception)
			{
				base.DeserializeSuccess = false;
			}
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x000043B0 File Offset: 0x000025B0
		public void WriteXml(XmlWriter writer)
		{
			base.WriteMeasurementBaseXml(writer);
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x000043B9 File Offset: 0x000025B9
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x0400010B RID: 267
		[Browsable(false)]
		public static List<CompositeMeasurementConfiguration> CompositeMeasurementConfigurationList = new List<CompositeMeasurementConfiguration>();

		// Token: 0x0400010C RID: 268
		[Browsable(false)]
		public static Dictionary<string, CompositeMeasurementConfiguration> CompositeMeasurementConfigurationDictionary = new Dictionary<string, CompositeMeasurementConfiguration>();
	}
}

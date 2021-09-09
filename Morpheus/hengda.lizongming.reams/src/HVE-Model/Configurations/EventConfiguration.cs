using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Model.Configurations
{
	// Token: 0x0200004F RID: 79
	public class EventConfiguration : Configuration, IXmlSerializable
	{
		// Token: 0x060001AC RID: 428 RVA: 0x00003B08 File Offset: 0x00001D08
		public EventConfiguration()
		{
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00003B10 File Offset: 0x00001D10
		public EventConfiguration(ConfigurationDefinition configDef, bool logOnChange, EventConfiguration.NotifyCriteria notifyCriteria, bool readOnly, bool factoryUse) : base(configDef, factoryUse)
		{
			this.LogOnChange = logOnChange;
			this.Notify = notifyCriteria;
			this.ReadOnly = readOnly;
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060001AE RID: 430 RVA: 0x00003B31 File Offset: 0x00001D31
		// (set) Token: 0x060001AF RID: 431 RVA: 0x00003B39 File Offset: 0x00001D39
		[Category("Event")]
		public bool LogOnChange { get; set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x00003B42 File Offset: 0x00001D42
		// (set) Token: 0x060001B1 RID: 433 RVA: 0x00003B4A File Offset: 0x00001D4A
		[Category("Event")]
		public EventConfiguration.NotifyCriteria Notify { get; set; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060001B2 RID: 434 RVA: 0x00003B53 File Offset: 0x00001D53
		// (set) Token: 0x060001B3 RID: 435 RVA: 0x00003B5B File Offset: 0x00001D5B
		[Browsable(false)]
		public bool ReadOnly { get; set; }

		// Token: 0x060001B4 RID: 436 RVA: 0x00003B64 File Offset: 0x00001D64
		public static EventConfiguration Deserialize(string xmlString)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(EventConfiguration));
			EventConfiguration result;
			using (StringReader stringReader = new StringReader(xmlString))
			{
				result = (EventConfiguration)xmlSerializer.Deserialize(stringReader);
			}
			return result;
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00003BB4 File Offset: 0x00001DB4
		public void ReadXml(XmlReader reader)
		{
			try
			{
				reader.MoveToContent();
				reader.ReadStartElement();
				base.ReadBaseXml(reader);
				this.LogOnChange = reader.ReadElementBoolean("LogOnChange");
				this.Notify = reader.ReadElementEnum<EventConfiguration.NotifyCriteria>("Notify");
				this.ReadOnly = reader.ReadElementBoolean("ReadOnly");
				reader.ReadEndElement();
				base.DeserializeSuccess = true;
			}
			catch (Exception)
			{
				base.DeserializeSuccess = false;
			}
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x00003C34 File Offset: 0x00001E34
		public void WriteXml(XmlWriter writer)
		{
			base.WriteBaseXml(writer);
			writer.WriteElementBoolean("LogOnChange", this.LogOnChange);
			writer.WriteElementEnum("Notify", this.Notify);
			writer.WriteElementBoolean("ReadOnly", this.ReadOnly);
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x00003C70 File Offset: 0x00001E70
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x040000E8 RID: 232
		[Browsable(false)]
		public static List<EventConfiguration> EventConfigurationList = new List<EventConfiguration>();

		// Token: 0x040000E9 RID: 233
		[Browsable(false)]
		public static Dictionary<int, EventConfiguration> EventConfigurationDictionary = new Dictionary<int, EventConfiguration>();

		// Token: 0x02000050 RID: 80
		public enum NotifyCriteria
		{
			// Token: 0x040000EE RID: 238
			OnChange,
			// Token: 0x040000EF RID: 239
			Always,
			// Token: 0x040000F0 RID: 240
			Disable
		}
	}
}

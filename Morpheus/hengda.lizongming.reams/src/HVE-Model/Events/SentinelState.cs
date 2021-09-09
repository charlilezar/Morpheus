using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
//using GalaSoft.MvvmLight;
using Model.Configurations;

namespace Model.Events
{
	// Token: 0x02000092 RID: 146
	public class SentinelState :/* ObservableObject,*/ IXmlSerializable
	{
		// Token: 0x06000473 RID: 1139 RVA: 0x0000AF84 File Offset: 0x00009184
		public SentinelState()
		{
			this.Time = DateTime.MinValue;
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x0000AFDC File Offset: 0x000091DC
		public SentinelState(string name)
		{
			this.Name = name;
			this.Time = DateTime.MinValue;
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x0000B038 File Offset: 0x00009238
		public SentinelState(string name, DateTime time, string currentState, string priorState, string comment)
		{
			//this._name = name;
			//this._currentState = currentState;
			//this._priorState = priorState;
			//this._time = time;
			//this._comment = comment;

			this.Name = name;
			this.CurrentState = currentState;
			this.PriorState = priorState;
			this.Time = time;
			this.Comment = comment;
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06000476 RID: 1142 RVA: 0x0000B0A7 File Offset: 0x000092A7
		// (set) Token: 0x06000477 RID: 1143 RVA: 0x0000B0AF File Offset: 0x000092AF
		public string Name { get; set; }
		//{
		//	get
		//	{
		//		return this._name;
		//	}
		//	set
		//	{
		//		base.Set<string>("Name", ref this._name, value);
		//	}
		//}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06000478 RID: 1144 RVA: 0x0000B0C4 File Offset: 0x000092C4
		// (set) Token: 0x06000479 RID: 1145 RVA: 0x0000B0CC File Offset: 0x000092CC
		public DateTime Time { get; set; }
		//{
		//	get
		//	{
		//		return this._time;
		//	}
		//	set
		//	{
		//		base.Set<DateTime>("Time", ref this._time, value);
		//	}
		//}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x0600047A RID: 1146 RVA: 0x0000B0E1 File Offset: 0x000092E1
		// (set) Token: 0x0600047B RID: 1147 RVA: 0x0000B0E9 File Offset: 0x000092E9
		public string CurrentState { get; set; }
		//{
		//	get
		//	{
		//		return this._currentState;
		//	}
		//	set
		//	{
		//		base.Set<string>("CurrentState", ref this._currentState, value);
		//	}
		//}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x0600047C RID: 1148 RVA: 0x0000B0FE File Offset: 0x000092FE
		// (set) Token: 0x0600047D RID: 1149 RVA: 0x0000B106 File Offset: 0x00009306
		public string PriorState { get; set; }
		//{
		//	get
		//	{
		//		return this._priorState;
		//	}
		//	set
		//	{
		//		base.Set<string>("PriorState", ref this._priorState, value);
		//	}
		//}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x0600047E RID: 1150 RVA: 0x0000B11B File Offset: 0x0000931B
		// (set) Token: 0x0600047F RID: 1151 RVA: 0x0000B123 File Offset: 0x00009323
		public string Comment { get; set; }
		//{
		//	get
		//	{
		//		return this._comment;
		//	}
		//	set
		//	{
		//		base.Set<string>("Comment", ref this._comment, value);
		//	}
		//}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x06000480 RID: 1152 RVA: 0x0000B138 File Offset: 0x00009338
		// (set) Token: 0x06000481 RID: 1153 RVA: 0x0000B140 File Offset: 0x00009340
		[XmlIgnore]
		public EventConfiguration Configuration { get; set; }
		//{
		//	get
		//	{
		//		return this._configuration;
		//	}
		//	set
		//	{
		//		base.Set<EventConfiguration>("Configuration", ref this._configuration, value);
		//	}
		//}

		// Token: 0x06000482 RID: 1154 RVA: 0x0000B158 File Offset: 0x00009358
		public static SentinelState DeserializeFromStream(MemoryStream stream)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.IgnoreComments = false;
			xmlReaderSettings.IgnoreWhitespace = true;
			XmlReader xmlReader = new XmlTextReader(stream);
			xmlReader = XmlReader.Create(xmlReader, xmlReaderSettings);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SentinelState));
			return (SentinelState)xmlSerializer.Deserialize(xmlReader);
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x0000B1A8 File Offset: 0x000093A8
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			reader.ReadStartElement();
			this.Name = reader.ReadElementString("Name");
			this.Time = reader.ReadElementDateTime("Time");
			this.CurrentState = reader.ReadElementString("CurrentState");
			this.PriorState = reader.ReadElementString("PriorState");
			this.Comment = reader.ReadElementString("Comment");
			reader.ReadEndElement();
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x0000B220 File Offset: 0x00009420
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString("Name", this.Name);
			writer.WriteElementDateTime("Time", this.Time);
			writer.WriteElementString("CurrentState", this.CurrentState);
			writer.WriteElementString("PriorState", this.PriorState);
			writer.WriteElementString("Comment", this.Comment);
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x0000B282 File Offset: 0x00009482
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x04000308 RID: 776
		public const string XmlElementName = "EventState";

		// Token: 0x04000309 RID: 777
		private const string NamePropertyName = "Name";

		// Token: 0x0400030A RID: 778
		private const string TimePropertyName = "Time";

		// Token: 0x0400030B RID: 779
		private const string CurrentStatePropertyName = "CurrentState";

		// Token: 0x0400030C RID: 780
		private const string PriorStatePropertyName = "PriorState";

		// Token: 0x0400030D RID: 781
		private const string CommentPropertyName = "Comment";

		// Token: 0x0400030E RID: 782
		private const string ConfigurationPropertyName = "Configuration";

		//// Token: 0x0400030F RID: 783
		//private string _name = "";

		//// Token: 0x04000310 RID: 784
		//private DateTime _time = DateTime.Now;

		//// Token: 0x04000311 RID: 785
		//private string _currentState = "";

		//// Token: 0x04000312 RID: 786
		//private string _priorState = "";

		//// Token: 0x04000313 RID: 787
		//private string _comment = "";

		// Token: 0x04000314 RID: 788
		private EventConfiguration _configuration;
	}
}

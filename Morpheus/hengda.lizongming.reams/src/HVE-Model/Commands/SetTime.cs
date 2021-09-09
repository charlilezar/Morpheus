using System;
using System.Xml.Serialization;

namespace Model.Commands
{
	// Token: 0x02000044 RID: 68
	public class SetTime
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x0600018F RID: 399 RVA: 0x0000373D File Offset: 0x0000193D
		// (set) Token: 0x0600018E RID: 398 RVA: 0x00003734 File Offset: 0x00001934
		[XmlElement(ElementName = "Time")]
		public string TimeString { get; set; }

		// Token: 0x06000190 RID: 400 RVA: 0x00003745 File Offset: 0x00001945
		public SetTime()
		{
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000374D File Offset: 0x0000194D
		public SetTime(DateTime time, bool ignoreTimeZone) : this()
		{
			this.TimeString = time.ToString(ignoreTimeZone ? "yyyy-MM-ddTHH\\:mm\\:ss" : "yyyy-MM-ddTHH\\:mm\\:sszzz");
		}

		// Token: 0x040000C0 RID: 192
		public const string XmlElementName = "SetTime";
	}
}

using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Model.Measurements
{
	// Token: 0x02000083 RID: 131
	public class Measurement
	{
		// Token: 0x0600039C RID: 924 RVA: 0x00009AF0 File Offset: 0x00007CF0
		static Measurement()
		{
			Measurement.s_XmlNamespace.Add("", "");
			Measurement.s_XmlSettings = new XmlWriterSettings();
			Measurement.s_XmlSettings.Indent = true;
			Measurement.s_XmlSettings.OmitXmlDeclaration = true;
			Measurement.s_XmlSettings.Encoding = Encoding.ASCII;
		}

		// Token: 0x0600039D RID: 925 RVA: 0x00009B4A File Offset: 0x00007D4A
		public Measurement()
		{
		}

		// Token: 0x0600039E RID: 926 RVA: 0x00009B52 File Offset: 0x00007D52
		protected Measurement(string name)
		{
			this.Name = name;
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x0600039F RID: 927 RVA: 0x00009B61 File Offset: 0x00007D61
		// (set) Token: 0x060003A0 RID: 928 RVA: 0x00009B69 File Offset: 0x00007D69
		public string Name { get; set; }

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x060003A1 RID: 929 RVA: 0x00009B72 File Offset: 0x00007D72
		// (set) Token: 0x060003A2 RID: 930 RVA: 0x00009B7A File Offset: 0x00007D7A
		public DateTime Time { get; set; }

		// Token: 0x040002BA RID: 698
		protected static XmlWriterSettings s_XmlSettings;

		// Token: 0x040002BB RID: 699
		protected static XmlSerializerNamespaces s_XmlNamespace = new XmlSerializerNamespaces();
	}
}

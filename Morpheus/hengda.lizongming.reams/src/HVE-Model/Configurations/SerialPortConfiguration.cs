using System;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Model.Configurations
{
	// Token: 0x0200006E RID: 110
	public class SerialPortConfiguration : Configuration, IXmlSerializable
	{
		// Token: 0x0600029D RID: 669 RVA: 0x00006694 File Offset: 0x00004894
		public static SerialPortConfiguration Deserialize(string xmlString)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SerialPortConfiguration));
			SerialPortConfiguration result;
			using (StringReader stringReader = new StringReader(xmlString))
			{
				result = (SerialPortConfiguration)xmlSerializer.Deserialize(stringReader);
			}
			return result;
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x0600029E RID: 670 RVA: 0x000066E4 File Offset: 0x000048E4
		// (set) Token: 0x0600029F RID: 671 RVA: 0x000066EC File Offset: 0x000048EC
		[Category("SerialPort")]
		public ComBaudRates BaudRate { get; set; }

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060002A0 RID: 672 RVA: 0x000066F5 File Offset: 0x000048F5
		// (set) Token: 0x060002A1 RID: 673 RVA: 0x000066FD File Offset: 0x000048FD
		[Category("SerialPort")]
		public Parity Parity { get; set; }

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x060002A2 RID: 674 RVA: 0x00006706 File Offset: 0x00004906
		// (set) Token: 0x060002A3 RID: 675 RVA: 0x0000670E File Offset: 0x0000490E
		[Category("SerialPort")]
		public int NumberOfBits
		{
			get
			{
				return this._numberOfBits;
			}
			set
			{
				if (value == 7 || value == 8)
				{
					this._numberOfBits = value;
				}
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x060002A4 RID: 676 RVA: 0x0000671F File Offset: 0x0000491F
		// (set) Token: 0x060002A5 RID: 677 RVA: 0x00006727 File Offset: 0x00004927
		[Browsable(false)]
		public RtsControl RTSControl { get; set; }

		// Token: 0x060002A6 RID: 678 RVA: 0x00006730 File Offset: 0x00004930
		public void ReadXml(XmlReader reader)
		{
			try
			{
				reader.MoveToContent();
				reader.ReadStartElement();
				base.ReadBaseXml(reader);
				this.BaudRate = reader.ReadElementEnum<ComBaudRates>("BaudRate");
				this.Parity = reader.ReadElementEnum<Parity>("Parity");
				this.NumberOfBits = reader.ReadElementInteger("NumberOfBits");
				this.RTSControl = reader.ReadElementEnum<RtsControl>("RTSControl");
				reader.ReadEndElement();
				base.DeserializeSuccess = true;
			}
			catch (Exception)
			{
				base.DeserializeSuccess = false;
			}
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x000067C0 File Offset: 0x000049C0
		public void WriteXml(XmlWriter writer)
		{
			base.WriteBaseXml(writer);
			writer.WriteElementEnum("BaudRate", this.BaudRate);
			writer.WriteElementEnum("Parity", this.Parity);
			writer.WriteElementInteger("NumberOfBits", this.NumberOfBits);
			writer.WriteElementEnum("RTSControl", this.RTSControl);
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x00006818 File Offset: 0x00004A18
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x04000217 RID: 535
		private int _numberOfBits;
	}
}

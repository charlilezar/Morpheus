using System;

namespace Model.Commands
{
	// Token: 0x0200002D RID: 45
	public class SetElectrometerId
	{
		// Token: 0x06000133 RID: 307 RVA: 0x00003311 File Offset: 0x00001511
		public SetElectrometerId()
		{
		}

		// Token: 0x06000134 RID: 308 RVA: 0x0000332F File Offset: 0x0000152F
		public SetElectrometerId(string serialNumber, string revision) : this()
		{
			this.SerialNumber = serialNumber;
			this.Revision = revision;
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000136 RID: 310 RVA: 0x00003368 File Offset: 0x00001568
		// (set) Token: 0x06000135 RID: 309 RVA: 0x00003345 File Offset: 0x00001545
		public string SerialNumber
		{
			get
			{
				return this._serialNumber;
			}
			set
			{
				if (value.Length > 20)
				{
					this._serialNumber = value.Substring(0, 20);
					return;
				}
				this._serialNumber = value;
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000138 RID: 312 RVA: 0x00003393 File Offset: 0x00001593
		// (set) Token: 0x06000137 RID: 311 RVA: 0x00003370 File Offset: 0x00001570
		public string Revision
		{
			get
			{
				return this._revision;
			}
			set
			{
				if (value.Length > 20)
				{
					this._revision = value.Substring(0, 20);
					return;
				}
				this._revision = value;
			}
		}

		// Token: 0x0400008F RID: 143
		public const string XmlElementName = "SetElectrometerId";

		// Token: 0x04000090 RID: 144
		public string _serialNumber = " ";

		// Token: 0x04000091 RID: 145
		public string _revision = " ";
	}
}

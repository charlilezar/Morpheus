using System;

namespace Model.Commands
{
	// Token: 0x02000004 RID: 4
	public class ElectrometerAcquisitionEnable
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000020A6 File Offset: 0x000002A6
		// (set) Token: 0x0600000B RID: 11 RVA: 0x000020AE File Offset: 0x000002AE
		public bool Enable { get; set; }

		// Token: 0x0600000C RID: 12 RVA: 0x000020B7 File Offset: 0x000002B7
		public ElectrometerAcquisitionEnable()
		{
			this.Enable = false;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000020C6 File Offset: 0x000002C6
		public ElectrometerAcquisitionEnable(bool enable)
		{
			this.Enable = enable;
		}

		// Token: 0x04000004 RID: 4
		public const string XmlElementName = "ElectrometerAcquisitionEnable";
	}
}

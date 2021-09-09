using System;

namespace Model.Commands
{
	// Token: 0x0200002A RID: 42
	public class CopyDatabaseFile
	{
		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600011E RID: 286 RVA: 0x00003211 File Offset: 0x00001411
		// (set) Token: 0x0600011D RID: 285 RVA: 0x00003208 File Offset: 0x00001408
		public string DestinationFileName { get; set; }

		// Token: 0x0600011F RID: 287 RVA: 0x00003219 File Offset: 0x00001419
		public CopyDatabaseFile()
		{
			this.DestinationFileName = "Copy_RSDetection.sdf";
		}

		// Token: 0x06000120 RID: 288 RVA: 0x0000322C File Offset: 0x0000142C
		public CopyDatabaseFile(string destinationFileName)
		{
			this.DestinationFileName = destinationFileName;
		}

		// Token: 0x04000084 RID: 132
		public const string XmlElementName = "CopyDatabaseFile";
	}
}

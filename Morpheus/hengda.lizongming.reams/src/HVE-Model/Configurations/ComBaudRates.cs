using System;
using System.ComponentModel;
//using CommonLibrary;

namespace Model.Configurations
{
	// Token: 0x0200005E RID: 94
	public enum ComBaudRates
	{
		// Token: 0x040001BD RID: 445
		////[FieldDisplayName("110")]
		CBR_110 = 110,
		// Token: 0x040001BE RID: 446
		//[FieldDisplayName("Not defined")]
		[Description("300")]
		CBR_300 = 300,
		// Token: 0x040001BF RID: 447
		//[FieldDisplayName("600")]
		CBR_600 = 600,
		// Token: 0x040001C0 RID: 448
		//[FieldDisplayName("1200")]
		CBR_1200 = 1200,
		// Token: 0x040001C1 RID: 449
		//[FieldDisplayName("2400")]
		CBR_2400 = 2400,
		// Token: 0x040001C2 RID: 450
		//[FieldDisplayName("4800")]
		CBR_4800 = 4800,
		// Token: 0x040001C3 RID: 451
		//[FieldDisplayName("9600")]
		CBR_9600 = 9600,
		// Token: 0x040001C4 RID: 452
		//[FieldDisplayName("14400")]
		CBR_14400 = 14400,
		// Token: 0x040001C5 RID: 453
		//[FieldDisplayName("19200")]
		CBR_19200 = 19200,
		// Token: 0x040001C6 RID: 454
		//[FieldDisplayName("38400")]
		CBR_38400 = 38400,
		// Token: 0x040001C7 RID: 455
		//[FieldDisplayName("56000")]
		CBR_56000 = 56000,
		// Token: 0x040001C8 RID: 456
		//[FieldDisplayName("57600")]
		CBR_57600 = 57600,
		// Token: 0x040001C9 RID: 457
		//[FieldDisplayName("115200")]
		CBR_115200 = 115200,
		// Token: 0x040001CA RID: 458
		//[FieldDisplayName("128000")]
		CBR_128000 = 128000,
		// Token: 0x040001CB RID: 459
		//[FieldDisplayName("230400")]
		CBR_230400 = 230400,
		// Token: 0x040001CC RID: 460
		//[FieldDisplayName("256000")]
		CBR_256000 = 256000,
		// Token: 0x040001CD RID: 461
		//[FieldDisplayName("460800")]
		CBR_460800 = 460800,
		// Token: 0x040001CE RID: 462
		//[FieldDisplayName("921600")]
		CBR_921600 = 921600
	}
}

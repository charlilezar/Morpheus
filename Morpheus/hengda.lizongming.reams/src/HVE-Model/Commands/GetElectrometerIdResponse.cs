using System;

namespace Model.Commands
{
	// Token: 0x0200002F RID: 47
	public class GetElectrometerIdResponse
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x0600013B RID: 315 RVA: 0x000033AC File Offset: 0x000015AC
		// (set) Token: 0x0600013A RID: 314 RVA: 0x000033A3 File Offset: 0x000015A3
		public string SerialNumber { get; set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600013D RID: 317 RVA: 0x000033BD File Offset: 0x000015BD
		// (set) Token: 0x0600013C RID: 316 RVA: 0x000033B4 File Offset: 0x000015B4
		public string Revision { get; set; }

		// Token: 0x0600013E RID: 318 RVA: 0x000033C5 File Offset: 0x000015C5
		public GetElectrometerIdResponse()
		{
		}

		// Token: 0x0600013F RID: 319 RVA: 0x000033CD File Offset: 0x000015CD
		public GetElectrometerIdResponse(string serialNumber, string revision)
		{
			this.SerialNumber = serialNumber;
			this.Revision = revision;
		}
	}
}

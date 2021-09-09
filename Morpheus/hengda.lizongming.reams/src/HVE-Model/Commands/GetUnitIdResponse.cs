using System;

namespace Model.Commands
{
	// Token: 0x02000017 RID: 23
	public class GetUnitIdResponse
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000063 RID: 99 RVA: 0x000024CF File Offset: 0x000006CF
		// (set) Token: 0x06000064 RID: 100 RVA: 0x000024D7 File Offset: 0x000006D7
		public UnitInfo UnitInformation { get; set; }

		// Token: 0x06000065 RID: 101 RVA: 0x000024E0 File Offset: 0x000006E0
		public GetUnitIdResponse()
		{
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000024E8 File Offset: 0x000006E8
		public GetUnitIdResponse(UnitInfo unitInfo)
		{
			this.UnitInformation = unitInfo;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000024F7 File Offset: 0x000006F7
		public void SetMACAddress(string macAddress)
		{
			this.UnitInformation.MacAddress = macAddress;
		}
	}
}

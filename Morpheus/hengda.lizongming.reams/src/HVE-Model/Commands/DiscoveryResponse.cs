using System;

namespace Model.Commands
{
	// Token: 0x0200002C RID: 44
	public class DiscoveryResponse
	{
		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000126 RID: 294 RVA: 0x00003277 File Offset: 0x00001477
		// (set) Token: 0x06000125 RID: 293 RVA: 0x0000326E File Offset: 0x0000146E
		public string HostName { get; set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000128 RID: 296 RVA: 0x00003288 File Offset: 0x00001488
		// (set) Token: 0x06000127 RID: 295 RVA: 0x0000327F File Offset: 0x0000147F
		public string Version { get; set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600012A RID: 298 RVA: 0x00003299 File Offset: 0x00001499
		// (set) Token: 0x06000129 RID: 297 RVA: 0x00003290 File Offset: 0x00001490
		public string IPAddress { get; set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x0600012C RID: 300 RVA: 0x000032AA File Offset: 0x000014AA
		// (set) Token: 0x0600012B RID: 299 RVA: 0x000032A1 File Offset: 0x000014A1
		public int IPPortNumber { get; set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x0600012E RID: 302 RVA: 0x000032BB File Offset: 0x000014BB
		// (set) Token: 0x0600012D RID: 301 RVA: 0x000032B2 File Offset: 0x000014B2
		public string CommChannelName { get; set; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000130 RID: 304 RVA: 0x000032CC File Offset: 0x000014CC
		// (set) Token: 0x0600012F RID: 303 RVA: 0x000032C3 File Offset: 0x000014C3
		public string UnitSerialNumber { get; set; }

		// Token: 0x06000131 RID: 305 RVA: 0x000032D4 File Offset: 0x000014D4
		public DiscoveryResponse()
		{
		}

		// Token: 0x06000132 RID: 306 RVA: 0x000032DC File Offset: 0x000014DC
		public DiscoveryResponse(string hostName, string version, string ipAddress, int ipPortNumber, string commChannelName, string unitSerialNumber)
		{
			this.HostName = hostName;
			this.Version = version;
			this.IPAddress = ipAddress;
			this.IPPortNumber = ipPortNumber;
			this.CommChannelName = commChannelName;
			this.UnitSerialNumber = unitSerialNumber;
		}

		// Token: 0x04000088 RID: 136
		public const string XmlElementName = "DiscoveryResponse";
	}
}

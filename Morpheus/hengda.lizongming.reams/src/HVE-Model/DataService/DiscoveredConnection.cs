using System;

namespace Model.DataService
{
	// Token: 0x02000076 RID: 118
	public class DiscoveredConnection
	{
		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000324 RID: 804 RVA: 0x00008E35 File Offset: 0x00007035
		// (set) Token: 0x06000325 RID: 805 RVA: 0x00008E3D File Offset: 0x0000703D
		public DiscoveredConnection.ConnectionType TypeOfConnection { get; set; }

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000326 RID: 806 RVA: 0x00008E46 File Offset: 0x00007046
		// (set) Token: 0x06000327 RID: 807 RVA: 0x00008E4E File Offset: 0x0000704E
		public string HostName { get; set; }

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000328 RID: 808 RVA: 0x00008E57 File Offset: 0x00007057
		// (set) Token: 0x06000329 RID: 809 RVA: 0x00008E5F File Offset: 0x0000705F
		public string IpAddress { get; set; }

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x0600032A RID: 810 RVA: 0x00008E68 File Offset: 0x00007068
		// (set) Token: 0x0600032B RID: 811 RVA: 0x00008E70 File Offset: 0x00007070
		public string Version { get; set; }

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x0600032C RID: 812 RVA: 0x00008E79 File Offset: 0x00007079
		// (set) Token: 0x0600032D RID: 813 RVA: 0x00008E81 File Offset: 0x00007081
		public int IpPortNumber { get; set; }

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x0600032E RID: 814 RVA: 0x00008E8A File Offset: 0x0000708A
		// (set) Token: 0x0600032F RID: 815 RVA: 0x00008E92 File Offset: 0x00007092
		public string CommPort { get; set; }

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000330 RID: 816 RVA: 0x00008E9B File Offset: 0x0000709B
		// (set) Token: 0x06000331 RID: 817 RVA: 0x00008EA3 File Offset: 0x000070A3
		public string UnitSerialNumber { get; set; }

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000332 RID: 818 RVA: 0x00008EAC File Offset: 0x000070AC
		public string ConnectionName
		{
			get
			{
				if (this.TypeOfConnection == DiscoveredConnection.ConnectionType.CommPort)
				{
					return this.CommPort;
				}
				return this.IpAddress;
			}
		}

		// Token: 0x06000333 RID: 819 RVA: 0x00008EC4 File Offset: 0x000070C4
		public DiscoveredConnection(DiscoveredConnection.ConnectionType type, string commPort, string hostName, string version, string unitSerialNumber)
		{
			this.TypeOfConnection = type;
			this.CommPort = commPort;
			this.HostName = hostName;
			this.Version = version;
			this.UnitSerialNumber = unitSerialNumber;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x00008EF1 File Offset: 0x000070F1
		public DiscoveredConnection(DiscoveredConnection.ConnectionType type, string ipAddress, int ipPortNumber, string hostName, string version, string unitSerialNumber)
		{
			this.TypeOfConnection = type;
			this.IpAddress = ipAddress;
			this.Version = version;
			this.IpPortNumber = ipPortNumber;
			this.HostName = hostName;
			this.UnitSerialNumber = unitSerialNumber;
		}

		// Token: 0x02000077 RID: 119
		public enum ConnectionType
		{
			// Token: 0x04000245 RID: 581
			Socket,
			// Token: 0x04000246 RID: 582
			CommPort
		}
	}
}

using System;
using Model.Electrometer;

namespace Model.Commands
{
	// Token: 0x02000030 RID: 48
	public class SetElectrometerConfiguration
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000140 RID: 320 RVA: 0x000033E3 File Offset: 0x000015E3
		// (set) Token: 0x06000141 RID: 321 RVA: 0x000033EB File Offset: 0x000015EB
		public bool WriteToEeprom { get; set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000143 RID: 323 RVA: 0x000033FD File Offset: 0x000015FD
		// (set) Token: 0x06000142 RID: 322 RVA: 0x000033F4 File Offset: 0x000015F4
		public ElectrometerConfiguration Configuration { get; set; }

		// Token: 0x06000144 RID: 324 RVA: 0x00003405 File Offset: 0x00001605
		public SetElectrometerConfiguration()
		{
		}

		// Token: 0x06000145 RID: 325 RVA: 0x0000340D File Offset: 0x0000160D
		public SetElectrometerConfiguration(ElectrometerConfiguration configuration, bool writeToEeprom) : this()
		{
			this.WriteToEeprom = writeToEeprom;
			this.Configuration = configuration;
		}

		// Token: 0x04000095 RID: 149
		public const string XmlElementName = "SetElectrometerConfiguration";
	}
}

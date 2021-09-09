using System;

namespace Model.Commands
{
	// Token: 0x02000029 RID: 41
	public class SetChargerParameters
	{
		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000117 RID: 279 RVA: 0x000031AB File Offset: 0x000013AB
		// (set) Token: 0x06000116 RID: 278 RVA: 0x000031A2 File Offset: 0x000013A2
		public double Voltage { get; set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000119 RID: 281 RVA: 0x000031BC File Offset: 0x000013BC
		// (set) Token: 0x06000118 RID: 280 RVA: 0x000031B3 File Offset: 0x000013B3
		public double Current { get; set; }

		// Token: 0x0600011A RID: 282 RVA: 0x000031C4 File Offset: 0x000013C4
		public SetChargerParameters()
		{
		}

		// Token: 0x0600011B RID: 283 RVA: 0x000031CC File Offset: 0x000013CC
		public SetChargerParameters(double voltage, double current)
		{
			this.Voltage = voltage;
			this.Current = current;
		}

		// Token: 0x0600011C RID: 284 RVA: 0x000031E2 File Offset: 0x000013E2
		public bool IsValid()
		{
			return this.Voltage < 8.4 && this.Current < 4.0;
		}

		// Token: 0x04000081 RID: 129
		public const string XmlElementName = "SetChargerParameters";
	}
}

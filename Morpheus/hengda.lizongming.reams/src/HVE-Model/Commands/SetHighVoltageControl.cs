using System;
using Model.Configurations;

namespace Model.Commands
{
	// Token: 0x02000039 RID: 57
	public class SetHighVoltageControl
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600015E RID: 350 RVA: 0x00003511 File Offset: 0x00001711
		// (set) Token: 0x0600015D RID: 349 RVA: 0x00003508 File Offset: 0x00001708
		public HighVoltageControlMethod ControlMethod { get; set; }

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000160 RID: 352 RVA: 0x00003522 File Offset: 0x00001722
		// (set) Token: 0x0600015F RID: 351 RVA: 0x00003519 File Offset: 0x00001719
		public double SampleInterval { get; set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000162 RID: 354 RVA: 0x00003533 File Offset: 0x00001733
		// (set) Token: 0x06000161 RID: 353 RVA: 0x0000352A File Offset: 0x0000172A
		public double Setpoint { get; set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000164 RID: 356 RVA: 0x00003544 File Offset: 0x00001744
		// (set) Token: 0x06000163 RID: 355 RVA: 0x0000353B File Offset: 0x0000173B
		public double Slope { get; set; }

		// Token: 0x06000165 RID: 357 RVA: 0x0000354C File Offset: 0x0000174C
		public SetHighVoltageControl()
		{
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00003554 File Offset: 0x00001754
		public SetHighVoltageControl(HighVoltageControlMethod method, double setPoint, double sampleInterval, double slope) : this()
		{
			this.ControlMethod = method;
			this.Setpoint = setPoint;
			this.SampleInterval = sampleInterval;
			this.Slope = slope;
		}

		// Token: 0x040000A4 RID: 164
		public const string XmlElementName = "SetHighVoltageControl";
	}
}

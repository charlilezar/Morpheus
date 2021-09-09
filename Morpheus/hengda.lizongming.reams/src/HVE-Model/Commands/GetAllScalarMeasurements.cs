using System;

namespace Model.Commands
{
	// Token: 0x0200003F RID: 63
	public class GetAllScalarMeasurements
	{
		// Token: 0x17000077 RID: 119
		// (get) Token: 0x0600016E RID: 366 RVA: 0x000035BF File Offset: 0x000017BF
		// (set) Token: 0x0600016F RID: 367 RVA: 0x000035C7 File Offset: 0x000017C7
		public bool UpdatesOnly { get; set; }

		// Token: 0x06000170 RID: 368 RVA: 0x000035D0 File Offset: 0x000017D0
		public GetAllScalarMeasurements()
		{
			this.UpdatesOnly = false;
		}

		// Token: 0x040000B0 RID: 176
		public const string XmlElementName = "GetAllScalarMeasurements";
	}
}

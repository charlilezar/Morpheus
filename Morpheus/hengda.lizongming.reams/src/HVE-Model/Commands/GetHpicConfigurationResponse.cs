using System;
using Model.Electrometer;

namespace Model.Commands
{
	// Token: 0x0200003C RID: 60
	public class GetHpicConfigurationResponse
	{
		// Token: 0x0600016A RID: 362 RVA: 0x00003598 File Offset: 0x00001798
		public GetHpicConfigurationResponse()
		{
		}

		// Token: 0x0600016B RID: 363 RVA: 0x000035A0 File Offset: 0x000017A0
		public GetHpicConfigurationResponse(HpicConfiguration hpicConfiguration)
		{
			this.HpicConfiguration = hpicConfiguration;
		}

		// Token: 0x040000AC RID: 172
		public const string XmlElementName = "GetHpicConfigurationResponse";

		// Token: 0x040000AD RID: 173
		public HpicConfiguration HpicConfiguration;
	}
}

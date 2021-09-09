using System;
using Model.Electrometer;

namespace Model.Commands
{
	// Token: 0x0200003A RID: 58
	public class SetHpicConfiguration
	{
		// Token: 0x06000167 RID: 359 RVA: 0x00003579 File Offset: 0x00001779
		public SetHpicConfiguration()
		{
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00003581 File Offset: 0x00001781
		public SetHpicConfiguration(HpicConfiguration hpicConfiguration)
		{
			this.HpicConfiguration = hpicConfiguration;
		}

		// Token: 0x040000A9 RID: 169
		public const string XmlElementName = "SetHpicConfiguration";

		// Token: 0x040000AA RID: 170
		public HpicConfiguration HpicConfiguration;
	}
}

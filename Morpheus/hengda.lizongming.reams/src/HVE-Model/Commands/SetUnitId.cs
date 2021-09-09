using System;

namespace Model.Commands
{
	// Token: 0x02000015 RID: 21
	public class SetUnitId
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600005E RID: 94 RVA: 0x0000249F File Offset: 0x0000069F
		// (set) Token: 0x0600005F RID: 95 RVA: 0x000024A7 File Offset: 0x000006A7
		public UnitInfo UnitInformation { get; set; }

		// Token: 0x06000060 RID: 96 RVA: 0x000024B0 File Offset: 0x000006B0
		public SetUnitId()
		{
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000024B8 File Offset: 0x000006B8
		public SetUnitId(UnitInfo unitInfo)
		{
			this.UnitInformation = unitInfo;
		}

		// Token: 0x04000029 RID: 41
		public const string XmlElementName = "SetUnitId";
	}
}

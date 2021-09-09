using System;

namespace Model.Commands
{
	// Token: 0x02000007 RID: 7
	public class SetState
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000020E5 File Offset: 0x000002E5
		// (set) Token: 0x06000011 RID: 17 RVA: 0x000020ED File Offset: 0x000002ED
		public string StateName { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000012 RID: 18 RVA: 0x000020F6 File Offset: 0x000002F6
		// (set) Token: 0x06000013 RID: 19 RVA: 0x000020FE File Offset: 0x000002FE
		public string StateValue { get; set; }

		// Token: 0x06000014 RID: 20 RVA: 0x00002107 File Offset: 0x00000307
		public SetState()
		{
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000210F File Offset: 0x0000030F
		public SetState(string stateName, string stateValue)
		{
			this.StateName = stateName;
			this.StateValue = stateValue;
		}

		// Token: 0x04000008 RID: 8
		public const string XmlElementName = "SetState";
	}
}

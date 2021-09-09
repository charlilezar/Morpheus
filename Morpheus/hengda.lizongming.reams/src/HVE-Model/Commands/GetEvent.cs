using System;

namespace Model.Commands
{
	// Token: 0x02000037 RID: 55
	public class GetEvent
	{
		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000157 RID: 343 RVA: 0x000034D0 File Offset: 0x000016D0
		// (set) Token: 0x06000156 RID: 342 RVA: 0x000034C7 File Offset: 0x000016C7
		public string Name { get; set; }

		// Token: 0x06000158 RID: 344 RVA: 0x000034D8 File Offset: 0x000016D8
		public GetEvent()
		{
		}

		// Token: 0x06000159 RID: 345 RVA: 0x000034E0 File Offset: 0x000016E0
		public GetEvent(string name)
		{
			this.Name = name;
		}

		// Token: 0x040000A1 RID: 161
		public const string XmlElementName = "GetEvent";
	}
}

using System;

namespace Model.Commands
{
	// Token: 0x02000002 RID: 2
	public abstract class XmlCommand
	{
		// Token: 0x06000001 RID: 1
		public abstract bool IsValid();

		// Token: 0x06000002 RID: 2
		public abstract string XmlElementName();
	}
}

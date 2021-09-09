using System;
using Model.DataService;

namespace Model.MvvmMessages
{
	// Token: 0x020000AC RID: 172
	public class mvvmUserControlSelected
	{
		// Token: 0x170001AA RID: 426
		// (get) Token: 0x060004DA RID: 1242 RVA: 0x0000B681 File Offset: 0x00009881
		// (set) Token: 0x060004DB RID: 1243 RVA: 0x0000B689 File Offset: 0x00009889
		public string UserControlName { get; private set; }

		// Token: 0x060004DC RID: 1244 RVA: 0x0000B692 File Offset: 0x00009892
		public mvvmUserControlSelected(SentryDataService dataService, string name)
		{
			this.UserControlName = name;
		}
	}
}

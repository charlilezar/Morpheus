using System;

namespace Model.Commands
{
	// Token: 0x02000026 RID: 38
	public class TestRequest
	{
		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000101 RID: 257 RVA: 0x00003099 File Offset: 0x00001299
		// (set) Token: 0x06000100 RID: 256 RVA: 0x00003090 File Offset: 0x00001290
		public string TestName { get; set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000103 RID: 259 RVA: 0x000030AA File Offset: 0x000012AA
		// (set) Token: 0x06000102 RID: 258 RVA: 0x000030A1 File Offset: 0x000012A1
		public string TestParameter { get; set; }

		// Token: 0x06000104 RID: 260 RVA: 0x000030B2 File Offset: 0x000012B2
		public TestRequest()
		{
		}

		// Token: 0x06000105 RID: 261 RVA: 0x000030BA File Offset: 0x000012BA
		public TestRequest(string testName, string testParameter)
		{
			this.TestName = testName;
			this.TestParameter = testParameter;
		}

		// Token: 0x04000077 RID: 119
		public const string XmlElementName = "TestRequest";
	}
}

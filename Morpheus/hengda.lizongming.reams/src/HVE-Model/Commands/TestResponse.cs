using System;

namespace Model.Commands
{
	// Token: 0x02000027 RID: 39
	public class TestResponse
	{
		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000107 RID: 263 RVA: 0x000030D9 File Offset: 0x000012D9
		// (set) Token: 0x06000106 RID: 262 RVA: 0x000030D0 File Offset: 0x000012D0
		public string TestName { get; set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000109 RID: 265 RVA: 0x000030EA File Offset: 0x000012EA
		// (set) Token: 0x06000108 RID: 264 RVA: 0x000030E1 File Offset: 0x000012E1
		public string TestParameter { get; set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x0600010B RID: 267 RVA: 0x000030FB File Offset: 0x000012FB
		// (set) Token: 0x0600010A RID: 266 RVA: 0x000030F2 File Offset: 0x000012F2
		public string Status { get; set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x0600010D RID: 269 RVA: 0x0000310C File Offset: 0x0000130C
		// (set) Token: 0x0600010C RID: 268 RVA: 0x00003103 File Offset: 0x00001303
		public bool Passed { get; set; }

		// Token: 0x0600010E RID: 270 RVA: 0x00003114 File Offset: 0x00001314
		public TestResponse()
		{
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000311C File Offset: 0x0000131C
		public TestResponse(string testName, string testParameter)
		{
			this.TestName = testName;
			this.TestParameter = testParameter;
			this.Status = "";
			this.Passed = false;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00003144 File Offset: 0x00001344
		public TestResponse(string testName, string testParameter, string status, bool passed) : this(testName, testParameter)
		{
			this.Status = status;
			this.Passed = passed;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000315D File Offset: 0x0000135D
		public TestResponse(string testName, string testParameter, string status)
		{
			this.TestName = testName;
			this.TestParameter = testParameter;
			this.Status = status;
		}

		// Token: 0x0400007A RID: 122
		public const string XmlElementName = "TestResponse";
	}
}

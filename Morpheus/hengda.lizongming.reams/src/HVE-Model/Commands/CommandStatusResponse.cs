using System;
using System.Diagnostics;

namespace Model.Commands
{
	// Token: 0x0200000A RID: 10
	public class CommandStatusResponse
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000018 RID: 24 RVA: 0x00002135 File Offset: 0x00000335
		// (set) Token: 0x06000019 RID: 25 RVA: 0x0000213D File Offset: 0x0000033D
		public string Command { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002146 File Offset: 0x00000346
		// (set) Token: 0x0600001B RID: 27 RVA: 0x0000214E File Offset: 0x0000034E
		public bool Status { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002160 File Offset: 0x00000360
		// (set) Token: 0x0600001C RID: 28 RVA: 0x00002157 File Offset: 0x00000357
		public string Message { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002171 File Offset: 0x00000371
		// (set) Token: 0x0600001E RID: 30 RVA: 0x00002168 File Offset: 0x00000368
		public int Elapsed { get; set; }

		// Token: 0x06000020 RID: 32 RVA: 0x00002179 File Offset: 0x00000379
		public CommandStatusResponse()
		{
			this.Command = "";
			this.Status = true;
			this.Message = "";
			this.Elapsed = 0;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000021A5 File Offset: 0x000003A5
		public CommandStatusResponse(string command, string message, bool status)
		{
			this.Command = command;
			this.Message = message;
			this.Status = status;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000021C2 File Offset: 0x000003C2
		public CommandStatusResponse(string command, string message, bool status, Stopwatch sw) : this(command, message, status)
		{
			this.Elapsed = (int)sw.ElapsedMilliseconds;
		}

		// Token: 0x0400000D RID: 13
		public const string XmlElementName = "CommandStatusResponse";
	}
}

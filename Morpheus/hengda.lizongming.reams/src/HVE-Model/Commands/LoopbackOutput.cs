using System;

namespace Model.Commands
{
	// Token: 0x02000025 RID: 37
	public class LoopbackOutput
	{
		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060000FA RID: 250 RVA: 0x00003028 File Offset: 0x00001228
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x0000301F File Offset: 0x0000121F
		public string ComPort { get; set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000FC RID: 252 RVA: 0x00003039 File Offset: 0x00001239
		// (set) Token: 0x060000FB RID: 251 RVA: 0x00003030 File Offset: 0x00001230
		public string Text { get; set; }

		// Token: 0x060000FD RID: 253 RVA: 0x00003041 File Offset: 0x00001241
		public LoopbackOutput()
		{
			this.ComPort = "";
			this.Text = "";
		}

		// Token: 0x060000FE RID: 254 RVA: 0x0000305F File Offset: 0x0000125F
		public LoopbackOutput(string comPort)
		{
			this.ComPort = comPort;
			this.Text = "1234567890qwertyuiopasdfghjklzxcvbnm";
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00003079 File Offset: 0x00001279
		public static bool IsLoopbackCorrect(LoopbackOutput recvdLoopback)
		{
			return recvdLoopback.Text == "1234567890qwertyuiopasdfghjklzxcvbnm";
		}

		// Token: 0x04000073 RID: 115
		public const string XmlElementName = "LoopbackOutput";

		// Token: 0x04000074 RID: 116
		private const string LoopbackText = "1234567890qwertyuiopasdfghjklzxcvbnm";
	}
}

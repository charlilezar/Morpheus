using System;

namespace Model.Electrometer
{
	// Token: 0x0200007D RID: 125
	public class Breakpoint
	{
		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000352 RID: 850 RVA: 0x00009369 File Offset: 0x00007569
		// (set) Token: 0x06000353 RID: 851 RVA: 0x00009371 File Offset: 0x00007571
		public int Temperature { get; set; }

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000354 RID: 852 RVA: 0x0000937A File Offset: 0x0000757A
		// (set) Token: 0x06000355 RID: 853 RVA: 0x00009382 File Offset: 0x00007582
		public double Value { get; set; }

		// Token: 0x06000356 RID: 854 RVA: 0x0000938B File Offset: 0x0000758B
		public Breakpoint()
		{
		}

		// Token: 0x06000357 RID: 855 RVA: 0x00009393 File Offset: 0x00007593
		public Breakpoint(int temperature, double value)
		{
			this.Temperature = temperature;
			this.Value = value;
		}
	}
}

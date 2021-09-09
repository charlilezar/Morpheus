using System;
using System.Text;
using System.Xml.Serialization;

namespace Model.Measurements
{
	// Token: 0x02000088 RID: 136
	[XmlInclude(typeof(GenericHistogram))]
	[XmlInclude(typeof(IntervalHistogram))]
	public class PerformanceHistogram
	{
		// Token: 0x17000128 RID: 296
		// (get) Token: 0x060003D5 RID: 981 RVA: 0x00009EFC File Offset: 0x000080FC
		// (set) Token: 0x060003D6 RID: 982 RVA: 0x00009F04 File Offset: 0x00008104
		public PerformanceHistogram.BinUnits Units { get; set; }

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x060003D7 RID: 983 RVA: 0x00009F0D File Offset: 0x0000810D
		// (set) Token: 0x060003D8 RID: 984 RVA: 0x00009F15 File Offset: 0x00008115
		public double MinBin { get; set; }

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x060003D9 RID: 985 RVA: 0x00009F1E File Offset: 0x0000811E
		// (set) Token: 0x060003DA RID: 986 RVA: 0x00009F26 File Offset: 0x00008126
		public double MaxBin { get; set; }

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x060003DB RID: 987 RVA: 0x00009F2F File Offset: 0x0000812F
		// (set) Token: 0x060003DC RID: 988 RVA: 0x00009F37 File Offset: 0x00008137
		public int NumberOfBins { get; set; }

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x060003DD RID: 989 RVA: 0x00009F40 File Offset: 0x00008140
		// (set) Token: 0x060003DE RID: 990 RVA: 0x00009F48 File Offset: 0x00008148
		public int OverRangeCount { get; set; }

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x060003DF RID: 991 RVA: 0x00009F51 File Offset: 0x00008151
		// (set) Token: 0x060003E0 RID: 992 RVA: 0x00009F59 File Offset: 0x00008159
		public int UnderRangeCount { get; set; }

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x060003E1 RID: 993 RVA: 0x00009F62 File Offset: 0x00008162
		// (set) Token: 0x060003E2 RID: 994 RVA: 0x00009F6A File Offset: 0x0000816A
		public double MaxOverrange { get; set; }

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x060003E3 RID: 995 RVA: 0x00009F73 File Offset: 0x00008173
		// (set) Token: 0x060003E4 RID: 996 RVA: 0x00009F7B File Offset: 0x0000817B
		public double MinUnderrange { get; set; }

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x060003E5 RID: 997 RVA: 0x00009F84 File Offset: 0x00008184
		// (set) Token: 0x060003E6 RID: 998 RVA: 0x00009F8C File Offset: 0x0000818C
		public int TotalCounts { get; set; }

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x060003E7 RID: 999 RVA: 0x00009F95 File Offset: 0x00008195
		// (set) Token: 0x060003E8 RID: 1000 RVA: 0x00009F9D File Offset: 0x0000819D
		[XmlIgnore]
		public double[] XArray { get; set; }

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x060003E9 RID: 1001 RVA: 0x00009FA6 File Offset: 0x000081A6
		// (set) Token: 0x060003EA RID: 1002 RVA: 0x00009FAE File Offset: 0x000081AE
		[XmlIgnore]
		public double[] Bins { get; set; }

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x060003EB RID: 1003 RVA: 0x00009FB8 File Offset: 0x000081B8
		// (set) Token: 0x060003EC RID: 1004 RVA: 0x00009FFC File Offset: 0x000081FC
		[XmlElement(ElementName = "BinValues")]
		public string BinValuesString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < this.NumberOfBins; i++)
				{
					stringBuilder.AppendFormat("{0},", (int)this.Bins[i]);
				}
				return stringBuilder.ToString();
			}
			set
			{
				string[] array = value.Split(new char[]
				{
					','
				});
				int num = array.Length - 1;
				this.Bins = new double[num];
				for (int i = 0; i < num; i++)
				{
					this.Bins[i] = (double)int.Parse(array[i]);
				}
			}
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x060003ED RID: 1005 RVA: 0x0000A04C File Offset: 0x0000824C
		// (set) Token: 0x060003EE RID: 1006 RVA: 0x0000A090 File Offset: 0x00008290
		[XmlElement(ElementName = "XValues")]
		public string XValuesString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < this.NumberOfBins; i++)
				{
					stringBuilder.AppendFormat("{0:0.000},", this.XArray[i]);
				}
				return stringBuilder.ToString();
			}
			set
			{
				string[] array = value.Split(new char[]
				{
					','
				});
				int num = array.Length - 1;
				this.XArray = new double[num];
				for (int i = 0; i < num; i++)
				{
					this.XArray[i] = double.Parse(array[i]);
				}
			}
		}

		// Token: 0x040002D5 RID: 725
		private readonly object _lockObject = new object();

		// Token: 0x040002D6 RID: 726
		protected double BinSpacing;

		// Token: 0x040002D7 RID: 727
		protected double A;

		// Token: 0x040002D8 RID: 728
		protected double B;

		// Token: 0x02000089 RID: 137
		public enum BinUnits
		{
			// Token: 0x040002E5 RID: 741
			Seconds,
			// Token: 0x040002E6 RID: 742
			MilliSeconds,
			// Token: 0x040002E7 RID: 743
			QueueCount
		}
	}
}

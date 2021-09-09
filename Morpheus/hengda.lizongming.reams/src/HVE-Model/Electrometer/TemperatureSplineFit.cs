using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Model.Electrometer
{
	// Token: 0x02000081 RID: 129
	public class TemperatureSplineFit
	{
		// Token: 0x17000111 RID: 273
		// (get) Token: 0x0600038E RID: 910 RVA: 0x00009895 File Offset: 0x00007A95
		// (set) Token: 0x0600038F RID: 911 RVA: 0x0000989D File Offset: 0x00007A9D
		[XmlIgnore]
		public double[] Values { get; private set; }

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000390 RID: 912 RVA: 0x000098A6 File Offset: 0x00007AA6
		// (set) Token: 0x06000391 RID: 913 RVA: 0x000098AE File Offset: 0x00007AAE
		public string Name { get; set; }

		// Token: 0x06000392 RID: 914 RVA: 0x000098B7 File Offset: 0x00007AB7
		public TemperatureSplineFit()
		{
		}

		// Token: 0x06000393 RID: 915 RVA: 0x000098CA File Offset: 0x00007ACA
		public TemperatureSplineFit(string name)
		{
			this.Name = name;
		}

		// Token: 0x06000394 RID: 916 RVA: 0x000098E4 File Offset: 0x00007AE4
		private static Tuple<double, double> FindSlopeIntercept(Breakpoint bp1, Breakpoint bp2)
		{
			double num = (bp2.Value - bp1.Value) / (double)(bp2.Temperature - bp1.Temperature);
			double item = bp1.Value - num * (double)bp1.Temperature;
			return new Tuple<double, double>(num, item);
		}

		// Token: 0x06000395 RID: 917 RVA: 0x00009928 File Offset: 0x00007B28
		private int FindBreakpointIndex(int temperature)
		{
			int count = this.Breakpoints.Count;
			for (int i = 0; i < count - 1; i++)
			{
				if (temperature >= this.Breakpoints[i].Temperature && temperature <= this.Breakpoints[i + 1].Temperature)
				{
					return i;
				}
			}
			return count - 1;
		}

		// Token: 0x06000396 RID: 918 RVA: 0x00009980 File Offset: 0x00007B80
		public void CalculateTemperatureRelateValues()
		{
			int count = this.Breakpoints.Count;
			this.m_MinTemperature = this.Breakpoints[0].Temperature;
			this.m_MaxTemperature = this.Breakpoints[count - 1].Temperature;
			this.m_TemperatureCount = this.m_MaxTemperature - this.m_MinTemperature;
			this.Values = new double[this.m_TemperatureCount + 1];
			for (int i = this.m_MinTemperature; i <= this.m_MaxTemperature; i++)
			{
				int num = this.FindBreakpointIndex(i);
				Tuple<double, double> tuple = TemperatureSplineFit.FindSlopeIntercept(this.Breakpoints[num], this.Breakpoints[num + 1]);
				this.Values[i - this.m_MinTemperature] = tuple.Item1 * (double)i + tuple.Item2;
			}
		}

		// Token: 0x06000397 RID: 919 RVA: 0x00009A4C File Offset: 0x00007C4C
		public void AddBreakpoint(int temperature, double value)
		{
			this.Breakpoints.Add(new Breakpoint(temperature, value));
		}

		// Token: 0x06000398 RID: 920 RVA: 0x00009A60 File Offset: 0x00007C60
		public double GetValue(int temperature)
		{
			double result;
			lock (TemperatureSplineFit.SplineLock)
			{
				int num = temperature - this.m_MinTemperature;
				if (num < 0)
				{
					num = 0;
				}
				else if (num > this.m_TemperatureCount)
				{
					num = this.m_TemperatureCount;
				}
				result = this.Values[num];
			}
			return result;
		}

		// Token: 0x06000399 RID: 921 RVA: 0x00009AC4 File Offset: 0x00007CC4
		public void Clear()
		{
			this.Breakpoints.Clear();
		}

		// Token: 0x0600039A RID: 922 RVA: 0x00009AD1 File Offset: 0x00007CD1
		public override string ToString()
		{
			return "SplineFit " + this.Name;
		}

		// Token: 0x04000278 RID: 632
		public List<Breakpoint> Breakpoints = new List<Breakpoint>();

		// Token: 0x04000279 RID: 633
		private int m_MinTemperature;

		// Token: 0x0400027A RID: 634
		private int m_MaxTemperature;

		// Token: 0x0400027B RID: 635
		private int m_TemperatureCount;

		// Token: 0x0400027C RID: 636
		public static object SplineLock = new object();
	}
}

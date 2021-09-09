using System;
using System.Collections.Generic;
using System.Linq;
using CommonLibrary;

namespace Model.DataService
{
	// Token: 0x0200007A RID: 122
	public class ScalarMeasurementPlotData
	{
		// Token: 0x0600033C RID: 828 RVA: 0x00008F82 File Offset: 0x00007182
		public ScalarMeasurementPlotData()
		{
			this._timeList = new List<DateTime>();
			this._valueList = new List<double>();
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x0600033D RID: 829 RVA: 0x00008FA0 File Offset: 0x000071A0
		public int Count
		{
			get
			{
				return this._timeList.Count;
			}
		}

		// Token: 0x0600033E RID: 830 RVA: 0x00008FAD File Offset: 0x000071AD
		public DateTime GetTime(int i)
		{
			return this._timeList[i];
		}

		// Token: 0x0600033F RID: 831 RVA: 0x00008FBB File Offset: 0x000071BB
		public double GetValue(int i)
		{
			return this._valueList[i];
		}

		// Token: 0x06000340 RID: 832 RVA: 0x00008FC9 File Offset: 0x000071C9
		public void AddValuePair(DateTime dt, float value)
		{
			this._timeList.Add(dt);
			this._valueList.Add((double)value);
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000341 RID: 833 RVA: 0x00008FE4 File Offset: 0x000071E4
		public double[] OADateArray
		{
			get
			{
				int count = this._timeList.Count;
				double[] array = new double[count];
				for (int i = 0; i < count; i++)
				{
					array[i] = this._timeList[i].ToOADate();
				}
				return array;
			}
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000342 RID: 834 RVA: 0x00009028 File Offset: 0x00007228
		public double[] NIDateArray
		{
			get
			{
				int count = this._timeList.Count;
				double[] array = new double[count];
				for (int i = 0; i < count; i++)
				{
					array[i] = this._timeList[i].ToDouble();
				}
				return array;
			}
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000343 RID: 835 RVA: 0x00009069 File Offset: 0x00007269
		public double[] ValueArray
		{
			get
			{
				return this._valueList.ToArray<double>();
			}
		}

		// Token: 0x04000249 RID: 585
		private readonly List<DateTime> _timeList;

		// Token: 0x0400024A RID: 586
		private readonly List<double> _valueList;
	}
}

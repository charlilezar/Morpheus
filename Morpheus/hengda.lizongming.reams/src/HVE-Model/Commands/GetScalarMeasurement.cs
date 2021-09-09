using System;

namespace Model.Commands
{
	// Token: 0x02000041 RID: 65
	public class GetScalarMeasurement
	{
		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000175 RID: 373 RVA: 0x0000360C File Offset: 0x0000180C
		// (set) Token: 0x06000174 RID: 372 RVA: 0x00003603 File Offset: 0x00001803
		public string Name { get; set; }

		// Token: 0x06000176 RID: 374 RVA: 0x00003614 File Offset: 0x00001814
		public GetScalarMeasurement()
		{
		}

		// Token: 0x06000177 RID: 375 RVA: 0x0000361C File Offset: 0x0000181C
		public GetScalarMeasurement(string name)
		{
			this.Name = name;
		}

		// Token: 0x040000B4 RID: 180
		public const string XmlElementName = "GetScalarMeasurement";
	}
}

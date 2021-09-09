using System;
using Model.Electrometer;

namespace Model.Commands
{
	// Token: 0x02000032 RID: 50
	public class GetElectrometerConfigurationResponse
	{
		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000148 RID: 328 RVA: 0x00003434 File Offset: 0x00001634
		// (set) Token: 0x06000147 RID: 327 RVA: 0x0000342B File Offset: 0x0000162B
		public ElectrometerConfiguration Configuration { get; set; }

		// Token: 0x06000149 RID: 329 RVA: 0x0000343C File Offset: 0x0000163C
		public GetElectrometerConfigurationResponse()
		{
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00003444 File Offset: 0x00001644
		public GetElectrometerConfigurationResponse(ElectrometerConfiguration configuration)
		{
			this.Configuration = configuration;
		}
	}
}

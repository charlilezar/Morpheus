using System;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Model.Events;

namespace Model.Measurements
{
	// Token: 0x02000090 RID: 144
	public class ElectrometerWaveform : IXmlSerializable
	{
		// Token: 0x1700016E RID: 366
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x0000A754 File Offset: 0x00008954
		// (set) Token: 0x06000443 RID: 1091 RVA: 0x0000A75C File Offset: 0x0000895C
		public int SamplingInterval { get; set; }

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x06000444 RID: 1092 RVA: 0x0000A765 File Offset: 0x00008965
		// (set) Token: 0x06000445 RID: 1093 RVA: 0x0000A76D File Offset: 0x0000896D
		public int Count { get; set; }

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x06000446 RID: 1094 RVA: 0x0000A776 File Offset: 0x00008976
		// (set) Token: 0x06000447 RID: 1095 RVA: 0x0000A77E File Offset: 0x0000897E
		public EventStates Gain { get; set; }

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x0000A787 File Offset: 0x00008987
		// (set) Token: 0x06000449 RID: 1097 RVA: 0x0000A78F File Offset: 0x0000898F
		public double HV { get; set; }

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x0600044A RID: 1098 RVA: 0x0000A798 File Offset: 0x00008998
		// (set) Token: 0x0600044B RID: 1099 RVA: 0x0000A7A0 File Offset: 0x000089A0
		public double Leakage { get; set; }

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x0600044C RID: 1100 RVA: 0x0000A7A9 File Offset: 0x000089A9
		// (set) Token: 0x0600044D RID: 1101 RVA: 0x0000A7B1 File Offset: 0x000089B1
		public double Capacitance { get; set; }

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x0600044E RID: 1102 RVA: 0x0000A7BA File Offset: 0x000089BA
		// (set) Token: 0x0600044F RID: 1103 RVA: 0x0000A7C2 File Offset: 0x000089C2
		public int ResetTimeMicroSeconds { get; set; }

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06000451 RID: 1105 RVA: 0x0000A7D4 File Offset: 0x000089D4
		// (set) Token: 0x06000450 RID: 1104 RVA: 0x0000A7CB File Offset: 0x000089CB
		public int StartIntegrationIndex { get; set; }

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06000452 RID: 1106 RVA: 0x0000A7DC File Offset: 0x000089DC
		// (set) Token: 0x06000453 RID: 1107 RVA: 0x0000A7E4 File Offset: 0x000089E4
		public int EndIntegrationIndex { get; set; }

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06000454 RID: 1108 RVA: 0x0000A7ED File Offset: 0x000089ED
		// (set) Token: 0x06000455 RID: 1109 RVA: 0x0000A7F5 File Offset: 0x000089F5
		public double IntegrationTime { get; set; }

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06000456 RID: 1110 RVA: 0x0000A7FE File Offset: 0x000089FE
		// (set) Token: 0x06000457 RID: 1111 RVA: 0x0000A806 File Offset: 0x00008A06
		public double NumberOfAveragedSlopes { get; set; }

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000458 RID: 1112 RVA: 0x0000A80F File Offset: 0x00008A0F
		// (set) Token: 0x06000459 RID: 1113 RVA: 0x0000A817 File Offset: 0x00008A17
		public double SecondsPerTick { get; set; }

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x0600045A RID: 1114 RVA: 0x0000A820 File Offset: 0x00008A20
		// (set) Token: 0x0600045B RID: 1115 RVA: 0x0000A828 File Offset: 0x00008A28
		public double[] SampleTime { get; set; }

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x0600045C RID: 1116 RVA: 0x0000A831 File Offset: 0x00008A31
		// (set) Token: 0x0600045D RID: 1117 RVA: 0x0000A839 File Offset: 0x00008A39
		public double[] VoltageData { get; set; }

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x0600045E RID: 1118 RVA: 0x0000A842 File Offset: 0x00008A42
		public double[] CurrentData
		{
			get
			{
				if (this.m_CurrentData == null)
				{
					this.m_CurrentData = new double[this.SampleTime.Length];
				}
				return this.m_CurrentData;
			}
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x0600045F RID: 1119 RVA: 0x0000A865 File Offset: 0x00008A65
		// (set) Token: 0x06000460 RID: 1120 RVA: 0x0000A86D File Offset: 0x00008A6D
		public double MaxCurrent
		{
			get
			{
				return this.m_MaxCurrent;
			}
			set
			{
				this.m_MaxCurrent = value;
			}
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06000461 RID: 1121 RVA: 0x0000A876 File Offset: 0x00008A76
		public double IntegrationStartVoltage
		{
			get
			{
				if (this.StartIntegrationIndex <= 0)
				{
					return 0.0;
				}
				return this.VoltageData[this.StartIntegrationIndex];
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06000462 RID: 1122 RVA: 0x0000A898 File Offset: 0x00008A98
		public double IntegrationEndVoltage
		{
			get
			{
				double result;
				try
				{
					if (this.EndIntegrationIndex < 0)
					{
						result = 0.0;
					}
					else
					{
						result = this.VoltageData[this.EndIntegrationIndex];
					}
				}
				catch (Exception ex)
				{
					SentryDebug.WriteException(ex);
					result = 0.0;
				}
				return result;
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06000463 RID: 1123 RVA: 0x0000A8F0 File Offset: 0x00008AF0
		public double IntegrationVoltage
		{
			get
			{
				return this.IntegrationEndVoltage - this.IntegrationStartVoltage;
			}
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x06000464 RID: 1124 RVA: 0x0000A8FF File Offset: 0x00008AFF
		public double Slope
		{
			get
			{
				if (this.IntegrationTime > 0.0)
				{
					return this.IntegrationVoltage / this.IntegrationTime;
				}
				return 0.0;
			}
		}

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x06000465 RID: 1125 RVA: 0x0000A929 File Offset: 0x00008B29
		public double IntegrationStartTime
		{
			get
			{
				if (this.StartIntegrationIndex <= 0)
				{
					return 0.0;
				}
				return this.SampleTime[this.StartIntegrationIndex];
			}
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000466 RID: 1126 RVA: 0x0000A94B File Offset: 0x00008B4B
		public double IntegrationEndTime
		{
			get
			{
				if (this.EndIntegrationIndex <= 0)
				{
					return 0.0;
				}
				return this.SampleTime[this.EndIntegrationIndex];
			}
		}

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06000467 RID: 1127 RVA: 0x0000A970 File Offset: 0x00008B70
		public double VStdDev
		{
			get
			{
				double num = 0.0;
				double num2 = 0.0;
				for (int i = 0; i < this.Count; i++)
				{
					double num3 = this.VoltageData[i];
					num += num3;
					num2 += num3 * num3;
				}
				double num4 = num / (double)this.Count;
				return Math.Sqrt(num2 / (double)this.Count - num4 * num4);
			}
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06000468 RID: 1128 RVA: 0x0000A9D4 File Offset: 0x00008BD4
		public double VAverage
		{
			get
			{
				double num = 0.0;
				for (int i = 0; i < this.Count; i++)
				{
					double num2 = this.VoltageData[i];
					num += num2;
				}
				return num / (double)this.Count;
			}
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06000469 RID: 1129 RVA: 0x0000AA12 File Offset: 0x00008C12
		public int IntegratorPointCount
		{
			get
			{
				return this.EndIntegrationIndex - this.StartIntegrationIndex + 1;
			}
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x0000AA24 File Offset: 0x00008C24
		public override string ToString()
		{
			string result;
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Count, IntegrationStartVoltage, IntegrationEndVoltage, IntegrationTime, IntegrationVoltage, Slope, StartIntegrationIndex, EndIntegrationIndex\r\n");
				stringBuilder.AppendFormat("{0},{1:0.000},{2:0.000},{3:0.000},{4:0.000},{5:0.00000},{6},({7})", new object[]
				{
					this.Count,
					this.IntegrationStartVoltage,
					this.IntegrationEndVoltage,
					this.IntegrationTime,
					this.IntegrationVoltage,
					this.Slope,
					this.StartIntegrationIndex,
					this.EndIntegrationIndex
				});
				result = stringBuilder.ToString();
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				result = "";
			}
			return result;
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x0000AAF4 File Offset: 0x00008CF4
		public string ToStringWithData()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.ToString());
			stringBuilder.AppendFormat("Time,Voltage,dV/dT,", new object[0]);
			double num = 0.0;
			double num2 = 0.0;
			for (int i = 0; i < this.Count; i++)
			{
				if (i == 0)
				{
					stringBuilder.AppendFormat("{0:0.0000},{1:0.0000},0\r\n", this.SampleTime[i], this.VoltageData[i]);
				}
				else
				{
					double num3 = (this.VoltageData[i] - num) / (this.SampleTime[i] - num2);
					stringBuilder.AppendFormat("{0:0.0000},{1:0.0000},{2:0.0000}\r\n", this.SampleTime[i], this.VoltageData[i], num3);
				}
				num = this.VoltageData[i];
				num2 = this.SampleTime[i];
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x0000ABD8 File Offset: 0x00008DD8
		public void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement();
			this.SamplingInterval = reader.ReadElementInteger("SamplingInterval");
			this.Count = reader.ReadElementInteger("Count");
			this.Gain = reader.ReadElementEnum<EventStates>("Gain");
			this.HV = reader.ReadElementDouble("HV");
			this.Leakage = reader.ReadElementDouble("Leakage");
			this.Capacitance = reader.ReadElementDouble("Capacitance");
			this.ResetTimeMicroSeconds = reader.ReadElementInteger("ResetTimeMicroSeconds");
			this.StartIntegrationIndex = reader.ReadElementInteger("StartIntegrationIndex");
			this.EndIntegrationIndex = reader.ReadElementInteger("EndIntegrationIndex");
			this.IntegrationTime = reader.ReadElementDouble("IntegrationTime");
			this.NumberOfAveragedSlopes = reader.ReadElementDouble("NumberOfAveragedSlopes");
			this.SecondsPerTick = reader.ReadElementDouble("SecondsPerTick");
			this.ReadVoltageData(reader);
			this.ReadSampleTimeTicks(reader);
			reader.ReadEndElement();
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x0000ACCC File Offset: 0x00008ECC
		private void ReadVoltageData(XmlReader reader)
		{
			string text = reader.ReadElementString("VoltageData");
			string[] array = text.Split(new char[]
			{
				','
			});
			int num = array.Length - 1;
			this.VoltageData = new double[num];
			for (int i = 0; i < num; i++)
			{
				this.VoltageData[i] = double.Parse(array[i], NumberStyles.Any, CultureInfo.InvariantCulture);
			}
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x0000AD34 File Offset: 0x00008F34
		private void ReadSampleTimeTicks(XmlReader reader)
		{
			reader.MoveToContent();
			string text = reader.ReadElementString("SampleTimeTicks");
			string[] array = text.Split(new char[]
			{
				','
			});
			int num = array.Length - 1;
			this.m_SampleTimeTicks = new long[num];
			this.SampleTime = new double[num];
			for (int i = 0; i < num; i++)
			{
				this.m_SampleTimeTicks[i] = long.Parse(array[i]) * 1000L;
				this.SampleTime[i] = (double)this.m_SampleTimeTicks[i] * this.SecondsPerTick;
			}
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x0000ADC4 File Offset: 0x00008FC4
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteElementInteger("SamplingInterval", this.SamplingInterval);
			writer.WriteElementInteger("Count", this.Count);
			writer.WriteElementEnum("Gain", this.Gain);
			writer.WriteElementEnum("Gain", this.Gain);
			writer.WriteElementDouble("HV", this.HV, "0.0");
			writer.WriteElementDouble("Leakage", this.Leakage, "0.000e-00");
			writer.WriteElementDouble("Capacitance", this.Capacitance, "0.000e-00");
			writer.WriteElementInteger("ResetTimeMicroSeconds", this.ResetTimeMicroSeconds);
			writer.WriteElementInteger("StartIntegrationIndex", this.StartIntegrationIndex);
			writer.WriteElementInteger("EndIntegrationIndex", this.EndIntegrationIndex);
			writer.WriteElementDouble("IntegrationTime", this.IntegrationTime, "0.000");
			writer.WriteElementDouble("NumberOfAveragedSlopes", this.NumberOfAveragedSlopes, "0");
			writer.WriteElementDouble("SecondsPerTick", this.SecondsPerTick, "");
			this.WriteVoltageDataElement(writer);
			this.WriteTimeElement(writer);
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x0000AEDC File Offset: 0x000090DC
		private void WriteVoltageDataElement(XmlWriter writer)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.Count; i++)
			{
				stringBuilder.AppendFormat("{0:0.0000},", this.VoltageData[i]);
			}
			writer.WriteElementString("VoltageData", stringBuilder.ToString());
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x0000AF2C File Offset: 0x0000912C
		private void WriteTimeElement(XmlWriter writer)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.Count; i++)
			{
				stringBuilder.AppendFormat("{0},", this.m_SampleTimeTicks[i] / 1000L);
			}
			writer.WriteElementString("m_SampleTimeTicks", stringBuilder.ToString());
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x0000AF81 File Offset: 0x00009181
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x040002F3 RID: 755
		private double m_MaxCurrent;

		// Token: 0x040002F4 RID: 756
		private double[] m_CurrentData;

		// Token: 0x040002F5 RID: 757
		public long[] m_SampleTimeTicks;
	}
}

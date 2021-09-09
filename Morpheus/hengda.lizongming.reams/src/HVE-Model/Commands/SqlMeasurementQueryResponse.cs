using System;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using CommonLibrary;

namespace Model.Commands
{
	// Token: 0x02000020 RID: 32
	public class SqlMeasurementQueryResponse : IXmlSerializable
	{
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x000029F1 File Offset: 0x00000BF1
		// (set) Token: 0x060000C2 RID: 194 RVA: 0x000029E8 File Offset: 0x00000BE8
		public string Name { get; set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x000029F9 File Offset: 0x00000BF9
		public bool BufferFull
		{
			get
			{
				return this.RecordsInThisResponse == 1000;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x00002A11 File Offset: 0x00000C11
		// (set) Token: 0x060000C5 RID: 197 RVA: 0x00002A08 File Offset: 0x00000C08
		public int RecordsInThisResponse { get; set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x00002A22 File Offset: 0x00000C22
		// (set) Token: 0x060000C7 RID: 199 RVA: 0x00002A19 File Offset: 0x00000C19
		public int RecordsSoFar { get; set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000CA RID: 202 RVA: 0x00002A33 File Offset: 0x00000C33
		// (set) Token: 0x060000C9 RID: 201 RVA: 0x00002A2A File Offset: 0x00000C2A
		public bool QueryStatus { get; set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000CC RID: 204 RVA: 0x00002A44 File Offset: 0x00000C44
		// (set) Token: 0x060000CB RID: 203 RVA: 0x00002A3B File Offset: 0x00000C3B
		public bool QueryResponseComplete { get; set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000CE RID: 206 RVA: 0x00002A55 File Offset: 0x00000C55
		// (set) Token: 0x060000CD RID: 205 RVA: 0x00002A4C File Offset: 0x00000C4C
		public bool UserTerminated { get; set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x00002A66 File Offset: 0x00000C66
		// (set) Token: 0x060000CF RID: 207 RVA: 0x00002A5D File Offset: 0x00000C5D
		public string Status { get; set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x00002A6E File Offset: 0x00000C6E
		public DateTime OldestTime
		{
			get
			{
				if (this.RecordsInThisResponse > 0)
				{
					return SqlMeasurementQueryResponse._timeArray[0];
				}
				return DateTime.MinValue;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x00002A8F File Offset: 0x00000C8F
		public DateTime NewestTime
		{
			get
			{
				if (this.RecordsInThisResponse > 0)
				{
					return SqlMeasurementQueryResponse._timeArray[this.RecordsInThisResponse - 1];
				}
				return DateTime.MinValue;
			}
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00002AB7 File Offset: 0x00000CB7
		public SqlMeasurementQueryResponse()
		{
			this.Name = "";
			this.QueryResponseComplete = false;
			this.QueryStatus = true;
			this.UserTerminated = false;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00002ADF File Offset: 0x00000CDF
		public SqlMeasurementQueryResponse(string measName)
		{
			this.Name = measName;
			this.QueryResponseComplete = false;
			this.QueryStatus = true;
			this.UserTerminated = false;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00002B03 File Offset: 0x00000D03
		public void AddToArray(DateTime time, float data)
		{
			SqlMeasurementQueryResponse._timeArray[this.RecordsInThisResponse] = time;
			SqlMeasurementQueryResponse._valueArray[this.RecordsInThisResponse] = data;
			this.RecordsInThisResponse++;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00002B36 File Offset: 0x00000D36
		public void ClearResponse()
		{
			this.RecordsInThisResponse = 0;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00002B3F File Offset: 0x00000D3F
		public DateTime GetTime(int i)
		{
			return SqlMeasurementQueryResponse._timeArray[i];
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00002B51 File Offset: 0x00000D51
		public float GetValue(int i)
		{
			return SqlMeasurementQueryResponse._valueArray[i];
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00002B5C File Offset: 0x00000D5C
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			reader.ReadStartElement();
			this.Name = reader.ReadElementString("Name");
			this.RecordsInThisResponse = reader.ReadElementInteger("RecordsInThisResponse");
			this.RecordsSoFar = reader.ReadElementInteger("RecordsSoFar");
			this.QueryStatus = reader.ReadElementBoolean("QueryStatus");
			this.QueryResponseComplete = reader.ReadElementBoolean("QueryResponseComplete");
			this.UserTerminated = reader.ReadElementBoolean("UserTerminated");
			this.Status = reader.ReadElementString("Status");
			this.ReadTimes(reader);
			this.ReadValues(reader);
			reader.ReadEndElement();
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00002C04 File Offset: 0x00000E04
		private void ReadValues(XmlReader reader)
		{
			string text = reader.ReadElementString("Values");
			string[] array = text.Split(new char[]
			{
				','
			});
			int num = array.Length - 1;
			for (int i = 0; i < num; i++)
			{
				SqlMeasurementQueryResponse._valueArray[i] = float.Parse(array[i], NumberStyles.Any, CultureInfo.InvariantCulture);
			}
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00002C60 File Offset: 0x00000E60
		private void ReadTimes(XmlReader reader)
		{
			reader.MoveToContent();
			string text = reader.ReadElementString("Times");
			string[] array = text.Split(new char[]
			{
				','
			});
			int num = array.Length - 1;
			for (int i = 0; i < num; i++)
			{
				SqlMeasurementQueryResponse._timeArray[i] = DateTime.Parse(array[i]);
			}
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00002CC4 File Offset: 0x00000EC4
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString("Name", this.Name);
			writer.WriteElementInteger("RecordsInThisResponse", this.RecordsInThisResponse);
			writer.WriteElementInteger("RecordsSoFar", this.RecordsSoFar);
			writer.WriteElementBoolean("QueryStatus", this.QueryStatus);
			writer.WriteElementBoolean("QueryResponseComplete", this.QueryResponseComplete);
			writer.WriteElementBoolean("UserTerminated", this.UserTerminated);
			writer.WriteElementString("Status", this.Status);
			this.WriteTimes(writer);
			this.WriteValues(writer);
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00002D58 File Offset: 0x00000F58
		private void WriteValues(XmlWriter writer)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.RecordsInThisResponse; i++)
			{
				float num = SqlMeasurementQueryResponse._valueArray[i];
				float num2 = SqlMeasurementQueryResponse._valueArray[i];
				float num3;
				if (num != num2)
				{
					num3 = SqlMeasurementQueryResponse._valueArray[i];
					SentryDebug.WriteLine(string.Concat(new string[]
					{
						"???? ",
						SqlMeasurementQueryResponse._timeArray[i].ToShortDateString(),
						"-",
						SqlMeasurementQueryResponse._timeArray[i].ToShortTimeString(),
						" ",
						i.ToString(),
						" ",
						num3.ToString(),
						" ",
						num.ToString(),
						" ",
						num2.ToString()
					}));
				}
				else
				{
					num3 = num;
				}
				string value = num3.ToString("0.000e0");
				stringBuilder.Append(value);
				stringBuilder.Append(",");
			}
			writer.WriteElementString("Values", stringBuilder.ToString());
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00002E80 File Offset: 0x00001080
		private void WriteTimes(XmlWriter writer)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.RecordsInThisResponse; i++)
			{
				stringBuilder.AppendFormat("{0},", SqlMeasurementQueryResponse._timeArray[i].SqlTime());
			}
			writer.WriteElementString("Times", stringBuilder.ToString());
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00002ED6 File Offset: 0x000010D6
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x0400005C RID: 92
		public const string XmlElementName = "SqlMeasurementQueryResponse";

		// Token: 0x0400005D RID: 93
		public const int MaxRecordsPerResponse = 1000;

		// Token: 0x0400005E RID: 94
		private static DateTime[] _timeArray = new DateTime[1000];

		// Token: 0x0400005F RID: 95
		private static float[] _valueArray = new float[1000];
	}
}

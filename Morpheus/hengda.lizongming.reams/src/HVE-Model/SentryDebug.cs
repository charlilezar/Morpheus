using System;
using System.IO;
using System.Text;
using CommonLibrary;
using Microsoft.Extensions.Logging;
//using Logger = Microsoft.Extensions.Logging.LoggerExtensions;

namespace Model
{
	// Token: 0x020000B0 RID: 176
	public static class SentryDebug
	{
		// Token: 0x170001AC RID: 428
		// (get) Token: 0x060004E7 RID: 1255 RVA: 0x0000B991 File Offset: 0x00009B91
		// (set) Token: 0x060004E8 RID: 1256 RVA: 0x0000B998 File Offset: 0x00009B98
		public static bool ContionalFlag { get; set; }

		// Token: 0x060004E9 RID: 1257 RVA: 0x0000B9A0 File Offset: 0x00009BA0
		public static void SetCallback(Action<string> outputCallback)
		{
			SentryDebug._outputCallback = outputCallback;
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x0000B9A8 File Offset: 0x00009BA8
		public static void Write(string msg)
		{
			DateTime now = DateTime.Now;
			double num = SentryDebug.DebugTimer.SampleDeltaSecondsReal();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0:HH:mm:ss} {1:0.000} {2}", now, num, msg);
			string obj = stringBuilder.ToString();
			if (SentryDebug._outputCallback != null)
			{
				SentryDebug._outputCallback(obj);
			}
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x0000BA00 File Offset: 0x00009C00
		public static void WriteLine(string msg)
		{
			DateTime now = DateTime.Now;
			double num = SentryDebug.DebugTimer.SampleDeltaSecondsReal();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0:HH:mm:ss} {1:0.000} {2}", now, num, msg);
			string text = stringBuilder.ToString();
			if (SentryDebug._outputCallback != null)
			{
				SentryDebug._outputCallback(text);
			}
			if (SentryDebug._debugLogEnable)
			{
				SentryDebug.WriteLogMessage(text);
			}
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x0000BA63 File Offset: 0x00009C63
		public static void WriteLineConditional(string msg)
		{
			if (SentryDebug.ContionalFlag)
			{
				SentryDebug.WriteLine(msg);
			}
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x0000BA74 File Offset: 0x00009C74
		public static void WriteException(Exception ex)
		{
			DateTime now = DateTime.Now;
			SentryDebug.DebugTimer.SampleDeltaSecondsReal();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0:HH:mm:ss} {1}\r\n{2}\r\n", now, ex.Message, ex.StackTrace);
			string obj = stringBuilder.ToString();
			if (SentryDebug._outputCallback != null)
			{
				SentryDebug._outputCallback(obj);
			}
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x0000BAD0 File Offset: 0x00009CD0
		public static void EnableLogFile()
		{
			try
			{
				//if (!File.Exists(SentryDebug._fileName))
				//{
				//	FileStream fileStream = new FileStream(SentryDebug._fileName, FileMode.CreateNew, FileAccess.Write);
				//	fileStream.Close();
				//}
				SentryDebug._debugLogEnable = true;
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x0000BB18 File Offset: 0x00009D18
		private static void WriteLogMessage(string message)
		{
			//FileStream fileStream = new FileStream(SentryDebug._fileName, FileMode.Append, FileAccess.Write);
			//fileStream.Write(Encoding.ASCII.GetBytes(message + "\r\n"), 0, message.Length + 2);
			//fileStream.Close();
			Logger.LogDebug(message);
		}

		// Token: 0x0400033D RID: 829
		private static readonly HighPerformanceTimer DebugTimer = new HighPerformanceTimer();

		// Token: 0x0400033E RID: 830
		private static Action<string> _outputCallback;

		// Token: 0x0400033F RID: 831
		//private static string _fileName = "DebugLog.txt";

		// Token: 0x04000340 RID: 832
		private static bool _debugLogEnable = false;

		public static  ILogger Logger { get; set; } = new Microsoft.Extensions.Logging.LoggerFactory().CreateLogger("HVE.UdpNotify");
		//private static readonly ILogger Logger = NLog.LogManager.GetCurrentClassLogger();
	}
}

using System;

namespace Model
{
	// Token: 0x020000AF RID: 175
	[Serializable]
	public class SentinelException : Exception
	{
		// Token: 0x060004E2 RID: 1250 RVA: 0x0000B90B File Offset: 0x00009B0B
		public SentinelException(Exception originalException, string message, string sourceValue) : base(message)
		{
			this._appSourceValue = sourceValue;
			SentryDebug.WriteException(originalException);
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x0000B921 File Offset: 0x00009B21
		public SentinelException(string message) : base(message)
		{
			this._appSourceValue = "";
			SentryDebug.WriteLine(message);
		}

		// Token: 0x060004E4 RID: 1252 RVA: 0x0000B93B File Offset: 0x00009B3B
		public SentinelException(string message, string sourceValue) : base(message)
		{
			this._appSourceValue = sourceValue;
			SentryDebug.WriteLine(message + "-" + sourceValue);
		}

		// Token: 0x060004E5 RID: 1253 RVA: 0x0000B95C File Offset: 0x00009B5C
		public SentinelException(Exception originalException) : base(originalException.Message)
		{
			if (originalException == null)
			{
				throw new ArgumentNullException("originalException");
			}
			this._appSourceValue = "";
			SentryDebug.WriteException(originalException);
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x060004E6 RID: 1254 RVA: 0x0000B989 File Offset: 0x00009B89
		public virtual string AppSource
		{
			get
			{
				return this._appSourceValue;
			}
		}

		// Token: 0x0400033C RID: 828
		private readonly string _appSourceValue;
	}
}

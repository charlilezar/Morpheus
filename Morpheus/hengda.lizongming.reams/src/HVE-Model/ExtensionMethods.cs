using System;
using System.Globalization;
using System.Xml;

namespace Model
{
	// Token: 0x020000B1 RID: 177
	public static class ExtensionMethods
	{
		// Token: 0x060004F1 RID: 1265 RVA: 0x0000BB78 File Offset: 0x00009D78
		public static string GetStringBetween(this string str, char firstChar, char lastChar)
		{
			int num = str.IndexOf(firstChar);
			int num2 = str.IndexOf(lastChar);
			if (num < 0 || num2 < 0)
			{
				return "";
			}
			string text = str.Substring(num + 1, num2 - num - 1);
			int num3 = text.IndexOf('/');
			if (num3 > 0)
			{
				return text.Substring(0, num3 - 1).Trim();
			}
			return text;
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x0000BBD0 File Offset: 0x00009DD0
		public static string GetStringBetween(this string str, string startStr, string endStr)
		{
			int num = str.IndexOf(startStr) + startStr.Length;
			int num2 = str.IndexOf(endStr);
			if (num >= 0 && num2 >= 0)
			{
				return str.Substring(num, num2 - num);
			}
			return "";
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x0000BC0C File Offset: 0x00009E0C
		public static T ReadElementEnum<T>(this XmlReader xmlReader, string elemName)
		{
			string value = xmlReader.ReadElementString(elemName);
			return (T)((object)Enum.Parse(typeof(T), value, true));
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x0000BC3C File Offset: 0x00009E3C
		public static DateTime ReadElementDateTime(this XmlReader xmlReader, string elemName)
		{
			string s = xmlReader.ReadElementString(elemName);
			DateTime result;
			try
			{
				result = DateTime.Parse(s);
			}
			catch (Exception)
			{
				result = DateTime.MinValue;
			}
			return result;
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x0000BC74 File Offset: 0x00009E74
		public static TimeSpan ReadElementTimeSpan(this XmlReader xmlReader, string elemName)
		{
			string s = xmlReader.ReadElementString(elemName);
			TimeSpan result;
			try
			{
				result = TimeSpan.Parse(s);
			}
			catch (Exception)
			{
				result = TimeSpan.Zero;
			}
			return result;
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x0000BCAC File Offset: 0x00009EAC
		public static int ReadElementInteger(this XmlReader xmlReader, string elemName)
		{
			int result;
			try
			{
				string s = xmlReader.ReadElementString(elemName);
				result = int.Parse(s);
			}
			catch (Exception)
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x0000BCE0 File Offset: 0x00009EE0
		public static bool ReadElementBoolean(this XmlReader xmlReader, string elemName)
		{
			string text = xmlReader.ReadElementString(elemName);
			return text.ToUpper() == "TRUE";
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x0000BD0C File Offset: 0x00009F0C
		public static char ReadElementChar(this XmlReader xmlReader, string elementName)
		{
			char result;
			try
			{
				string text = xmlReader.ReadElementString(elementName);
				result = text[0];
			}
			catch (Exception)
			{
				result = ' ';
			}
			return result;
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x0000BD44 File Offset: 0x00009F44
		public static double ReadElementDouble(this XmlReader xmlReader, string elemName)
		{
			double result;
			try
			{
				string s = xmlReader.ReadElementString(elemName);
				result = double.Parse(s, NumberStyles.Any, CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				result = 0.0;
			}
			return result;
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x0000BD8C File Offset: 0x00009F8C
		public static float ReadElementFloat(this XmlReader xmlReader, string elemName)
		{
			float result;
			try
			{
				string s = xmlReader.ReadElementString(elemName);
				result = float.Parse(s, NumberStyles.Any, CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				result = 0f;
			}
			return result;
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x0000BDD0 File Offset: 0x00009FD0
		public static string ParseForElement(this string xmlString, string elementName)
		{
			int num = xmlString.IndexOf(elementName);
			if (num < 0)
			{
				return "";
			}
			int num2 = num + elementName.Length + 1;
			int num3 = xmlString.IndexOf("<", num2);
			if (num2 == num3)
			{
				return "";
			}
			string text = xmlString.Substring(num2, num3 - num2);
			if (text[0] == '/')
			{
				return "";
			}
			return text;
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x0000BE2E File Offset: 0x0000A02E
		public static void WriteElementInteger(this XmlWriter writer, string name, int value)
		{
			writer.WriteElementString(name, value.ToString());
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x0000BE3E File Offset: 0x0000A03E
		public static void WriteElementChar(this XmlWriter writer, string name, char value)
		{
			writer.WriteElementString(name, value.ToString());
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x0000BE4E File Offset: 0x0000A04E
		public static void WriteElementDouble(this XmlWriter writer, string name, double value, string format)
		{
			writer.WriteElementString(name, value.ToString(format));
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x0000BE5F File Offset: 0x0000A05F
		public static void WriteElementBoolean(this XmlWriter writer, string name, bool value)
		{
			writer.WriteElementString(name, value.ToString());
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x0000BE6F File Offset: 0x0000A06F
		public static void WriteElementEnum<T>(this XmlWriter writer, string name, T value)
		{
			writer.WriteElementString(name, value.ToString());
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x0000BE85 File Offset: 0x0000A085
		public static void WriteElementDateTime(this XmlWriter writer, string name, DateTime time)
		{
			writer.WriteElementString(name, time.ToString("yyyy-MM-ddTHH\\:mm\\:ssZ"));
		}
	}
}

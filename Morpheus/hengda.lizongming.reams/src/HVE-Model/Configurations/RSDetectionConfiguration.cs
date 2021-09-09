using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using CommonLibrary;

namespace Model.Configurations
{
	// Token: 0x02000069 RID: 105
	public class RSDetectionConfiguration : Configuration, IXmlSerializable
	{
		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000251 RID: 593 RVA: 0x00005DD7 File Offset: 0x00003FD7
		// (set) Token: 0x06000252 RID: 594 RVA: 0x00005DDE File Offset: 0x00003FDE
		public static RSDetectionConfiguration Instance
		{
			get
			{
				return RSDetectionConfiguration._instance;
			}
			set
			{
				RSDetectionConfiguration._instance = value;
			}
		}

		// Token: 0x06000253 RID: 595 RVA: 0x00005DE8 File Offset: 0x00003FE8
		public RSDetectionConfiguration() : base(ConfigurationDefinition.RSDetectionConfiguration, false)
		{
			this.UnitAddress = '0';
			this.TCPSocketPort = 3010;
			this.UdpSocketPort = 3011;
			this.DefaultGateway = "192.168.0.1";
			this.SubnetMask = "255.255.255.0";
			this._staticIpAddress = "192.168.0.111";
			this.UseStaticIPAddress = false;
			this.LoggingEnable = true;
			this.EnablePingConnections = true;
			this.PingTimeout = 30;
			this.NotifyEnable = true;
			this.DebugLoggingEnable = true;
			this.EnablePerformanceMonitoring = true;
			this.BiasVoltageOnAtStartup = true;
			this.UnitName = "RSDetection";
			this.DateFormat = RSDetectionConfiguration.LegacyDateFormat.MM_DD_YY;
			this.MetDataInDCommand = false;
			this.NonStandardChecksum = false;
			this.FirmwareVersion = "Unknown";
			this.OSVersion = "Unknown";
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000254 RID: 596 RVA: 0x00005EAF File Offset: 0x000040AF
		// (set) Token: 0x06000255 RID: 597 RVA: 0x00005EB8 File Offset: 0x000040B8
		[Category("Network")]
		public string UnitName
		{
			get
			{
				return this._unitName;
			}
			set
			{
				char c = value[0];
				if (c < 'A')
				{
					return;
				}
				if (c > 'z')
				{
					return;
				}
				if (value.Length > 15)
				{
					this._unitName = value.Substring(0, 15);
					return;
				}
				this._unitName = value;
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000256 RID: 598 RVA: 0x00005EFA File Offset: 0x000040FA
		// (set) Token: 0x06000257 RID: 599 RVA: 0x00005F02 File Offset: 0x00004102
		[Category("Network")]
		[Browsable(false)]
		public int TCPSocketPort { get; set; }

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000258 RID: 600 RVA: 0x00005F0B File Offset: 0x0000410B
		// (set) Token: 0x06000259 RID: 601 RVA: 0x00005F13 File Offset: 0x00004113
		[Category("Network")]
		[Browsable(false)]
		public int UdpSocketPort { get; set; }

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x0600025A RID: 602 RVA: 0x00005F1C File Offset: 0x0000411C
		// (set) Token: 0x0600025B RID: 603 RVA: 0x00005F24 File Offset: 0x00004124
		[Category("Network")]
		public string StaticIpAddress
		{
			get
			{
				return this._staticIpAddress;
			}
			set
			{
				if (value.IsValidIpAddress())
				{
					this._staticIpAddress = value;
				}
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x0600025C RID: 604 RVA: 0x00005F35 File Offset: 0x00004135
		// (set) Token: 0x0600025D RID: 605 RVA: 0x00005F3D File Offset: 0x0000413D
		[Category("Network")]
		public string DefaultGateway
		{
			get
			{
				return this._defaultGateway;
			}
			set
			{
				if (value.IsValidIpAddress())
				{
					this._defaultGateway = value;
				}
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x0600025E RID: 606 RVA: 0x00005F4E File Offset: 0x0000414E
		// (set) Token: 0x0600025F RID: 607 RVA: 0x00005F56 File Offset: 0x00004156
		[Category("Network")]
		public string SubnetMask
		{
			get
			{
				return this._subnetMask;
			}
			set
			{
				if (value.IsValidIpAddress())
				{
					this._subnetMask = value;
				}
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000260 RID: 608 RVA: 0x00005F67 File Offset: 0x00004167
		// (set) Token: 0x06000261 RID: 609 RVA: 0x00005F6F File Offset: 0x0000416F
		[Category("Network")]
		public bool UseStaticIPAddress { get; set; }

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000262 RID: 610 RVA: 0x00005F78 File Offset: 0x00004178
		// (set) Token: 0x06000263 RID: 611 RVA: 0x00005F80 File Offset: 0x00004180
		[Category("Network")]
		public bool EnablePingConnections { get; set; }

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000264 RID: 612 RVA: 0x00005F89 File Offset: 0x00004189
		// (set) Token: 0x06000265 RID: 613 RVA: 0x00005F91 File Offset: 0x00004191
		[Category("Network")]
		[Browsable(false)]
		public int PingTimeout { get; set; }

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000266 RID: 614 RVA: 0x00005F9A File Offset: 0x0000419A
		// (set) Token: 0x06000267 RID: 615 RVA: 0x00005FA2 File Offset: 0x000041A2
		[Category("Control")]
		public bool LoggingEnable { get; set; }

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000268 RID: 616 RVA: 0x00005FAB File Offset: 0x000041AB
		// (set) Token: 0x06000269 RID: 617 RVA: 0x00005FB3 File Offset: 0x000041B3
		[Category("Control")]
		public bool NotifyEnable { get; set; }

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x0600026A RID: 618 RVA: 0x00005FBC File Offset: 0x000041BC
		// (set) Token: 0x0600026B RID: 619 RVA: 0x00005FC4 File Offset: 0x000041C4
		[Category("Control")]
		[Browsable(false)]
		public bool DebugLoggingEnable { get; set; }

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x0600026C RID: 620 RVA: 0x00005FCD File Offset: 0x000041CD
		// (set) Token: 0x0600026D RID: 621 RVA: 0x00005FD5 File Offset: 0x000041D5
		[Browsable(false)]
		[Category("Control")]
		public bool EnablePerformanceMonitoring { get; set; }

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x0600026E RID: 622 RVA: 0x00005FDE File Offset: 0x000041DE
		// (set) Token: 0x0600026F RID: 623 RVA: 0x00005FE6 File Offset: 0x000041E6
		[Category("Control")]
		public bool BiasVoltageOnAtStartup { get; set; }

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000270 RID: 624 RVA: 0x00005FEF File Offset: 0x000041EF
		// (set) Token: 0x06000271 RID: 625 RVA: 0x00005FF7 File Offset: 0x000041F7
		[Category("Legacy")]
		public char UnitAddress
		{
			get
			{
				return this._unitAddress;
			}
			set
			{
				if (value <= 'Z' && value >= '0' && value != '@')
				{
					this._unitAddress = value;
				}
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000272 RID: 626 RVA: 0x0000600F File Offset: 0x0000420F
		// (set) Token: 0x06000273 RID: 627 RVA: 0x00006017 File Offset: 0x00004217
		[Category("Legacy")]
		public RSDetectionConfiguration.LegacyDateFormat DateFormat { get; set; }

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000274 RID: 628 RVA: 0x00006020 File Offset: 0x00004220
		// (set) Token: 0x06000275 RID: 629 RVA: 0x00006028 File Offset: 0x00004228
		[Category("Legacy")]
		[Description("Include wind speed and wind direction in legacy \"D\" command.\nFormat: D Y M SSSS HHHH BB.BB AAAAA EEEE CC\nTrue - HHHH - Wind Speed, EEEE - Wind Direction\nFalse - HHHH - High Voltage, EEEE - Minutes Averging <Defalut>")]
		public bool MetDataInDCommand { get; set; }

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000276 RID: 630 RVA: 0x00006031 File Offset: 0x00004231
		// (set) Token: 0x06000277 RID: 631 RVA: 0x00006039 File Offset: 0x00004239
		[Category("Legacy")]
		[Description("Non-Standard checksum/CRC combination for legacy commands.\nTrue - Use CRC combination\nFalse - Sum of digits not including command or checksum itself <Default>")]
		public bool NonStandardChecksum { get; set; }

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000278 RID: 632 RVA: 0x00006042 File Offset: 0x00004242
		// (set) Token: 0x06000279 RID: 633 RVA: 0x0000604A File Offset: 0x0000424A
		[ReadOnly(true)]
		[Category("General")]
		public string FirmwareVersion { get; set; }

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x0600027A RID: 634 RVA: 0x00006053 File Offset: 0x00004253
		// (set) Token: 0x0600027B RID: 635 RVA: 0x0000605B File Offset: 0x0000425B
		[Category("General")]
		[ReadOnly(true)]
		public string OSVersion { get; set; }

		// Token: 0x0600027C RID: 636 RVA: 0x00006064 File Offset: 0x00004264
		public static RSDetectionConfiguration Deserialize(string xmlString)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(RSDetectionConfiguration));
			RSDetectionConfiguration result;
			using (StringReader stringReader = new StringReader(xmlString))
			{
				result = (RSDetectionConfiguration)xmlSerializer.Deserialize(stringReader);
			}
			return result;
		}

		// Token: 0x0600027D RID: 637 RVA: 0x000060B4 File Offset: 0x000042B4
		public void ReadXml(XmlReader reader)
		{
			try
			{
				reader.MoveToContent();
				reader.ReadStartElement();
				base.ReadBaseXml(reader);
				this.UnitName = reader.ReadElementString("UnitName");
				this.TCPSocketPort = reader.ReadElementInteger("TCPSocketPort");
				this.UdpSocketPort = reader.ReadElementInteger("UDPSocketPort");
				this.StaticIpAddress = reader.ReadElementString("StaticIpAddress");
				this.DefaultGateway = reader.ReadElementString("DefaultGateway");
				this.SubnetMask = reader.ReadElementString("SubnetMask");
				this.UseStaticIPAddress = reader.ReadElementBoolean("UseStaticIPAddress");
				this.LoggingEnable = reader.ReadElementBoolean("LoggingEnable");
				this.NotifyEnable = reader.ReadElementBoolean("NotifyEnable");
				this.DebugLoggingEnable = reader.ReadElementBoolean("DebugLoggingEnable");
				this.EnablePerformanceMonitoring = reader.ReadElementBoolean("EnablePerformanceMonitoring");
				this.BiasVoltageOnAtStartup = reader.ReadElementBoolean("BiasVoltageOnAtStartup");
				this.UnitAddress = reader.ReadElementChar("UnitAddress");
				this.DateFormat = reader.ReadElementEnum<RSDetectionConfiguration.LegacyDateFormat>("DateFormat");
				this.MetDataInDCommand = reader.ReadElementBoolean("MetDataInDCommand");
				this.NonStandardChecksum = reader.ReadElementBoolean("NonStandardChecksum");
				this.EnablePingConnections = reader.ReadElementBoolean("EnablePingConnections");
				this.PingTimeout = reader.ReadElementInteger("PingTimeout");
				this.FirmwareVersion = reader.ReadElementString("FirmwareVersion");
				this.OSVersion = reader.ReadElementString("OSVersion");
				reader.ReadEndElement();
				base.DeserializeSuccess = true;
			}
			catch (Exception)
			{
				base.DeserializeSuccess = false;
			}
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00006260 File Offset: 0x00004460
		public void WriteXml(XmlWriter writer)
		{
			base.WriteBaseXml(writer);
			writer.WriteElementString("UnitName", this.UnitName);
			writer.WriteElementInteger("TCPSocketPort", this.TCPSocketPort);
			writer.WriteElementInteger("UDPSocketPort", this.UdpSocketPort);
			writer.WriteElementString("StaticIpAddress", this.StaticIpAddress);
			writer.WriteElementString("DefaultGateway", this.DefaultGateway);
			writer.WriteElementString("SubnetMask", this.SubnetMask);
			writer.WriteElementBoolean("UseStaticIPAddress", this.UseStaticIPAddress);
			writer.WriteElementBoolean("LoggingEnable", this.LoggingEnable);
			writer.WriteElementBoolean("NotifyEnable", this.NotifyEnable);
			writer.WriteElementBoolean("DebugLoggingEnable", this.DebugLoggingEnable);
			writer.WriteElementBoolean("EnablePerformanceMonitoring", this.EnablePerformanceMonitoring);
			writer.WriteElementBoolean("BiasVoltageOnAtStartup", this.BiasVoltageOnAtStartup);
			writer.WriteElementChar("UnitAddress", this.UnitAddress);
			writer.WriteElementEnum("DateFormat", this.DateFormat);
			writer.WriteElementBoolean("MetDataInDCommand", this.MetDataInDCommand);
			writer.WriteElementBoolean("NonStandardChecksum", this.NonStandardChecksum);
			writer.WriteElementBoolean("EnablePingConnections", this.EnablePingConnections);
			writer.WriteElementInteger("PingTimeout", this.PingTimeout);
			writer.WriteElementString("FirmwareVersion", this.FirmwareVersion);
			writer.WriteElementString("OSVersion", this.OSVersion);
		}

		// Token: 0x0600027F RID: 639 RVA: 0x000063C8 File Offset: 0x000045C8
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x040001F7 RID: 503
		[Browsable(false)]
		public const byte SyncByte = 240;

		// Token: 0x040001F8 RID: 504
		private static RSDetectionConfiguration _instance;

		// Token: 0x040001F9 RID: 505
		private char _unitAddress;

		// Token: 0x040001FA RID: 506
		private string _unitName;

		// Token: 0x040001FB RID: 507
		private string _staticIpAddress;

		// Token: 0x040001FC RID: 508
		private string _defaultGateway;

		// Token: 0x040001FD RID: 509
		private string _subnetMask;

		// Token: 0x040001FE RID: 510
		[Browsable(false)]
		public static bool DisplayFactoryConfiguration = true;

		// Token: 0x0200006A RID: 106
		public enum LegacyDateFormat
		{
			// Token: 0x0400020F RID: 527
			MM_DD_YY,
			// Token: 0x04000210 RID: 528
			DD_MM_YY,
			// Token: 0x04000211 RID: 529
			YY_MM_DD
		}
	}
}

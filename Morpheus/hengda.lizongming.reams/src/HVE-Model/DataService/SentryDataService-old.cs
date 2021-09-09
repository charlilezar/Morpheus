using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
//using CommonLibrary;
//using GalaSoft.MvvmLight.Messaging;
using Model.Commands;
using Model.Configurations;
using Model.Electrometer;
using Model.Events;
using Model.Measurements;
//using Model.MvvmMessages;
using Model.Notifications;

namespace Model.DataService
{
	// Token: 0x02000073 RID: 115
	public class SentryDataService
	{
		// Token: 0x170000DE RID: 222
		// (get) Token: 0x060002DA RID: 730 RVA: 0x0000755B File Offset: 0x0000575B
		// (set) Token: 0x060002DB RID: 731 RVA: 0x00007562 File Offset: 0x00005762
		public static IPAddress LocalIpAddress { get; set; }

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x060002DC RID: 732 RVA: 0x0000756A File Offset: 0x0000576A
		// (set) Token: 0x060002DD RID: 733 RVA: 0x00007572 File Offset: 0x00005772
		public string ConnectionName { get; private set; }

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x060002DE RID: 734 RVA: 0x0000757B File Offset: 0x0000577B
		// (set) Token: 0x060002DF RID: 735 RVA: 0x00007583 File Offset: 0x00005783
		public bool IsOpen { get; private set; }

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x060002E0 RID: 736 RVA: 0x0000758C File Offset: 0x0000578C
		// (set) Token: 0x060002E1 RID: 737 RVA: 0x00007594 File Offset: 0x00005794
		public bool IsSerialConnection { get; private set; }

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x060002E2 RID: 738 RVA: 0x0000759D File Offset: 0x0000579D
		// (set) Token: 0x060002E3 RID: 739 RVA: 0x000075A4 File Offset: 0x000057A4
		public static bool IgnoreIpAddress { get; set; }

		/// <summary>
		/// 缓存测量值
		/// </summary>
		public static List<ScalarMeasurement> CacheScalarMeasurement { get; set; }

		// Token: 0x060002E4 RID: 740 RVA: 0x000075B8 File Offset: 0x000057B8
		public static List<IPAddress> GetHostIpAddress()
		{
			SentryDataService.LocalIpAddress = null;
			IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
			List<IPAddress> list = (from ip in hostEntry.AddressList
			where ip.AddressFamily == AddressFamily.InterNetwork
			select ip).Distinct().ToList<IPAddress>();  
			//hiaming过滤多余IP
			if (list.Count == 1)
			{
				SentryDataService.LocalIpAddress = list[0];
			}
			return list;
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x0000761C File Offset: 0x0000581C
		private SentryDataService(Stream stream, string connectionName, Messenger mvvmMessenger, bool isSerial)
		{
			this._mvvmMessenger = mvvmMessenger;
			if (!isSerial)
			{
				//UdpNotifyInput.OpenUdpClient(0, mvvmMessenger);
				//hiaming将本地UDP端口从随机改为固定 13005,从UdpNotifyInput.NotifyPortNumber设定的
				UdpNotifyInput.OpenUdpClient(UdpNotifyInput.NotifyPortNumber, mvvmMessenger);
			}
			this._stream = stream;
			this.ConnectionName = connectionName;
			SentryDebug.WriteLine("Created Service-" + this.ConnectionName + "");
			SentryDataService.ServiceDictionary.Add(connectionName, this);
			this._stream.BeginRead(this._receiveBuffer, 0, 5000000, new AsyncCallback(this.ReceiveCallback), null);
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x000076C8 File Offset: 0x000058C8
		public static Tuple<SentryDataService, Exception> StartSerialPortDataService(string commPortName, int baudRate, bool rtsControl)
		{
			Tuple<SentryDataService, Exception> result;
			try
			{
				SerialPort serialPort = new SerialPort(commPortName, baudRate, Parity.None)
				{
					RtsEnable = rtsControl
				};
				serialPort.Open();
				Stream baseStream = serialPort.BaseStream;
				SentryDataService sentryDataService = new SentryDataService(baseStream, commPortName, (Messenger)Messenger.Default, true)
				{
					_serialPort = serialPort,
					IsSerialConnection = true,
					IsOpen = true
				};
				//mvvmConnectionMessage message = new mvvmConnectionMessage(sentryDataService, mvvmConnectionMessage.ConnectedStatus.Connected);
				//sentryDataService._mvvmMessenger.Send<mvvmConnectionMessage>(message);
				result = new Tuple<SentryDataService, Exception>(sentryDataService, null);
			}
			catch (Exception)
			{
				result = new Tuple<SentryDataService, Exception>(null, new ApplicationException("Unable to open serial port " + commPortName));
			}
			return result;
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x00007774 File Offset: 0x00005974
		/// <summary>
		/// 启动TCP连接到设备
		/// </summary>
		/// <param name="ipAddress"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		public static Tuple<SentryDataService, Exception> StartSocketDataService(string ipAddress, int port)
		{
			Tuple<SentryDataService, Exception> result;
			try
			{
				Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				if (ipAddress == "localhost")
				{
					ipAddress = SentryDataService.LocalIpAddress.ToString();
				}
				IPAddress address = IPAddress.Parse(ipAddress);
				IPEndPoint remoteEP = new IPEndPoint(address, port);
				socket.Connect(remoteEP);
				Stream stream = new NetworkStream(socket);
				SentryDataService sentryDataService = new SentryDataService(stream, ipAddress, (Messenger)Messenger.Default, false)
				{
					IsSerialConnection = false,
					IsOpen = true
				};
				mvvmConnectionMessage message = new mvvmConnectionMessage(sentryDataService, mvvmConnectionMessage.ConnectedStatus.Connected);
				sentryDataService._mvvmMessenger.Send<mvvmConnectionMessage>(message);
				result = new Tuple<SentryDataService, Exception>(sentryDataService, null);
			}
			catch (SocketException item)
			{
				result = new Tuple<SentryDataService, Exception>(null, item);
			}
			return result;
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x0000782C File Offset: 0x00005A2C
		public static Tuple<SentryDataService, Exception> StartSocketDataService(Messenger mvvmMessenger, string ipAddress, int port = 0)
		{
			Tuple<SentryDataService, Exception> result;
			try
			{
				Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				if (ipAddress == "localhost")
				{
					ipAddress = SentryDataService.LocalIpAddress.ToString();
				}
				IPAddress address = IPAddress.Parse(ipAddress);
				IPEndPoint remoteEP = new IPEndPoint(address, port);
				socket.Connect(remoteEP);
				Stream stream = new NetworkStream(socket);
				SentryDataService sentryDataService = new SentryDataService(stream, ipAddress, mvvmMessenger, false)
				{
					IsSerialConnection = false,
					IsOpen = true
				};
				mvvmConnectionMessage message = new mvvmConnectionMessage(sentryDataService, mvvmConnectionMessage.ConnectedStatus.Connected);
				sentryDataService._mvvmMessenger.Send<mvvmConnectionMessage>(message);
				result = new Tuple<SentryDataService, Exception>(sentryDataService, null);
			}
			catch (SocketException item)
			{
				result = new Tuple<SentryDataService, Exception>(null, item);
			}
			return result;
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x000078DC File Offset: 0x00005ADC
		public void SetState(string stateName, string stateValue)
		{
			SetState obj = new SetState(stateName, stateValue);
			this.SendXml(Serializers.Serializer<SetState>(obj));
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00007900 File Offset: 0x00005B00
		public void SetNotifyEnable(bool enable)
		{
			NotifyEnable obj = new NotifyEnable(enable, UdpNotifyInput.NotifyPortNumber);
			this.SendXml(Serializers.Serializer<NotifyEnable>(obj));
		}

		// Token: 0x060002EB RID: 747 RVA: 0x00007928 File Offset: 0x00005B28
		public void SetElectrometerAcquisitionEnable(bool enable)
		{
			ElectrometerAcquisitionEnable obj = new ElectrometerAcquisitionEnable(enable);
			this.SendXml(Serializers.Serializer<ElectrometerAcquisitionEnable>(obj));
		}

		// Token: 0x060002EC RID: 748 RVA: 0x0000794C File Offset: 0x00005B4C
		public void GetAllConfigurations()
		{
			GetAllConfigurations obj = new GetAllConfigurations();
			this.SendXml(Serializers.Serializer<GetAllConfigurations>(obj));
		}

		// Token: 0x060002ED RID: 749 RVA: 0x0000796C File Offset: 0x00005B6C
		public void GetAllEvents()
		{
			GetAllEvents obj = new GetAllEvents();
			this.SendXml(Serializers.Serializer<GetAllEvents>(obj));
		}

		// Token: 0x060002EE RID: 750 RVA: 0x0000798C File Offset: 0x00005B8C
		public void GetEvent(string eventName)
		{
			GetEvent obj = new GetEvent(eventName);
			this.SendXml(Serializers.Serializer<GetEvent>(obj));
		}

		// Token: 0x060002EF RID: 751 RVA: 0x000079B0 File Offset: 0x00005BB0
		public void SetTime(DateTime time, bool ingnoreTimeZone)
		{
			SetTime obj = new SetTime(time, ingnoreTimeZone);
			this.SendXml(Serializers.Serializer<SetTime>(obj));
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x000079D4 File Offset: 0x00005BD4
		public void GetTime()
		{
			GetTime obj = new GetTime();
			this.SendXml(Serializers.Serializer<GetTime>(obj));
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x000079F4 File Offset: 0x00005BF4
		public void GetAllMeasurements(bool updatesOnly)
		{
			GetAllScalarMeasurements obj = new GetAllScalarMeasurements();
			this.SendXml(Serializers.Serializer<GetAllScalarMeasurements>(obj));
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00007A14 File Offset: 0x00005C14
		public void GetScalarMeasurement(string measName)
		{
			GetScalarMeasurement obj = new GetScalarMeasurement(measName);
			this.SendXml(Serializers.Serializer<GetScalarMeasurement>(obj));
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x00007A38 File Offset: 0x00005C38
		public bool SetConfiguration(Configuration config)
		{
			SetConfiguration obj = new SetConfiguration(config);
			return this.SendXml(Serializers.Serializer<SetConfiguration>(obj));
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00007A60 File Offset: 0x00005C60
		public bool GetConfiguration(string name)
		{
			GetConfiguration obj = new GetConfiguration(name);
			return this.SendXml(Serializers.Serializer<GetConfiguration>(obj));
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x00007A88 File Offset: 0x00005C88
		public void SendWaveformTrigger(WaveformTriggerMode mode)
		{
			WaveformTrigger obj = new WaveformTrigger(mode);
			this.SendXml(Serializers.Serializer<WaveformTrigger>(obj));
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x00007AAC File Offset: 0x00005CAC
		public void SendTestRequest(string testName, string testParameter)
		{
			TestRequest obj = new TestRequest(testName, testParameter);
			this.SendXml(Serializers.Serializer<TestRequest>(obj));
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x00007AD0 File Offset: 0x00005CD0
		public void SetHighVoltageControl(HighVoltageControlMethod method, double setpoint, double slope, double sampleInterval)
		{
			SetHighVoltageControl obj = new SetHighVoltageControl(method, (double)((float)setpoint), (double)((float)sampleInterval), (double)((float)slope));
			this.SendXml(Serializers.Serializer<SetHighVoltageControl>(obj));
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x00007AFC File Offset: 0x00005CFC
		public void SendSqlQuery(string query)
		{
			SqlQuery obj = new SqlQuery(query);
			this.SendXml(Serializers.Serializer<SqlQuery>(obj));
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x00007B20 File Offset: 0x00005D20
		public void SendMeasurementQuery(string name, DateTime start, DateTime end)
		{
			SqlMeasurementQuery obj = new SqlMeasurementQuery(name, start, end);
			this.SendXml(Serializers.Serializer<SqlMeasurementQuery>(obj));
		}

		// Token: 0x060002FA RID: 762 RVA: 0x00007B44 File Offset: 0x00005D44
		public void CancelMeasurementQuery()
		{
			SqlMeasurementQuery obj = new SqlMeasurementQuery(true);
			this.SendXml(Serializers.Serializer<SqlMeasurementQuery>(obj));
		}

		// Token: 0x060002FB RID: 763 RVA: 0x00007B68 File Offset: 0x00005D68
		public void SendEventQuery(string name, DateTime start, DateTime end)
		{
			SqlEventQuery obj = new SqlEventQuery(name, start, end);
			this.SendXml(Serializers.Serializer<SqlEventQuery>(obj));
		}

		// Token: 0x060002FC RID: 764 RVA: 0x00007B8C File Offset: 0x00005D8C
		public void SendAllEventQuery(DateTime start, DateTime end)
		{
			SqlEventQuery obj = new SqlEventQuery("All", start, end);
			this.SendXml(Serializers.Serializer<SqlEventQuery>(obj));
		}

		// Token: 0x060002FD RID: 765 RVA: 0x00007BB4 File Offset: 0x00005DB4
		public void GetDatabaseSummary(bool includeRecordCount)
		{
			GetDatabaseSummary obj = new GetDatabaseSummary(includeRecordCount);
			this.SendXml(Serializers.Serializer<GetDatabaseSummary>(obj));
		}

		// Token: 0x060002FE RID: 766 RVA: 0x00007BD8 File Offset: 0x00005DD8
		public void GetDatabaseMeasurementSummary(string measName, DateTime start, DateTime end)
		{
			GetDatabaseMeasurementSummary obj = new GetDatabaseMeasurementSummary(measName, start, end);
			this.SendXml(Serializers.Serializer<GetDatabaseMeasurementSummary>(obj));
		}

		// Token: 0x060002FF RID: 767 RVA: 0x00007BFC File Offset: 0x00005DFC
		public void DeleteDatabaseRecordsOlderThan(string table, DateTime time, bool compact)
		{
			DeleteDatabaseRecords obj = new DeleteDatabaseRecords(table, time, compact);
			this.SendXml(Serializers.Serializer<DeleteDatabaseRecords>(obj));
		}

		// Token: 0x06000300 RID: 768 RVA: 0x00007C20 File Offset: 0x00005E20
		public void SetElectrometerId(string serialNumber, string revision)
		{
			SetElectrometerId obj = new SetElectrometerId(serialNumber, revision);
			this.SendXml(Serializers.Serializer<SetElectrometerId>(obj));
		}

		// Token: 0x06000301 RID: 769 RVA: 0x00007C44 File Offset: 0x00005E44
		public void GetElectrometerId()
		{
			GetElectrometerId obj = new GetElectrometerId();
			this.SendXml(Serializers.Serializer<GetElectrometerId>(obj));
		}

		// Token: 0x06000302 RID: 770 RVA: 0x00007C64 File Offset: 0x00005E64
		public void SetUnitId(UnitInfo unitInfo)
		{
			SetUnitId obj = new SetUnitId(unitInfo);
			this.SendXml(Serializers.Serializer<SetUnitId>(obj));
		}

		// Token: 0x06000303 RID: 771 RVA: 0x00007C88 File Offset: 0x00005E88
		public void GetUnitId()
		{
			GetUnitId obj = new GetUnitId();
			this.SendXml(Serializers.Serializer<GetUnitId>(obj));
		}

		// Token: 0x06000304 RID: 772 RVA: 0x00007CA8 File Offset: 0x00005EA8
		public void GetHpicConfiguration()
		{
			GetHpicConfiguration obj = new GetHpicConfiguration();
			this.SendXml(Serializers.Serializer<GetHpicConfiguration>(obj));
		}

		// Token: 0x06000305 RID: 773 RVA: 0x00007CC8 File Offset: 0x00005EC8
		public void SetHpicConfiguration(HpicConfiguration config)
		{
			SetHpicConfiguration obj = new SetHpicConfiguration(config);
			this.SendXml(Serializers.Serializer<SetHpicConfiguration>(obj));
		}

		// Token: 0x06000306 RID: 774 RVA: 0x00007CEC File Offset: 0x00005EEC
		public void SetElectrometerConfiguration(ElectrometerConfiguration config, bool writeToEeprom)
		{
			SetElectrometerConfiguration obj = new SetElectrometerConfiguration(config, writeToEeprom);
			this.SendXml(Serializers.Serializer<SetElectrometerConfiguration>(obj));
		}

		// Token: 0x06000307 RID: 775 RVA: 0x00007D10 File Offset: 0x00005F10
		public void GetElectrometerConfiguration()
		{
			GetElectrometerConfiguration obj = new GetElectrometerConfiguration();
			this.SendXml(Serializers.Serializer<GetElectrometerConfiguration>(obj));
		}

		// Token: 0x06000308 RID: 776 RVA: 0x00007D30 File Offset: 0x00005F30
		public void SetElectrometerConfigurationToDefault()
		{
			SetElectrometerConfigurationToDefault obj = new SetElectrometerConfigurationToDefault();
			this.SendXml(Serializers.Serializer<SetElectrometerConfigurationToDefault>(obj));
		}

		// Token: 0x06000309 RID: 777 RVA: 0x00007D50 File Offset: 0x00005F50
		public void GetPerformanceMeasurement(string name)
		{
			GetPerformanceMeasurement obj = new GetPerformanceMeasurement(name);
			this.SendXml(Serializers.Serializer<GetPerformanceMeasurement>(obj));
		}

		// Token: 0x0600030A RID: 778 RVA: 0x00007D74 File Offset: 0x00005F74
		public void ClearPerformanceMeasurement(string name)
		{
			ClearPerformanceHistogram obj = new ClearPerformanceHistogram(name);
			this.SendXml(Serializers.Serializer<ClearPerformanceHistogram>(obj));
		}

		// Token: 0x0600030B RID: 779 RVA: 0x00007D98 File Offset: 0x00005F98
		public void SetChargerParameter(double voltage, double current)
		{
			SetChargerParameters obj = new SetChargerParameters(voltage, current);
			this.SendXml(Serializers.Serializer<SetChargerParameters>(obj));
		}

		// Token: 0x0600030C RID: 780 RVA: 0x00007DBC File Offset: 0x00005FBC
		public bool SendDiscovery(string commChannelName)
		{
			bool result;
			try
			{
				SentryDebug.WriteLine("SendDiscovery" + commChannelName);
				Discovery obj = new Discovery(commChannelName);
				string ouputText = Serializers.Serializer<Discovery>(obj);
				this.SendXml(ouputText);
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600030D RID: 781 RVA: 0x00007E0C File Offset: 0x0000600C
		public void LogTestData(LogTestMeasurements testMeasurements)
		{
			this.SendXml(Serializers.Serializer<LogTestMeasurements>(testMeasurements));
		}

		// Token: 0x0600030E RID: 782 RVA: 0x00007E1B File Offset: 0x0000601B
		public void StartFirmwareUpdate()
		{
			this.SendXml(Serializers.Serializer<InitiateFirmwareUpdate>(new InitiateFirmwareUpdate()));
		}

		// Token: 0x0600030F RID: 783 RVA: 0x00007E2E File Offset: 0x0000602E
		public void ClearDebugLogFile()
		{
			this.SendXml(Serializers.Serializer<ClearDebugLog>(new ClearDebugLog()));
		}

		// Token: 0x06000310 RID: 784 RVA: 0x00007E41 File Offset: 0x00006041
		public void CopyDatabaseFile()
		{
			this.SendXml(Serializers.Serializer<CopyDatabaseFile>(new CopyDatabaseFile()));
		}

		// Token: 0x06000311 RID: 785 RVA: 0x00007E54 File Offset: 0x00006054
		public void CompactDatabaseFile()
		{
			this.SendXml(Serializers.Serializer<SqlDatabaseCompact>(new SqlDatabaseCompact()));
		}

		// Token: 0x06000312 RID: 786 RVA: 0x00007E67 File Offset: 0x00006067
		public void CleanDatabaseFile()
		{
			this.SendXml(Serializers.Serializer<SqlDatabaseClean>(new SqlDatabaseClean()));
		}

		// Token: 0x06000313 RID: 787 RVA: 0x00007E7C File Offset: 0x0000607C
		public bool SendXml(string ouputText)
		{
			bool result=false;
			try
			{
				int length = ouputText.Length;
				byte[] array = new byte[length + 5];
				array[0] = 240;
				Array.Copy(BitConverter.GetBytes(length), 0, array, 1, 4);
				Array.Copy(Encoding.ASCII.GetBytes(ouputText), 0, array, 5, length);
				if (DebugXml)
				{
					Logger.Trace($" ###发送 SendXml request lenght:{length} :\r\n" + "```\r\n" + ouputText + "\r\n```\r\n");
				}
				if (this._stream != null)
				{
					this._stream.Write(array, 0, length + 5);
					result = true;
				}
			}
			catch (Exception ex)
			{
				SentryDebug.WriteLine(ouputText);
				SentryDebug.WriteException(ex);
				result = false;
			}
			return result;
		}

		// Token: 0x06000314 RID: 788 RVA: 0x00007EFC File Offset: 0x000060FC
		public void SendCloseConnectionRequest()
		{
			CloseConnection obj = new CloseConnection();
			this.SendXml(Serializers.Serializer<CloseConnection>(obj));
		}

		// Token: 0x06000315 RID: 789 RVA: 0x00007F1C File Offset: 0x0000611C
		public void SendRestartUnitRequest()
		{
			RestartUnit obj = new RestartUnit();
			this.SendXml(Serializers.Serializer<RestartUnit>(obj));
		}

		// Token: 0x06000316 RID: 790 RVA: 0x00007F3C File Offset: 0x0000613C
		public void CloseSerialPort()
		{
			if (this._serialPort != null)
			{
				SentryDebug.WriteLine("CloseSerialPort");
				this._serialPort.Close();
				this._serialPort.Dispose();
				this._serialPort = null;
			}
		}

		// Token: 0x06000317 RID: 791 RVA: 0x00007F6D File Offset: 0x0000616D
		public void CloseDataService()
		{
			this._dataServiceClosing = true;
			SentryDataService.ServiceDictionary.Remove(this.ConnectionName);
			if (this._stream != null)
			{
				this._stream.Close();
			}
			if (this.IsSerialConnection)
			{
				this._serialPort.Dispose();
			}
		}

		// Token: 0x06000318 RID: 792 RVA: 0x00007FAD File Offset: 0x000061AD
		public void CloseUnitConnection()
		{
			if (this._stream != null)
			{
				this.SendCloseConnectionRequest();
			}
		}

		// Token: 0x06000319 RID: 793 RVA: 0x00007FC0 File Offset: 0x000061C0
		private void DumpBuffer(byte[] buffer, int count)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Serial Bytes recvd={0}\r\n", count);
			for (int i = 0; i < count; i++)
			{
				stringBuilder.AppendFormat("{0:X},", buffer[i]);
				if (i % 16 == 0)
				{
					stringBuilder.AppendLine();
				}
			}
			SentryDebug.WriteLine(stringBuilder.ToString());
		}

		// Token: 0x0600031A RID: 794 RVA: 0x00008020 File Offset: 0x00006220
		private bool FindSyncByte(int bytesReadFromBuffer)
		{
			for (int i = this._receiveBufferPointer; i < bytesReadFromBuffer; i++)
			{
				if (this._receiveBuffer[i] == 240)
				{
					this._receiveBufferPointer = i + 1;
					this._responseBufferPointer = 0;
					this._responseState = SentryDataService.ReadResponseState.WaitingForByteCount;
					SentryDebug.WriteLineConditional("m_ReceiveBufferPointer=" + this._receiveBufferPointer.ToString());
					return this._receiveBufferPointer < bytesReadFromBuffer;
				}
			}
			return false;
		}

		// Token: 0x0600031B RID: 795 RVA: 0x0000808C File Offset: 0x0000628C
		private bool ReadResponseByteCount(int readBufferLength, ref int bytesRead)
		{
			int num = 4 - this._lengthBytesRead;
			int num2 = readBufferLength - this._receiveBufferPointer;
			if (num2 < num)
			{
				SentryDebug.WriteLine("Partial length buffer recvd");
				Array.Copy(this._receiveBuffer, this._receiveBufferPointer, this._lengthBuffer, this._lengthBytesRead, num2);
				this._lengthBytesRead += num2;
				return false;
			}
			Array.Copy(this._receiveBuffer, this._receiveBufferPointer, this._lengthBuffer, this._lengthBytesRead, num);
			this._receiveBufferPointer += num;
			this._lengthBytesRead = 0;
			bytesRead = BitConverter.ToInt32(this._lengthBuffer, 0);
			if (this._receiveBufferPointer > 4 && this._receiveBuffer[this._receiveBufferPointer - 5] != 240)
			{
				this._responseState = SentryDataService.ReadResponseState.WaitingForSync;
				return true;
			}
			int num3 = bytesRead;
			SentryDebug.WriteLineConditional("Byte count=" + bytesRead.ToString());
			this._responseState = SentryDataService.ReadResponseState.ReadingResponse;
			return readBufferLength > num;
		}

		// Token: 0x0600031C RID: 796 RVA: 0x0000817C File Offset: 0x0000637C
		private bool ReadAndProcessResponse(int readBufferLength, int responseLength)
		{
			SentryDebug.WriteLineConditional("responseLength=" + responseLength.ToString());
			int num = readBufferLength - this._receiveBufferPointer;
			int num2 = responseLength - this._responseBufferPointer;
			SentryDebug.WriteLineConditional("bytesRemainingToReadIntoResponseBuffer=" + num2.ToString());
			if (num >= num2)
			{
				SentryDebug.WriteLineConditional(string.Concat(new string[]
				{
					"Cmd Complete ",
					num.ToString(),
					"-",
					num2.ToString(),
					" - ",
					this._responseBufferPointer.ToString()
				}));
				Array.Copy(this._receiveBuffer, this._receiveBufferPointer, this._responseBuffer, this._responseBufferPointer, num2);
				this._receiveBufferPointer += num2;
				this._responseBufferPointer = 0;
				this._responseState = SentryDataService.ReadResponseState.WaitingForSync;
				int num3 = 500;
				if (responseLength < num3)
				{
					num3 = responseLength;
				}
				string @string = Encoding.ASCII.GetString(this._responseBuffer, 0, num3);
				SentryDebug.WriteLineConditional("Cmd Complete(patial)\r" + @string);
				this.ProcessResponse(this._responseBuffer, responseLength);
				return readBufferLength > this._receiveBufferPointer;
			}
			if (Array.IndexOf<byte>(this._receiveBuffer, 240, this._receiveBufferPointer, num) >= 0 || num2 < 0 || num < 0)
			{
				SentryDebug.WriteLine("Lost Sync");
				this._responseState = SentryDataService.ReadResponseState.WaitingForSync;
				this._mvvmMessenger.Send<mvvmMeasurementQueryResponse>(new mvvmMeasurementQueryResponse(this, null));
				return true;
			}
			Array.Copy(this._receiveBuffer, this._receiveBufferPointer, this._responseBuffer, this._responseBufferPointer, num);
			this._receiveBufferPointer += num;
			this._responseBufferPointer += num;
			SentryDebug.WriteLineConditional("Char count so far " + this._responseBufferPointer.ToString());
			return false;
		}

		// Token: 0x0600031D RID: 797 RVA: 0x00008340 File Offset: 0x00006540
		public void ReceiveCallback(IAsyncResult ar)
		{
			try
			{
				int num = this._stream.EndRead(ar);
				SentryDebug.WriteLineConditional("Bytes Read=" + num.ToString());
				this._receiveBufferPointer = 0;
				bool flag = true;
				while (flag)
				{
					if (this._responseState == SentryDataService.ReadResponseState.WaitingForSync)
					{
						flag = this.FindSyncByte(num);
					}
					else if (this._responseState == SentryDataService.ReadResponseState.WaitingForByteCount)
					{
						flag = this.ReadResponseByteCount(num, ref this._responseByteCount);
					}
					else
					{
						flag = this.ReadAndProcessResponse(num, this._responseByteCount);
					}
				}
				if (this._dataServiceClosing)
				{
					this.CloseDataService();
					mvvmConnectionMessage message = new mvvmConnectionMessage(this, mvvmConnectionMessage.ConnectedStatus.Closed);
					this._mvvmMessenger.Send<mvvmConnectionMessage>(message);
				}
				else
				{
					this._stream.BeginRead(this._receiveBuffer, 0, 5000000, new AsyncCallback(this.ReceiveCallback), null);
				}
			}
			catch (Exception ex)
			{
				this._stream = null;
				this.IsOpen = false;
				SentryDebug.WriteLine(ex.ToString());
			}
		}

		// Token: 0x0600031E RID: 798 RVA: 0x00008424 File Offset: 0x00006624
		public void ProcessResponse(byte[] buffer, int length)
		{
			string @string = Encoding.ASCII.GetString(buffer, 0, length);
			///SentryDebug.WriteLine("response received " + @string);
			if (DebugXml)
			{
				Logger.Trace($" ==="+(IsSerialConnection ? "串口":"TCP")+$"接收到 Response : lenght:{length} :\r\n```\r\n" + @string+"\r\n```\r\n");
			}

			Tuple<object, Type> tuple = SentryDataService.DeserializeUnknownCommand(@string);
			if (tuple.Item2 == typeof(CommandStatusResponse))
			{
				if (((CommandStatusResponse)tuple.Item1).Command == "CloseConnection")
				{
					this._dataServiceClosing = true;
				}
				this._mvvmMessenger.Send<mvvmCommandStatusResponse>(new mvvmCommandStatusResponse(this, (CommandStatusResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(CommandStatusResponse))
			{
				if (((CommandStatusResponse)tuple.Item1).Command == "RestartUnit")
				{
					this._dataServiceClosing = true;
				}
				this._mvvmMessenger.Send<mvvmCommandStatusResponse>(new mvvmCommandStatusResponse(this, (CommandStatusResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(ScalarMeasurementResponse))
			{
				this._mvvmMessenger.Send<mvvmScalarMeasurement>(new mvvmScalarMeasurement(this, (ScalarMeasurementResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(EventResponse))
			{
				this._mvvmMessenger.Send<mvvmSentinelState>(new mvvmSentinelState(this, (EventResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(PerformanceMeasurementResponse))
			{
				this._mvvmMessenger.Send<mvvmPerformanceMeasurementResponse>(new mvvmPerformanceMeasurementResponse(this, (PerformanceMeasurementResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(GetAllEventsResponse))
			{
				this._mvvmMessenger.Send<mvvmGetAllEventsResponse>(new mvvmGetAllEventsResponse(this, (GetAllEventsResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(GetAllConfigurationsResponse))
			{
				this._mvvmMessenger.Send<mvvmGetAllConfigurationsResponse>(new mvvmGetAllConfigurationsResponse(this, (GetAllConfigurationsResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(GetConfigurationResponse))
			{
				this._mvvmMessenger.Send<mvvmGetConfigurationResponse>(new mvvmGetConfigurationResponse(this, (GetConfigurationResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(GetAllScalarMeasurementsResponse))
			{
				this._mvvmMessenger.Send<mvvmGetAllScalarMeasurementsResponse>(new mvvmGetAllScalarMeasurementsResponse(this, (GetAllScalarMeasurementsResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(GetElectrometerConfigurationResponse))
			{
				this._mvvmMessenger.Send<mvvmGetElectrometerConfigurationResponse>(new mvvmGetElectrometerConfigurationResponse(this, (GetElectrometerConfigurationResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(GetElectrometerIdResponse))
			{
				this._mvvmMessenger.Send<mvvmGetElectrometerIdResponse>(new mvvmGetElectrometerIdResponse(this, (GetElectrometerIdResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(GetUnitIdResponse))
			{
				this._mvvmMessenger.Send<mvvmGetUnitIdResponse>(new mvvmGetUnitIdResponse(this, (GetUnitIdResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(GetHpicConfigurationResponse))
			{
				this._mvvmMessenger.Send<mvvmGetHpicConfigurationResponse>(new mvvmGetHpicConfigurationResponse(this, (GetHpicConfigurationResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(SqlQueryResponse))
			{
				this._mvvmMessenger.Send<mvvmDatasetResponse>(new mvvmDatasetResponse(this, (SqlQueryResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(SqlEventQueryResponse))
			{
				this._mvvmMessenger.Send<mvvmEventQueryResponse>(new mvvmEventQueryResponse(this, (SqlEventQueryResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(SqlMeasurementQueryResponse))
			{
				this._mvvmMessenger.Send<mvvmMeasurementQueryResponse>(new mvvmMeasurementQueryResponse(this, (SqlMeasurementQueryResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(TestResponse))
			{
				this._mvvmMessenger.Send<mvvmTestResponse>(new mvvmTestResponse(this, (TestResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(GetDatabaseSummaryResponse))
			{
				this._mvvmMessenger.Send<mvvmGetDatabaseSummaryResponse>(new mvvmGetDatabaseSummaryResponse(this, (GetDatabaseSummaryResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(DiscoveryResponse))
			{
				this._mvvmMessenger.Send<DiscoveryResponse>((DiscoveryResponse)tuple.Item1);
				return;
			}
			if (tuple.Item2 == typeof(GetTimeResponse))
			{
				this._mvvmMessenger.Send<mvvmGetTimeResponse>(new mvvmGetTimeResponse(this, (GetTimeResponse)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(ScalarMeasurement))
			{
				this._mvvmMessenger.Send<mvvmScalarMeasurement>(new mvvmScalarMeasurement(this, (ScalarMeasurement)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(SentinelState))
			{
				this._mvvmMessenger.Send<mvvmSentinelState>(new mvvmSentinelState(this, (SentinelState)tuple.Item1));
				return;
			}
			if (tuple.Item2 == typeof(BatteryMeasurement))
			{
				this._mvvmMessenger.Send<mvvmBatteryMeasurement>(new mvvmBatteryMeasurement(this, (BatteryMeasurement)tuple.Item1));
				return;
			}
			SentryDebug.WriteLine("Unrecognized response received " + @string);
		}

		// Token: 0x0600031F RID: 799 RVA: 0x00008944 File Offset: 0x00006B44
		private static Tuple<object, Type> DeserializeUnknownCommand(string xmlCommand)
		{
			Tuple<object, Type> result;
			try
			{
				if (xmlCommand.Contains(typeof(CommandStatusResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<CommandStatusResponse>(xmlCommand), typeof(CommandStatusResponse));
				}
				else if (xmlCommand.Contains(typeof(GetAllConfigurationsResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<GetAllConfigurationsResponse>(xmlCommand), typeof(GetAllConfigurationsResponse));
				}
				else if (xmlCommand.Contains(typeof(GetConfigurationResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<GetConfigurationResponse>(xmlCommand), typeof(GetConfigurationResponse));
				}
				else if (xmlCommand.Contains(typeof(GetAllScalarMeasurementsResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<GetAllScalarMeasurementsResponse>(xmlCommand), typeof(GetAllScalarMeasurementsResponse));
				}
				else if (xmlCommand.Contains(typeof(GetAllEventsResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<GetAllEventsResponse>(xmlCommand), typeof(GetAllEventsResponse));
				}
				else if (xmlCommand.Contains(typeof(EventResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<EventResponse>(xmlCommand), typeof(EventResponse));
				}
				else if (xmlCommand.Contains(typeof(PerformanceMeasurementResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<PerformanceMeasurementResponse>(xmlCommand), typeof(PerformanceMeasurementResponse));
				}
				else if (xmlCommand.Contains(typeof(TestResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<TestResponse>(xmlCommand), typeof(TestResponse));
				}
				else if (xmlCommand.Contains(typeof(SqlQueryResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<SqlQueryResponse>(xmlCommand), typeof(SqlQueryResponse));
				}
				else if (xmlCommand.Contains(typeof(SqlEventQueryResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<SqlEventQueryResponse>(xmlCommand), typeof(SqlEventQueryResponse));
				}
				else if (xmlCommand.Contains(typeof(SqlMeasurementQueryResponse).Name))
				{
					SqlMeasurementQueryResponse item = Serializers.Deserialize<SqlMeasurementQueryResponse>(xmlCommand);
					result = new Tuple<object, Type>(item, typeof(SqlMeasurementQueryResponse));
				}
				else if (xmlCommand.Contains(typeof(GetElectrometerIdResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<GetElectrometerIdResponse>(xmlCommand), typeof(GetElectrometerIdResponse));
				}
				else if (xmlCommand.Contains(typeof(GetUnitIdResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<GetUnitIdResponse>(xmlCommand), typeof(GetUnitIdResponse));
				}
				else if (xmlCommand.Contains(typeof(GetHpicConfigurationResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<GetHpicConfigurationResponse>(xmlCommand), typeof(GetHpicConfigurationResponse));
				}
				else if (xmlCommand.Contains(typeof(GetElectrometerConfigurationResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<GetElectrometerConfigurationResponse>(xmlCommand), typeof(GetElectrometerConfigurationResponse));
				}
				else if (xmlCommand.Contains(typeof(ScalarMeasurementResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<ScalarMeasurementResponse>(xmlCommand), typeof(ScalarMeasurementResponse));
				}
				else if (xmlCommand.Contains(typeof(GetDatabaseSummaryResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<GetDatabaseSummaryResponse>(xmlCommand), typeof(GetDatabaseSummaryResponse));
				}

				//hiamin 注：原来没有Discovery这一样，也不能加，否则就发现不了设备了

				else if (xmlCommand.Contains(typeof(DiscoveryResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<DiscoveryResponse>(xmlCommand), typeof(DiscoveryResponse));
				}
				else if (xmlCommand.Contains(typeof(GetTimeResponse).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<GetTimeResponse>(xmlCommand), typeof(GetTimeResponse));
				}
				else if (xmlCommand.Contains(typeof(ScalarMeasurement).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<ScalarMeasurement>(xmlCommand), typeof(ScalarMeasurement));
				}
				else if (xmlCommand.Contains(typeof(SentinelState).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<SentinelState>(xmlCommand), typeof(SentinelState));
				}
				else if (xmlCommand.Contains(typeof(BatteryMeasurement).Name))
				{
					result = new Tuple<object, Type>(Serializers.Deserialize<BatteryMeasurement>(xmlCommand), typeof(BatteryMeasurement));
				}
				else
				{
					result = null;
				}

			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000320 RID: 800 RVA: 0x00008DBC File Offset: 0x00006FBC
		private void DumpReceivedBuffer(int bytesRead)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 20;
			if (bytesRead < num)
			{
				num = bytesRead;
			}
			stringBuilder.AppendFormat("DumpReceivedBuffer bytes read=" + bytesRead.ToString(), new object[0]);
			for (int i = 0; i < num; i++)
			{
				stringBuilder.AppendFormat("{0}-", this._receiveBuffer[i]);
			}
			SentryDebug.WriteLine(stringBuilder.ToString());
		}

		// Token: 0x04000224 RID: 548
		public const int NotifyInputSocketPort = 0;

		// Token: 0x04000225 RID: 549
		private const int InputBufferSize = 5000000;

		// Token: 0x04000226 RID: 550
		public static Dictionary<string, SentryDataService> ServiceDictionary = new Dictionary<string, SentryDataService>();

		// Token: 0x04000227 RID: 551
		private Stream _stream;

		// Token: 0x04000228 RID: 552
		private SerialPort _serialPort;

		// Token: 0x04000229 RID: 553
		//private readonly Messenger _mvvmMessenger;

		// Token: 0x0400022A RID: 554
		private bool _dataServiceClosing;

		// Token: 0x0400022B RID: 555
		private readonly byte[] _receiveBuffer = new byte[5000000];

		// Token: 0x0400022C RID: 556
		private int _receiveBufferPointer;

		// Token: 0x0400022D RID: 557
		private readonly byte[] _responseBuffer = new byte[5000000];

		// Token: 0x0400022E RID: 558
		private int _responseBufferPointer;

		// Token: 0x0400022F RID: 559
		private readonly byte[] _lengthBuffer = new byte[4];

		// Token: 0x04000230 RID: 560
		private int _lengthBytesRead;

		// Token: 0x04000231 RID: 561
		private int _responseByteCount;

		// Token: 0x04000232 RID: 562
		private SentryDataService.ReadResponseState _responseState;

		// Token: 0x02000074 RID: 116
		private enum ReadResponseState
		{
			// Token: 0x0400023A RID: 570
			WaitingForSync,
			// Token: 0x0400023B RID: 571
			WaitingForByteCount,
			// Token: 0x0400023C RID: 572
			ReadingResponse
		}

		//hiaming 打印xml通讯细节，日志会很多，启动时命令行添加   /debugxml可开启
		public static bool DebugXml = false;
		internal static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

	}
}

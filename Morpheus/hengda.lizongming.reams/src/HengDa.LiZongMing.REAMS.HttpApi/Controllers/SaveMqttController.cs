//using HengDa.LiZongMing.Yun.Devices;
//using HengDa.LiZongMing.Yun.Devices.Dtos;
//using HengDa.LiZongMing.Yun.Devices.Services;
using HengDa.LiZongMing.REAMS.ZCQ;
using HengDa.LiZongMing.Yun.Localization;
//using HengDa.LiZongMing.Yun.Localization;
//using HengDa.LiZongMing.Yun.MongoDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
//using Microsoft.EntityFrameworkCore;
//using MongoDB.Bson;
//using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Uow;


namespace HengDa.LiZongMing.REAMS.Controllers
{
    /* Inherit your controllers from this class.
     */
    [Route("api/[controller]")]
    public class SaveDtuMqttController : AbpController
    {
        public class MqttMessage
        {
            //{
            //  "topic": "t/a",
            //  "payload": "{\"msg222\":\"hello\"}",
            //  "clientid": "c_emqx"
            //}
            public string topic { get; set; }
            public string act { get; set; }
            public string clientid { get; set; }
            public long? timestamp { get; set; }
            public string username { get; set; }

            /// <summary>
            /// 消息主内容
            /// </summary>
            public string payload { get; set; }
        }

        Regex rNum = new Regex(@"^\d+$");

        //protected readonly YunDllMongoDbContext dbContext;
        //protected readonly DeviceCustomerRepository repDeviceCustomer;
        //protected readonly DeviceSettingRepository repDeviceSetting;
        //protected readonly DeviceLogRepository repDeviceLog;
        //protected readonly DeviceTestRepository repDeviceTest;

        //protected readonly DeviceCustomerService servDeviceCustomer;
        protected readonly IRepository<ZcqRecord, Guid> repZcqRecord;
        //protected readonly IRepository<DeviceSetting, Guid> repDeviceSetting;
        //protected readonly IRepository<DeviceLog, Guid> repDeviceLog;
        //protected readonly IRepository<DeviceTest, Guid> repDeviceTest;
        public SaveDtuMqttController(/*YunDllMongoDbContext dbContext*/
                                    //DeviceCustomerService servDeviceCustomer,
                                    //IRepository<DeviceCustomer, Guid> repDeviceCustomer,
                                    //IRepository<DeviceSetting, Guid> repDeviceSetting,
                                    //IRepository<DeviceLog, Guid> repDeviceLog,
                                    IRepository<ZcqRecord, Guid> repZcqRecord

            )
        {
            //this.servDeviceCustomer = servDeviceCustomer;
            this.repZcqRecord = repZcqRecord;
            //this.repDeviceSetting = repDeviceSetting;
            //this.repDeviceLog = repDeviceLog;
            //this.repDeviceTest = repDeviceTest;

            //this.dbContext = dbContext;

            //LocalizationResource = typeof(YunResource);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return Ok("请用post访问 ByTopic ");
        }
        //[HttpPost]
        //public ActionResult Index([FromBody] MqttMessage mqttMessage)
        //{
        //    string drivername = "testdriver"; // string json = "{ 'name':'未命名' }";
        //    BsonDocument document = new BsonDocument();
        //    document["topic"] = mqttMessage.topic;
        //    document["clientid"] = mqttMessage.clientid;
        //    document["timestamp"] = mqttMessage.timestamp;
        //    document["username"] = mqttMessage.username;
        //    if (mqttMessage.payload.IsNullOrWhiteSpace())
        //    {
        //        return Ok(0);
        //    }
        //    else if (mqttMessage.payload.StartsWith("{"))
        //    {
        //        document.Merge(BsonDocument.Parse(mqttMessage.payload));
        //    }
        //    else //是纯文件消息或hex消息
        //    {
        //        document["payload"] = mqttMessage.payload;
        //    }

        //    var topics = mqttMessage.topic.Split('/');

        //    //用产品名做表名
        //    drivername = topics.Length > 2 ? topics[2] : (topics.Length > 1 ? topics[1]: topics[0]);
        //    if (document.TryGetValue("Act", out var Cmd))
        //    {
        //        drivername += "_" + document["Act"].AsString;
        //    }


        //    //var db = dbContext.Database取到的都是null;
        //    var client = new MongoClient("mongodb://localhost:27017");

        //    var db = client.GetDatabase("test");

        //    var collection = db.GetCollection<BsonDocument>(drivername); //插入数据的表
        //    collection.InsertOne(document);

        //    return Ok(true);
        //}


        /// <summary>
        /// 按主题分类实体类型保存
        /// </summary>
        /// <param name="mqttMessage"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Index([FromBody] MqttMessage mqttMessage)
        {
            //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            //var h = Aming.Core.ConvertHelper.StringToHexString("系统开机区", System.Text.Encoding.GetEncoding("gbk"), " ");

            //var bytes = Request.Body.GetAllBytes();
            //var g=System.Text.Encoding.GetEncoding("gbk").GetString(bytes);

            BsonDocument document = new BsonDocument();
            if (mqttMessage.payload.IsNullOrWhiteSpace())
            {
                return Ok(0);
            }
            else if (mqttMessage.payload.StartsWith("{") || mqttMessage.payload.StartsWith("["))
            {
                document.Merge(BsonDocument.Parse(mqttMessage.payload));
            }
            else //是纯文件消息或hex消息
            {
                document["payload"] = mqttMessage.payload;
            }

            var topics = mqttMessage.topic.Split('/');

            //用产品名做表名
            string Productid = topics.Length > 2 ? topics[2] : (topics.Length > 1 ? topics[1] : topics[0]);
            if (mqttMessage.act.IsNullOrWhiteSpace())
            {
                mqttMessage.act = topics.Last(); //用最后一段当做Act
                                                 //if (document.TryGetValue("Act", out var Act1))
                                                 //{
                                                 //    //drivername += "_" + document["Act"].AsString;
                                                 //    Act = document["Act"].AsString;
                                                 //}
            }
            document["productid"] = Productid;

            Logger.LogDebug("后台收到消息持久化" + JsonConvert.SerializeObject(mqttMessage));

            switch (mqttMessage.act)
            {
                case "post_test":
                    return await SaveDeviceTest(mqttMessage.act, mqttMessage);
                //case "post_status":
                //    return await SaveDeviceRunStatus(mqttMessage.act, mqttMessage);
                case "post_setting":
                    return await SaveDeviceSetting(mqttMessage.act, mqttMessage);
                case "post_log":
                    return await SaveDeviceLog(mqttMessage.act, mqttMessage);
                case "offline":
                    //断线消息, DTU设置后,发送的主题里IMEI字符不会替换,/sys/a1JQZ8t1NmF/IMEI/thing/deviceinfo/offline 来源需要从clientid判断
                    return await SaveOffline(mqttMessage.act, mqttMessage.clientid, mqttMessage);

                default:
                    if (mqttMessage.act == "post" && !document.GetValue("imei", string.Empty).ToString().IsNullOrWhiteSpace())
                    {
                        //上线消息,DTU可以设置成,上线发布 { "ver":"1.8.11","csq":31,"imei":"866714048852098","iccid":"89860469101970562471"}
                        return await SaveOnline(mqttMessage.act, mqttMessage.clientid, mqttMessage);
                    }
                    else
                    {
                        //通用保存
                        //return SaveBson(mqttMessage.act, mqttMessage, document);
                    }
                    break;
            }
            return Ok(0);

        }

        private async Task<ActionResult> SaveDeviceTest(string act, MqttMessage mqttMessage)
        {

            var model = JsonConvert.DeserializeObject<DeviceTest>(mqttMessage.payload);
            var rpt = this.repDeviceTest;// this.ServiceProvider.GetService<DeviceTestRepository>();

            try
            {
                //============================= 发现一个神奇的事情,数据表里一条记录都没有时,插入会失败,好象和自动的创建时间有关
                //但是,有数据时就能插入成功,以后待查(也许是abp要查老的ID有关系?????)
                await rpt.InsertAsync(model/*, autoSave: true*/); 
                //实测用InsertAsync失败,用Collection.InsertOneAsync成功了,可能和参数有效性方面有关系
                                                                  //await rpt.Collection.InsertOneAsync(model);
                                                                  //更新在线时间
                await TryUpdateDeviceCustomer(false, model.SN, 0, 0, mqttMessage.timestamp, null);
            }
            catch (Exception ex)
            {
                Logger.LogDebug("数据写入出错!" + ex.ToString());
            }

            return Ok(true);
        }

        //private async Task<ActionResult> SaveDeviceRunStatus(string act, MqttMessage mqttMessage)
        //{
        //    var model = JsonConvert.DeserializeObject<DeviceRunStatus>(mqttMessage.payload);
        //    var rpt = this.ServiceProvider.GetService<DeviceRunStatusRepository>();

        //    try
        //    {
        //        await rpt.InsertAsync(model, autoSave: true); //实测用InsertAsync失败,用Collection.InsertOneAsync成功了,可能和参数有效性方面有关系
        //                                                      //await rpt.Collection.InsertOneAsync(model);

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogDebug(ex.ToString());
        //    }

        //    return Ok(true);
        //}


        // [UnitOfWork]
        private async Task<ActionResult> SaveDeviceSetting(string act, MqttMessage mqttMessage)
        {

            /***** 调试
             POST http://192.168.1.82:44359/api/SaveMqtt
             Content-Type: text/json
 {
"topic": "/sys/a1JQZ8t1NmF/866714044866415/thing/event/property/post",
"act": "post_setting",
"clientid": "mqttjs_93c84b41",
"timestamp": "1602756292839",
"username": "88888888",
"payload": "{\"act\":\"post_setting\",\"workMode\":2,\"runtime\":6888,\"sn\":\"86671404486641594499\",\"creationTime\":\"2020-10-15 17:30:10\",\"batchNumber\":\"ABC123456\",\"geoLocation_Lng\":108.54626,\"geoLocation_Lat\":33.6526,\"taskStartTime\":\"2020-10-15 16:47:37\",\"settingFlow\":62.11714966,\"sampleCount\":21,\"sampleTime\":34,\"sampleTimespan\":69,\"sampleTotalVolume\":33.54,\"testTimespan\":34,\"extraTime\":95,\"testRecordCount\":89,\"testRecordTimespan\":15,\"maxRatio\":99.26731662246371,\"minRatio\":45.54191346022,\"alphaAdd\":68.6232791,\"betaAdd\":81.0269822219,\"testAndSampling\":true,\"rSSI\":3.69,\"humidity\":38.627320724675,\"temperature\":40.33135,\"waterRequire\":56.0326255,\"windSpeed\":17.32510503784697,\"atmosphere\":26.2086816394288}"
}
              */
            var model = JsonConvert.DeserializeObject<DeviceSetting>(mqttMessage.payload);
            var rpt = this.repDeviceSetting;// this.ServiceProvider.GetService<DeviceSettingRepository>();
            if (rNum.IsMatch(model.SN))  //目前只有纯数字的才是有效的号码,来自DTU设备
            {

                try
                {

                    //model.SetId(new Guid());
                    await rpt.InsertAsync(model/*, autoSave: true*/);
                    //实测用InsertAsync失败,用Collection.InsertOneAsync成功了,可能和参数有效性方面有关系
                    //await rpt.Collection.InsertOneAsync(model);

                    await TryUpdateDeviceCustomer(true, model.SN, model.GeoLocation_Lat, model.GeoLocation_Lng, mqttMessage.timestamp,
                        edit:(setting)=>{
                            if (model.WorkStatus>=0&& model.WorkStatus<=2) {
                                setting.WorkStatus = model.WorkStatus;
                            }
                        return setting;
                    });


                }
                catch (Exception ex)
                {
                    Logger.LogDebug("数据写入出错!" + ex.ToString());
                }
            }
            return Ok(true);
        }

        /// <summary>
        /// 试着添加新设备,以SN为准
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [UnitOfWork]
        private async Task TryUpdateDeviceCustomer(bool autoAdd, string SN, decimal Lat, decimal Lng, long? updateOnline = null, long? updateOffline = null,Func<DeviceCustomer, DeviceCustomer> edit=null )
        {
            try
            {
                //检查sn,新设备要加入设备表
                var rptDeviceCustomer = this.repDeviceCustomer;// this.ServiceProvider.GetService<DeviceCustomerRepository>();
                bool isAdd = false;
                DeviceCustomer deviceCustomer = rptDeviceCustomer.AsNoTracking().Where(m => m.SN == SN).FirstOrDefault();
                if (deviceCustomer == null)
                {
                    if (!autoAdd)
                    {
                        //不自动加入所以结束
                        return;
                    }
                    isAdd = true;
                    deviceCustomer = new DeviceCustomer(/*GuidGenerator.Create()*/)
                    {
                        SN = SN,
                        Name = "新设备" + SN,
                        CustomerName = "未登记客户",
                        GeoLocation_Lat = Lat,
                        GeoLocation_Lng = Lng,
                        //CreationTime = DateTime.Now,
                        LastTimeOnline = DateTime.UtcNow.ToLocalTime(),
                        LastTimeOffline = DateTime.MinValue,
                        UserName = SN,
                        Password = "88888888",
                        //        Contact = "1",
                        //        Email = "ff@dfdf.com",
                        //        Mobile = "33",
                        //        Remark = "1",
                        WorkStatus = 1
                    };


                }

                if (updateOffline.HasValue)
                {
                    deviceCustomer.LastTimeOffline = FromUnixTime(updateOffline.ToString()).ToLocalTime();
                }
                else if (updateOnline.HasValue)
                {
                    deviceCustomer.LastTimeOnline = FromUnixTime(updateOnline.ToString()).ToLocalTime();
                    if (deviceCustomer.LastTimeOffline <  DateTime.Parse("2020-1-1")) //修复默认值
                        deviceCustomer.LastTimeOffline = deviceCustomer.LastTimeOnline;
                }
                else if (Lat > 0 && Lng > 0)
                {
                    deviceCustomer.GeoLocation_Lat = Lat;
                    deviceCustomer.GeoLocation_Lng = Lng;
                }
                if (edit != null)
                {
                    deviceCustomer= edit(deviceCustomer); //外部的修改属性
                }

                if (isAdd)
                {

                    await rptDeviceCustomer.InsertAsync(deviceCustomer, autoSave: true);
                    //await this.servDeviceCustomer.CreateAsync(deviceCustomer);

                }
                else
                {
                    await rptDeviceCustomer.UpdateAsync(deviceCustomer, autoSave: true);
                    //await this.servDeviceCustomer.UpdateAsync(deviceCustomer.Id, deviceCustomer);
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug("数据写入出错!" + ex.ToString());
            }
        }

        private async Task<ActionResult> SaveDeviceLog(string act, MqttMessage mqttMessage)
        {

            var model = JsonConvert.DeserializeObject<DeviceLog>(mqttMessage.payload);
            var rpt = this.repDeviceLog;// this.ServiceProvider.GetService<DeviceLogRepository>();

            try
            {
                await rpt.InsertAsync(model/*, autoSave: true*/); //实测用InsertAsync失败,用Collection.InsertOneAsync成功了,可能和参数有效性方面有关系
                //await rpt.Collection.InsertOneAsync(model);

            }
            catch (Exception ex)
            {
                Logger.LogDebug("数据写入出错!" + ex.ToString());
            }
            return Ok(true);
        }
        /// <summary>
        /// 设备意外掉线,由mqtt为他发布遗言信息
        /// </summary>
        /// <param name="act"></param>
        /// <param name="sn"></param>
        /// <returns></returns>

        private async Task<ActionResult> SaveOffline(string act, string sn, MqttMessage mqttMessage)
        {
            await TryUpdateDeviceCustomer(false, sn, 0, 0, null, mqttMessage.timestamp);
            if (rNum.IsMatch(sn))  //目前只有纯数字的才是有效的号码,来自DTU设备
            {

                //添加日志
                //string tex = mqttMessage.payload.ToString(); //内容才是真真的sn号
                var model = new DeviceLog()
                {
                    Act = "",
                    SN = sn,
                    Level = 3,
                    CreationTime = DateTime.UtcNow.ToLocalTime(),
                    Text = $"设备[{sn}]超时掉线"

                };
                Logger.LogInformation(model.Text);
                var rpt = this.repDeviceLog;//  this.ServiceProvider.GetService<DeviceLogRepository>();

                try
                {
                    await rpt.InsertAsync(model/*, autoSave: true*/); //实测用InsertAsync失败,用Collection.InsertOneAsync成功了,可能和参数有效性方面有关系
                    //await rpt.Collection.InsertOneAsync(model);

                }
                catch (Exception ex)
                {
                    Logger.LogDebug("数据写入出错!" + ex.ToString());
                }
            }

            return Ok(true);
        }

        /// <summary>
        /// 设备开机上线消息
        /// </summary>
        /// <param name="act"></param>
        /// <param name="sn"></param>
        /// <returns></returns>

        private async Task<ActionResult> SaveOnline(string act, string sn, MqttMessage mqttMessage)
        {

            //更新在线时间
            await TryUpdateDeviceCustomer(false, sn, 0, 0, mqttMessage.timestamp, null);

            //添加日志
            string tex = mqttMessage.payload.ToString();
            var model = new DeviceLog()
            {
                Act = "",
                SN = sn,
                Level = 3,
                CreationTime = DateTime.UtcNow.ToLocalTime(),
                Text = $"设备[{sn}]正常接上线(连通后台服务器),报告" + tex

            };
            Logger.LogInformation(model.Text);
            var rpt = this.repDeviceLog;// this.ServiceProvider.GetService<DeviceLogRepository>();

            try
            {
                await rpt.InsertAsync(model/*, autoSave: true*/); //实测用InsertAsync失败,用Collection.InsertOneAsync成功了,可能和参数有效性方面有关系
                //await rpt.Collection.InsertOneAsync(model);

            }
            catch (Exception ex)
            {
                Logger.LogDebug(ex.ToString());
            }
            return Ok(true);
        }

        /// <summary>
        /// 保存任意内容
        /// </summary>
        /// <param name="drivername"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        private ActionResult SaveBson(string act, MqttMessage mqttMessage, BsonDocument document)
        {
            //暂时不用了
            //string drivername = "mqtt_" + document["productid"] + "_" + act;
            ////BsonDocument document = new BsonDocument();
            //document["topic"] = mqttMessage.topic;
            //document["clientid"] = mqttMessage.clientid;
            //document["timestamp"] = mqttMessage.timestamp;
            //document["username"] = mqttMessage.username;

            //document["act"] = act;

            ////var db = dbContext.Database取到的都是null;
            //var client = new MongoClient("mongodb://localhost:27017");

            //var db = client.GetDatabase("Yun");

            //var collection = db.GetCollection<BsonDocument>(drivername); //插入数据的表
            //collection.InsertOne(document);

            return Ok(true);
        }

        private ActionResult CallZcqRunStatus(string act)
        {
            //TODO: 发往硬件的命令

            return Ok(true);
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime FromUnixTime(string timeStamp)
        {
            if (timeStamp.Length > 10)
            {
                timeStamp = timeStamp.Substring(0, 10);
            }
            DateTime dateTimeStart = new DateTime(1970, 1, 1);
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dateTimeStart.Add(toNow);
        }
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="nowTime">当前时间</param>
        /// <returns>时间戳(type:long)</returns>
        static long ToUnixTime(DateTime nowTime)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (long)Math.Round((nowTime - startTime).TotalMilliseconds, MidpointRounding.AwayFromZero);
        }
    }
}
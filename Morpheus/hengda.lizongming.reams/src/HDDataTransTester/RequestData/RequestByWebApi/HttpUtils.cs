using DataPlatformTrans.DataEntitys.MessageEntity;
using HDDataTransTester.RequestData.RequestDataEntitys;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HDDataTransTester.RequestData.RequestByWebApi
{
    public class HttpUitls
    {
        #region WEBAPI配置项
        /// <summary>
        /// 超时时长，单位：毫秒
        /// </summary>
        public static int TimeoutMillSecond = 10;
        public static string APIURL_Domain = "reams.hengda.show";
        public static string APIURL_ZCQREAL = "https://reams.hengda.show:44360/api/app/zcq-run-status";
        public static string APIURL_HveREAL = "https://reams.hengda.show:44360/api/app/hve-record";
        public static string APIURL_NBSREAL = "https://reams.hengda.show:44360/api/app/nbs-run-status";
        public static string APIURL_AtmosphereREAL = "https://reams.hengda.show:44360/api/app/atmosphere-record";
        public static string APIURL_ZJCREAL = "https://reams.hengda.show:44360/api/app/zjc-record";
        #endregion
        public static async Task<RtnMessage<string>> GetAsync(RequestDataFilter filter, string sUrl)
        {
            //由于我们是通过Get访问的，所以要将参数统一添加到URL上
            //string sUrlSpliter = "?";
            if(filter!=null)
            {
                sUrl += "?TimeStart" + filter.TimeStart.ToString("yyyy-MM-dd HH:mm:ss");
                sUrl += "&DeviceId=" + filter.DeviceId;
                if (!String.IsNullOrEmpty(filter.TenantId))
                {
                    sUrl +=  "&TenantId=" + filter.TenantId;
                }
                if (!String.IsNullOrEmpty(filter.Sorting))
                {
                    sUrl += "&Sorting=" + filter.Sorting;
                }
                if (filter.TimeEnd != null)
                {
                    sUrl += "&TimeEnd=" + ((DateTime)filter.TimeEnd).ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (filter.SkipCount>0)
                {
                    sUrl += "&SkipCount=" + filter.SkipCount.ToString();
                }
                if (filter.MaxResultCount > 0)
                {
                    sUrl += "&MaxResultCount=" + filter.MaxResultCount.ToString();
                }
            }
            //System.GC.Collect();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
            //request.Timeout = TimeoutMillSecond;
            request.Proxy = null;
            request.KeepAlive = false;
            request.Method = "GET";
            request.ContentType = "application/json; charset=UTF-8";
            request.AutomaticDecompression = DecompressionMethods.GZip;
            HttpWebResponse response = null;
            Stream myResponseStream = null;
            StreamReader myStreamReader = null;
            try
            {
                #region 添加cookie
                request.CookieContainer = new CookieContainer();
                Cookie cookie = new Cookie();
                string strPath = "/";
                string strDomain = APIURL_Domain;
                // Cookie myCookie = new Cookie("mockFlagForTesting", "true", "/", "safeqa.thomson.com");
                request.CookieContainer.Add(new Cookie(".AspNetCore.Antiforgery.SDHCQWFtr34", "CfDJ8HpoCjSM-rxDhPS7oQMJVaHODOXM5MA9UgS5OMZZE_iKmG6t-cG4my4F4rLxTwwZmFQJNRS0RT-s-V7VF-_oUNpfEfMtIHP6LnD8048VzPQUxPe-erNV_CtEXxoW24sBClPRheX0biczD_Iji9JRLX0", strPath, strDomain));
                request.CookieContainer.Add(new Cookie(".AspNetCore.Culture", "c%3Den%7Cuic%3Den", strPath, strDomain));
                request.CookieContainer.Add(new Cookie(".AspNetCore.Identity.Application", "CfDJ8HpoCjSM-rxDhPS7oQMJVaEiozrarTIXhIv69bJe4h2zSaqcxYdN9FGJBdmT-LiMwulo1s7tWPTvgmMvjW_Ul-6puzumaFH-17-TzdInDcakPtM8oteepf8rxfY7G-OTHVfk2IYN7s1_5B1CLXWeoKmVB_fkcLb2E_GwiBm_jd5AaccTlHc8-k18F6AXXE7DjBeKUkF2lBLK5KZNReFXr1HJBYJINfowBqzzHDdzBNpTvrBHIdsx4N4NpOPakG4pUSDh32TVxeitNns3fQWehf9rodZ-s-8m4priijDJc4uLCP533jxd_YWLWkdJSkVSVe9iJDOxEvbimTR6lvGqjH6nOEwPCkrs7O159-QXV5n94TdXIsQd9_D0frs8Gs7jdpM2dYmJ9TsCKM5BTFpZ7ZfMFzMNkkp-jCL68otWe5G_Z-pddmXc5bzlBhIhiinVRBoxyfad1kxQEikqGFk1pu_A6il-gPKSzfpWrqX14wY3-dkVFoiDG0cU_OkWwJjIlStl1d-8fOsLPHtALtXrw5uaip0bTqMKnLHmBJtvvEaVt8h0B2LooU6gmdDpmZqbXoL8M9zS3pnyIlBYxAsZlWHkG-WT1Nx0qOVOxjGOF7ElYc60q5YbW7jpZFB0u3Tgp2JYhT4Z8Enyuu8LoBC8_deBiR9fDbcE-r30myzf54Aa6r3P-XCOD3_0N2G3RH45MAK3qLuXQAsDyDpWXUtwjeOroU9wtz8nlWQWASdUok-nTpmo2H_L_5zlcl_LTZw08jMoaWjgSgcwECM8RtUCpnHMiGwWkiMdZ9hZ3RqIPzF10UDHSBbpOrF2j3JtNCLAWcTB44A2BMuZrrMqNcsWjmUZAy2u2bl6L19cgAx1c5NMax_wUg9d1jbPAK3WV5SkFcyijRT9LWX3CoRbD1tvowzFEnF3yGUcNmW8zaE6zRb9xIT3_u2zu80rLQGIyNAPP64-f7YitPaLjfsvJH1WtovOh6CkU_sHJDKNZbrOEUP0bvk-pWbYoP9LG9IZicYoPpQnzgBIkNf1OpJabCaQ-Wggpf_5UD-ImhkWFDF-2G4OzvKANa2Urqmd5l7KBNqlN44VOPfXAVnJRHbpyU4mxmEIUGQq3qTTWoiFSGZF7wjO4UPDEdtRQzDlSKnWuNHILSnvaYmz3qQ4bvyRKqLsEgYPvYRpNpaBNG3w9nfSsWG6iXcmt1f9fnDne2AnykNNHroU0cAKITrwlc6X6V25CSAW_gn0TpxGUuWrdEv59lqX", strPath, strDomain));
                request.CookieContainer.Add(new Cookie("XSRF-TOKEN", "CfDJ8HpoCjSM-rxDhPS7oQMJVaHX6uI5Hp--elNvvL592fk3R_sY8HHGZOGTZO5ppTjMhpVuXuPK7d5csVm0u99mq2H4i5n_e0khmyiGBNnMjrLIq3iKYAji1LADF5XYSv60n30t0eeKKgGelXMsRF9utC6eDkBiMOQkKnNGNBqeLsBTRikei7PfRTUMYDGyM29ZKw", strPath, strDomain));
                request.CookieContainer.Add(new Cookie("__tenant", "39f99bf5-c4eb-33d2-a623-591eec290001", strPath, strDomain));
                request.CookieContainer.Add(new Cookie("idsrv.session", "0980CB2128124905F0FDC07A974F3994", strPath, strDomain));
                #endregion
                response = (HttpWebResponse)(await request.GetResponseAsync());
                myResponseStream = response.GetResponseStream();
                myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                RtnMessage<string> rtnMessage = new RtnMessage<string>();
                rtnMessage.Result = myStreamReader.ReadToEnd();
                return rtnMessage;
            }
            catch (Exception ex)
            {
                return new RtnMessage<string>(string.Format("{0}({1})", ex.Message, ex.Source));
            }
            finally {
                if (request != null)
                    request.Abort();
                if (response != null)
                    response.Close();
                if (myStreamReader != null)
                    myStreamReader.Close();
                if (myResponseStream != null)
                    myResponseStream.Close();
            }
        }

        public static string Post(string Url, string sData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            //request.Referer = Referer;
            byte[] bytes = Encoding.UTF8.GetBytes(sData);
            request.ContentType = "application/json";
            // request.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            //request.TransferEncoding = "application/json";
            request.ContentLength = bytes.Length;
            Stream myResponseStream = request.GetRequestStream();
            myResponseStream.Write(bytes, 0, bytes.Length);
            myResponseStream.Flush();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            myResponseStream.Close();

            if (response != null)
            {
                response.Close();
            }
            if (request != null)
            {
                request.Abort();
            }
            return retString;
        }
        public static bool PostTry(string sUrl, string sData, out string sResult, out string sErr)
        {
            try
            {
                sResult = Post(sUrl, sData);
            }
            catch (Exception ex)
            {
                sErr = $"调用接口[{sUrl}]出错:{ex.Message}({ex.Source})";
                sResult = string.Empty;
                return false;
            }
            sErr = string.Empty;
            return true;
        }
    }
}

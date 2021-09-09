using Microsoft.AspNetCore.Mvc;
using System;

namespace HengDa.LiZongMing.REAMS.Web.Pages
{
    public class IndexModel : REAMSPageModel
    {
        public IActionResult OnGet()
        {
            //有静态目录时跳转
            //var d = new System.IO.DirectoryInfo("wwwroot/html");
            //Console.WriteLine( "检查"+(d.Exists? "有":"没有") +"目录:"+ d.FullName);
            //if (d.Exists)
            //{
            //    return new RedirectResult("/html");
            //}

            //有静态文件时直接显示
            var d = new System.IO.FileInfo("wwwroot/index.html");
            Console.WriteLine("检查" + (d.Exists ? "有" : "没有") + "目录:" + d.FullName);
            if (d.Exists)
            {
                return new FileStreamResult(d.OpenRead(), "text/html");
            }
            return Page();
        }
    }
}
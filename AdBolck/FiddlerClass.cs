using System;
using System.Collections.Generic;
using System.Threading;
using Fiddler;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;
namespace AdBolck
{
    class FiddlerClass
    {
        private static bool Flag = true;
        private static APPConfigurtiong AppConfing = new APPConfigurtiong();
        public void FiddlerProxy() {
            //初始化配置信息
            var isTrue = AppConfing.InitializeComponent();
            if (!isTrue)
            {
                InstallCertificate();
            }
            else {
                FiddlerApplication.Prefs.SetStringPref("fiddler.certmaker.bc.cert", AppConfing.GetCert());
                FiddlerApplication.Prefs.SetStringPref("fiddler.certmaker.bc.key", AppConfing.GetKey());
            }
            Dictionary<string, string> AdBlockConfig = new Dictionary<string, string>();
            AdBlockConfig.Add("qiyi", "t7z.cupid.iqiyi.com");
            AdBlockConfig.Add("youku", "valf.atm.youku.com/vf");
            AdBlockConfig.Add("qq", "vlive.qqvideo.tc.qq.com");
            AdBlockConfig.Add("qqlive", "variety.tc.qq.com");
            AdBlockConfig.Add("youtube", "googleads.g.doubleclick.net");
            AdBlockConfig.Add("letv", "ark.letv.com");
            AdBlockConfig.Add("letvLive", "fz.letv.com");
            AdBlockConfig.Add("mgtv", "da.mgtv.com/pc");
            FiddlerApplication.BeforeRequest += delegate(Fiddler.Session oSession) {
                
                oSession.bBufferResponse = true;
                foreach (var obj in AdBlockConfig) {
                    if (oSession.uriContains(obj.Value)) {
                        oSession.oRequest.FailSession(404, "Blocked", "Fiddler blocked request");
                        var NameLincal = this.ResedStr(obj.Key);
                        Console.WriteLine($"检测到{NameLincal}的广告，程序自动过滤中，您无需作任何操作！");
                        
                    }
                }
            };
            
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
            Console.WriteLine("过滤器启动中......");
            Fiddler.FiddlerApplication.Startup(8887,FiddlerCoreStartupFlags.DecryptSSL);
            Console.WriteLine("按Ctrl+C退出程序.");
            while (Flag) {

                Thread.Sleep(1000);
            }

        }
        
        public string ResedStr(string str) {

            switch (str) {

                case "youku":
                    str = "优酷";
                    break;
                case "qiyi":
                    str = "爱奇艺";
                    break;
                case "qq":
                    str = "腾讯视频";
                    break;
                case "qqlive":
                    str = "腾讯视频";
                    break;
                case "youtube":
                    str = "Youtube";
                    break;
                case "letv":
                    str = "乐视TV";
                    break;
                case "letvLive":
                    str = "乐视TV";
                    break;
                case "mgtv":
                    str = "芒果TV";
                    break;
                default:
                    str = "未知站点";
                    break;
            }
            return str;
        }

        public static bool InstallCertificate()
        {
            if (!CertMaker.rootCertExists())
            {
                if (!CertMaker.createRootCert())
                    return false;

                if (!CertMaker.trustRootCert())
                    return false;
                AppConfing.SetCert(FiddlerApplication.Prefs.GetStringPref("fiddler.certmaker.bc.cert", null));
                AppConfing.SetKey(FiddlerApplication.Prefs.GetStringPref("fiddler.certmaker.bc.key", null));
                AppConfing.UpdateFile();
            }

            return true;
        }

        public static bool UninstallCertificate()
        {
            if (CertMaker.rootCertExists())
            {
                if (!CertMaker.removeFiddlerGeneratedCerts(true))
                    return false;
            }
            return true;
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("程序即将退出...");
            e.Cancel = true;
            FiddlerApplication.Shutdown();
            //UninstallCertificate();
            Thread.Sleep(750);
            Flag = false;
        }











    }
}

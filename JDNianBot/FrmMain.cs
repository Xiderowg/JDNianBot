using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace JDNianBot
{
    public partial class FrmMain : Form
    {
        /// <summary>
        /// Cancellation Token
        /// </summary>
        private CancellationTokenSource CTS { get; set; }

        /// <summary>
        /// 用户Cookie容器
        /// </summary>
        private CookieContainer CookieContainer { get; set; } = null;


        /// <summary>
        /// 
        /// </summary>
        private HttpClient Session { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        private HttpClientHandler Handler { get; set; } = null;

        /// <summary>
        /// 用户名
        /// </summary>
        private string UserName { get; set; } = "正在加载中...";

        /// <summary>
        /// 神仙书院题库
        /// </summary>
        private JArray SXSYTK { get; set; }

        /// <summary>
        /// 用户隐秘用户名
        /// </summary>
        private string UserSecretName { get => new string(new char[] { UserName[0], '*', UserName[UserName.Length - 1] }); }

        /// <summary>
        /// 用户是否绑定手机号
        /// </summary>
        private bool UserHasMobile { get; set; } = false;

        /// <summary>
        /// 当前等级
        /// </summary>
        private int CurrentLevel { get; set; } = 0;

        /// <summary>
        /// 最大等级
        /// </summary>
        private int MaxLevel { get; set; } = 0;

        /// <summary>
        /// 当前爆竹数
        /// </summary>
        private int CurrentScore { get; set; } = 0;

        /// <summary>
        /// 下一等级需要的爆竹数
        /// </summary>
        private int NextScore { get; set; } = 0;

        /// <summary>
        /// 当前红包数目
        /// </summary>
        private int CurrentRedPack { get; set; } = 0;

        /// <summary>
        /// 当前金币个数
        /// </summary>
        private int CurrentCoin { get; set; } = 200;

        /// <summary>
        /// 任务凭证
        /// </summary>
        private string SecretP { get; set; } = "";

        /// <summary>
        /// 邀请ID
        /// </summary>
        private string InviteID { get; set; } = "";

        /// <summary>
        /// 神仙书院邀请ID
        /// </summary>
        private string SXSYInviteID { get; set; } = "";

        /// <summary>
        /// PK邀请ID
        /// </summary>
        private string PKInviteID { get; set; } = "";

        /// <summary>
        /// 特殊活动邀请ID
        /// </summary>
        private string SpecialInviteID { get; set; } = "";

        /// <summary>
        /// 随机Token
        /// </summary>
        private string RandomToken { get; set; } = Utils.GetRandomString(32, true, true, false, false, "");

        /// <summary>
        /// 随机Callstack
        /// </summary>
        private string RandomCallStack { get; set; } = Utils.GetRandomString(32, true, true, false, false, "");

        /// <summary>
        /// 神仙书院Token
        /// </summary>
        private string SXSYToken { get; set; } = "jd17919499fb7031e5";

        /// <summary>
        /// 自动收取爆竹的剩余时间
        /// </summary>
        private int AutoCollectCountDown { get; set; }

        /// <summary>
        /// 自动做任务的剩余时间
        /// </summary>
        private int AutoDoTaskCountDown { get; set; }

        /// <summary>
        /// 自动夺红包倒计时
        /// </summary>
        private int AutoStealRedPackCountDown { get; set; }


        public FrmMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_Load(object sender, EventArgs e)
        {
            // 读取保存在本地的Cookie
            InitCookieContainer();
            // 更新用户信息
            UpdateInfo();
            // 更新控件
            Cbo_HelpType.SelectedIndex = 0;
            notifyIcon1.Text = "JDNianBot [" + UserName + "]";
            // 初始化神仙书院题库
            Task.Run(() => InitSXSYTK());
        }

        private void InitSXSYTK()
        {
            if (SXSYTK != null) return;
            SXSYTK = JsonConvert.DeserializeObject(Properties.Resources.SXSYTK) as JArray;
        }

        private void InitSession()
        {
            if (Session != null) return;
            Handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = CookieContainer
            };
            Session = new HttpClient(Handler);
            Session.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "jdapp;iPhone;9.3.8;13.6;ce0ffafdce8af84f700b27d236aafc9c980bcbdc;network/4g;ADID/AF8F4ECA-6259-4F47-8054-B8A529B143AE;supportApplePay/0;hasUPPay/0;hasOCPay/0;model/iPhone10,5;addressid/138295868;supportBestPay/0;appBuild/167538;jdSupportDarkMode/0;pv/1058.16;apprpd/DongdongChat_Main;ref/SCSChatViewController;psq/1;ads/;psn/ce0ffafdce8af84f700b27d236aafc9c980bcbdc|4415;jdv/0|kong|t_1000089893_157_0_184__7136ea10b0b4ee7f|tuiguang|0b0e056a469346e4908ef3886445191f|1612256246;adk/;app_device/IOS;pap/JA2015_311210|9.3.8|IOS 13.6;Mozilla/5.0 (iPhone; CPU iPhone OS 13_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148;supportJDSHWK/1");
        }

        /// <summary>
        /// 初始化CookieContainer
        /// </summary>
        private void InitCookieContainer()
        {
            string filePath = System.IO.Path.Combine(Application.StartupPath, "user.dat");
            if (System.IO.File.Exists(filePath))
            {
                CookieContainer = Utils.ReadCookiesFromDisk(filePath);
                InitSession();
                // 检查登录状态
                if (!CheckLoginStatus())
                {
                    MessageBox.Show("本地保存的CK已经过期，请您重新登录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Btn_Login_Click(null, null);
                }
                else
                {
                    Btn_Login.Text = "重新登录";
                }
            }
        }

        /// <summary>
        /// 检查登录状态
        /// </summary>
        /// <returns></returns>
        private bool CheckLoginStatus()
        {
            if (CookieContainer == null) return false;
            if (Session == null) return false;
            string queryAPI = "https://wq.jd.com/user/info/QueryJDUserInfo?sceneval=2&g_login_type=1&callback=";
            Session.DefaultRequestHeaders.Referrer = new Uri("https://wq.jd.com/user/info/QueryJDUserInfo?sceneval=2&g_login_type=1&callback=");
            Session.DefaultRequestHeaders.Add("Accept", "*/*");
            Session.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            var response = Session.GetAsync(queryAPI).Result;
            var responseByte = response.Content.ReadAsByteArrayAsync().Result;
            var responseStr = Encoding.UTF8.GetString(responseByte);
            JObject responseJo;
            try
            {
                responseJo = (JObject)JsonConvert.DeserializeObject(responseStr);
            }
            catch
            {
                return false;
            }
            if (responseJo["retcode"].ToString() == "0")
            {
                // 获取用户名
                UserName = responseJo["base"]["nickname"].ToString();
                string outputText = string.Format("{0} [登录账号]登录成功，用户名为：{1}", DateTime.Now.ToString("HH:mm:ss"), UserName);
                UpdateTextToForm(Environment.NewLine + outputText);
                // 获取手机号
                if (string.IsNullOrWhiteSpace(responseJo["base"]["mobile"].ToString()))
                {
                    outputText = string.Format("{0} [账号检查]检查到用户未绑定手机号，将不进行神仙书院任务", DateTime.Now.ToString("HH:mm:ss"));
                    UserHasMobile = false;
                    UpdateTextToForm(Environment.NewLine + outputText);
                }
                else
                {
                    UserHasMobile = true;
                }

                return true;
            }
            else
            {
                return false;
            }

        }

        private void TmpFunction()
        {
            if (CookieContainer == null) return;
            if (Session == null) InitSession();
            string postAPI = "https://api.m.jd.com/client.action?functionId=nian_getHomeData";
            string outputText;
            HttpClient client = Session;
            // 读取神仙书院信息
            string format = "https://api.m.jd.com/client.action?functionId=mcxhd_brandcity_homePage&appid=publicUseApi&body=%7B%22lat%22:%22%22,%22lng%22:%22%22,%22token%22:%22{0}%22%7D&t=1611239407910&client=wh5&clientVersion=1.0.0";
            string sxsyAPI = string.Format(format, SXSYToken);
            client.DefaultRequestHeaders.Referrer = new Uri("https://h5.m.jd.com/babelDiy/Zeus/4XjemYYyPScjmGyjej78M6nsjZvj/index.html");
            var result =  client.GetAsync(sxsyAPI).Result;
            var resultJson =  result.Content.ReadAsStringAsync().Result;
            var jo = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (result.StatusCode == HttpStatusCode.OK && jo["retCode"].ToString() == "200")
            {
                CurrentCoin = Convert.ToInt32(jo["result"]["userCoinNum"].ToString());
            }
            // 读取炸年兽信息
            client.DefaultRequestHeaders.Referrer = new Uri("https://wbbny.m.jd.com/babelDiy/Zeus/2cKMj86srRdhgWcKonfExzK4ZMBy/index.html");
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("functionId", "nian_getHomeData"));
            nvc.Add(new KeyValuePair<string, string>("body", "{}"));
            nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
            nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
            nvc.Add(new KeyValuePair<string, string>("uuid", "865166028601832-00811e4d5577"));
            var message = new HttpRequestMessage(HttpMethod.Post, postAPI) { Content = new FormUrlEncodedContent(nvc) };
            result =  client.SendAsync(message).Result;
            resultJson =  result.Content.ReadAsStringAsync().Result;
            jo = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (jo["data"]["success"].ToString().ToLower().Trim() == "true")
            {
                // 读取secretp以及登记信息
                var homeInfo = jo["data"]["result"]["homeMainInfo"];
                SecretP = homeInfo["secretp"].ToString();
                var raiseInfo = homeInfo["raiseInfo"];
                CurrentLevel = Convert.ToInt32(raiseInfo["scoreLevel"].ToString());
                MaxLevel = 30;
                CurrentRedPack = Convert.ToInt32(raiseInfo["redNum"].ToString());
                CurrentScore = Convert.ToInt32(raiseInfo["remainScore"].ToString());
                NextScore = Convert.ToInt32(raiseInfo["nextLevelScore"].ToString()) - Convert.ToInt32(raiseInfo["usedScore"].ToString());
            }
            else
            {
                outputText = string.Format("{0} [登录账号]登录失败，请重新登录！", DateTime.Now.ToString("HH:mm:ss"));
                UpdateTextToForm(Environment.NewLine + outputText);
                return;
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        private async void UpdateInfo(bool doUpgrade = false)
        {
            if (CookieContainer == null) return;
            if (Session == null) InitSession();
            string postAPI = "https://api.m.jd.com/client.action?functionId=nian_getHomeData";
            string outputText;
            HttpClient client = Session;
            // 读取神仙书院信息
            string format = "https://api.m.jd.com/client.action?functionId=mcxhd_brandcity_homePage&appid=publicUseApi&body=%7B%22lat%22:%22%22,%22lng%22:%22%22,%22token%22:%22{0}%22%7D&t=1611239407910&client=wh5&clientVersion=1.0.0";
            string sxsyAPI = string.Format(format, SXSYToken);
            client.DefaultRequestHeaders.Referrer = new Uri("https://h5.m.jd.com/babelDiy/Zeus/4XjemYYyPScjmGyjej78M6nsjZvj/index.html");
            var result = await client.GetAsync(sxsyAPI);
            var resultJson = await result.Content.ReadAsStringAsync();
            var jo = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (result.StatusCode == HttpStatusCode.OK && jo["retCode"].ToString() == "200")
            {
                CurrentCoin = Convert.ToInt32(jo["result"]["userCoinNum"].ToString());
            }
            // 读取炸年兽信息
            client.DefaultRequestHeaders.Add("Origin", "https://wbbny.m.jd.com");
            client.DefaultRequestHeaders.Referrer = new Uri("https://wbbny.m.jd.com/babelDiy/Zeus/2cKMj86srRdhgWcKonfExzK4ZMBy/index.html");
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("functionId", "nian_getHomeData"));
            nvc.Add(new KeyValuePair<string, string>("body", "{}"));
            nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
            nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
            nvc.Add(new KeyValuePair<string, string>("uuid", "865166028601832-00811e4d5577"));
            var message = new HttpRequestMessage(HttpMethod.Post, postAPI) { Content = new FormUrlEncodedContent(nvc) };
            result = await client.SendAsync(message);
            resultJson = await result.Content.ReadAsStringAsync();
            jo = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (jo["data"]["success"].ToString().ToLower().Trim() == "true")
            {
                // 读取secretp以及登记信息
                var homeInfo = jo["data"]["result"]["homeMainInfo"];
                SecretP = homeInfo["secretp"].ToString();
                var raiseInfo = homeInfo["raiseInfo"];
                CurrentLevel = Convert.ToInt32(raiseInfo["scoreLevel"].ToString());
                MaxLevel = 30;
                CurrentRedPack = Convert.ToInt32(raiseInfo["redNum"].ToString());
                CurrentScore = Convert.ToInt32(raiseInfo["remainScore"].ToString());
                NextScore = Convert.ToInt32(raiseInfo["nextLevelScore"].ToString()) - Convert.ToInt32(raiseInfo["usedScore"].ToString());
            }
            else
            {
                outputText = string.Format("{0} [登录账号]登录失败，请重新登录！", DateTime.Now.ToString("HH:mm:ss"));
                UpdateTextToForm(Environment.NewLine + outputText);
                return;
            }
            // TODO: 读取PK信息
            // 更新信息到控件上
            UpdateInfoToForm();
            notifyIcon1.Text = "JDNianBot [" + UserName + "]";
            // pk登录
            PKSignin();
            // 升级
            if (doUpgrade)
            {
                while (CurrentScore >= NextScore)
                {
                    DoUpdate();
                    await Task.Delay(1000);
                }
            }
        }

        private delegate void delegateUpdateInfoToForm();
        private void UpdateInfoToForm()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new delegateUpdateInfoToForm(UpdateInfoToForm));
            }
            else
            {
                Lbl_CurrentAccount.Text = UserName;
                Lbl_CurrentRedPack.Text = CurrentRedPack.ToString();
                Lbl_Level.Text = string.Format("{0}（{1}%）", CurrentLevel, ((double)CurrentScore / NextScore * 100).ToString("F2"));
                Lbl_Score.Text = CurrentScore.ToString();
                Lbl_NextScore.Text = NextScore.ToString();
                Lbl_Coin.Text = CurrentCoin.ToString();
            }
        }

        /// <summary>
        /// 登陆按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Login_Click(object sender, EventArgs e)
        {
            FrmLogin frmLogin = new FrmLogin();
            if (frmLogin.ShowDialog() == DialogResult.OK)
            {
                this.CookieContainer = frmLogin.CookieContainer;
                if (Session != null)
                {
                    Session.Dispose();
                    Session = null;
                }
                InitSession();
                if (CheckLoginStatus())
                {
                    UpdateInfo();
                }
                else
                {
                    MessageBox.Show("扫码登录失败！请尝试使用Cookie登录！", "信息");
                    this.CookieContainer = null;
                }
            }
        }

        /// <summary>
        /// 刷新按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Refresh_Click(object sender, EventArgs e)
        {
            Lbl_CurrentAccount.Text = "...";
            Lbl_CurrentRedPack.Text = "...";
            Lbl_Level.Text = "...";
            Lbl_Score.Text = "...";
            Lbl_NextScore.Text = "...";
            Lbl_Coin.Text = "...";
            UpdateInfo();
        }

        /// <summary>
        /// 开始任务按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_StartTask_Click(object sender, EventArgs e)
        {
            Btn_StartTask.Enabled = false;
            // 执行基础任务
            CTS = new CancellationTokenSource();
            Task main = new Task(() => GetAndDoTasks(CTS.Token), CTS.Token);
            main.Start();
        }

        private delegate void delegateUpdateTextToForm(string text);
        private void UpdateTextToForm(string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new delegateUpdateTextToForm(UpdateTextToForm), text);
            }
            else
            {
                Txt_Output.AppendText(Environment.NewLine + text);
            }
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        private void GetAndDoTasks(CancellationToken ct)
        {
            if (Session == null) InitSession();
            int MAX_COUNT = 3;
            for (int i = 0; i < MAX_COUNT; i++)
            {
                // 获取所有任务
                var tasks = GetTasks();
                // 执行所有任务
                foreach (var task in tasks)
                {
                    DoTask(task);
                    Thread.Sleep(1000);
                    if (ct.IsCancellationRequested)
                    {
                        UpdateTextToForm(Environment.NewLine + string.Format("{0} [结果输出] 任务执行已被用户终止", DateTime.Now.ToString("HH:mm:ss")));
                        SetButtonEnable(Btn_StartTask, true);
                        return;
                    }
                }
                // 获取所有神仙书院的任务
                if (UserHasMobile)
                {
                    var sxsyTasks = GetSXSYTasks();
                    foreach (var task in sxsyTasks)
                    {
                        DoSXXYTask(task);
                        Thread.Sleep(2000);
                        if (ct.IsCancellationRequested)
                        {
                            UpdateTextToForm(Environment.NewLine + string.Format("{0} [结果输出] 任务执行已被用户终止", DateTime.Now.ToString("HH:mm:ss")));
                            SetButtonEnable(Btn_StartTask, true);
                            return;
                        }
                    }
                }
            }
            // 收爆竹
            DoCollect();
            // 执行AR任务
            DoArGame();
            // 领取优惠券
            DoKillCoupon();
            // 升级
            UpdateInfo(doUpgrade: true);
            // 神仙书院答题
            if(Chk_AutoAnswer.Checked)
                DoSXSYAnswerQuestion();
            string outputText;
            outputText = string.Format("{0} [结果输出] 所有任务执行完毕", DateTime.Now.ToString("HH:mm:ss"));
            UpdateTextToForm(Environment.NewLine + outputText);
            SetButtonEnable(Btn_StartTask, true);
        }

        private delegate void delegateSetButtonEnable(Button btn, bool enabled);
        private void SetButtonEnable(Button btn, bool enabled)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new delegateSetButtonEnable(SetButtonEnable), btn, enabled);
            }
            else
            {
                btn.Enabled = enabled;
            }
        }

        /// <summary>
        /// 进行升级操作
        /// </summary>
        private void DoUpdate()
        {
            if (CookieContainer == null) return;
            const string raiseAPI = "https://api.m.jd.com/client.action?functionId=nian_raise";
            HttpClient client = Session;
            string outputText;
            client.DefaultRequestHeaders.Referrer = new Uri("https://wbbny.m.jd.com/babelDiy/Zeus/2cKMj86srRdhgWcKonfExzK4ZMBy/index.html");
            string timeStamp = Utils.GetTimeStampLong();
            string sessionC = Utils.GetTimeStampSuperLong();
            string randSign = Utils.GetRandomString(40, false, false, true, false, "");
            string randStr = Utils.GetRandomString(10, true, true, true, false, "");
            string randIntStr = Utils.GetRandomString(6, true, false, false, false, "");
            string format = "{{\"ss\":\"{{\\\"extraData\\\":{{\\\"is_trust\\\":true,\\\"sign\\\":\\\"{0}\\\",\\\"fpb\\\":\\\"\\\",\\\"time\\\":{1},\\\"encrypt\\\":\\\"3\\\",\\\"nonstr\\\":\\\"{2}\\\",\\\"jj\\\":\\\"\\\",\\\"token\\\":\\\"{3}\\\",\\\"cf_v\\\":\\\"1.0.1\\\",\\\"client_version\\\":\\\"2.2.1\\\",\\\"call_stack\\\":\\\"{4}\\\",\\\"session_c\\\":\\\"{5}\\\",\\\"buttonid\\\":\\\"\\\",\\\"sceneid\\\":\\\"\\\"}},\\\"secretp\\\":\\\"{6}\\\",\\\"random\\\":\\\"{7}\\\"}}\"}}";
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("functionId", "nian_raise"));
            nvc.Add(new KeyValuePair<string, string>("body", string.Format(format, randSign, timeStamp, randStr, RandomToken, RandomCallStack, sessionC, SecretP, randIntStr)));
            nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
            nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
            var message = new HttpRequestMessage(HttpMethod.Post, raiseAPI) { Content = new FormUrlEncodedContent(nvc) };
            var result = client.SendAsync(message).Result;
            var resultJson = result.Content.ReadAsStringAsync().Result;
            var resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resultJo["data"]["success"].ToString().ToLower().Trim() == "true")
            {
                string currentLevel = resultJo["data"]["result"]["raiseInfo"]["scoreLevel"].ToString();
                outputText = string.Format("{0} [炸年兽] 升级成功，当前等级{1}级", DateTime.Now.ToString("HH:mm:ss"), currentLevel);
            }
            else
            {
                outputText = string.Format("{0} [炸年兽] 升级失败，{1}", DateTime.Now.ToString("HH:mm:ss"), resultJo["data"]["bizMsg"].ToString());
            }
            UpdateTextToForm(Environment.NewLine + outputText);
            UpdateInfo();
        }

        /// <summary>
        /// 收爆竹
        /// </summary>
        private void DoCollect()
        {
            if (CookieContainer == null) return;
            const string collectAPI = "https://api.m.jd.com/client.action?functionId=nian_collectProduceScore";
            HttpClient client = Session;
            string outputText;
            client.DefaultRequestHeaders.Referrer = new Uri("https://wbbny.m.jd.com/babelDiy/Zeus/2cKMj86srRdhgWcKonfExzK4ZMBy/index.html");
            string timeStamp = Utils.GetTimeStampLong();
            string sessionC = Utils.GetTimeStampSuperLong();
            string randSign = Utils.GetRandomString(40, true, false, true, false, "");
            string randStr = Utils.GetRandomString(10, true, true, true, false, "");
            string randIntStr = Utils.GetRandomString(6, true, false, false, false, "");
            string format = "{{\"ss\":\"{{\\\"extraData\\\":{{\\\"is_trust\\\":true,\\\"sign\\\":\\\"{0}\\\",\\\"fpb\\\":\\\"\\\",\\\"time\\\":{1},\\\"encrypt\\\":\\\"3\\\",\\\"nonstr\\\":\\\"{2}\\\",\\\"jj\\\":\\\"\\\",\\\"token\\\":\\\"{3}\\\",\\\"cf_v\\\":\\\"1.0.1\\\",\\\"client_version\\\":\\\"2.2.1\\\",\\\"call_stack\\\":\\\"{4}\\\",\\\"session_c\\\":\\\"{5}\\\",\\\"buttonid\\\":\\\"\\\",\\\"sceneid\\\":\\\"\\\"}},\\\"secretp\\\":\\\"{6}\\\",\\\"random\\\":\\\"{7}\\\"}}\"}}";
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("functionId", "nian_collectProduceScore"));
            nvc.Add(new KeyValuePair<string, string>("body", string.Format(format, randSign, timeStamp, randStr, RandomToken, RandomCallStack, sessionC, SecretP, randIntStr)));
            nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
            nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));

            StringBuilder test = new StringBuilder();
            test.Append("functionId=nian_collectProduceScore&body=").Append(string.Format(format, randSign, timeStamp, randStr, RandomToken, RandomCallStack, sessionC, SecretP, randIntStr))
                .Append("&client=wh5&").Append("clientVersion=1.0.0");


            //var message = new HttpRequestMessage(HttpMethod.Post, collectAPI) { Content = new FormUrlEncodedContent(nvc) };
            var message = new HttpRequestMessage(HttpMethod.Post, collectAPI) { Content = new StringContent(test.ToString(),Encoding.UTF8, "application/x-www-form-urlencoded") };
            var result = client.SendAsync(message).Result;
            var resultJson = result.Content.ReadAsStringAsync().Result;
            var resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resultJo["data"]["success"].ToString().ToLower().Trim() == "true")
            {
                string collectCount = resultJo["data"]["result"]["produceScore"].ToString();
                outputText = string.Format("{0} [收爆竹] 成功收取到{1}个爆竹", DateTime.Now.ToString("HH:mm:ss"), collectCount);
            }
            else
            {
                outputText = string.Format("{0} [收爆竹] 收取失败，{1}", DateTime.Now.ToString("HH:mm:ss"), resultJo["data"]["bizMsg"].ToString());
            }
            UpdateTextToForm(Environment.NewLine + outputText);
        }


        /// <summary>
        /// 获取所有任务
        /// </summary>
        /// <returns></returns>
        private List<JDTask> GetTasks()
        {
            List<JDTask> tasks = new List<JDTask>();
            const string taskUrl = "https://api.m.jd.com/client.action?functionId=nian_getTaskDetail";
            const string feedUrl = "https://api.m.jd.com/client.action?functionId=nian_getFeedDetail";
            const string shopUrl = "https://api.m.jd.com/client.action?functionId=qryCompositeMaterials";
            const string shopLotteryUrl = "https://api.m.jd.com/client.action?functionId=nian_shopLotteryInfo";
            const string specialGiftUrl = "https://api.m.jd.com/client.action?functionId=nian_getSpecialGiftDetail";
            HttpClient client = Session;
            client.DefaultRequestHeaders.Remove("Origin");
            client.DefaultRequestHeaders.Referrer = new Uri("https://wbbny.m.jd.com/babelDiy/Zeus/2cKMj86srRdhgWcKonfExzK4ZMBy/index.html");
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("functionId", "nian_getTaskDetail"));
            nvc.Add(new KeyValuePair<string, string>("body", "{}"));
            nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
            nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
            var message = new HttpRequestMessage(HttpMethod.Post, taskUrl) { Content = new FormUrlEncodedContent(nvc) };
            var result = client.SendAsync(message).Result;
            var resultJson = result.Content.ReadAsStringAsync().Result;
            int feedId = -1;
            JObject jo = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (jo["data"]["success"].ToString().ToLower().Trim() == "true")
            {
                // 读取邀请ID以及任务信息
                var resultJO = jo["data"]["result"];
                InviteID = resultJO["inviteId"].ToString();
                var taskVos = resultJO["taskVos"] as JArray;
                foreach (JObject info in taskVos)
                {
                    // 如果任务已经完成了，那就直接跳过
                    int maxTimes = Convert.ToInt32(info["maxTimes"].ToString());
                    int currentTimes = Convert.ToInt32(info["times"].ToString());
                    if (currentTimes >= maxTimes) continue;
                    // 取得任务信息
                    int taskID = Convert.ToInt32(info["taskId"].ToString());
                    string taskName = info["taskName"].ToString();
                    string[] itemID = null;
                    int waitDuration = Convert.ToInt32(info["waitDuration"].ToString());
                    JDTaskType taskType = JDTaskType.Unknown;
                    // 对不同类型的任务进行区分处理
                    IList<string> keys = info.Properties().Select(p => p.Name).ToList();
                    bool stopFlag = false;
                    foreach (var key in keys)
                    {
                        if (stopFlag) break;
                        switch (key)
                        {
                            case "simpleRecordInfoVo":
                                // 签到
                                string idStr = info[key]["itemId"].ToString();
                                itemID = new string[1] { idStr };
                                taskType = JDTaskType.Checkin;
                                stopFlag = true;
                                break;
                            case "subTasksCommon":
                                // 子项目类任务
                                JArray subList = info[key] as JArray;
                                itemID = subList.AsEnumerable().Select(s => s["itemId"].ToString()).Take(maxTimes - currentTimes).ToArray();
                                stopFlag = true;
                                break;
                            case "shoppingActivityVos":
                            case "browseShopVo":
                                // 逛店
                                // 浏览
                                taskType = JDTaskType.Browse;
                                goto case "subTasksCommon";
                            case "brandMemberVos":
                                // 加入会员
                                taskType = JDTaskType.BeMember;
                                goto case "subTasksCommon";
                            case "assistTaskDetailVo":
                                // 好友互助
                                stopFlag = true;
                                break;
                            default:
                                // 其他情况
                                break;
                        }
                    }
                    // 构建任务
                    if (itemID != null)
                    {
                        JDTask jDTask = new JDTask
                        {
                            TaskID = taskID,
                            ItemID = itemID,
                            Name = taskName,
                            WaitDuration = waitDuration,
                            Type = taskType
                        };
                        tasks.Add(jDTask);
                    }
                    // 如果是浏览加购的任务，提取taskID后面用
                    else if (info["subTitleName"].ToString().StartsWith("浏览并加购"))
                    {
                        feedId = Convert.ToInt32(info["taskId"].ToString());
                    }
                }
            }

            // 获取浏览并加购的信息
            if (feedId >= 0)
            {
                nvc = new List<KeyValuePair<string, string>>();
                nvc.Add(new KeyValuePair<string, string>("functionId", "nian_getFeedDetail"));
                nvc.Add(new KeyValuePair<string, string>("body", string.Format("{{\"taskId\":\"{0}\"}}", feedId)));
                nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                message = new HttpRequestMessage(HttpMethod.Post, feedUrl) { Content = new FormUrlEncodedContent(nvc) };
                result = client.SendAsync(message).Result;
                resultJson = result.Content.ReadAsStringAsync().Result;
                jo = (JObject)JsonConvert.DeserializeObject(resultJson);
                if (jo["data"]["success"].ToString().ToLower().Trim() == "true")
                {
                    // 读取需要加购的信息
                    var resultJO = jo["data"]["result"];
                    JArray taskList = resultJO["addProductVos"] as JArray;
                    foreach (var task in taskList)
                    {
                        // 如果任务已经完成了，那就直接跳过
                        int maxTimes = Convert.ToInt32(task["maxTimes"].ToString());
                        int currentTimes = Convert.ToInt32(task["times"].ToString());
                        if (currentTimes >= maxTimes) continue;
                        JArray itemList = task["productInfoVos"] as JArray;
                        // 取得任务信息
                        int taskID = Convert.ToInt32(task["taskId"].ToString());
                        string taskName = task["taskName"].ToString();
                        JDTaskType taskType = JDTaskType.AddCart;
                        int itemCount = 0;
                        string[] itemID = new string[maxTimes];
                        foreach (var item in itemList)
                        {
                            // 如果说需要加购的物品已经够多了，那就停止增加任务
                            itemCount += 1;
                            if (itemCount > maxTimes) break;
                            itemID[itemCount - 1] = item["itemId"].ToString();
                        }
                        // 构建任务
                        JDTask jDTask = new JDTask
                        {
                            TaskID = taskID,
                            ItemID = itemID,
                            Name = taskName,
                            WaitDuration = 0,
                            Type = taskType
                        };
                        tasks.Add(jDTask);
                    }
                }
            }

            // 获取所有店铺任务
            // 店铺签到
            nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("functionId", "qryCompositeMaterials"));
            nvc.Add(new KeyValuePair<string, string>("body", "{\"qryParam\":\"[{\\\"type\\\":\\\"advertGroup\\\",\\\"mapTo\\\":\\\"homeFeedBannerT\\\",\\\"id\\\":\\\"05143017\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"mapTo\\\":\\\"homeFeedBannerS\\\",\\\"id\\\":\\\"05144045\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"mapTo\\\":\\\"homeFeedBannerA\\\",\\\"id\\\":\\\"05144046\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"mapTo\\\":\\\"homeFeedBannerB\\\",\\\"id\\\":\\\"05144047\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"mapTo\\\":\\\"domainShopData\\\",\\\"id\\\":\\\"05139136\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"mapTo\\\":\\\"domainShopData2\\\",\\\"id\\\":\\\"05144271\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"mapTo\\\":\\\"domainShopToT\\\",\\\"id\\\":\\\"05152048\\\"}]\", \"activityId\":\"2cKMj86srRdhgWcKonfExzK4ZMBy\", \"pageId\":\"\", \"reqSrc\":\"\", \"applyKey\":\"21beast\" }"));
            nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
            nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
            message = new HttpRequestMessage(HttpMethod.Post, shopUrl) { Content = new FormUrlEncodedContent(nvc) };
            result = client.SendAsync(message).Result;
            resultJson = result.Content.ReadAsStringAsync().Result;
            jo = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (jo["msg"].ToString().ToLower().Trim() == "success")
            {
                var shopList = jo["data"]["domainShopData2"]["list"] as JArray;
                string[] itemId = shopList.Select(s => s["link"].ToString()).ToArray();
                JDTask task = new JDTask
                {
                    Name = "店铺签到",
                    ItemID = itemId,
                    Type = JDTaskType.ShopSignin
                };
                tasks.Add(task);
            }

            // 店铺抽奖
            nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("functionId", "qryCompositeMaterials"));
            nvc.Add(new KeyValuePair<string, string>("body", "{\"qryParam\":\"[{\\\"type\\\":\\\"advertGroup\\\",\\\"mapTo\\\":\\\"viewLogo\\\",\\\"id\\\":\\\"05149412\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"mapTo\\\":\\\"bottomLogo\\\",\\\"id\\\":\\\"05149413\\\"}]\", \"activityId\":\"2cKMj86srRdhgWcKonfExzK4ZMBy\", \"pageId\":\"\", \"reqSrc\":\"\", \"applyKey\":\"21beast\" }"));
            nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
            nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
            message = new HttpRequestMessage(HttpMethod.Post, shopUrl) { Content = new FormUrlEncodedContent(nvc) };
            result = client.SendAsync(message).Result;
            resultJson = result.Content.ReadAsStringAsync().Result;
            jo = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (jo["msg"].ToString().ToLower().Trim() == "success")
            {
                var shopList = jo["data"]["bottomLogo"]["list"] as JArray;
                string[] shopId = shopList.Select(s => s["desc"].ToString()).ToArray();
                // 轮训所有商店，获取所有任务
                foreach (var id in shopId)
                {
                    nvc = new List<KeyValuePair<string, string>>();
                    nvc.Add(new KeyValuePair<string, string>("functionId", "nian_shopLotteryInfo"));
                    nvc.Add(new KeyValuePair<string, string>("body", string.Format("{{\"shopSign\":\"{0}\"}}", id)));
                    nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                    nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                    message = new HttpRequestMessage(HttpMethod.Post, shopLotteryUrl) { Content = new FormUrlEncodedContent(nvc) };
                    result = client.SendAsync(message).Result;
                    resultJson = result.Content.ReadAsStringAsync().Result;
                    jo = (JObject)JsonConvert.DeserializeObject(resultJson);
                    if (jo["code"].ToString() == "0")
                    {
                        var taskList = jo["data"]["result"]["taskVos"] as JArray;
                        foreach (JObject task in taskList)
                        {
                            // 如果任务已经完成了，那就直接跳过
                            int maxTimes = Convert.ToInt32(task["maxTimes"].ToString());
                            int currentTimes = Convert.ToInt32(task["times"].ToString());
                            if (currentTimes >= maxTimes) continue;
                            // 提取任务信息
                            int taskID = Convert.ToInt32(task["taskId"].ToString());
                            string taskName = task["taskName"].ToString();
                            string[] itemID = null;
                            JDTaskType taskType = JDTaskType.ShopLottery;
                            // 对不同类型的任务进行区分处理
                            IList<string> keys = task.Properties().Select(p => p.Name).ToList();
                            bool stopFlag = false;
                            foreach (var key in keys)
                            {
                                if (stopFlag) break;
                                switch (key)
                                {
                                    case "simpleRecordInfoVo":
                                        // 签到
                                        string idStr = task[key]["itemId"].ToString();
                                        itemID = new string[1] { idStr };
                                        stopFlag = true;
                                        break;
                                    case "shoppingActivityVos":
                                    case "browseShopVo":
                                    case "brandMemberVos":
                                        // 子项目类任务
                                        JArray subList = task[key] as JArray;
                                        itemID = subList.AsEnumerable().Select(s => s["itemId"].ToString()).ToArray();
                                        stopFlag = true;
                                        break;
                                    default:
                                        // 其他情况
                                        break;
                                }
                            }
                            // 构建任务
                            if (itemID != null)
                            {
                                JDTask jDTask = new JDTask
                                {
                                    TaskID = taskID,
                                    ItemID = itemID,
                                    Name = taskName,
                                    Type = taskType,
                                    ShopSign = id
                                };
                                tasks.Add(jDTask);
                            }
                        }
                    }
                }
            }

            // 小程序任务
            nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("functionId", "nian_getTaskDetail"));
            nvc.Add(new KeyValuePair<string, string>("body", "{\"appSign\":\"2\",\"channel\":1}"));
            nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
            nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
            message = new HttpRequestMessage(HttpMethod.Post, taskUrl) { Content = new FormUrlEncodedContent(nvc) };
            result = client.SendAsync(message).Result;
            resultJson = result.Content.ReadAsStringAsync().Result;
            jo = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (jo["data"]["success"].ToString().ToLower().Trim() == "true")
            {
                // 读取邀请ID以及任务信息
                var resultJO = jo["data"]["result"];
                var taskVos = resultJO["taskVos"] as JArray;
                foreach (JObject info in taskVos)
                {
                    // 如果任务已经完成了，那就直接跳过
                    int maxTimes = Convert.ToInt32(info["maxTimes"].ToString());
                    int currentTimes = Convert.ToInt32(info["times"].ToString());
                    if (currentTimes >= maxTimes) continue;
                    // 取得任务信息
                    int taskID = Convert.ToInt32(info["taskId"].ToString());
                    string taskName = info["taskName"].ToString();
                    string[] itemID = null;
                    int waitDuration = Convert.ToInt32(info["waitDuration"].ToString());
                    JDTaskType taskType = JDTaskType.Unknown;
                    // 对不同类型的任务进行区分处理
                    IList<string> keys = info.Properties().Select(p => p.Name).ToList();
                    bool stopFlag = false;
                    foreach (var key in keys)
                    {
                        if (stopFlag) break;
                        switch (key)
                        {
                            case "simpleRecordInfoVo":
                                // 签到
                                stopFlag = true;
                                break;
                            case "subTasksCommon":
                                // 子项目类任务
                                JArray subList = info[key] as JArray;
                                itemID = subList.AsEnumerable().Select(s => s["itemId"].ToString()).ToArray();
                                stopFlag = true;
                                break;
                            case "shoppingActivityVos":
                            case "browseShopVo":
                                // 逛店
                                // 浏览
                                taskType = JDTaskType.Browse;
                                goto case "subTasksCommon";
                            case "brandMemberVos":
                            case "assistTaskDetailVo":
                                // 加入会员和好友互助
                                stopFlag = true;
                                break;
                            default:
                                // 其他情况
                                break;
                        }
                    }
                    // 构建任务
                    if (itemID != null)
                    {
                        JDTask jDTask = new JDTask
                        {
                            TaskID = taskID,
                            ItemID = itemID,
                            Name = taskName,
                            WaitDuration = waitDuration,
                            Type = taskType
                        };
                        tasks.Add(jDTask);
                    }
                }
            }

            // 特殊任务
            nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("functionId", "nian_getSpecialGiftDetail"));
            nvc.Add(new KeyValuePair<string, string>("body", "{}"));
            nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
            nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
            message = new HttpRequestMessage(HttpMethod.Post, specialGiftUrl) { Content = new FormUrlEncodedContent(nvc) };
            result = client.SendAsync(message).Result;
            resultJson = result.Content.ReadAsStringAsync().Result;
            jo = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (jo["data"]["success"].ToString().ToLower().Trim() == "true")
            {
                // 读取邀请ID以及任务信息
                var resultJO = jo["data"]["result"];
                var taskVos = resultJO["taskVos"] as JArray;
                foreach (JObject info in taskVos)
                {
                    // 如果任务已经完成了，那就直接跳过
                    int maxTimes = Convert.ToInt32(info["maxTimes"].ToString());
                    int currentTimes = Convert.ToInt32(info["times"].ToString());
                    if (currentTimes >= maxTimes) continue;
                    // 取得任务信息
                    int taskID = Convert.ToInt32(info["taskId"].ToString());
                    string taskName = info["taskName"].ToString();
                    string[] itemID = null;
                    JDTaskType taskType = JDTaskType.Unknown;
                    // 对不同类型的任务进行区分处理
                    IList<string> keys = info.Properties().Select(p => p.Name).ToList();
                    bool stopFlag = false;
                    foreach (var key in keys)
                    {
                        if (stopFlag) break;
                        switch (key)
                        {
                            case "simpleRecordInfoVo":
                                // 分享给一位好友
                                taskType = JDTaskType.SpecialGift;
                                itemID = new string[1] { info[key]["itemId"].ToString() };
                                stopFlag = true;
                                break;
                            case "subTasksCommon":
                                // 子项目类任务
                                JArray subList = info[key] as JArray;
                                itemID = subList.AsEnumerable().Select(s => s["itemId"].ToString()).ToArray();
                                stopFlag = true;
                                break;
                            case "shoppingActivityVos":
                                taskType = JDTaskType.SpecialGift;
                                goto case "subTasksCommon";
                            default:
                                // 其他情况
                                break;
                        }
                    }
                    // 构建任务
                    if (itemID != null)
                    {
                        JDTask jDTask = new JDTask
                        {
                            TaskID = taskID,
                            ItemID = itemID,
                            Name = taskName,
                            Type = taskType
                        };
                        tasks.Add(jDTask);
                    }
                }
            }
            return tasks;
        }


        /// <summary>
        /// 做AR游戏
        /// </summary>
        private void DoArGame()
        {
            if (CookieContainer == null) return;
            // AR游戏逻辑
            const string UserAPI = "https://arvractivity.jd.com/nian/arNianGetUser.do";
            const string StartAPI = "https://arvractivity.jd.com/nian/arNianStartGame.do";
            const string EndAPI = "https://arvractivity.jd.com/nian/arNianEndGame.do";
            HttpClient client = Session;
            client.DefaultRequestHeaders.Referrer = new Uri("https://wbbny.m.jd.com/babelDiy/Zeus/2cKMj86srRdhgWcKonfExzK4ZMBy/index.html");
            var message = new HttpRequestMessage(HttpMethod.Get, UserAPI);
            var result = client.SendAsync(message).Result;
            var resultJson = result.Content.ReadAsStringAsync().Result;
            var resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resultJo["code"].ToString().Trim() == "200")
            {
                int gameCount = Convert.ToInt32(resultJo["rv"]["maxGameNum"].ToString()) - Convert.ToInt32(resultJo["rv"]["gameNum"].ToString());
                int level = Convert.ToInt32(resultJo["rv"]["level"].ToString());
                for (int i = 0; i < gameCount; i++)
                {
                    string outputText = string.Format("{0} [{1}]执行中...{2} <{3}/{4}>", DateTime.Now.ToString("HH:mm:ss"), "AR游戏", "等待8秒...", i + 1, gameCount);
                    UpdateTextToForm(Environment.NewLine + outputText);
                    var nvc = new List<KeyValuePair<string, string>>();
                    nvc.Add(new KeyValuePair<string, string>("level", level.ToString()));
                    message = new HttpRequestMessage(HttpMethod.Post, StartAPI) { Content = new FormUrlEncodedContent(nvc) };
                    result = client.SendAsync(message).Result;
                    resultJson = result.Content.ReadAsStringAsync().Result;
                    resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
                    string randStr = resultJo["rv"]["random"].ToString();
                    Thread.Sleep(8000);
                    nvc = new List<KeyValuePair<string, string>>();
                    nvc.Add(new KeyValuePair<string, string>("random", randStr));
                    nvc.Add(new KeyValuePair<string, string>("type", "1"));
                    nvc.Add(new KeyValuePair<string, string>("level", level.ToString()));
                    message = new HttpRequestMessage(HttpMethod.Post, EndAPI) { Content = new FormUrlEncodedContent(nvc) };
                    result = client.SendAsync(message).Result;
                    resultJson = result.Content.ReadAsStringAsync().Result;
                    resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
                    string bonus = resultJo["rv"]["winAward"].ToString();
                    outputText = string.Format("{0} [{1}]执行完毕，获得奖励{2}爆竹 <{3}/{4}>", DateTime.Now.ToString("HH:mm:ss"), "AR游戏", bonus, i + 1, gameCount);
                    UpdateTextToForm(Environment.NewLine + outputText);
                }
            }
        }

        /// <summary>
        /// 秒杀优惠券
        /// </summary>
        private void DoKillCoupon()
        {
            if (CookieContainer == null) return;
            // 领优惠券逻辑
            const string couponAPI = "https://api.m.jd.com/client.action?functionId=nian_killCouponList";
            const string killAPI = "https://api.m.jd.com/client.action?functionId=nian_killCoupon";
            HttpClient client = Session;
            client.DefaultRequestHeaders.Referrer = new Uri("https://wbbny.m.jd.com/babelDiy/Zeus/2cKMj86srRdhgWcKonfExzK4ZMBy/index.html");
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("functionId", "nian_killCouponList"));
            nvc.Add(new KeyValuePair<string, string>("body", "{}"));
            nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
            nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
            var message = new HttpRequestMessage(HttpMethod.Post, couponAPI) { Content = new FormUrlEncodedContent(nvc) };
            var result = client.SendAsync(message).Result;
            var resultJson = result.Content.ReadAsStringAsync().Result;
            var resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resultJo["data"]["success"].ToString().ToLower().Trim() == "true")
            {
                var couponList = resultJo["data"]["result"] as JArray;
                string[] couponIds = couponList.Select(s => s["skuId"].ToString()).ToArray();
                foreach (var id in couponIds)
                {
                    string timeStamp = Utils.GetTimeStampLong();
                    string sessionC = Utils.GetTimeStampSuperLong();
                    string randSign = Utils.GetRandomString(40, false, false, true, false, "");
                    string randStr = Utils.GetRandomString(10, true, true, true, false, "");
                    string randIntStr = Utils.GetRandomString(6, true, false, false, false, "");
                    string format = "{{\"skuId\":{0},\"ss\":\"{{\\\"extraData\\\":{{\\\"is_trust\\\":true,\\\"sign\\\":\\\"{1}\\\",\\\"fpb\\\":\\\"\\\",\\\"time\\\":{2},\\\"encrypt\\\":\\\"2\\\",\\\"nonstr\\\":\\\"{3}\\\",\\\"jj\\\":\\\"\\\",\\\"token\\\":\\\"\\\",\\\"cf_v\\\":\\\"1.0.1\\\",\\\"client_version\\\":\\\"2.2.1\\\",\\\"call_stack\\\":\\\"d3bd90bf525418114cceb94ba45e3ab8\\\",\\\"session_c\\\":\\\"{4}\\\",\\\"buttonid\\\":\\\"\\\",\\\"sceneid\\\":\\\"\\\"}},\\\"secretp\\\":\\\"{5}\\\",\\\"random\\\":\\\"{6}\\\"}}\"}}";
                    nvc = new List<KeyValuePair<string, string>>();
                    nvc.Add(new KeyValuePair<string, string>("functionId", "nian_killCoupon"));
                    nvc.Add(new KeyValuePair<string, string>("body", string.Format(format, id, randSign, timeStamp, randStr, sessionC, SecretP, randIntStr)));
                    nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                    nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                    message = new HttpRequestMessage(HttpMethod.Post, killAPI) { Content = new FormUrlEncodedContent(nvc) };
                    result = client.SendAsync(message).Result;
                    resultJson = result.Content.ReadAsStringAsync().Result;
                    resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
                    Thread.Sleep(2000);
                }
            }
        }

        /// <summary>
        /// 进行互助
        /// </summary>
        /// <param name="inviteCodeRaw"></param>
        private void DoHelp(string inviteCodeRaw)
        {
            Task.Run(() =>
            {
                string[] inviteCodes;
                // 判断一下是不是有多个inviteCode
                if (inviteCodeRaw.IndexOf(",") > 0)
                {
                    inviteCodes = inviteCodeRaw.Split(',');
                }
                else
                {
                    inviteCodes = new string[1] { inviteCodeRaw };
                }
                // 助力
                const string infoAPI = "https://api.m.jd.com/client.action?functionId=nian_getHomeData";
                const string helpAPI = "https://api.m.jd.com/client.action?functionId=nian_collectScore";
                foreach (var code in inviteCodes)
                {
                    HttpClient client = Session;
                    client.DefaultRequestHeaders.Referrer = new Uri("https://wbbny.m.jd.com/babelDiy/Zeus/2cKMj86srRdhgWcKonfExzK4ZMBy/index.html");
                    var nvc = new List<KeyValuePair<string, string>>();
                    nvc.Add(new KeyValuePair<string, string>("functionId", "nian_getHomeData"));
                    nvc.Add(new KeyValuePair<string, string>("body", string.Format("{{\"inviteId\":\"{0}\"}}", code)));
                    nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                    nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                    var message = new HttpRequestMessage(HttpMethod.Post, infoAPI) { Content = new FormUrlEncodedContent(nvc) };
                    var result = client.SendAsync(message).Result;
                    var resultJson = result.Content.ReadAsStringAsync().Result;
                    var resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
                    if (resultJo["data"]["success"].ToString().ToLower().Trim() == "true")
                    {
                        var tmpJo = resultJo["data"]["result"]["homeMainInfo"];
                        string itemId = tmpJo["guestInfo"]["itemId"].ToString();
                        string taskId = tmpJo["guestInfo"]["taskId"].ToString();
                        string helperName = tmpJo["guestInfo"]["nickName"].ToString();
                        string timeStamp = Utils.GetTimeStampLong();
                        string sessionC = Utils.GetTimeStampSuperLong();
                        string randSign = Utils.GetRandomString(40, false, false, true, false, "");
                        string randStr = Utils.GetRandomString(10, true, true, true, false, "");
                        string randIntStr = Utils.GetRandomString(6, true, false, false, false, "");
                        string format = "{{\"taskId\":{0},\"ss\":\"{{\\\"extraData\\\":{{\\\"is_trust\\\":true,\\\"sign\\\":\\\"{1}\\\",\\\"fpb\\\":\\\"\\\",\\\"time\\\":{2},\\\"encrypt\\\":\\\"2\\\",\\\"nonstr\\\":\\\"{3}\\\",\\\"jj\\\":\\\"\\\",\\\"token\\\":\\\"{4}\\\",\\\"cf_v\\\":\\\"1.0.1\\\",\\\"client_version\\\":\\\"2.2.1\\\",\\\"call_stack\\\":\\\"{5}\\\",\\\"session_c\\\":\\\"{6}\\\",\\\"buttonid\\\":\\\"\\\",\\\"sceneid\\\":\\\"\\\"}},\\\"secretp\\\":\\\"{7}\\\",\\\"random\\\":\\\"{8}\\\"}}\",\"itemId\":\"{9}\",\"inviteId\":\"{10}\"}}";
                        string postBody = string.Format(format, taskId, randSign, timeStamp, randStr, RandomToken, RandomCallStack, sessionC, SecretP, randIntStr, itemId, code);
                        nvc = new List<KeyValuePair<string, string>>();
                        nvc.Add(new KeyValuePair<string, string>("functionId", "nian_collectScore"));
                        nvc.Add(new KeyValuePair<string, string>("body", postBody));
                        nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                        nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                        message = new HttpRequestMessage(HttpMethod.Post, helpAPI) { Content = new FormUrlEncodedContent(nvc) };
                        result = client.SendAsync(message).Result;
                        resultJson = result.Content.ReadAsStringAsync().Result;
                        resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
                        if (resultJo["code"].ToString() == "0")
                        {
                            string retMsg = resultJo["data"]["bizMsg"].ToString();
                            if (retMsg.Trim() == "success")
                            {
                                retMsg = string.Format("助力成功，被助力ID：{0}，助力ID：{1}", helperName, UserSecretName);
                            }
                            else
                            {
                                retMsg += string.Format("，被助力ID：{0}，助力ID：{1}", helperName, UserSecretName);
                            }
                            UpdateTextToForm(System.Environment.NewLine + string.Format("{0} [好友助力]{1}", DateTime.Now.ToString("HH:mm:ss"), retMsg));
                        }
                        else
                        {
                            UpdateTextToForm(System.Environment.NewLine + string.Format("{0} [好友助力]为ID:{1}助力失败！", DateTime.Now.ToString("HH:mm:ss"), helperName));
                        }
                    }
                    else
                    {
                        UpdateTextToForm(System.Environment.NewLine + string.Format("{0} [好友助力]为好友助力失败！", DateTime.Now.ToString("HH:mm:ss")));
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        /// <summary>
        /// 进行任务
        /// </summary>
        /// <param name="task"></param>
        private void DoTask(JDTask task)
        {
            if (CookieContainer == null) return;
            const string postAPI = "https://api.m.jd.com/client.action?functionId=nian_collectScore";
            const string postSpecialAPI = "https://api.m.jd.com/client.action?functionId=nian_collectSpecialGift";
            const string shopSignAPI_Read = "https://api.m.jd.com/client.action?functionId=nian_shopSignInRead";
            const string shopSignAPI_Write = "https://api.m.jd.com/client.action?functionId=nian_shopSignInWrite";
            const string shopLotteryAPI = "https://api.m.jd.com/client.action?functionId=nian_doShopLottery";
            HttpClient client = Session;
            client.DefaultRequestHeaders.Referrer = new Uri("https://api.m.jd.com/client.action?functionId=nian_getTaskDetail");
            for (int i = 0; i < task.ItemID.Length; i++)
            {
                // 执行任务
                string timeStamp = Utils.GetTimeStampLong();
                string sessionC = Utils.GetTimeStampSuperLong();
                string randSign = Utils.GetRandomString(40, false, false, true, false, "");
                string randStr = Utils.GetRandomString(10, true, true, true, false, "");
                string randIntStr = Utils.GetRandomString(6, true, false, false, false, "");
                string format, postBody, targetUrl;
                switch (task.Type)
                {
                    case JDTaskType.BeMember:
                    case JDTaskType.AddCart:
                    case JDTaskType.Checkin:
                        format = "{{\"taskId\":{0},\"ss\":\"{{\\\"extraData\\\":{{\\\"is_trust\\\":true,\\\"sign\\\":\\\"{1}\\\",\\\"fpb\\\":\\\"\\\",\\\"time\\\":{2},\\\"encrypt\\\":\\\"2\\\",\\\"nonstr\\\":\\\"{3}\\\",\\\"jj\\\":\\\"\\\",\\\"token\\\":\\\"{4}\\\",\\\"cf_v\\\":\\\"1.0.1\\\",\\\"client_version\\\":\\\"2.2.1\\\",\\\"call_stack\\\":\\\"{5}\\\",\\\"session_c\\\":\\\"{6}\\\",\\\"buttonid\\\":\\\"\\\",\\\"sceneid\\\":\\\"\\\"}},\\\"secretp\\\":\\\"{7}\\\",\\\"random\\\":\\\"{8}\\\"}}\",\"itemId\":\"{9}\"}}";
                        postBody = string.Format(format, task.TaskID, randSign, timeStamp, randStr, RandomToken, RandomCallStack, sessionC, SecretP, randIntStr, task.ItemID[i]);
                        targetUrl = postAPI;
                        break;
                    case JDTaskType.SpecialGift:
                        format = "{{\"taskId\":{0},\"ss\":\"{{\\\"extraData\\\":{{\\\"is_trust\\\":true,\\\"sign\\\":\\\"{1}\\\",\\\"fpb\\\":\\\"\\\",\\\"time\\\":{2},\\\"encrypt\\\":\\\"2\\\",\\\"nonstr\\\":\\\"{3}\\\",\\\"jj\\\":\\\"\\\",\\\"token\\\":\\\"{4}\\\",\\\"cf_v\\\":\\\"1.0.1\\\",\\\"client_version\\\":\\\"2.2.1\\\",\\\"call_stack\\\":\\\"{5}\\\",\\\"session_c\\\":\\\"{6}\\\",\\\"buttonid\\\":\\\"\\\",\\\"sceneid\\\":\\\"\\\"}},\\\"secretp\\\":\\\"{7}\\\",\\\"random\\\":\\\"{8}\\\"}}\",\"itemId\":\"{9}\"}}";
                        postBody = string.Format(format, task.TaskID, randSign, timeStamp, randStr, RandomToken, RandomCallStack, sessionC, SecretP, randIntStr, task.ItemID[i]);
                        targetUrl = postSpecialAPI;
                        break;
                    case JDTaskType.Browse:
                        format = "{{\"taskId\":{0},\"ss\":\"{{\\\"extraData\\\":{{\\\"is_trust\\\":true,\\\"sign\\\":\\\"{1}\\\",\\\"fpb\\\":\\\"\\\",\\\"time\\\":{2},\\\"encrypt\\\":\\\"2\\\",\\\"nonstr\\\":\\\"{3}\\\",\\\"jj\\\":\\\"\\\",\\\"token\\\":\\\"{4}\\\",\\\"cf_v\\\":\\\"1.0.1\\\",\\\"client_version\\\":\\\"2.2.1\\\",\\\"call_stack\\\":\\\"{5}\\\",\\\"session_c\\\":\\\"{6}\\\",\\\"buttonid\\\":\\\"\\\",\\\"sceneid\\\":\\\"\\\"}},\\\"secretp\\\":\\\"{7}\\\",\\\"random\\\":\\\"{8}\\\"}}\",\"itemId\":\"{9}\",\"actionType\":1}}";
                        postBody = string.Format(format, task.TaskID, randSign, timeStamp, randStr, RandomToken, RandomCallStack, sessionC, SecretP, randIntStr, task.ItemID[i]);
                        targetUrl = postAPI;
                        break;
                    case JDTaskType.ShopLottery:
                        format = "{{\"taskId\":{0},\"ss\":\"{{\\\"extraData\\\":{{\\\"is_trust\\\":true,\\\"sign\\\":\\\"{1}\\\",\\\"fpb\\\":\\\"\\\",\\\"time\\\":{2},\\\"encrypt\\\":\\\"2\\\",\\\"nonstr\\\":\\\"{3}\\\",\\\"jj\\\":\\\"\\\",\\\"token\\\":\\\"{4}\\\",\\\"cf_v\\\":\\\"1.0.1\\\",\\\"client_version\\\":\\\"2.2.1\\\",\\\"call_stack\\\":\\\"{5}\\\",\\\"session_c\\\":\\\"{6}\\\",\\\"buttonid\\\":\\\"\\\",\\\"sceneid\\\":\\\"\\\"}},\\\"secretp\\\":\\\"{7}\\\",\\\"random\\\":\\\"{8}\\\"}}\",\"itemId\":\"{9}\",\"shopSign\":\"{10}\"}}";
                        postBody = string.Format(format, task.TaskID, randSign, timeStamp, randStr, RandomToken, RandomCallStack, sessionC, SecretP, randIntStr, task.ItemID[i], task.ShopSign);
                        targetUrl = postAPI;
                        break;
                    case JDTaskType.ShopSignin:
                        // ShopSiginInRead逻辑
                        var tmpNvc = new List<KeyValuePair<string, string>>();
                        tmpNvc.Add(new KeyValuePair<string, string>("functionId", "nian_shopSignInRead"));
                        tmpNvc.Add(new KeyValuePair<string, string>("body", string.Format("{{\"shopSign\":\"{0}\"}}", task.ItemID[i])));
                        tmpNvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                        tmpNvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                        var tmpMessage = new HttpRequestMessage(HttpMethod.Post, shopSignAPI_Read) { Content = new FormUrlEncodedContent(tmpNvc) };
                        var tmpResult = client.SendAsync(tmpMessage).Result;
                        var tmpResultJson = tmpResult.Content.ReadAsStringAsync().Result;
                        JObject tmpResultJo = (JObject)JsonConvert.DeserializeObject(tmpResultJson);
                        if (tmpResultJo["data"]["success"].ToString().ToLower().Trim() != "true")
                            continue;
                        format = "{{\"shopSign\":\"{0}\",\"ss\":\"{{\\\"extraData\\\":{{\\\"jj\\\":1,\\\"is_trust\\\":true,\\\"sign\\\":\\\"\\\",\\\"fpb\\\":\\\"\\\",\\\"time\\\":\\\"{1}\\\",\\\"encrypt\\\":\\\"\\\",\\\"nonstr\\\":\\\"\\\",\\\"token\\\":\\\"\\\",\\\"cf_v\\\":\\\"1.0.1\\\",\\\"client_version\\\":\\\"2.2.1\\\",\\\"call_stack\\\":\\\"\\\",\\\"session_c\\\":\\\"\\\",\\\"buttonid\\\":\\\"\\\",\\\"sceneid\\\":\\\"\\\"}},\\\"secretp\\\":\\\"{2}\\\",\\\"random\\\":\\\"\\\"}}\"}}";
                        postBody = string.Format(format, task.ItemID[i], timeStamp, SecretP);
                        targetUrl = shopSignAPI_Write;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                var nvc = new List<KeyValuePair<string, string>>();
                nvc.Add(new KeyValuePair<string, string>("functionId", task.Type == JDTaskType.ShopSignin ? "nian_shopSignInWrite" : "nian_collectScore"));
                nvc.Add(new KeyValuePair<string, string>("body", postBody));
                nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                var message = new HttpRequestMessage(HttpMethod.Post, targetUrl) { Content = new FormUrlEncodedContent(nvc) };
                var result = client.SendAsync(message).Result;
                var resultJson = result.Content.ReadAsStringAsync().Result;
                JObject resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
                // 如果调用成功，进入下一步，获取奖励，否则的话稍后再试
                if (resultJo["code"].ToString() == "0")
                {
                    if (resultJo["data"]["bizCode"].ToString() == "0")
                    {
                        Random rand = new Random();
                        Thread.Sleep((int)((task.WaitDuration + rand.NextDouble() * 2) * 1000));
                        if (task.Type == JDTaskType.Browse)
                        {
                            string outputText = string.Format("{0} [{1}]执行中...{2} <{3}/{4}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, task.WaitDuration > 0 ? string.Format("等待{0}秒...", task.WaitDuration) : "", i + 1, task.ItemID.Length);
                            UpdateTextToForm(Environment.NewLine + outputText);
                            format = "{{\"taskId\":{0},\"ss\":\"{{\\\"extraData\\\":{{\\\"is_trust\\\":true,\\\"sign\\\":\\\"{1}\\\",\\\"fpb\\\":\\\"\\\",\\\"time\\\":{2},\\\"encrypt\\\":\\\"2\\\",\\\"nonstr\\\":\\\"{3}\\\",\\\"jj\\\":\\\"\\\",\\\"token\\\":\\\"\\\",\\\"cf_v\\\":\\\"1.0.1\\\",\\\"client_version\\\":\\\"2.2.1\\\",\\\"call_stack\\\":\\\"d3bd90bf525418114cceb94ba45e3ab8\\\",\\\"session_c\\\":\\\"{4}\\\",\\\"buttonid\\\":\\\"\\\",\\\"sceneid\\\":\\\"\\\"}},\\\"secretp\\\":\\\"{5}\\\",\\\"random\\\":\\\"{6}\\\"}}\",\"itemId\":\"{7}\",\"actionType\":0}}";
                            postBody = string.Format(format, task.TaskID, randSign, timeStamp, randStr, sessionC, SecretP, randIntStr, task.ItemID[i]);
                            nvc = new List<KeyValuePair<string, string>>();
                            nvc.Add(new KeyValuePair<string, string>("functionId", "nian_collectScore"));
                            nvc.Add(new KeyValuePair<string, string>("body", postBody));
                            nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                            nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                            message = new HttpRequestMessage(HttpMethod.Post, postAPI) { Content = new FormUrlEncodedContent(nvc) };
                            result = client.SendAsync(message).Result;
                            resultJson = result.Content.ReadAsStringAsync().Result;
                            resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
                            if (resultJo["data"]["success"].ToString().ToLower().Trim() == "true")
                            {
                                string taskBonus = resultJo["data"]["result"]["successToast"].ToString();
                                outputText = string.Format("{0} [{1}]{2} <{3}/{4}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, taskBonus, i + 1, task.ItemID.Length);
                                UpdateTextToForm(Environment.NewLine + outputText);
                                continue;
                            }
                            else
                            {
                                outputText = string.Format("{0} [{1}]任务执行失败，稍后重试 <{2}/{3}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, i + 1, task.ItemID.Length);
                                UpdateTextToForm(Environment.NewLine + outputText);
                                continue;
                            }
                        }
                        else if (task.Type == JDTaskType.ShopLottery)
                        {
                            // 店铺抽奖
                            string outputText = string.Format("{0} [{1}]执行中...{2} <{3}/{4}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, task.WaitDuration > 0 ? string.Format("等待{0}秒...", task.WaitDuration) : "", i + 1, task.ItemID.Length);
                            UpdateTextToForm(Environment.NewLine + outputText);
                            while (true)
                            {
                                nvc = new List<KeyValuePair<string, string>>();
                                nvc.Add(new KeyValuePair<string, string>("functionId", "nian_doShopLottery"));
                                nvc.Add(new KeyValuePair<string, string>("body", string.Format("{{\"shopSign\":\"{0}\"}}", task.ShopSign)));
                                nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                                nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                                message = new HttpRequestMessage(HttpMethod.Post, shopLotteryAPI) { Content = new FormUrlEncodedContent(nvc) };
                                result = client.SendAsync(message).Result;
                                resultJson = result.Content.ReadAsStringAsync().Result;
                                resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
                                if (resultJo["data"]["success"].ToString().ToLower().Trim() == "true")
                                {
                                    string bonus = resultJo["data"]["result"]["score"] != null ? resultJo["data"]["result"]["score"].ToString() : "0";
                                    outputText = string.Format("{0} [{1}]执行完毕，获得{2}爆竹<{3}/{4}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, bonus, i + 1, task.ItemID.Length);
                                    UpdateTextToForm(Environment.NewLine + outputText);
                                    if (resultJo["data"]["result"]["lotteryNum"] == null || resultJo["data"]["result"]["lotteryNum"].ToString() == "0")
                                        break;
                                }
                                else
                                {
                                    outputText = string.Format("{0} [{1}]执行失败 <{2}/{3}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, i + 1, task.ItemID.Length);
                                    UpdateTextToForm(Environment.NewLine + outputText);
                                    break;
                                }
                                Thread.Sleep(2000);
                            }
                        }
                        else if (task.Type == JDTaskType.BeMember)
                        {
                            string outputText = string.Format("{0} [{1}]执行中...<{2}/{3}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, i + 1, task.ItemID.Length);
                            UpdateTextToForm(Environment.NewLine + outputText);
                            string taskBonus = "入会成功！";
                            outputText = string.Format("{0} [{1}]{2} <{3}/{4}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, taskBonus, i + 1, task.ItemID.Length);
                            UpdateTextToForm(Environment.NewLine + outputText);
                            continue;
                        }
                        else if (task.Type == JDTaskType.ShopSignin || task.Type == JDTaskType.Checkin)
                        {
                            string outputText = string.Format("{0} [{1}]执行中...<{2}/{3}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, i + 1, task.ItemID.Length);
                            UpdateTextToForm(Environment.NewLine + outputText);
                            string taskBonus = string.Format("签到成功！获得{0}个爆竹", resultJo["data"]["result"]["score"].ToString());
                            outputText = string.Format("{0} [{1}]{2} <{3}/{4}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, taskBonus, i + 1, task.ItemID.Length);
                            UpdateTextToForm(Environment.NewLine + outputText);
                            continue;
                        }
                        else if (task.Type == JDTaskType.AddCart)
                        {
                            string outputText = string.Format("{0} [{1}]执行中...<{2}/{3}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, i + 1, task.ItemID.Length);
                            UpdateTextToForm(Environment.NewLine + outputText);
                            var tmpJo = resultJo["data"]["result"];
                            if (tmpJo["maxTimes"].ToString() == tmpJo["times"].ToString())
                            {
                                string taskBonus = string.Format("{0} [{1}]{2} <{3}/{4}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, string.Format("执行成功，获得{0}个爆竹", tmpJo["score"].ToString()), i + 1, task.ItemID.Length);
                                UpdateTextToForm(Environment.NewLine + taskBonus);
                            }
                            continue;
                        }
                        else
                        {
                            string outputText = string.Format("{0} [{1}]执行中...<{2}/{3}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, i + 1, task.ItemID.Length);
                            UpdateTextToForm(Environment.NewLine + outputText);
                            outputText = string.Format("{0} [{1}]特殊任务执行成功！ <{2}/{3}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, i + 1, task.ItemID.Length);
                            UpdateTextToForm(Environment.NewLine + outputText);
                            continue;
                        }
                    }
                    else
                    {
                        int bizCode = Convert.ToInt32(resultJo["data"]["bizCode"].ToString());
                        string bizMessage = resultJo["data"]["bizMsg"].ToString();
                        string outputText = string.Format("{0} [{1}]任务失败，错误信息：{2}，{3} <{4}/{5}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, bizCode, bizMessage, i + 1, task.ItemID.Length);
                        UpdateTextToForm(Environment.NewLine + outputText);
                        // 入会奖励领取失败时尝试入会
                        if (task.Type == JDTaskType.BeMember && bizCode == -1)
                        {
                            var brandRegUrl = new Uri(resultJo["data"]["result"]["brandRegUrl"].ToString());
                            var query = HttpUtility.ParseQueryString(brandRegUrl.Query);
                            string venderId = query.Get("venderId");
                            string shopId = query.Get("shopId");
                            string channel = query.Get("channel");
                            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
                            format = "{{\"venderId\":\"{0}\",\"shopId\":\"{1}\",\"bindByVerifyCodeFlag\":1,\"registerExtend\":{{}},\"writeChildFlag\":0,\"channel\":{2}}}";
                            queryString.Add("appid", "jd_shop_member");
                            queryString.Add("functionId", "bindWithVender");
                            queryString.Add("body", string.Format(format, venderId, shopId, channel));
                            queryString.Add("client", "H5");
                            queryString.Add("clientVersion", "9.2.0");
                            queryString.Add("uuid", "12354");
                            queryString.Add("jsonp", "");
                            string beMemberUrl = "https://api.m.jd.com/client.action?" + queryString.ToString();
                            message = new HttpRequestMessage(HttpMethod.Get, beMemberUrl);
                            result = client.SendAsync(message).Result;
                            resultJson = result.Content.ReadAsStringAsync().Result;
                            resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
                            outputText = string.Format("{0} [{1}] 尝试入会，返回结果：{2}", DateTime.Now.ToString("HH:mm:ss"), task.Name, resultJo["message"].ToString());
                            UpdateTextToForm(Environment.NewLine + outputText);
                        }
                        // 店铺签到任务如果已经做过了提早退出
                        if (task.Type == JDTaskType.ShopSignin && bizCode == -1)
                            break;
                        // 今日已达到上限
                        if (bizCode == -2)
                        {
                            outputText = string.Format("{0} [{1}]今日已达上限，该任务将不再执行...<{2}/{3}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, i + 1, task.ItemID.Length);
                            UpdateTextToForm(Environment.NewLine + outputText);
                            break;
                        }
                        continue;
                    }
                }
                else
                {
                    string outputText = string.Format("{0} [{1}]调用接口返回失败 <{2}/{3}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, i + 1, task.ItemID.Length);
                    UpdateTextToForm(Environment.NewLine + outputText);
                    continue;
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 获取神仙书院的任务
        /// </summary>
        /// <returns></returns>
        private List<SXSYTask> GetSXSYTasks()
        {
            List<SXSYTask> tasks = new List<SXSYTask>();
            if (string.IsNullOrWhiteSpace(Txt_Lat.Text)) return tasks;
            if (string.IsNullOrWhiteSpace(Txt_Lat.Text)) return tasks;
            string lat = Txt_Lat.Text.Trim();
            string lon = Txt_Lon.Text.Trim();
            string format = "https://api.m.jd.com/client.action?functionId=mcxhd_brandcity_taskList&appid=publicUseApi&body=%7B%22lat%22:{0},%22lng%22:{1},%22token%22:%22{2}%22%7D&t={3}&client=wh5&clientVersion=1.0.0";
            string taskUrl = string.Format(format, lat, lon, SXSYToken, Utils.GetTimeStampLong());
            HttpClient client = Session;
            client.DefaultRequestHeaders.Remove("Origin");
            client.DefaultRequestHeaders.Referrer = new Uri("https://h5.m.jd.com/babelDiy/Zeus/4XjemYYyPScjmGyjej78M6nsjZvj/index.html");
            var message = new HttpRequestMessage(HttpMethod.Post, taskUrl);
            var result = client.SendAsync(message).Result;
            var resultJson = result.Content.ReadAsStringAsync().Result;
            JObject jo = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (jo["retCode"].ToString() == "200")
            {
                // 读取任务信息
                var resultJO = jo["result"];
                var taskVos = resultJO["tasks"] as JArray;
                foreach (JObject info in taskVos)
                {
                    // 如果任务已经完成了，那就直接跳过
                    int maxTimes = Convert.ToInt32(info["maxTimes"].ToString());
                    int currentTimes = Convert.ToInt32(info["times"].ToString());
                    if (currentTimes >= maxTimes) continue;
                    // 取得任务信息
                    string taskName = info["taskName"].ToString();
                    int waitDuration = Convert.ToInt32(info["waitDuration"].ToString());
                    JArray subItems = info["subItem"] as JArray;
                    List<string> itemTokens = new List<string>();
                    List<string> taskTokens = new List<string>();
                    foreach (var item in subItems)
                    {
                        string itemToken = item["itemToken"].ToString();
                        string taskToken = item["taskToken"].ToString();
                        itemTokens.Add(itemToken);
                        taskTokens.Add(taskToken);
                    }
                    // 构建任务
                    if (taskName == "邀请朋友一起玩" || taskName == "开通联合品牌会员") continue;
                    SXSYTask task = new SXSYTask
                    {
                        Name = taskName,
                        ItemToken = itemTokens.ToArray(),
                        TaskToken = taskTokens.ToArray(),
                        WaitDuration = waitDuration
                    };
                    tasks.Add(task);
                }
            }
            return tasks;
        }

        /// <summary>
        /// 神仙书院互助
        /// </summary>
        /// <param name="inviteCodeRaw"></param>
        private void DoSXXYHelp(string inviteCodeRaw)
        {
            Task.Run(() =>
            {
                string[] inviteCodes;
                // 判断一下是不是有多个inviteCode
                if (inviteCodeRaw.IndexOf(",") > 0)
                {
                    inviteCodes = inviteCodeRaw.Split(',');
                }
                else
                {
                    inviteCodes = new string[1] { inviteCodeRaw };
                }
                var client = Session;
                client.DefaultRequestHeaders.Referrer = new Uri("https://h5.m.jd.com/babelDiy/Zeus/4XjemYYyPScjmGyjej78M6nsjZvj/index.html");
                for (int i = 0; i < inviteCodes.Length; i++)
                {
                    string format = "https://api.m.jd.com/client.action?functionId=mcxhd_brandcity_doTask&appid=publicUseApi&body=%7B%22itemToken%22:%22{0}%22,%22token%22:%22{1}%22%7D&t={2}&client=wh5&clientVersion=1.0.0";
                    string beforeAPI = string.Format(format, inviteCodes[i], SXSYToken, Utils.GetTimeStampLong());
                    var response = Session.GetAsync(beforeAPI).Result;
                    var responseStr = response.Content.ReadAsStringAsync().Result;
                    var responseJo = (JObject)JsonConvert.DeserializeObject(responseStr);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string retMsg = responseJo["retMessage"].ToString();
                        if (retMsg.Trim() == "success")
                            retMsg = "为好友助力成功";
                        UpdateTextToForm(System.Environment.NewLine + string.Format("{0} [好友助力]{1} <{2}/{3}>", DateTime.Now.ToString("HH:mm:ss"), retMsg, i + 1, inviteCodes.Length));
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        /// <summary>
        /// PK互助
        /// </summary>
        /// <param name="inviteCodeRaw"></param>
        private void DoPKHelp(string inviteCodeRaw)
        {
            Task.Run(() =>
            {
                string[] inviteCodes;
                // 判断一下是不是有多个inviteCode
                if (inviteCodeRaw.IndexOf(",") > 0)
                {
                    inviteCodes = inviteCodeRaw.Split(',');
                }
                else
                {
                    inviteCodes = new string[1] { inviteCodeRaw };
                }
                var client = Session;
                client.DefaultRequestHeaders.Referrer = new Uri("https://wbbny.m.jd.com/babelDiy/Zeus/2cKMj86srRdhgWcKonfExzK4ZMBy/index.html");
                for (int i = 0; i < inviteCodes.Length; i++)
                {
                    // 获取助力对象信息
                    string API = "https://api.m.jd.com/client.action?functionId=nian_pk_getHomeData";
                    var nvc = new List<KeyValuePair<string, string>>();
                    nvc.Add(new KeyValuePair<string, string>("functionId", "nian_pk_assistGroup"));
                    nvc.Add(new KeyValuePair<string, string>("body", string.Format("{{\"inviteId\":\"{0}\"}}", inviteCodes[i])));
                    nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                    nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                    var message = new HttpRequestMessage(HttpMethod.Post, API) { Content = new FormUrlEncodedContent(nvc) };
                    var result = client.SendAsync(message).Result;
                    var resultJson = result.Content.ReadAsStringAsync().Result;
                    JObject jo = (JObject)JsonConvert.DeserializeObject(resultJson);
                    if (jo["code"].ToString() == "0")
                    {
                        string helperName = jo["data"]["result"]["groupGuestInfo"]["masterUserName"].ToString();
                        // 助力
                        API = "https://api.m.jd.com/client.action?functionId=nian_pk_assistGroup";
                        string timeStamp = Utils.GetTimeStampLong();
                        string sessionC = Utils.GetTimeStampSuperLong();
                        string randSign = Utils.GetRandomString(40, false, false, true, false, "");
                        string randStr = Utils.GetRandomString(10, true, true, true, false, "");
                        string randIntStr = Utils.GetRandomString(6, true, false, false, false, "");
                        string format = "{{\"confirmFlag\":1,\"ss\":\"{{\\\"extraData\\\":{{\\\"is_trust\\\":true,\\\"sign\\\":\\\"{0}\\\",\\\"fpb\\\":\\\"\\\",\\\"time\\\":{1},\\\"encrypt\\\":\\\"2\\\",\\\"nonstr\\\":\\\"{2}\\\",\\\"jj\\\":\\\"\\\",\\\"token\\\":\\\"{3}\\\",\\\"cf_v\\\":\\\"1.0.1\\\",\\\"client_version\\\":\\\"2.2.1\\\",\\\"call_stack\\\":\\\"{4}\\\",\\\"session_c\\\":\\\"{5}\\\",\\\"buttonid\\\":\\\"\\\",\\\"sceneid\\\":\\\"\\\"}},\\\"secretp\\\":\\\"{6}\\\",\\\"random\\\":\\\"{7}\\\"}}\",\"inviteId\":\"{8}\"}}";
                        string postBody = string.Format(format, randSign, timeStamp, randStr, RandomToken, RandomCallStack, sessionC, SecretP, randIntStr, inviteCodes[i]);
                        nvc = new List<KeyValuePair<string, string>>();
                        nvc.Add(new KeyValuePair<string, string>("functionId", "nian_pk_assistGroup"));
                        nvc.Add(new KeyValuePair<string, string>("body", postBody));
                        nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                        nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                        message = new HttpRequestMessage(HttpMethod.Post, API) { Content = new FormUrlEncodedContent(nvc) };
                        result = client.SendAsync(message).Result;
                        resultJson = result.Content.ReadAsStringAsync().Result;
                        jo = (JObject)JsonConvert.DeserializeObject(resultJson);
                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            string retMsg = jo["data"]["bizMsg"].ToString();
                            if (retMsg.Trim() == "success")
                                retMsg = string.Format("助力成功，被助力ID：{0}，助力ID：{1}", helperName, UserSecretName);
                            else
                                retMsg += string.Format("，被助力ID：{0}，助力ID：{1}", helperName, UserSecretName);
                            UpdateTextToForm(System.Environment.NewLine + string.Format("{0} [PK好友助力]{1} <{2}/{3}>", DateTime.Now.ToString("HH:mm:ss"), retMsg, i + 1, inviteCodes.Length));
                        }
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        /// <summary>
        /// 特殊活动互助
        /// </summary>
        /// <param name="inviteCodeRaw"></param>
        private void DoSpecialHelp(string inviteCodeRaw)
        {
            Task.Run(() =>
            {
                string[] inviteCodes;
                // 判断一下是不是有多个inviteCode
                if (inviteCodeRaw.IndexOf(",") > 0)
                {
                    inviteCodes = inviteCodeRaw.Split(',');
                }
                else
                {
                    inviteCodes = new string[1] { inviteCodeRaw };
                }
                string postAPI = "https://api.m.jd.com/client.action?functionId=nian_getHomeData";
                HttpClient client = Session;
                client.DefaultRequestHeaders.Add("Origin", "https://wbbny.m.jd.com");
                client.DefaultRequestHeaders.Referrer = new Uri("https://wbbny.m.jd.com/babelDiy/Zeus/2cKMj86srRdhgWcKonfExzK4ZMBy/index.html");
                for (int i = 0; i < inviteCodes.Length; i++)
                {
                    var nvc = new List<KeyValuePair<string, string>>();
                    nvc.Add(new KeyValuePair<string, string>("functionId", "nian_getHomeData"));
                    nvc.Add(new KeyValuePair<string, string>("body", string.Format("{{\"inviteId\":\"{0}\"}}", inviteCodes[i])));
                    nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                    nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                    nvc.Add(new KeyValuePair<string, string>("uuid", "865166028601832-00811e4d5577"));
                    var message = new HttpRequestMessage(HttpMethod.Post, postAPI) { Content = new FormUrlEncodedContent(nvc) };
                    var result = client.SendAsync(message).Result;
                    var resultJson = result.Content.ReadAsStringAsync().Result;
                    var jo = (JObject)JsonConvert.DeserializeObject(resultJson);
                    if (jo["data"]["bizCode"].ToString() == "0")
                    {
                        var resultJo = jo["data"]["result"]["homeMainInfo"]["guestInfo"];
                        string itemId = resultJo["itemId"].ToString();
                        string taskId = resultJo["taskId"].ToString();
                        string helperName = resultJo["nickName"].ToString();
                        // 进行助力
                        string timeStamp = Utils.GetTimeStampLong();
                        string sessionC = Utils.GetTimeStampSuperLong();
                        string randSign = Utils.GetRandomString(40, false, false, true, false, "");
                        string randStr = Utils.GetRandomString(10, true, true, true, false, "");
                        string randIntStr = Utils.GetRandomString(6, true, false, false, false, "");
                        postAPI = "https://api.m.jd.com/client.action?functionId=nian_collectSpecialGift";
                        string format = "{{\"taskId\":{0},\"ss\":\"{{\\\"extraData\\\":{{\\\"is_trust\\\":true,\\\"sign\\\":\\\"{1}\\\",\\\"fpb\\\":\\\"\\\",\\\"time\\\":{2},\\\"encrypt\\\":\\\"2\\\",\\\"nonstr\\\":\\\"{3}\\\",\\\"jj\\\":\\\"\\\",\\\"token\\\":\\\"{4}\\\",\\\"cf_v\\\":\\\"1.0.1\\\",\\\"client_version\\\":\\\"2.2.1\\\",\\\"call_stack\\\":\\\"{5}\\\",\\\"session_c\\\":\\\"{6}\\\",\\\"buttonid\\\":\\\"\\\",\\\"sceneid\\\":\\\"\\\"}},\\\"secretp\\\":\\\"{7}\\\",\\\"random\\\":\\\"{8}\\\"}}\",\"itemId\":\"{9}\",\"inviteId\":\"{10}\"}}";
                        string postBody = string.Format(format, taskId, randSign, timeStamp, randStr, RandomToken, RandomCallStack, sessionC, SecretP, randIntStr, itemId, inviteCodes[i]);
                        nvc = new List<KeyValuePair<string, string>>();
                        nvc.Add(new KeyValuePair<string, string>("functionId", "nian_collectSpecialGift"));
                        nvc.Add(new KeyValuePair<string, string>("body", postBody));
                        nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                        nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                        message = new HttpRequestMessage(HttpMethod.Post, postAPI) { Content = new FormUrlEncodedContent(nvc) };
                        result = client.SendAsync(message).Result;
                        resultJson = result.Content.ReadAsStringAsync().Result;
                        resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
                        if (resultJo["code"].ToString() == "0")
                        {
                            string retMsg = resultJo["data"]["bizMsg"].ToString();
                            if (retMsg.Trim() == "success")
                            {
                                retMsg = string.Format("助力成功，被助力ID：{0}，助力ID：{1}", helperName, UserSecretName);
                            }
                            else
                            {
                                retMsg += string.Format("，被助力ID：{0}，助力ID：{1}", helperName, UserSecretName);
                            }
                            UpdateTextToForm(System.Environment.NewLine + string.Format("{0} [好友助力]{1}", DateTime.Now.ToString("HH:mm:ss"), retMsg));
                        }
                        else
                        {
                            UpdateTextToForm(System.Environment.NewLine + string.Format("{0} [好友助力]为ID：{1}助力失败！", DateTime.Now.ToString("HH:mm:ss"), helperName));
                        }
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        /// <summary>
        /// 做神仙学院的任务
        /// </summary>
        /// <param name="task"></param>
        private void DoSXXYTask(SXSYTask task)
        {
            var client = Session;
            client.DefaultRequestHeaders.Referrer = new Uri("https://h5.m.jd.com/babelDiy/Zeus/4XjemYYyPScjmGyjej78M6nsjZvj/index.html");
            for (int i = 0; i < task.ItemToken.Length; i++)
            {
                string format = "https://api.m.jd.com/client.action?functionId=mcxhd_brandcity_doTask&appid=publicUseApi&body=%7B%22itemToken%22:%22{0}%22,%22token%22:%22{1}%22%7D&t={2}&client=wh5&clientVersion=1.0.0";
                string beforeAPI = string.Format(format, task.ItemToken[i], SXSYToken, Utils.GetTimeStampLong());
                var response = Session.GetAsync(beforeAPI).Result;
                var responseStr = response.Content.ReadAsStringAsync().Result;
                var responseJo = (JObject)JsonConvert.DeserializeObject(responseStr);
                if (response.StatusCode == HttpStatusCode.OK && responseJo["retCode"].ToString() == "200")
                {
                    string outputText = string.Format("{0} [{1}] 执行中，等待{2}秒... <{3}/{4}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, task.WaitDuration, i + 1, task.ItemToken.Length); ;
                    UpdateTextToForm(System.Environment.NewLine + outputText);
                    Random rand = new Random();
                    Thread.Sleep((int)((task.WaitDuration + rand.NextDouble() * 2) * 1000));
                    format = "https://api.m.jd.com/client.action?functionId=qryViewkitCallbackResult&body=%7B%22dataSource%22%3A%22newshortAward%22%2C%22method%22%3A%22getTaskAward%22%2C%22reqParams%22%3A%22%7B%5C%22taskToken%5C%22%3A%5C%22{0}%5C%22%7D%22%7D&client=wh5&clientVersion=1.0.0";
                    string afterAPI = string.Format(format, task.TaskToken[i]);
                    response = Session.GetAsync(afterAPI).Result;
                    responseStr = response.Content.ReadAsStringAsync().Result;
                    responseJo = (JObject)JsonConvert.DeserializeObject(responseStr);
                    if (response.StatusCode == HttpStatusCode.OK && responseJo["code"].ToString() == "0")
                    {
                        string bonus = responseJo["toast"]["subTitle"].ToString();
                        outputText = string.Format("{0} [{1}] {2} <{3}/{4}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, bonus, i + 1, task.ItemToken.Length);
                        UpdateTextToForm(System.Environment.NewLine + outputText);
                    }
                    else
                    {
                        outputText = string.Format("{0} [{1}] 任务执行失败，稍后重试...  <{2}/{3}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, i + 1, task.ItemToken.Length);
                        UpdateTextToForm(System.Environment.NewLine + outputText);
                    }
                }
                else
                {
                    string outputText = string.Format("{0} [{1}] 任务申请失败，稍后重试... <{2}/{3}>", DateTime.Now.ToString("HH:mm:ss"), task.Name, i + 1, task.ItemToken.Length);
                    UpdateTextToForm(System.Environment.NewLine + outputText);
                    continue;
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// PK登录
        /// </summary>
        private void PKSignin()
        {
            if (CookieContainer == null) return;
            string taskUrl = "https://api.m.jd.com/client.action?functionId=nian_pk_getMsgPopup";
            HttpClient client = Session;
            client.DefaultRequestHeaders.Remove("Origin");
            client.DefaultRequestHeaders.Referrer = new Uri("https://wbbny.m.jd.com/babelDiy/Zeus/2cKMj86srRdhgWcKonfExzK4ZMBy/index.html");
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("functionId", "nian_pk_getMsgPopup"));
            nvc.Add(new KeyValuePair<string, string>("body", "{}"));
            nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
            nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
            var message = new HttpRequestMessage(HttpMethod.Post, taskUrl) { Content = new FormUrlEncodedContent(nvc) };
            var result = client.SendAsync(message).Result;
        }

        /// <summary>
        /// 神仙书院答题
        /// </summary>
        private void DoSXSYAnswerQuestion()
        {
            if (CookieContainer == null) return;
            if (!UserHasMobile) return;
            int syCount = CurrentCoin / 100;
            if (syCount > 15) syCount = 15;
            for (int t = 0; t < syCount; t++)
            {
                string timeStamp = Utils.GetTimeStampLong();
                string outputText = "";
                // 构建问题列表API
                string questionUrl = string.Format("https://api.m.jd.com/client.action?functionId=mcxhd_brandcity_getQuestions&appid=publicUseApi&body=%7B%22lat%22:%22%22,%22lng%22:%22%22,%22token%22:%22{0}%22%7D&t={1}&client=wh5&clientVersion=1.0.0", SXSYToken, timeStamp);
                HttpClient client = Session;
                client.DefaultRequestHeaders.Remove("Origin");
                client.DefaultRequestHeaders.Referrer = new Uri("https://h5.m.jd.com/babelDiy/Zeus/4XjemYYyPScjmGyjej78M6nsjZvj/index.html");
                var result = client.GetAsync(questionUrl).Result;
                var responseStr = result.Content.ReadAsStringAsync().Result;
                var responseJo = (JObject)JsonConvert.DeserializeObject(responseStr);
                if (responseJo["retCode"].ToString() == "200")
                {
                    outputText = string.Format("{0} [神仙书院答题] 获取列表成功，正在进行第{1}轮答题...", DateTime.Now.ToString("HH:mm:ss"), t + 1);
                    UpdateTextToForm(Environment.NewLine + outputText);
                    // 读取所有问题
                    var questionsJo = responseJo["result"]["questionList"] as JArray;
                    int questionCount = questionsJo.Count;
                    for (int i = 0; i < questionCount; i++)
                    {
                        var jo = questionsJo[i];
                        string id = jo["questionId"].ToString();
                        string token = jo["questionToken"].ToString();
                        var options = jo["options"].AsEnumerable().Select(s => s["optionId"].ToString()).ToArray();
                        string candiOption = options[0];
                        // 查询问题答案
                        //result = client.GetAsync("https://api.2610086.com/jd.question?questionId=" + id).Result;
                        //responseStr = result.Content.ReadAsStringAsync().Result;
                        //responseJo = (JObject)JsonConvert.DeserializeObject(responseStr);
                        //if (responseJo["code"].ToString() == "200")
                        //{
                        //    // 如果得到了答案，那用答案里的
                        //    candiOption = responseJo["data"]["optionId"].ToString();
                        //}

                        // 查找本地题库
                        var match = SXSYTK.FirstOrDefault(s => s["questionId"].ToString() == id);
                        if (match != null)
                        {
                            var correctAns = (JObject)JsonConvert.DeserializeObject(match["correct"].ToString());
                            candiOption = correctAns["optionId"].ToString();
                        }
                        var rand = new Random();
                        int randInt = rand.Next(1, 3);
                        string answerUrl = string.Format("https://api.m.jd.com/client.action?functionId=mcxhd_brandcity_answerQuestion&appid=publicUseApi&body=%7B%22token%22:%22{0}%22,%22costTime%22:{1},%22questionToken%22:%22{2}%22,%22optionId%22:%22{3}%22%7D&t={4}&client=wh5&clientVersion=1.0.0", SXSYToken, randInt, token, candiOption, timeStamp);
                        result = client.GetAsync(answerUrl).Result;
                        responseStr = result.Content.ReadAsStringAsync().Result;
                        responseJo = (JObject)JsonConvert.DeserializeObject(responseStr);
                        if (responseJo["retCode"].ToString() == "200" && responseJo["result"]["isCorrect"].ToString() == "1")
                        {
                            string score = responseJo["result"]["score"].ToString();
                            outputText = string.Format("{0} [神仙书院答题] 第{1}题答题成功，获得{2}积分", DateTime.Now.ToString("HH:mm:ss"), i + 1, score);
                            UpdateTextToForm(Environment.NewLine + outputText);
                        }
                        else
                        {
                            outputText = string.Format("{0} [神仙书院答题] 第{1}题答题失败...", DateTime.Now.ToString("HH:mm:ss"), i + 1);
                            UpdateTextToForm(Environment.NewLine + outputText);
                        }
                        Thread.Sleep(randInt * 1000);
                    }
                }
                Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// PK偷红包
        /// </summary>
        private void DoPKStealRedPack()
        {
            Task.Run(() =>
            {
                const string infoAPI = "https://api.m.jd.com/client.action?functionId=nian_pk_getStealForms";
                const string stealAPI = "https://api.m.jd.com/client.action?functionId=nian_pk_doSteal";
                string outputText;
                HttpClient client = Session;
                client.DefaultRequestHeaders.Remove("Origin");
                client.DefaultRequestHeaders.Referrer = new Uri("https://wbbny.m.jd.com/babelDiy/Zeus/2cKMj86srRdhgWcKonfExzK4ZMBy/index.html");
                var nvc = new List<KeyValuePair<string, string>>();
                nvc.Add(new KeyValuePair<string, string>("functionId", "nian_pk_getHomeData"));
                nvc.Add(new KeyValuePair<string, string>("body", "{}"));
                nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                var message = new HttpRequestMessage(HttpMethod.Post, infoAPI) { Content = new FormUrlEncodedContent(nvc) };
                var result = client.SendAsync(message).Result;
                var resultJson = result.Content.ReadAsStringAsync().Result;
                var resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
                if (resultJo["code"].ToString() == "0")
                {
                    // 获取可以抢的红包
                    var groupJo = resultJo["data"]["result"]["stealGroups"] as JArray;
                    string[] stealCodes = groupJo.AsEnumerable().Select(d => d["id"].ToString()).ToArray();
                    for (int i = 0; i < stealCodes.Length; i++)
                    {
                        // 偷红包
                        outputText = string.Format("{0} [PK抢红包] 正在抢红包...<{1}/{2}>", DateTime.Now.ToString("HH:mm:ss"), i + 1, stealCodes.Length);
                        UpdateTextToForm(System.Environment.NewLine + outputText);
                        string timeStamp = Utils.GetTimeStampLong();
                        string sessionC = Utils.GetTimeStampSuperLong();
                        string randSign = Utils.GetRandomString(40, false, false, true, false, "");
                        string randStr = Utils.GetRandomString(10, true, true, true, false, "");
                        string randIntStr = Utils.GetRandomString(6, true, false, false, false, "");
                        string format = "{{\"ss\":\"{{\\\"extraData\\\":{{\\\"is_trust\\\":true,\\\"sign\\\":\\\"{0}\\\",\\\"fpb\\\":\\\"\\\",\\\"time\\\":{1},\\\"encrypt\\\":\\\"2\\\",\\\"nonstr\\\":\\\"{2}\\\",\\\"jj\\\":\\\"\\\",\\\"token\\\":\\\"{3}\\\",\\\"cf_v\\\":\\\"1.0.1\\\",\\\"client_version\\\":\\\"2.2.1\\\",\\\"call_stack\\\":\\\"{4}\\\",\\\"session_c\\\":\\\"{5}\\\",\\\"buttonid\\\":\\\"\\\",\\\"sceneid\\\":\\\"\\\"}},\\\"secretp\\\":\\\"{6}\\\",\\\"random\\\":\\\"{7}\\\"}}\",\"stealId\":\"{8}\"}}";
                        string postBody = string.Format(format, randSign, timeStamp, randStr, RandomToken, RandomCallStack, sessionC, SecretP, randIntStr, stealCodes[i]);
                        nvc = new List<KeyValuePair<string, string>>();
                        nvc.Add(new KeyValuePair<string, string>("functionId", "nian_pk_doSteal"));
                        nvc.Add(new KeyValuePair<string, string>("body", postBody));
                        nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                        nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                        message = new HttpRequestMessage(HttpMethod.Post, stealAPI) { Content = new FormUrlEncodedContent(nvc) };
                        result = client.SendAsync(message).Result;
                        resultJson = result.Content.ReadAsStringAsync().Result;
                        resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
                        if (resultJo["code"].ToString() == "0")
                        {
                            outputText = string.Format("{0} [PK抢红包]{1} <{2}/{3}>", DateTime.Now.ToString("HH:mm:ss"), resultJo["data"]["bizMsg"], i + 1, stealCodes.Length);
                            UpdateTextToForm(System.Environment.NewLine + outputText);
                        }
                        else
                        {
                            outputText = string.Format("{0} [PK抢红包]调用接口失败 <{1}/{2}>", DateTime.Now.ToString("HH:mm:ss"), i + 1, stealCodes.Length);
                            UpdateTextToForm(System.Environment.NewLine + outputText);
                        }
                        Thread.Sleep(3000);
                    }
                }
                else
                {
                    outputText = string.Format("{0} [PK抢红包]无法获取队伍列表", DateTime.Now.ToString("HH:mm:ss"));
                    UpdateTextToForm(System.Environment.NewLine + outputText);
                }
            });
        }


        /// <summary>
        /// 停止任务按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_StopTask_Click(object sender, EventArgs e)
        {
            Btn_StopTask.Enabled = false;
            if (CTS != null)
            {
                CTS.Cancel();
            }
        }

        /// <summary>
        /// 窗口关闭时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 保存CookieContainer
            if (CookieContainer != null)
            {
                string filePath = System.IO.Path.Combine(Application.StartupPath, "user.dat");
                Utils.WriteCookiesToDisk(filePath, CookieContainer);
            }
        }

        private void Lbl_Level_Click(object sender, EventArgs e)
        {
            // 升级
            DoUpdate();
        }

        private void Lbl_Score_Click(object sender, EventArgs e)
        {
            // 收爆竹
            DoCollect();
        }

        private void Chk_AutoCollect_Click(object sender, EventArgs e)
        {
            if (Chk_AutoCollect.Checked)
            {
                DoCollect();
                TM_Collect.Enabled = true;
                AutoCollectCountDown = 3600;
            }
            else
            {
                TM_Collect.Enabled = false;
            }
        }

        private void Chk_AutoDoTasks_Click(object sender, EventArgs e)
        {
            if (Chk_AutoDoTasks.Checked)
            {
                TM_DoTasks.Enabled = true;
                var now = DateTime.Now;
                AutoDoTaskCountDown = (int)((new DateTime(now.Year, now.Month, now.Day, 0, 1, 0).AddDays(1) - now).TotalSeconds);
            }
            else
            {
                TM_DoTasks.Enabled = false;
            }
        }

        private void TM_Collect_Tick(object sender, EventArgs e)
        {
            AutoCollectCountDown -= 1;
            if (AutoCollectCountDown == 0)
            {
                DoCollect();
                AutoCollectCountDown = 3600;
            }
        }

        private void TM_DoTasks_Tick(object sender, EventArgs e)
        {
            AutoDoTaskCountDown -= 1;
            if (AutoDoTaskCountDown == 0)
            {
                Btn_StartTask_Click(null, null);
                var now = DateTime.Now;
                AutoDoTaskCountDown = (int)((new DateTime(now.Year, now.Month, now.Day, 0, 1, 0).AddDays(1) - now).TotalSeconds);
            }
        }

        private void Btn_CopyMine_Click(object sender, EventArgs e)
        {
            switch (Cbo_HelpType.SelectedIndex)
            {
                case 0:
                    // PK
                    string taskUrl = "https://api.m.jd.com/client.action?functionId=nian_pk_getHomeData";
                    HttpClient client = Session;
                    client.DefaultRequestHeaders.Remove("Origin");
                    client.DefaultRequestHeaders.Referrer = new Uri("https://wbbny.m.jd.com/babelDiy/Zeus/2cKMj86srRdhgWcKonfExzK4ZMBy/index.html");
                    var nvc = new List<KeyValuePair<string, string>>();
                    nvc.Add(new KeyValuePair<string, string>("functionId", "nian_pk_getHomeData"));
                    nvc.Add(new KeyValuePair<string, string>("body", "{}"));
                    nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                    nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                    var message = new HttpRequestMessage(HttpMethod.Post, taskUrl) { Content = new FormUrlEncodedContent(nvc) };
                    var result = client.SendAsync(message).Result;
                    var resultJson = result.Content.ReadAsStringAsync().Result;
                    JObject jo = (JObject)JsonConvert.DeserializeObject(resultJson);
                    if (jo["data"]["bizCode"].ToString() == "0")
                    {
                        // 读取邀请ID
                        var resultJO = jo["data"]["result"];
                        PKInviteID = resultJO["groupInfo"]["groupAssistInviteId"].ToString();
                    }
                    Txt_Output.AppendText(System.Environment.NewLine + string.Format("{0} 助力代码：{1}，已经复制到剪贴板", DateTime.Now.ToString("HH:mm:ss"), PKInviteID));
                    Clipboard.SetText(PKInviteID);
                    break;
                case 1:
                    // 助力
                    taskUrl = "https://api.m.jd.com/client.action?functionId=nian_getTaskDetail";
                    client = Session;
                    client.DefaultRequestHeaders.Remove("Origin");
                    client.DefaultRequestHeaders.Referrer = new Uri("https://wbbny.m.jd.com/babelDiy/Zeus/2cKMj86srRdhgWcKonfExzK4ZMBy/index.html");
                    nvc = new List<KeyValuePair<string, string>>();
                    nvc.Add(new KeyValuePair<string, string>("functionId", "nian_getTaskDetail"));
                    nvc.Add(new KeyValuePair<string, string>("body", "{}"));
                    nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                    nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                    message = new HttpRequestMessage(HttpMethod.Post, taskUrl) { Content = new FormUrlEncodedContent(nvc) };
                    result = client.SendAsync(message).Result;
                    resultJson = result.Content.ReadAsStringAsync().Result;
                    jo = (JObject)JsonConvert.DeserializeObject(resultJson);
                    if (jo["data"]["success"].ToString().ToLower().Trim() == "true")
                    {
                        // 读取邀请ID
                        var resultJO = jo["data"]["result"];
                        InviteID = resultJO["inviteId"].ToString();
                    }
                    Txt_Output.AppendText(System.Environment.NewLine + string.Format("{0} 助力代码：{1}，已经复制到剪贴板", DateTime.Now.ToString("HH:mm:ss"), InviteID));
                    Clipboard.SetText(InviteID);
                    break;
                case 2:
                    // 书院
                    if (string.IsNullOrWhiteSpace(Txt_Lat.Text)) return;
                    if (string.IsNullOrWhiteSpace(Txt_Lat.Text)) return;
                    if (!UserHasMobile) return;
                    string lat = Txt_Lat.Text.Trim();
                    string lon = Txt_Lon.Text.Trim();
                    string format = "https://api.m.jd.com/client.action?functionId=mcxhd_brandcity_taskList&appid=publicUseApi&body=%7B%22lat%22:{0},%22lng%22:{1},%22token%22:%22{2}%22%7D&t={3}&client=wh5&clientVersion=1.0.0";
                    taskUrl = string.Format(format, lat, lon, SXSYToken, Utils.GetTimeStampLong());
                    client = Session;
                    client.DefaultRequestHeaders.Remove("Origin");
                    client.DefaultRequestHeaders.Referrer = new Uri("https://h5.m.jd.com/babelDiy/Zeus/4XjemYYyPScjmGyjej78M6nsjZvj/index.html");
                    message = new HttpRequestMessage(HttpMethod.Post, taskUrl);
                    result = client.SendAsync(message).Result;
                    resultJson = result.Content.ReadAsStringAsync().Result;
                    jo = (JObject)JsonConvert.DeserializeObject(resultJson);
                    if (jo["retCode"].ToString() == "200")
                    {
                        // 读取任务信息
                        var resultJO = jo["result"];
                        var taskVos = resultJO["tasks"] as JArray;
                        foreach (JObject info in taskVos)
                        {
                            // 取得任务信息
                            string taskName = info["taskName"].ToString();
                            if (taskName != "邀请朋友一起玩") continue;
                            JArray subItems = info["subItem"] as JArray;
                            foreach (var item in subItems)
                            {
                                SXSYInviteID = item["itemToken"].ToString();
                            }
                            Txt_Output.AppendText(System.Environment.NewLine + string.Format("{0} 助力代码：{1}，已经复制到剪贴板", DateTime.Now.ToString("HH:mm:ss"), SXSYInviteID));
                            Clipboard.SetText(SXSYInviteID);
                        }
                    }
                    break;
                case 3:
                    // 特殊任务
                    taskUrl = "https://api.m.jd.com/client.action?functionId=nian_getSpecialGiftDetail";
                    client = Session;
                    client.DefaultRequestHeaders.Remove("Origin");
                    client.DefaultRequestHeaders.Referrer = new Uri("https://wbbny.m.jd.com/babelDiy/Zeus/2cKMj86srRdhgWcKonfExzK4ZMBy/index.html");
                    nvc = new List<KeyValuePair<string, string>>();
                    nvc.Add(new KeyValuePair<string, string>("functionId", "nian_getSpecialGiftDetail"));
                    nvc.Add(new KeyValuePair<string, string>("body", "{}"));
                    nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                    nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                    message = new HttpRequestMessage(HttpMethod.Post, taskUrl) { Content = new FormUrlEncodedContent(nvc) };
                    result = client.SendAsync(message).Result;
                    resultJson = result.Content.ReadAsStringAsync().Result;
                    jo = (JObject)JsonConvert.DeserializeObject(resultJson);
                    if (jo["code"].ToString() == "0")
                    {
                        // 读取邀请ID
                        var resultJO = jo["data"]["result"];
                        SpecialInviteID = resultJO["inviteId"].ToString();
                    }
                    Txt_Output.AppendText(System.Environment.NewLine + string.Format("{0} 助力代码：{1}，已经复制到剪贴板", DateTime.Now.ToString("HH:mm:ss"), SpecialInviteID));
                    Clipboard.SetText(SpecialInviteID);
                    break;
            }
        }

        private void Btn_DoHelp_Click(object sender, EventArgs e)
        {
            string code = Txt_InviteCode.Text.Trim();
            if (string.IsNullOrWhiteSpace(code))
            {
                Txt_Output.AppendText(System.Environment.NewLine + string.Format("{0} 助力代码不能为空", DateTime.Now.ToString("HH:mm:ss")));
                return;
            }
            switch (Cbo_HelpType.SelectedIndex)
            {
                case 0:
                    // PK
                    DoPKHelp(code);
                    break;
                case 1:
                    // 年兽
                    DoHelp(code);
                    break;
                case 2:
                    // 书院
                    if (UserHasMobile)
                        DoSXXYHelp(code);
                    break;
                case 3:
                    // 特殊
                    DoSpecialHelp(code);
                    break;
            }
        }

        private void Btn_HelpAuthor_Click(object sender, EventArgs e)
        {
            switch (Cbo_HelpType.SelectedIndex)
            {
                case 0:
                    // PK
                    string code = "IgNWdiLGaPadvnCTW1LyhvJd2kn7deeda6yZy-hjdzc_sE7Sut71b4mvan4";
                    DoPKHelp(code);
                    break;
                case 1:
                    // 年兽
                    code = "cgxZaDX8fumNsWeaYkD8hdUd5OaVNjqpWLBgZq8DF4Asv1N7rZ4e";
                    DoHelp(code);
                    break;
                case 2:
                    // 书院
                    code = "39xIs4YwE5Z7CPQQ2bc4rvuRg1JcqyoSZmHNyFv5KgbJBsnBcE1FeWtYaGh4E";
                    if (UserHasMobile)
                        DoSXXYHelp(code);
                    break;
            }
        }

        private void TM_PreventSleep_Tick(object sender, EventArgs e)
        {
            SystemSleep.ResetIdle();
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                this.notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                notifyIcon1.Visible = false;
                this.ShowInTaskbar = true;
            }

        }

        private void Chk_AutoRedPack_Click(object sender, EventArgs e)
        {
            if (Chk_AutoRedPack.Checked)
            {
                var now = DateTime.Now;
                AutoStealRedPackCountDown = (int)((new DateTime(now.Year, now.Month, now.Day, 20, 0, 0) - now).TotalSeconds);
                TM_RedPacket.Enabled = true;
            }
            else
            {
                TM_RedPacket.Enabled = false;
            }
        }

        private void TM_RedPacket_Tick(object sender, EventArgs e)
        {
            AutoStealRedPackCountDown--;
            if (AutoStealRedPackCountDown <= 0)
            {
                DoPKStealRedPack();
                TM_RedPacket.Enabled = false;
            }
        }
    }
}

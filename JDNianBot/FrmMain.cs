using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JDNianBot
{
    public partial class FrmMain : Form
    {
        /// <summary>
        /// 用户Cookie容器
        /// </summary>
        private CookieContainer CookieContainer { get; set; } = null;

        /// <summary>
        /// 用户名
        /// </summary>
        private string UserName { get; set; } = "正在加载中...";

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
        /// 任务凭证
        /// </summary>
        private string SecretP { get; set; } = "";

        /// <summary>
        /// 邀请ID
        /// </summary>
        private string InviteID { get; set; } = "";


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
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        private async void UpdateInfo()
        {
            if (CookieContainer == null) return;
            string postAPI = "https://api.m.jd.com/client.action?functionId=nian_getHomeData";
            var collections = CookieContainer.GetCookies(new Uri(postAPI));
            foreach (Cookie cookie in collections)
            {
                if (cookie.Name == "pt_pin")
                {
                    UserName = cookie.Value;
                    break;
                }
            }
            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = CookieContainer,
                UseCookies = true
            };
            string outputText;
            using (HttpClient client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("User-Agent", "jdapp;android;8.5.12;5.1.1;865166028601832-00811e4d5577;network/wifi;model/LYA-AL10;addressid/0;aid/2567026c65094d3e;oaid/;osVer/22;appBuild/73078;partner/jdtopc;Mozilla/5.0 (Linux; Android 5.1.1; LYA-AL10 Build/LYZ28N; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/52.0.2743.100 Mobile Safari/537.36");
                client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
                client.DefaultRequestHeaders.Add("Origin", "https://wbbny.m.jd.com");
                client.DefaultRequestHeaders.Referrer = new Uri("https://wbbny.m.jd.com/babelDiy/Zeus/2cKMj86srRdhgWcKonfExzK4ZMBy/index.html");
                var nvc = new List<KeyValuePair<string, string>>();
                nvc.Add(new KeyValuePair<string, string>("functionId", "nian_getHomeData"));
                nvc.Add(new KeyValuePair<string, string>("body", "{}"));
                nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                nvc.Add(new KeyValuePair<string, string>("uuid", "865166028601832-00811e4d5577"));
                var message = new HttpRequestMessage(HttpMethod.Post, postAPI) { Content = new FormUrlEncodedContent(nvc) };
                var result = await client.SendAsync(message);
                var resultJson = await result.Content.ReadAsStringAsync();
                JObject jo = (JObject)JsonConvert.DeserializeObject(resultJson);
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
                    outputText = string.Format("{0} [登录账号]登录失败，请重新登录！", DateTime.Now.ToString("hh:mm:ss"));
                    Txt_Output.AppendText(Environment.NewLine + outputText);
                    return;
                }
            }
            outputText = string.Format("{0} [登录账号]登录成功，用户名为：{1}", DateTime.Now.ToString("hh:mm:ss"), UserName);
            Txt_Output.AppendText(Environment.NewLine + outputText);
            // 更新信息到控件上
            Lbl_CurrentAccount.Text = UserName;
            Lbl_CurrentRedPack.Text = CurrentRedPack.ToString();
            Lbl_Level.Text = string.Format("{0}（{1}%）", CurrentLevel, (CurrentLevel / MaxLevel * 100).ToString("F2"));
            Lbl_Score.Text = CurrentScore.ToString();
            Lbl_NextScore.Text = NextScore.ToString();
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
                UpdateInfo();
            }
        }

        /// <summary>
        /// 刷新按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Refresh_Click(object sender, EventArgs e)
        {
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
            Task.Run(() => { GetAndDoTasks(); }).Start();
        }

        private delegate void delegateGetAndDoTasks();
        /// <summary>
        /// 执行任务
        /// </summary>
        private void GetAndDoTasks()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new delegateGetAndDoTasks(GetAndDoTasks));
            }
            else
            {
                // 获取所有任务
                List<JDTask> tasks = new List<JDTask>();
                const string taskUrl = "https://api.m.jd.com/client.action?functionId=nian_getTaskDetail";
                const string feedUrl = "https://api.m.jd.com/client.action?functionId=nian_getFeedDetail";
                const string shopUrl = "https://api.m.jd.com/client.action?functionId=qryCompositeMaterials";
                const string shopLotteryUrl = "https://api.m.jd.com/client.action?functionId=nian_shopLotteryInfo";
                HttpClientHandler handler = new HttpClientHandler
                {
                    CookieContainer = CookieContainer,
                    UseCookies = true
                };
                using (HttpClient client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "jdapp;android;8.5.12;5.1.1;865166028601832-00811e4d5577;network/wifi;model/LYA-AL10;addressid/0;aid/2567026c65094d3e;oaid/;osVer/22;appBuild/73078;partner/jdtopc;Mozilla/5.0 (Linux; Android 5.1.1; LYA-AL10 Build/LYZ28N; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/52.0.2743.100 Mobile Safari/537.36");
                    client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
                    client.DefaultRequestHeaders.Add("Origin", "https://wbbny.m.jd.com");
                    client.DefaultRequestHeaders.Referrer = new Uri("https://wbbny.m.jd.com/babelDiy/Zeus/2cKMj86srRdhgWcKonfExzK4ZMBy/index.html");
                    var nvc = new List<KeyValuePair<string, string>>();
                    nvc.Add(new KeyValuePair<string, string>("functionId", "nian_getTaskDetail"));
                    nvc.Add(new KeyValuePair<string, string>("body", ""));
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
                                int taskID = feedId;
                                string taskName = task["taskName"].ToString();
                                JDTaskType taskType = JDTaskType.Checkin;
                                int itemCount = 0;
                                string[] itemID = new string[maxTimes];
                                foreach (var item in itemList)
                                {
                                    // 如果说需要加购的物品已经够多了，那就停止增加任务
                                    itemCount += 1;
                                    if (itemCount > maxTimes) break;
                                    itemID[itemCount - 1] = item["itemID"].ToString();
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
                    if (jo["data"]["sucess"].ToString().ToLower().Trim() == "true")
                    {
                        var shopList = jo["data"]["domainShopData2"]["list"] as JArray;
                        string[] itemId = shopList.Select(s => s["link"].ToString()).ToArray();
                        JDTask task = new JDTask
                        {
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
                    if (jo["data"]["sucess"].ToString().ToLower().Trim() == "true")
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
                            if (jo["data"]["sucess"].ToString().ToLower().Trim() == "true")
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
                }
                // 执行所有任务
                foreach (var task in tasks)
                {
                    DoTask(task);
                }
            }
        }

        private void DoArGame()
        {
            // TODO: AR游戏逻辑
        }

        private async void DoTask(JDTask task)
        {
            if (CookieContainer == null) return;
            const string postAPI = "https://api.m.jd.com/client.action?functionId=nian_collectScore";
            const string shopSignAPI_Read = "https://api.m.jd.com/client.action?functionId=nian_shopSignInRead";
            const string shopSignAPI_Write = "https://api.m.jd.com/client.action?functionId=nian_shopSignInWrite";
            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = CookieContainer,
                UseCookies = true
            };
            using (HttpClient client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("User-Agent", "jdapp;android;8.5.12;5.1.1;865166028601832-00811e4d5577;network/wifi;model/LYA-AL10;addressid/0;aid/2567026c65094d3e;oaid/;osVer/22;appBuild/73078;partner/jdtopc;Mozilla/5.0 (Linux; Android 5.1.1; LYA-AL10 Build/LYZ28N; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/52.0.2743.100 Mobile Safari/537.36");
                client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
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
                        case JDTaskType.Checkin:
                            format = "{{\"taskId\":{0},\"ss\":\"{{\\\"extraData\\\":{{\\\"is_trust\\\":true,\\\"sign\\\":\\\"{1}\\\",\\\"fpb\\\":\\\"\\\",\\\"time\\\":{2},\\\"encrypt\\\":\\\"2\\\",\\\"nonstr\\\":\\\"{3}\\\",\\\"jj\\\":\\\"\\\",\\\"token\\\":\\\"\\\",\\\"cf_v\\\":\\\"1.0.0\\\",\\\"client_version\\\":\\\"2.2.1\\\",\\\"call_stack\\\":\\\"d3bd90bf525418114cceb94ba45e3ab8\\\",\\\"session_c\\\":\\\"{4}\\\",\\\"buttonid\\\":\\\"\\\",\\\"sceneid\\\":\\\"\\\"}},\\\"secretp\\\":\\\"{5}\\\",\\\"random\\\":\\\"{6}\\\"}}\",\"itemId\":\"{7}\"}}";
                            postBody = string.Format(format, task.TaskID, randSign, timeStamp, randStr, sessionC, SecretP, randIntStr, task.ItemID[i]);
                            targetUrl = postAPI;
                            break;
                        case JDTaskType.Browse:
                            format = "{{\"taskId\":{0},\"ss\":\"{{\\\"extraData\\\":{{\\\"is_trust\\\":true,\\\"sign\\\":\\\"{1}\\\",\\\"fpb\\\":\\\"\\\",\\\"time\\\":{2},\\\"encrypt\\\":\\\"2\\\",\\\"nonstr\\\":\\\"{3}\\\",\\\"jj\\\":\\\"\\\",\\\"token\\\":\\\"\\\",\\\"cf_v\\\":\\\"1.0.0\\\",\\\"client_version\\\":\\\"2.2.1\\\",\\\"call_stack\\\":\\\"d3bd90bf525418114cceb94ba45e3ab8\\\",\\\"session_c\\\":\\\"{4}\\\",\\\"buttonid\\\":\\\"\\\",\\\"sceneid\\\":\\\"\\\"}},\\\"secretp\\\":\\\"{5}\\\",\\\"random\\\":\\\"{6}\\\"}}\",\"itemId\":\"{7}\",\"actionType\":1}}";
                            postBody = string.Format(format, task.TaskID, randSign, timeStamp, randStr, sessionC, SecretP, randIntStr, task.ItemID[i]);
                            targetUrl = postAPI;
                            break;
                        case JDTaskType.ShopLottery:
                            format = "{{\"taskId\":{0},\"ss\":\"{{\\\"extraData\\\":{{\\\"is_trust\\\":true,\\\"sign\\\":\\\"{1}\\\",\\\"fpb\\\":\\\"\\\",\\\"time\\\":{2},\\\"encrypt\\\":\\\"2\\\",\\\"nonstr\\\":\\\"{3}\\\",\\\"jj\\\":\\\"\\\",\\\"token\\\":\\\"\\\",\\\"cf_v\\\":\\\"1.0.0\\\",\\\"client_version\\\":\\\"2.2.1\\\",\\\"call_stack\\\":\\\"d3bd90bf525418114cceb94ba45e3ab8\\\",\\\"session_c\\\":\\\"{4}\\\",\\\"buttonid\\\":\\\"\\\",\\\"sceneid\\\":\\\"\\\"}},\\\"secretp\\\":\\\"{5}\\\",\\\"random\\\":\\\"{6}\\\"}}\",\"itemId\":\"{7}\",\"shopSign\":\"{8}\"}}";
                            postBody = string.Format(format, task.TaskID, randSign, timeStamp, randStr, sessionC, SecretP, randIntStr, task.ItemID[i], task.ShopSign);
                            targetUrl = postAPI;
                            break;
                        case JDTaskType.ShopSignin:
                            // TODO: ShopSiginInRead逻辑
                            format = "{{\"shopSign\":{0},\"ss\":\"{{\\\"extraData\\\":{{\\\"is_trust\\\":true,\\\"sign\\\":\\\"{1}\\\",\\\"fpb\\\":\\\"\\\",\\\"time\\\":{2},\\\"encrypt\\\":\\\"2\\\",\\\"nonstr\\\":\\\"{3}\\\",\\\"jj\\\":\\\"\\\",\\\"token\\\":\\\"\\\",\\\"cf_v\\\":\\\"1.0.0\\\",\\\"client_version\\\":\\\"2.2.1\",\\\"call_stack\\\":\\\"d3bd90bf525418114cceb94ba45e3ab8\\\",\\\"session_c\\\":\\\"{4}\\\",\\\"buttonid\\\":\\\"\\\",\\\"sceneid\\\":\\\"\\\"}},\\\"secretp\\\":\\\"{5}\\\",\\\"random\\\":\\\"{6}\\\"}}\"}}";
                            postBody = string.Format(format, task.ItemID[i], randSign, timeStamp, randStr, sessionC, SecretP, randIntStr);
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
                    var result = await client.SendAsync(message);
                    var resultJson = await result.Content.ReadAsStringAsync();
                    JObject resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
                    // 如果调用成功，进入下一步，获取奖励，否则的话稍后再试
                    if (resultJo["data"]["success"].ToString().ToLower().Trim() == "true")
                    {
                        if (task.Type == JDTaskType.Browse)
                        {
                            string outputText = string.Format("{0} [{1}]执行中...{2} <{3}/{4}>", DateTime.Now.ToString("hh:mm:ss"), task.Name, task.WaitDuration > 0 ? string.Format("等待{0}秒...", task.WaitDuration) : "", i + 1, task.ItemID.Length);
                            Txt_Output.AppendText(Environment.NewLine + outputText);
                            Random rand = new Random();
                            await Task.Delay((int)((task.WaitDuration + rand.NextDouble() * 2) * 1000));
                            format = "{{\"taskId\":{0},\"ss\":\"{{\"extraData\":{{\"is_trust\":true,\"sign\":\"{1}\",\"fpb\":\"\",\"time\":{2},\"encrypt\":\"2\",\"nonstr\":\"{3}\",\"jj\":\"\",\"token\":\"\",\"cf_v\":\"1.0.0\",\"client_version\":\"2.2.1\",\"call_stack\":\"d3bd90bf525418114cceb94ba45e3ab8\",\"session_c\":\"{4}\",\"buttonid\":\"\",\"sceneid\":\"\"}},\"secretp\":\"{5}\",\"random\":\"{6}\"}}\",\"itemId\":\"{7}\",\"actionType\":0}}";
                            postBody = string.Format(format, task.TaskID, randSign, timeStamp, randStr, sessionC, SecretP, randIntStr, task.ItemID[i]);
                            nvc = new List<KeyValuePair<string, string>>();
                            nvc.Add(new KeyValuePair<string, string>("functionId", "nian_collectScore"));
                            nvc.Add(new KeyValuePair<string, string>("body", postBody));
                            nvc.Add(new KeyValuePair<string, string>("client", "wh5"));
                            nvc.Add(new KeyValuePair<string, string>("clientVersion", "1.0.0"));
                            message = new HttpRequestMessage(HttpMethod.Post, postAPI) { Content = new FormUrlEncodedContent(nvc) };
                            result = await client.SendAsync(message);
                            resultJson = await result.Content.ReadAsStringAsync();
                            resultJo = (JObject)JsonConvert.DeserializeObject(resultJson);
                            if (resultJo["data"]["success"].ToString().ToLower().Trim() == "true")
                            {
                                string taskBonus = resultJo["data"]["result"]["successToast"].ToString();
                                outputText = string.Format("{0} [{1}]{2} <{3}/{4}>", DateTime.Now.ToString("hh:mm:ss"), task.Name, taskBonus, i + 1, task.ItemID.Length);
                                Txt_Output.AppendText(Environment.NewLine + outputText);
                                continue;
                            }
                            else
                            {
                                outputText = string.Format("{0} [{1}]任务执行失败，稍后重试 <{2}/{3}>", DateTime.Now.ToString("hh:mm:ss"), task.Name, i + 1, task.ItemID.Length);
                                Txt_Output.AppendText(Environment.NewLine + outputText);
                                continue;
                            }
                        }
                        else
                        {
                            string outputText = string.Format("{0} [{1}]执行中...<{2}/{3}>", DateTime.Now.ToString("hh:mm:ss"), task.Name, i + 1, task.ItemID.Length);
                            Txt_Output.AppendText(Environment.NewLine + outputText);
                            string taskBonus = resultJo["data"]["result"]["successToast"].ToString();
                            outputText = string.Format("{0} [{1}]{2} <{3}/{4}>", DateTime.Now.ToString("hh:mm:ss"), task.Name, taskBonus, i + 1, task.ItemID.Length);
                            Txt_Output.AppendText(Environment.NewLine + outputText);
                            continue;
                        }
                    }
                    else
                    {
                        int bizCode = Convert.ToInt32(resultJo["data"]["bizCode"].ToString());
                        string bizMessage = resultJo["data"]["bizMsg"].ToString();
                        string outputText = string.Format("{0} [{1}]任务失败，错误代码：{2}，{3} <{4}/{5}>", DateTime.Now.ToString("hh:mm:ss"), task.Name, bizCode, bizMessage, i + 1, task.ItemID.Length);
                        Txt_Output.AppendText(Environment.NewLine + outputText);
                        // TODO: 入会奖励领取失败时尝试入会
                        if (task.Type == JDTaskType.BeMember && bizCode == -1)
                        {

                        }
                        // 今日已达到上限
                        if (bizCode == -2)
                        {
                            break;
                        }
                        continue;
                    }
                    // TODO:店铺抽奖逻辑
                }
            }
        }

        /// <summary>
        /// 停止任务按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_StopTask_Click(object sender, EventArgs e)
        {
            Btn_StopTask.Enabled = false;
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
    }
}

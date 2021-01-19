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
                if (cookie.Name == "pt_pin" || cookie.Name == "pwdt_id")
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
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    // 读取secretp以及任务信息
                    JObject jo = (JObject)JsonConvert.DeserializeObject(resultJson);
                    var homeInfo = jo["data"]["result"]["homeMainInfo"];
                    SecretP = homeInfo["secretp"].ToString();
                    var raiseInfo = homeInfo["raiseInfo"];
                    CurrentLevel = Convert.ToInt32(raiseInfo["scoreLevel"].ToString());
                    MaxLevel = 30;
                    CurrentRedPack = Convert.ToInt32(raiseInfo["redNum"].ToString());
                    CurrentScore = Convert.ToInt32(raiseInfo["remainScore"].ToString());
                    NextScore = Convert.ToInt32(raiseInfo["nextLevelScore"].ToString()) - Convert.ToInt32(raiseInfo["usedScore"].ToString());
                }
            }
            // 更新信息到控件上
            Lbl_CurrentAccount.Text = UserName;
            Lbl_CurrentRedPack.Text = CurrentRedPack.ToString();
            Lbl_Level.Text = string.Format("{0}（{1}%）", CurrentLevel, (CurrentScore / NextScore).ToString("F2"));
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
            Btn_StartTask.Enabled = true;
            
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

            }
        }

        private void DoTask(JDTask task)
        {
            if (CookieContainer == null) return;
            string postAPI = "https://api.m.jd.com/client.action?functionId=nian_ckCollectScore";
            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = CookieContainer,
                UseCookies = true
            };
            using(HttpClient client=new HttpClient(handler))
            {

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

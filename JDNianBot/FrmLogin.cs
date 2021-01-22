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
    public partial class FrmLogin : Form
    {
        /// <summary>
        /// Cookies容器
        /// </summary>
        public CookieContainer CookieContainer { get; set; }

        private HttpClient SessionClient { get; set; }

        private HttpClientHandler SessionHandler { get; set; }

        /// <summary>
        /// 查询标识符
        /// </summary>
        private string SToken { get; set; }

        /// <summary>
        /// 查询标识符
        /// </summary>
        private string Token { get; set; }

        /// <summary>
        /// OKL标识符
        /// </summary>
        private string OKLToken { get; set; }

        public FrmLogin()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体加载后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmLogin_Load(object sender, EventArgs e)
        {
            // 初始化Session
            InitSession();
            // 加载二维码
            LoadQRCode();
        }

        private void InitSession()
        {
            CookieContainer = new CookieContainer();
            SessionHandler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = CookieContainer
            };
            SessionClient = new HttpClient(SessionHandler);
            SessionClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
            SessionClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
        }

        /// <summary>
        /// 加载二维码
        /// </summary>
        private async void LoadQRCode()
        {
            //string timeStamp = Utils.GetTimeStampLong();
            //string qrUrl = "https://qr.m.jd.com/show?appid=133&size=147&t=" + timeStamp;
            //var client = SessionClient;
            //client.DefaultRequestHeaders.Referrer = new Uri("https://passport.jd.com/new/login.aspx");
            //byte[] qrData = await client.GetByteArrayAsync(qrUrl);
            //var cookies = CookieContainer.GetCookies(new Uri(qrUrl));
            //CookieContainer.SetCookies(new Uri("https://qr.m.jd.com"), "QRCodeKey=" + cookies["QRCodeKey"].Value);
            //this.Token = cookies["wlfstk_smdl"].Value;
            //Bitmap qrImage = Utils.ByteToImage(qrData);
            //pictureBox1.Image = qrImage;

            // 获取s_token
            SessionClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            SessionClient.DefaultRequestHeaders.Add("ContentType", "application/x-www-form-urlencoded");
            SessionClient.DefaultRequestHeaders.Referrer = new Uri("https://plogin.m.jd.com/login/login?appid=300&returnurl=https%3A%2F%2Fwq.jd.com%2Fpassport%2FLoginRedirect%3Fstate%3D1101078047599%26returnurl%3Dhttps%253A%252F%252Fhome.m.jd.com%252FmyJd%252Fnewhome.action%253Fsceneval%253D2%2526ufc%253D%2526%252FmyJd%252Fhome.action&source=wq_passport");
            var response = await SessionClient.GetAsync(@"https://plogin.m.jd.com/cgi-bin/mm/new_login_entrance?lang=chs&appid=300&returnurl=https:%2F%2Fwq.jd.com%2Fpassport%2FLoginRedirect%3Fstate%3D1101078047599%26returnurl%3Dhttps%253A%252F%252Fhome.m.jd.com%252FmyJd%252Fnewhome.action%253Fsceneval%253D2%2526ufc%253D%2526%252FmyJd%252Fhome.action&source=wq_passport");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Lbl_QRStatus.Text = "二维码加载失败，请重试或采用Cookie登录";
                return;
            }
            var responseText = await response.Content.ReadAsStringAsync();
            var responseJo = (JObject)JsonConvert.DeserializeObject(responseText);
            SToken = responseJo["s_token"].ToString();
            // 获取token和okl_token
            string tokenAPI = string.Format("https://plogin.m.jd.com/cgi-bin/m/tmauthreflogurl?s_token={0}&v={1}&remember=true", SToken, Utils.GetTimeStampLong());
            var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("lang", "chs"),
                new KeyValuePair<string, string>("appid", "300"),
                new KeyValuePair<string, string>("returnurl", "https%3A%2F%2Fwqlogin2.jd.com%2Fpassport%2FLoginRedirect%3Fstate%3D1100399130787%26returnurl%3D%252F%252Fhome.m.jd.com%252FmyJd%252Fnewhome.action%253Fsceneval%253D2%2526ufc%253D%2526%252FmyJd%252Fhome.action"),
                new KeyValuePair<string, string>("source", "wq_passport")
            };
            var message = new HttpRequestMessage(HttpMethod.Post, tokenAPI) { Content = new FormUrlEncodedContent(nvc) };
            response = await SessionClient.SendAsync(message);
            responseText = await response.Content.ReadAsStringAsync();
            responseJo = (JObject)JsonConvert.DeserializeObject(responseText);
            if (response.StatusCode != HttpStatusCode.OK || responseJo["errcode"].ToString() != "0")
            {
                Lbl_QRStatus.Text = "二维码加载失败...";
                return;
            }
            Token = responseJo["token"].ToString();
            var collection = CookieContainer.GetCookies(new Uri("https://plogin.m.jd.com"));
            foreach (Cookie cookie in collection)
            {
                if (cookie.Name == "okl_token")
                    OKLToken = cookie.Value;
            }
            if (string.IsNullOrWhiteSpace(OKLToken))
            {
                Lbl_QRStatus.Text = "二维码加载失败...";
                return;
            }
            // 构建二维码URL
            string qrUrl = string.Format("https://plogin.m.jd.com/cgi-bin/m/tmauth?client_type=m&appid=300&token={0}", Token);
            // 生成二维码
            Bitmap qrImage = Utils.GenQRCode(qrUrl, 147);
            Lbl_QRStatus.Text = "等待用户进行扫码...";
            pictureBox1.Image = qrImage;
            TM_FetchStatus.Enabled = true;
        }

        private void FrmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        /// <summary>
        /// 通过ticket来获取最终的Cookie
        /// </summary>
        /// <param name="ticket"></param>
        //private void GetCookieByTicket(string ticket)
        //{
        //    string getAPI = "https://passport.jd.com/uc/qrCodeTicketValidation?t=" + ticket;
        //    var client = SessionClient;
        //    client.DefaultRequestHeaders.Referrer = new Uri("https://passport.jd.com/new/login.aspx");
        //    var response = client.GetAsync(getAPI).Result;
        //    // 提取关键Cookies
        //    var collections = Utils.GetAllCookies(CookieContainer);
        //    CookieContainer = new CookieContainer();
        //    foreach(var cookie in collections)
        //    {

        //    }
        //}

        /// <summary>
        /// 轮询API时钟事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TM_FetchStatus_Tick(object sender, EventArgs e)
        {
            //if (string.IsNullOrWhiteSpace(Token)) return;
            //Random rand = new Random();
            //int randInt = (int)(rand.NextDouble() * 10000000);
            //string checkAPI = string.Format("https://qr.m.jd.com/check?callback=jQuery{0}&appid=133&token={1}&_={2}", randInt, Token, Utils.GetTimeStampLong());
            //var client = SessionClient;
            //client.DefaultRequestHeaders.Referrer = new Uri("https://passport.jd.com/new/login.aspx");
            //HttpResponseMessage res = client.GetAsync(checkAPI).Result;
            //var jsonStr = res.Content.ReadAsStringAsync().Result.Split('(', ')')[1];
            //var ret = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);
            //// 如果扫码成功，那么获取Cookie，否则更新信息到界面上
            //if (ret["code"] == "200")
            //{
            //    GetCookieByTicket(ret["ticket"]);
            //    TM_FetchStatus.Enabled = false;
            //    this.DialogResult = DialogResult.OK;
            //    return;
            //}
            //else
            //{
            //    Lbl_QRStatus.Text = ret["msg"];
            //}

            if (string.IsNullOrWhiteSpace(Token)) return;
            if (string.IsNullOrWhiteSpace(OKLToken)) return;
            // 对查询API进行轮询
            string checkAPI = string.Format("https://plogin.m.jd.com/cgi-bin/m/tmauthchecktoken?&token={0}&ou_state=0&okl_token={1}", Token, OKLToken);
            var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("lang", "chs"),
                new KeyValuePair<string, string>("appid", "300"),
                new KeyValuePair<string, string>("returnurl", "https%3A%2F%2Fwqlogin2.jd.com%2Fpassport%2FLoginRedirect%3Fstate%3D1100399130787%26returnurl%3D%252F%252Fhome.m.jd.com%252FmyJd%252Fnewhome.action%253Fsceneval%253D2%2526ufc%253D%2526%252FmyJd%252Fhome.action"),
                new KeyValuePair<string, string>("source", "wq_passport")
            };
            var message = new HttpRequestMessage(HttpMethod.Post, checkAPI) { Content = new FormUrlEncodedContent(nvc) };
            var response = SessionClient.SendAsync(message).Result;
            var responseText = response.Content.ReadAsStringAsync().Result;
            var responseJo = (JObject)JsonConvert.DeserializeObject(responseText);
            // 如果扫码成功，那么得到的CookieContainer就是所要的，否则更新信息到界面上
            if (responseJo["errcode"].ToString() == "0")
            {
                // 设置一下关键Cookie为永不过期
                var cookies = CookieContainer.GetCookies(new Uri("https://api.m.jd.com"));
                foreach (Cookie cookie in cookies)
                {
                    cookie.Expires = DateTime.Now.AddMonths(1);
                }
                TM_FetchStatus.Enabled = false;
                this.DialogResult = DialogResult.OK;
                return;
            }
            else
            {
                Lbl_QRStatus.Text = responseJo["message"].ToString();
            }
        }

        private bool CheckLoginStatus()
        {
            if (CookieContainer == null) return false;
            if (SessionClient == null) return false;
            string queryAPI = "https://wq.jd.com/user/info/QueryJDUserInfo?sceneval=2&g_login_type=1&callback=";
            SessionClient.DefaultRequestHeaders.Referrer = new Uri("https://wq.jd.com/user/info/QueryJDUserInfo?sceneval=2&g_login_type=1&callback=");
            SessionClient.DefaultRequestHeaders.Add("Accept", "*/*");
            SessionClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            var response = SessionClient.GetAsync(queryAPI).Result;
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
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 使用Cookie登录按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Confirm_Click(object sender, EventArgs e)
        {
            string cookieRaw = Txt_Cookies.Text.Trim();
            if (string.IsNullOrWhiteSpace(cookieRaw))
            {
                MessageBox.Show("Cookie不可为空!");
                return;
            }
            string[] cookieList = cookieRaw.Split(';');
            // 设置CookieContainer
            foreach (var cookieStr in cookieList)
            {
                if (string.IsNullOrWhiteSpace(cookieStr)) continue;
                string[] tmpStrs = cookieStr.Split('=');
                Cookie cookie = new Cookie
                {
                    Domain = ".jd.com",
                    Name = tmpStrs[0],
                    Value = tmpStrs[1],
                    Expires = DateTime.Now.AddMonths(1),
                    Path = "/"
                };
                CookieContainer.Add(cookie);
            }
            if (CheckLoginStatus())
            {
                TM_FetchStatus.Enabled = false;
                this.DialogResult = DialogResult.OK;
                return;
            }
            else
            {
                MessageBox.Show("输入的Cookie已失效!");
            }
        }
    }
}

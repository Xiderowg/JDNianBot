using Newtonsoft.Json;
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

        /// <summary>
        /// 查询标识符
        /// </summary>
        private string Token { get; set; }

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
            // 加载二维码
            LoadQRCode();
        }

        /// <summary>
        /// 加载二维码
        /// </summary>
        private async void LoadQRCode()
        {
            string timeStamp = Utils.GetTimeStampLong();
            string qrUrl = "https://qr.m.jd.com/show?appid=133&size=343&t=" + timeStamp;
            CookieContainer = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = CookieContainer,
                UseCookies = true
            };
            using (HttpClient client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
                client.DefaultRequestHeaders.Referrer = new Uri("https://passport.jd.com/");
                byte[] qrData = await client.GetByteArrayAsync(qrUrl);
                var cookies = CookieContainer.GetCookies(new Uri(qrUrl));
                this.Token = cookies["wlfstk_smdl"].Value;
                Bitmap qrImage = Utils.ByteToImage(qrData);
                pictureBox1.Image = qrImage;
                TM_FetchStatus.Enabled = true;
                Lbl_QRStatus.Text = "等待用户进行扫码...";
            }
        }

        private void FrmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        /// <summary>
        /// 通过ticket来获取最终的Cookie
        /// </summary>
        /// <param name="ticket"></param>
        private void GetCookieByTicket(string ticket)
        {
            string getAPI = "https://passport.jd.com/uc/qrCodeTicketValidation?t=" + ticket;
            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = CookieContainer,
                UseCookies = true
            };
            using (HttpClient client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
                client.DefaultRequestHeaders.Referrer = new Uri("https://passport.jd.com/");
                var response = client.GetAsync(getAPI).Result;
                // 清除没啥用的Cookies
                foreach (Cookie cc in CookieContainer.GetCookies(new Uri("https://www.jd.com")))
                {
                    if (cc.Name != "pt_pin" || cc.Name != "pt_key")
                    {
                        cc.Expired = true;
                    }
                }
            }
        }

        /// <summary>
        /// 轮询API时钟事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TM_FetchStatus_Tick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Token)) return;
            Random rand = new Random();
            int randInt = (int)(rand.NextDouble() * 10000000);
            string checkAPI = string.Format("https://qr.m.jd.com/check?callback=jQuery{0}&appid=133&token={1}&_={2}", randInt, Token, Utils.GetTimeStampLong());
            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = CookieContainer,
                UseCookies = true
            };
            using (HttpClient client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
                client.DefaultRequestHeaders.Referrer = new Uri("https://passport.jd.com/");
                HttpResponseMessage res = client.GetAsync(checkAPI).Result;
                var jsonStr = res.Content.ReadAsStringAsync().Result.Split('(', ')')[1];
                var ret = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);
                // 如果扫码成功，那么获取Cookie，否则更新信息到界面上
                if (ret["code"] == "200")
                {
                    GetCookieByTicket(ret["ticket"]);
                    TM_FetchStatus.Enabled = false;
                    this.DialogResult = DialogResult.OK;
                    return;
                }
                else
                {
                    Lbl_QRStatus.Text = ret["msg"];
                }
            }
        }
    }
}

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace KakaoEmoticonDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string GetItemCode(string url)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Linux; Android 5.0; SM-G900P Build/LRX21T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Mobile Safari/537.36");
                string html = wc.DownloadString(url);
                string[] result = html.Split(new string[] { "item_code" }, StringSplitOptions.None);
                return result[1].Split('\'')[1].Split('\'')[0];
            }
            catch (Exception ex)
            {
                return "Error";
            }

        }

        private string GetName(string url)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Linux; Android 5.0; SM-G900P Build/LRX21T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Mobile Safari/537.36");
                string html = wc.DownloadString(url);
                string[] result = html.Split(new string[] { "prefix: " }, StringSplitOptions.None);
                return result[1].Split('\"')[1].Split('\"')[0];
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private string[] GetThumbUrl(string jsonUrl)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(jsonUrl);

                string responseText = string.Empty;

                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";
                request.UserAgent = "Mozilla/5.0";

                using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
                {
                    HttpStatusCode status = resp.StatusCode;
                    Console.WriteLine(status);

                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }
                return JObject.Parse(responseText)["body"].ToObject<string[]>();
            }
            catch (Exception ex)
            {
                return new string[] { "error" };
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string base_url = "https://e.kakao.com/detail/thumb_url?item_code=";
            string input_url = textBox1.Text;
            string json_url = base_url + GetItemCode(input_url);
            string[] result = GetThumbUrl(json_url);
            int size = result.Length;
            if (GetName(input_url) == null)
            {
                MessageBox.Show("에러가 발생하였습니다.");
            }
            else
            {
                string folderPath = Application.StartupPath + "\\" + GetName(input_url);
                DirectoryInfo di = new DirectoryInfo(folderPath);
                if (di.Exists == false)
                {
                    di.Create();
                }
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Minimum = 0;
                progressBar1.Maximum = size;
                progressBar1.Step = 1;
                progressBar1.Value = 0;
                using (WebClient webClient = new WebClient())
                {
                    for (int i = 0; i < size; i++)
                    {
                        webClient.DownloadFile(result[i], folderPath + "\\" + (i + 1).ToString() + ".png");
                        progressBar1.PerformStep();
                    }
                }

            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

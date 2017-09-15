using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace VideoCollector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            TextBox.CheckForIllegalCrossThreadCalls = false;
        }
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
                return;
            if (e.KeyChar == (char)1)       // Ctrl-A 相当于输入了AscII=1的控制字符
            {
                textBox.SelectAll();
                e.Handled = true;      // 不再发出“噔”的声音
            }
        }

        #region 视频URL
        private void button1_Click(object sender, EventArgs e)
        {
            Thread thread1 = new Thread(Mythread1);
            thread1.Priority = ThreadPriority.Highest;
            thread1.IsBackground = true;
            thread1.Start();

        }
        private void Mythread1()
        {
            try
            {
                if (HotURL.Text != "" && URL.Text != "")
                {
                    VideoURLAttr.Text = GetHttpLinks(URL.Text);
                    VideoURLAttr.Text = splitURLA(VideoURLAttr.Text, KeyWord.Text, HotURL.Text);
                }
                else
                {
                    MessageBox.Show("请把值输入完整");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("程序错误！！");
            }
        } 
        #endregion

        #region 获取A标签
        public static string GetHttpLinks(string url)
        {
            string web_url = url;
            string all_code = "";
            string URLName = "";
            HttpWebRequest all_codeRequest = (HttpWebRequest)WebRequest.Create(web_url);
            WebResponse all_codeResponse = all_codeRequest.GetResponse();
            StreamReader the_Reader = new StreamReader(all_codeResponse.GetResponseStream());
            all_code = the_Reader.ReadToEnd();
            the_Reader.Close();
            ArrayList my_list = new ArrayList();
            string p = @"(?i)<a\s[^>]*?href=(['""]?)(?!javascript|__doPostBack)(?<url>[^'""\s*#<>]+)[^>]*>";
            Regex re = new Regex(p, RegexOptions.IgnoreCase);
            MatchCollection mc = re.Matches(all_code);
            for (int i = 0; i <= mc.Count - 1; i++)
            {
                bool _foo = false;
                string name = mc[i].ToString();
                foreach (string list in my_list)
                {
                    if (name == list)
                    {
                        _foo = true;
                        break;
                    }
                }//过滤

                if (!_foo)
                {
                    URLName += name + "·";
                }
            }
            return URLName;
        }
        #endregion
        #region 找关键的地址
        /// <summary>
        /// 找关键的地址
        /// </summary>
        /// <param name="URLA">传过来的带标签的地址</param>
        /// <param name="KeyWord">关键字</param>
        ///<param name="Hot">URL头</param>
        /// <returns></returns>
        public static string splitURLA(string URLA, string KeyWord, string Hot)
        {
            string[] AHTML = URLA.Split('·');
            string AString = "", temp;
            for (int i = 0; i < AHTML.Length - 1; i++)
            {
                if (AHTML[i].Contains(KeyWord))
                {
                    temp = GetHtmlAHref(AHTML[i]) + "·";
                    AString += Hot + temp;
                }
            }
            int j = AString.Split('·').Length;
            return AString;
        }
        #endregion
        #region a标签中的herf中的连接地址
        /// <summary>
        /// a标签中的herf中的连接地址
        /// </summary>
        /// <param name="href"></param>
        /// <returns></returns>
        public static string GetHtmlAHref(string href)
        {
            Match m = Regex.Match(href, @"(?is)<a[^>]*?href=(['""\s]?)(?<href>[^'""\s]*)\1[^>]*?>");
            if (m.Groups.Count > 1)
            {
                return m.Groups["href"].Value;
            }
            return "";
        }
        #endregion


        //------------------------------------------------------------------

        #region 视频简介
        private void button2_Click(object sender, EventArgs e)
        {
            Thread thread1 = new Thread(Mythread);
            thread1.Priority = ThreadPriority.Highest;
            thread1.IsBackground = true;
            thread1.Start();
        }
        private void Mythread()
        {
            try
            {
                if (URL.Text != "")
                {
                    VideoConent.Text = GetHtmlP(URL.Text);
                }
                else
                {
                    MessageBox.Show("请输入完整的值");
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("程序错误！！");
            }
        } 
        #endregion
        #region 获取搜索网页中的p标签
        /// <summary>
        /// 获取搜索网页中的p标签
        /// </summary>
        /// <param name="HtmlSrout"></param>
        /// <returns></returns>
        public static string GetHtmlP(string HtmlSrout)
        {
            string temp = "";
            string all_code = "";
            HttpWebRequest all_codeRequest = (HttpWebRequest)WebRequest.Create(HtmlSrout);
            WebResponse all_codeResponse = all_codeRequest.GetResponse();
            StreamReader the_Reader = new StreamReader(all_codeResponse.GetResponseStream());
            all_code = the_Reader.ReadToEnd();
            the_Reader.Close();
            ArrayList my_list = new ArrayList();
            Regex Reg = new Regex(@"(?is)(?<=<p [\S]*?>).*?(?=</p>)", RegexOptions.IgnoreCase);//(?is)(?<=<p [\S]*?>).*?(?=</p>)
            MatchCollection mc = Reg.Matches(all_code);

            for (int i = 0; i < mc.Count; i++)
            {
                bool _foo = false;
                string name = mc[i].ToString();
                foreach (string list in my_list)
                {
                    if (name == list)
                    {
                        _foo = true;
                        break;
                    }
                }//过滤

                if (!_foo)
                {
                    temp += name + "·";
                }
            }
            return temp;
        }
        #endregion

        private void URLAttr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
            {
                ((TextBox)sender).SelectAll();
            }  
        }



        

    }
}

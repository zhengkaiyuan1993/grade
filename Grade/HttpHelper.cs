using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Grade
{
    class HttpVisiter
    {
        private string url = null;
        private string url_code = null;
        private string view_state = null;
        private CookieContainer cookie = null;
        private string host = null;
        private string defaultUrl;
        private string mainUrl;


        //info
        private string username;
        private string name = null;
        public HttpVisiter(string url)
        {
            this.url = url;
        }

        public string GetName()
        {
            return name;
        }

        public Stream GetCheckCode()
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "GET";
                req.Accept = "*/*";
                req.UserAgent = "Mozilla/5.0";
                req.Headers.Add("Cache-Control", "max-age=0");
                cookie = new CookieContainer();
                req.CookieContainer = cookie;
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                host = req.Host;
                defaultUrl = resp.ResponseUri.ToString();
                url_code = defaultUrl.Replace("default2.aspx", "");
                string reqStr = string.Empty;
                using (StreamReader reader = new StreamReader(resp.GetResponseStream(), Encoding.GetEncoding("GB2312")))
                {
                    reqStr = reader.ReadToEnd();
                }
                view_state = GetViewState(reqStr);
                HttpWebRequest reqeust = (HttpWebRequest)WebRequest.Create(url_code + "CheckCode.aspx");
                reqeust.Method = "Get";
                reqeust.Accept = "*/*";
                reqeust.Headers.Add("Cache-Control", "max-age=0");
                reqeust.UserAgent = "Mozilla/5.0";
                reqeust.CookieContainer = cookie;
                HttpWebResponse response = (HttpWebResponse)reqeust.GetResponse();
                MemoryStream ms = null;
                using (var stream = response.GetResponseStream())
                {
                    Byte[] buffer = new Byte[response.ContentLength];
                    int offset = 0, actuallyRead = 0;
                    do
                    {
                        actuallyRead = stream.Read(buffer, offset, buffer.Length - offset);
                        offset += actuallyRead;
                    }
                    while (actuallyRead > 0);
                    ms = new MemoryStream(buffer);
                }
                response.Close();
                return ms;
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
                return null;
            }
        }
        private string GetViewState(string str)
        {
            string key = "__VIEWSTATE";
            int start = str.IndexOf(key) + 11;
            int x = str.IndexOf("value=\"", start) + 7;
            int y = str.IndexOf("\"", x);
            return str.Substring(x, y - x);
        }

        private string GetValue(string key, string str, int start = 0)
        {
            int x = str.IndexOf(key, start) + key.Length + 2;
            int y = str.IndexOf("\"", x);
            return str.Substring(x, y - x);
        }

        private string GetContent(string str, int start = 0)
        {
            int x = str.IndexOf(">", start) + 1;
            int y = str.IndexOf("<", x);
            return str.Substring(x, y - x);
        }
        public bool Login(string userName, string password, string checkCode)
        {
            try
            {
                this.username = userName;
                string postdata = string.Format("__VIEWSTATE={0}&txtUserName={1}&Textbox1=&TextBox2={2}&txtSecretCode={3}&RadioButtonList1=%D1%A7%C9%FA&Button1=&lbLanguage=&hidPdrs=&hidsc=", HttpUtility.UrlEncode(view_state), HttpUtility.UrlEncode(userName), HttpUtility.UrlEncode(password), HttpUtility.UrlEncode(checkCode));
                byte[] postdatabyte = Encoding.GetEncoding("GB2312").GetBytes(postdata);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(defaultUrl);
                request.ServicePoint.Expect100Continue = false;
                request.Method = "POST";
                request.Headers.Add("Cache-Control", "max-age=0");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                request.Referer = url_code + "default2.aspx";
                request.ContentLength = postdatabyte.Length;
                request.KeepAlive = true;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Upgrade-Insecure-Requests", "1");
                request.CookieContainer = cookie;
                request.AllowAutoRedirect = false;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(postdatabyte, 0, postdatabyte.Length);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();
                string location = response.GetResponseHeader("Location");
                if (string.IsNullOrWhiteSpace(location))
                {
                    return false;
                }
                mainUrl = "http://" + host + location;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(mainUrl);
                req.Method = "GET";
                req.CookieContainer = cookie;
                req.Referer = defaultUrl;
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                req.Headers.Add("Accept-Encoding", "gzip, deflate");
                req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                req.Headers.Add("Upgrade-Insecure-Requests", "1");
                req.KeepAlive = true;
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                string str = string.Empty;
                bool gzip = string.Equals(resp.Headers["Vary"], "Accept-Encoding", StringComparison.OrdinalIgnoreCase);
                str = DecompressGzip(resp.GetResponseStream(), gzip);
                resp.Close();
                int i = str.IndexOf("<span id=\"xhxm\">");
                int j = str.IndexOf("同学");
                name = str.Substring(i + 16, j - i - 16);
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
                return false;
            }
            return true;
        }
        public List<EnglisgGradeModel> GetEndlishGrade()
        {
            try
            {
                string post = url_code + "xsdjkscx.aspx?" + string.Format("xh={0}&xm={1}gnmkdm=N121606", HttpUtility.UrlEncode(username), HttpUtility.UrlEncode(name));
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(post);
                req.Method = "GET";
                req.CookieContainer = cookie;
                req.KeepAlive = true;
                req.Referer = mainUrl;
                req.AllowAutoRedirect = false;
                req.Headers.Add("Upgrade-Insecure-Requests", "1");
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                req.Headers.Add("Accept-Encoding", "gzip, deflate");
                req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                string str = string.Empty;
                bool gzip = string.Equals(resp.Headers["Vary"], "Accept-Encoding", StringComparison.OrdinalIgnoreCase);
                str = DecompressGzip(resp.GetResponseStream(), gzip);
                resp.Close();
                if (string.IsNullOrWhiteSpace(str) || !str.Contains("__VIEWSTATE"))
                {
                    return null;
                }
                List<string> tab = GetReg("<table", "</table>", str);
                if (tab.Count <= 0)
                {
                    return null;
                }
                List<string> trs = GetReg("<tr", "</tr>", tab[0]);
                List<EnglisgGradeModel> list = new List<EnglisgGradeModel>();
                foreach (string tr in trs.Skip(1))
                {
                    List<string> tds = GetReg("<td>", "</td>", tr);
                    EnglisgGradeModel model = new EnglisgGradeModel();
                    model.Year = HttpUtility.HtmlDecode(GetContent(tds[0]));
                    model.Semester = HttpUtility.HtmlDecode(GetContent(tds[1]));
                    model.Name = HttpUtility.HtmlDecode(GetContent(tds[2]));
                    model.ID = HttpUtility.HtmlDecode(GetContent(tds[3]));
                    model.Date = HttpUtility.HtmlDecode(GetContent(tds[4]));
                    model.Grade = HttpUtility.HtmlDecode(GetContent(tds[5]));
                    model.ListenGrade = HttpUtility.HtmlDecode(GetContent(tds[6]));
                    model.ReadGrade = HttpUtility.HtmlDecode(GetContent(tds[7]));
                    model.WriteGrade = HttpUtility.HtmlDecode(GetContent(tds[8]));
                    model.AllGrade = HttpUtility.HtmlDecode(GetContent(tds[9]));
                    list.Add(model);
                }
                return list;
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
                return null;
            }
        }
        public List<GradeModel> GetGrade()
        {
            try
            {
                string post = url_code + "xscj_gc.aspx?" + string.Format("xh={0}&xm={1}gnmkdm=N121605", HttpUtility.UrlEncode(username), HttpUtility.UrlEncode(name));
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(post);
                req.Method = "GET";
                req.CookieContainer = cookie;
                req.KeepAlive = true;
                req.Referer = mainUrl;
                req.AllowAutoRedirect = false;
                req.Headers.Add("Upgrade-Insecure-Requests", "1");
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                req.Headers.Add("Accept-Encoding", "gzip, deflate");
                req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                string str = string.Empty;
                bool gzip = string.Equals(resp.Headers["Vary"], "Accept-Encoding", StringComparison.OrdinalIgnoreCase);
                str = DecompressGzip(resp.GetResponseStream(), gzip);
                resp.Close();
                if (string.IsNullOrWhiteSpace(str) || !str.Contains("__VIEWSTATE"))
                {
                    return null;
                }
                var viewState = GetViewState(str);
                return GetGradeDetail(viewState);
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
                return null;
            }



            //string location2 = post2;//dDwxNDM1NzMwODY2O3Q8O2w8aTwxPjs+O2w8dDw7bDxpPDE+O2k8Mz47aTw1PjtpPDk+Oz47bDx0PHA8bDxWaXNpYmxlOz47bDxvPGY+Oz4+O2w8aTw3Pjs+O2w8dDxAMDw7Ozs7Ozs7Ozs7Pjs7Pjs+Pjt0PEAwPHA8cDxsPERhdGFLZXlzO18hSXRlbUNvdW50Oz47bDxsPD47aTwwPjs+Pjs+Ozs7Ozs7Ozs+Ozs+O3Q8cDxwPGw8VmlzaWJsZTs+O2w8bzxmPjs+Pjs+Ozs+O3Q8O2w8aTwxPjs+O2w8dDxAMDw7Ozs7Ozs7Ozs7Pjs7Pjs+Pjs+Pjs+Pjs+82XDVlv7CkxSqsr4yV0d+9YiILs=
            //string post3 = "__VIEWSTATE=dDwxODI2NTc3MzMwO3Q8cDxsPHhoOz47bDwxMjA1MTAwNDI2Oz4%2BO2w8aTwxPjs%2BO2w8dDw7bDxpPDE%2BO2k8Mz47aTw1PjtpPDc%2BO2k8OT47aTwxMT47aTwxMz47aTwxNj47aTwyNj47aTwyNz47aTwyOD47aTwzNT47aTwzNz47aTwzOT47aTw0MT47aTw0NT47PjtsPHQ8cDxwPGw8VGV4dDs%2BO2w85a2m5Y%2B377yaMTIwNTEwMDQyNjs%2BPjs%2BOzs%2BO3Q8cDxwPGw8VGV4dDs%2BO2w85aeT5ZCN77ya6YOR5Yev5YWDOz4%2BOz47Oz47dDxwPHA8bDxUZXh0Oz47bDzlrabpmaLvvJrkv6Hmga%2Flt6XnqIvlrabpmaI7Pj47Pjs7Pjt0PHA8cDxsPFRleHQ7PjtsPOS4k%2BS4mu%2B8mjs%2BPjs%2BOzs%2BO3Q8cDxwPGw8VGV4dDs%2BO2w855S15a2Q5L%2Bh5oGv5bel56iLOz4%2BOz47Oz47dDxwPHA8bDxUZXh0Oz47bDzooYzmlL%2Fnj63vvJrnlLXlrZAyMDEyLTM7Pj47Pjs7Pjt0PHA8cDxsPFRleHQ7PjtsPDIwMTIwNTExOz4%2BOz47Oz47dDx0PHA8cDxsPERhdGFUZXh0RmllbGQ7RGF0YVZhbHVlRmllbGQ7PjtsPFhOO1hOOz4%2BOz47dDxpPDU%2BO0A8XGU7MjAxNS0yMDE2OzIwMTQtMjAxNTsyMDEzLTIwMTQ7MjAxMi0yMDEzOz47QDxcZTsyMDE1LTIwMTY7MjAxNC0yMDE1OzIwMTMtMjAxNDsyMDEyLTIwMTM7Pj47Pjs7Pjt0PHA8O3A8bDxvbmNsaWNrOz47bDx3aW5kb3cucHJpbnQoKVw7Oz4%2BPjs7Pjt0PHA8O3A8bDxvbmNsaWNrOz47bDx3aW5kb3cuY2xvc2UoKVw7Oz4%2BPjs7Pjt0PHA8cDxsPFZpc2libGU7PjtsPG88dD47Pj47Pjs7Pjt0PEAwPDs7Ozs7Ozs7Ozs%2BOzs%2BO3Q8QDA8Ozs7Ozs7Ozs7Oz47Oz47dDxAMDw7Ozs7Ozs7Ozs7Pjs7Pjt0PDtsPGk8MD47aTwxPjtpPDI%2BO2k8ND47PjtsPHQ8O2w8aTwwPjtpPDE%2BOz47bDx0PDtsPGk8MD47aTwxPjs%2BO2w8dDxAMDw7Ozs7Ozs7Ozs7Pjs7Pjt0PEAwPDs7Ozs7Ozs7Ozs%2BOzs%2BOz4%2BO3Q8O2w8aTwwPjtpPDE%2BOz47bDx0PEAwPDs7Ozs7Ozs7Ozs%2BOzs%2BO3Q8QDA8Ozs7Ozs7Ozs7Oz47Oz47Pj47Pj47dDw7bDxpPDA%2BOz47bDx0PDtsPGk8MD47PjtsPHQ8QDA8Ozs7Ozs7Ozs7Oz47Oz47Pj47Pj47dDw7bDxpPDA%2BO2k8MT47PjtsPHQ8O2w8aTwwPjs%2BO2w8dDxAMDxwPHA8bDxWaXNpYmxlOz47bDxvPGY%2BOz4%2BOz47Ozs7Ozs7Ozs7Pjs7Pjs%2BPjt0PDtsPGk8MD47PjtsPHQ8QDA8cDxwPGw8VmlzaWJsZTs%2BO2w8bzxmPjs%2BPjs%2BOzs7Ozs7Ozs7Oz47Oz47Pj47Pj47dDw7bDxpPDA%2BOz47bDx0PDtsPGk8MD47PjtsPHQ8cDxwPGw8VGV4dDs%2BO2w8SE5DSjs%2BPjs%2BOzs%2BOz4%2BOz4%2BOz4%2BO3Q8QDA8Ozs7Ozs7Ozs7Oz47Oz47Pj47Pj47PsGtYjdJkWDU8mtEDl8EhORSEnpJ&ddlXN=&ddlXQ=&Button2=%D4%DA%D0%A3%D1%A7%CF%B0%B3%C9%BC%A8%B2%E9%D1%AF";
            //HttpWebRequest req3 = (HttpWebRequest)WebRequest.Create(location2);
            //req3.Method = "POST";
            //req3.Headers.Add("Accept-Encoding", "gzip,deflate");
            //req3.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
            //req3.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            //req3.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
            //req3.ContentType = "application/x-www-form-urlencoded";
            //req3.Referer = location2;
            //req3.KeepAlive = true;
            //byte[] postdatabyte3 = Encoding.UTF8.GetBytes(post3);
            //req3.ContentLength = postdatabyte3.Length;
            //using (Stream stream = req3.GetRequestStream())
            //{
            //    stream.Write(postdatabyte3, 0, postdatabyte3.Length);
            //}
            //HttpWebResponse resp3 = (HttpWebResponse)req3.GetResponse();
            //string str3 = string.Empty;
            //using (StreamReader reader = new StreamReader(resp3.GetResponseStream(), Encoding.GetEncoding("GB2312")))
            //{
            //    str3 = reader.ReadToEnd();
            //}
            //resp3.Close();
        }
        public ExamInfo SearchExam()
        {
            try
            {
                string post = url_code + "xskscx.aspx?" + string.Format("xh={0}&xm={1}&gnmkdm=N121604", HttpUtility.UrlEncode(username), HttpUtility.UrlEncode(name));
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(post);
                req.Method = "GET";
                req.CookieContainer = cookie;
                req.KeepAlive = true;
                req.Referer = mainUrl;
                req.AllowAutoRedirect = false;
                req.Headers.Add("Upgrade-Insecure-Requests", "1");
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                req.Headers.Add("Accept-Encoding", "gzip, deflate");
                req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                string str = string.Empty;
                bool gzip = string.Equals(resp.Headers["Vary"], "Accept-Encoding", StringComparison.OrdinalIgnoreCase);
                str = DecompressGzip(resp.GetResponseStream(), gzip);
                resp.Close();
                if (string.IsNullOrWhiteSpace(str) || !str.Contains("__VIEWSTATE"))
                {
                    return null;
                }
                ExamInfo info = new ExamInfo();
                info.ViewState = GetViewState(str);
                List<string> tab = GetReg("<select", "</select>", str);
                if (tab.Count < 2)
                {
                    return null;
                }
                List<string> ops = GetReg("<option", "</option>", tab[0]);
                info.Year = new List<KeyValue>();
                foreach (string op in ops)
                {
                    KeyValue kv = new KeyValue();
                    kv.Key = GetValue("value", op);
                    kv.Value = GetContent(op);
                    info.Year.Add(kv);
                }
                info.Semester = new List<KeyValue>();
                List<string> ops2 = GetReg("<option", "</option>", tab[1]);
                info.Semester = new List<KeyValue>();
                foreach (string op in ops2)
                {
                    KeyValue kv = new KeyValue();
                    kv.Key = GetValue("value", op);
                    kv.Value = GetContent(op);
                    info.Semester.Add(kv);
                }
                return info;
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
                return null;
            }
        }
        public List<ExamModel> SearchExamDetail(string viewState, string year, string semester)
        {
            try
            {
                string post = url_code + "xskscx.aspx?" + string.Format("xh={0}&xm={1}&gnmkdm=N121604", HttpUtility.UrlEncode(username), HttpUtility.UrlEncode(name));
                string postdata = string.Format("__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE={0}&xnd={1}&xqd={2}", HttpUtility.UrlEncode(viewState), HttpUtility.UrlEncode(year), HttpUtility.UrlEncode(semester));
                byte[] postdatabyte = Encoding.GetEncoding("GB2312").GetBytes(postdata);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(post);
                request.ServicePoint.Expect100Continue = false;
                request.Method = "POST";
                request.Headers.Add("Cache-Control", "max-age=0");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                request.Referer = post;
                request.ContentLength = postdatabyte.Length;
                request.KeepAlive = true;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Upgrade-Insecure-Requests", "1");
                request.CookieContainer = cookie;
                request.AllowAutoRedirect = false;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(postdatabyte, 0, postdatabyte.Length);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string str = string.Empty;
                bool gzip = string.Equals(response.Headers["Vary"], "Accept-Encoding", StringComparison.OrdinalIgnoreCase);
                str = DecompressGzip(response.GetResponseStream(), gzip);
                response.Close();
                if (string.IsNullOrWhiteSpace(str) || !str.Contains("__VIEWSTATE"))
                {
                    return null;
                }

                List<string> tab = GetReg("<table", "</table>", str);
                if (tab.Count <= 0)
                {
                    return null;
                }
                List<string> trs = GetReg("<tr", "</tr>", tab[0]);
                List<ExamModel> list = new List<ExamModel>();
                foreach (string tr in trs.Skip(1))
                {
                    List<string> tds = GetReg("<td", "</td>", tr);
                    ExamModel model = new ExamModel();
                    model.Number = HttpUtility.HtmlDecode(GetContent(tds[0]));
                    model.ClassName = HttpUtility.HtmlDecode(GetContent(tds[1]));
                    model.Name = HttpUtility.HtmlDecode(GetContent(tds[2]));
                    model.Date = HttpUtility.HtmlDecode(GetContent(tds[3]));
                    model.Area = HttpUtility.HtmlDecode(GetContent(tds[4]));
                    model.Type = HttpUtility.HtmlDecode(GetContent(tds[5]));
                    model.Site = HttpUtility.HtmlDecode(GetContent(tds[6]));
                    model.School = HttpUtility.HtmlDecode(GetContent(tds[7]));
                    list.Add(model);
                }
                return list;
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
                return null;
            }
        }
        public ExamInfo SearchReExam()
        {
            try
            {
                string post = url_code + "XsBkKsCx.aspx?" + string.Format("xh={0}&xm={1}&gnmkdm=N121618", HttpUtility.UrlEncode(username), HttpUtility.UrlEncode(name));
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(post);
                req.Method = "GET";
                req.CookieContainer = cookie;
                req.KeepAlive = true;
                req.Referer = mainUrl;
                req.AllowAutoRedirect = false;
                req.Headers.Add("Upgrade-Insecure-Requests", "1");
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                req.Headers.Add("Accept-Encoding", "gzip, deflate");
                req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                string str = string.Empty;
                bool gzip = string.Equals(resp.Headers["Vary"], "Accept-Encoding", StringComparison.OrdinalIgnoreCase);
                str = DecompressGzip(resp.GetResponseStream(), gzip);
                resp.Close();
                if (string.IsNullOrWhiteSpace(str) || !str.Contains("__VIEWSTATE"))
                {
                    return null;
                }
                ExamInfo info = new ExamInfo();
                info.ViewState = GetViewState(str);
                List<string> tab = GetReg("<select", "</select>", str);
                if (tab.Count < 2)
                {
                    return null;
                }
                List<string> ops = GetReg("<option", "</option>", tab[0]);
                info.Year = new List<KeyValue>();
                foreach (string op in ops)
                {
                    KeyValue kv = new KeyValue();
                    kv.Key = GetValue("value", op);
                    kv.Value = GetContent(op);
                    info.Year.Add(kv);
                }
                info.Semester = new List<KeyValue>();
                List<string> ops2 = GetReg("<option", "</option>", tab[1]);
                info.Semester = new List<KeyValue>();
                foreach (string op in ops2)
                {
                    KeyValue kv = new KeyValue();
                    kv.Key = GetValue("value", op);
                    kv.Value = GetContent(op);
                    info.Semester.Add(kv);
                }
                return info;
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
                return null;
            }
        }
        public List<ExamModel> SearchReExamDetail(string viewState, string year, string semester)
        {
            try
            {
                string post = url_code + "XsBkKsCx.aspx?" + string.Format("xh={0}&xm={1}&gnmkdm=N121618", HttpUtility.UrlEncode(username), HttpUtility.UrlEncode(name));
                string postdata = string.Format("__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE={0}&xnd={1}&xqd={2}", HttpUtility.UrlEncode(viewState), HttpUtility.UrlEncode(year), HttpUtility.UrlEncode(semester));
                byte[] postdatabyte = Encoding.GetEncoding("GB2312").GetBytes(postdata);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(post);
                request.ServicePoint.Expect100Continue = false;
                request.Method = "POST";
                request.Headers.Add("Cache-Control", "max-age=0");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                request.Referer = post;
                request.ContentLength = postdatabyte.Length;
                request.KeepAlive = true;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Upgrade-Insecure-Requests", "1");
                request.CookieContainer = cookie;
                request.AllowAutoRedirect = false;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(postdatabyte, 0, postdatabyte.Length);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string str = string.Empty;
                bool gzip = string.Equals(response.Headers["Vary"], "Accept-Encoding", StringComparison.OrdinalIgnoreCase);
                str = DecompressGzip(response.GetResponseStream(), gzip);
                response.Close();
                if (string.IsNullOrWhiteSpace(str) || !str.Contains("__VIEWSTATE"))
                {
                    return null;
                }

                List<string> tab = GetReg("<table", "</table>", str);
                if (tab.Count <= 0)
                {
                    return null;
                }
                List<string> trs = GetReg("<tr", "</tr>", tab[0]);
                List<ExamModel> list = new List<ExamModel>();
                foreach (string tr in trs.Skip(1))
                {
                    List<string> tds = GetReg("<td", "</td>", tr);
                    ExamModel model = new ExamModel();
                    model.Number = HttpUtility.HtmlDecode(GetContent(tds[0]));
                    model.ClassName = HttpUtility.HtmlDecode(GetContent(tds[1]));
                    model.Name = HttpUtility.HtmlDecode(GetContent(tds[2]));
                    model.Date = HttpUtility.HtmlDecode(GetContent(tds[3]));
                    model.Area = HttpUtility.HtmlDecode(GetContent(tds[4]));
                    model.Site = HttpUtility.HtmlDecode(GetContent(tds[5]));
                    model.Type = HttpUtility.HtmlDecode(GetContent(tds[6]));
                    list.Add(model);
                }
                return list;
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
                return null;
            }
        }

        private List<GradeModel> GetGradeDetail(string viewState)
        {
            try
            {
                string post = url_code + "xscj_gc.aspx?" + string.Format("xh={0}&xm={1}&gnmkdm=N121605", HttpUtility.UrlEncode(username), HttpUtility.UrlEncode(name));
                string postdata = string.Format("__VIEWSTATE={0}&ddlXN=&ddlXQ=&Button2=%D4%DA%D0%A3%D1%A7%CF%B0%B3%C9%BC%A8%B2%E9%D1%AF", HttpUtility.UrlEncode(viewState));
                byte[] postdatabyte = Encoding.GetEncoding("GB2312").GetBytes(postdata);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(post);
                request.ServicePoint.Expect100Continue = false;
                request.Method = "POST";
                request.Headers.Add("Cache-Control", "max-age=0");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                request.Referer = post;
                request.ContentLength = postdatabyte.Length;
                request.KeepAlive = true;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Upgrade-Insecure-Requests", "1");
                request.CookieContainer = cookie;
                request.AllowAutoRedirect = false;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(postdatabyte, 0, postdatabyte.Length);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string str = string.Empty;
                bool gzip = string.Equals(response.Headers["Vary"], "Accept-Encoding", StringComparison.OrdinalIgnoreCase);
                str = DecompressGzip(response.GetResponseStream(), gzip);
                response.Close();
                if (string.IsNullOrWhiteSpace(str) || !str.Contains("__VIEWSTATE"))
                {
                    return null;
                }
                List<string> tab = GetReg("<table", "</table>", str);
                if (tab.Count <= 0)
                {
                    return null;
                }
                List<string> trs = GetReg("<tr", "</tr>", tab[0]);
                List <GradeModel> list = new List<GradeModel>();
                foreach (string tr in trs.Skip(1))
                {
                    List<string> tds = GetReg("<td", "</td>", tr);
                    GradeModel model = new GradeModel();
                    model.Year = HttpUtility.HtmlDecode(GetContent(tds[0])).Trim();
                    model.Semester = HttpUtility.HtmlDecode(GetContent(tds[1])).Trim();
                    model.ClassCode = HttpUtility.HtmlDecode(GetContent(tds[2])).Trim();
                    model.ClassName = HttpUtility.HtmlDecode(GetContent(tds[3])).Trim();
                    model.ClassNature = HttpUtility.HtmlDecode(GetContent(tds[4])).Trim();
                    model.ClassOwnership = HttpUtility.HtmlDecode(GetContent(tds[5])).Trim();
                    model.Credit = HttpUtility.HtmlDecode(GetContent(tds[6])).Trim();
                    model.GradePoint = HttpUtility.HtmlDecode(GetContent(tds[7])).Trim();
                    model.Grade = HttpUtility.HtmlDecode(GetContent(tds[8])).Trim();
                    model.MinorFlag = HttpUtility.HtmlDecode(GetContent(tds[9])).Trim();
                    model.ExaminationGrade = HttpUtility.HtmlDecode(GetContent(tds[10])).Trim();
                    model.ReworkGrade = HttpUtility.HtmlDecode(GetContent(tds[11])).Trim();
                    model.CollegeName = HttpUtility.HtmlDecode(GetContent(tds[12])).Trim();
                    model.Note = HttpUtility.HtmlDecode(GetContent(tds[13])).Trim();
                    model.ReworkFlag = HttpUtility.HtmlDecode(GetContent(tds[14])).Trim();
                    model.ClassEnglishName = HttpUtility.HtmlDecode(GetContent(tds[15])).Trim();
                    list.Add(model);
                }
                return list;
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
                return null;
            }
        }
        private List<string> GetReg(string begin, string end, string str)
        {
            List<string> lt = new List<string>();
            int x = -1;
            int y = -1;
            int st = 0;
            do
            {
                x = str.IndexOf(begin, st);
                if (x < 0)
                {
                    return lt;
                }
                y = str.IndexOf(end, x);
                if (y < 0)
                {
                    return lt;
                }
                st = y + end.Length;
                lt.Add(str.Substring(x, st - x));
            }
            while (true);
        }

        private string DecompressGzip(Stream stm, bool gzip)
        {
            if (gzip)
            {
                string strHTML = "";
                GZipStream stream = new GZipStream(stm, CompressionMode.Decompress);//解压缩
                using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("gb2312")))//中文编码处理
                {
                    strHTML = reader.ReadToEnd();
                }
                return strHTML;
            }
            else
            {
                string strHTML = "";
                using (StreamReader reader = new StreamReader(stm, Encoding.GetEncoding("gb2312")))//中文编码处理
                {
                    strHTML = reader.ReadToEnd();
                }
                return strHTML;
            }
        }
    }
}

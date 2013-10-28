using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MvcTest.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult WebSiteParserTest()
        {
            string txtContent = getHTML(@"https://www.acttab.com.au/keno/results");

            string[] arrContent = txtContent.Split(new string[] { "<tbody>" }, StringSplitOptions.None);
            txtContent = arrContent[1];
            arrContent = txtContent.Split(new string[] { "</tbody>" }, StringSplitOptions.None);
            txtContent = arrContent[0];
            txtContent = txtContent.Replace(" class=\"even\"", "");
            txtContent = txtContent.Replace(" class=\"odd\"", "");
            txtContent = txtContent.Replace("<", "_{");
            txtContent = txtContent.Replace(">", "}_");
            txtContent = txtContent.Replace(" ", "");
            txtContent = txtContent.Replace("_{/td}_", "_{/td}_ ");
            txtContent = txtContent.Replace("_{/tr}_", "_{/tr}_ ");
            txtContent = txtContent.Replace("\r", "");
            txtContent = txtContent.Replace("\n", "");
            txtContent = txtContent.Replace("\t", "");


            string txtPattern = @"\b_{tr}__{td}_(?<drawno>\S*)_{/td}_\b", txtPattern2 = @"\b_{tdclass=""no_site""}_(?<result>\S*)_{/td}_\b";

            Dictionary<int, string> dicDrawNo = new Dictionary<int, string>(), dicResult = new Dictionary<int, string>();

            int num1 = 0, num2 = 0;

            MatchCollection matches = Regex.Matches(txtContent, txtPattern);
            MatchCollection matches2 = Regex.Matches(txtContent, txtPattern2);

            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                dicDrawNo.Add(num1, groups["drawno"].Value);
                num1 += 1;
            }

            foreach (Match match in matches2)
            {
                GroupCollection groups = match.Groups;
                dicResult.Add(num2, groups["result"].Value);
                num2 += 1;
            }

            for (int num = 0; num < num1; num += 1)
            {
                ViewBag.Content += "<div>" + dicDrawNo[num] + ": " + dicResult[num] + "</div>";
            }
            return View();
        }

        public ActionResult WebSiteParserTest2() {
            string txtContent = getHTML(@"http://www.wclc.com/winning-numbers/keno.htm?drawNum=854712");
            string[] arrContent = txtContent.Split(new string[] { "<table class=\"kenoTable\">" }, StringSplitOptions.None);
            txtContent = arrContent[1];
            arrContent = txtContent.Split(new string[] { "</table>" }, StringSplitOptions.None);
            txtContent = arrContent[0];
            arrContent = txtContent.Split(new string[] { "</tr>" }, StringSplitOptions.None);
            txtContent = arrContent[1];

            txtContent = txtContent.Replace("<tr>", "");
            txtContent = txtContent.Replace(" ", "");
            txtContent = txtContent.Replace("class=\"kenoDrawNumber\"", "");
            txtContent = txtContent.Replace("<", "_{");
            txtContent = txtContent.Replace(">", "}_");
            txtContent = txtContent.Replace("\r", "");
            txtContent = txtContent.Replace("\n", "");
            txtContent = txtContent.Replace("\t", "");
            txtContent = txtContent.Replace("{/td}_", "{/td}_ ");

            string txtPattern = @"\b_{td}_(?<num>\S*)_{/td}_\b", tmpString="";

            MatchCollection matches = Regex.Matches(txtContent, txtPattern);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                tmpString += groups["num"].Value + ",";
                
            }
            ViewBag.Content = tmpString.Substring(0, tmpString.Length - 1);
            //ViewBag.Content = txtContent;
            return View();
        }

        internal string getHTML(string Url) {
            string txtUrl = Url,
                txtContent = string.Empty;
            HttpWebResponse response = null;
            StreamReader readStream = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(txtUrl);
                request.MaximumAutomaticRedirections = 1;
                request.MaximumResponseHeadersLength = 4;
                request.Timeout = 30 * 1000; // if within x seconds still cannot scrape result, then give up

                // Set credentials to use for this request.
                request.Credentials = CredentialCache.DefaultCredentials;

                response = (HttpWebResponse)request.GetResponse();
                Stream receiveStream = response.GetResponseStream();
                readStream = new StreamReader(receiveStream, Encoding.UTF8);
                txtContent = readStream.ReadToEnd();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }

                if (readStream != null)
                {
                    readStream.Close();
                }
            }

            return txtContent;
        }
    }
}

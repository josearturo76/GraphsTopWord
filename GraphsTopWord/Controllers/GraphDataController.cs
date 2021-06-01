using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GraphsTopWord.Models;
using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;

namespace GraphsTopWord.Controllers
{
    public class GraphDataController : Controller
    {
        public static string txtSite = string.Empty;

        public ActionResult GraphMainView()
        {
            return View();
        }

        public ActionResult getColumnChart(ReadURL model)
        {
            txtSite = model.EnterURL;
            return PartialView("_ColumnChart");
        }

        public ActionResult getPieChart(ReadURL model)
        {
            txtSite = model.EnterURL;
            return PartialView("_PieChart");
        }

        public ActionResult getLineChart(ReadURL model)
        {
            txtSite = model.EnterURL;
            return PartialView("_LineChart");
        }

        [HttpPost]
        public ActionResult TopWordsResults() //ReadURL model
        {
            txtSite = Regex.IsMatch(txtSite, @"(http?://|https?://).*") ? txtSite : "https://" + txtSite;
            var EnterURL = txtSite;
            //Validate Site
            Uri uriResult;
            bool validURL = Uri.TryCreate(EnterURL, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            
            if (validURL)
            {
                return Json(GetResult(EnterURL), JsonRequestBehavior.AllowGet);
            }
            else
            {
                TempData["Message"] = "Invalid URL! Please, refresh the page and enter a valid WebSite!";
                return View(TempData["Message"]);
            }

        }

        public List<DataResult> GetResult(string EnterUrl)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            List<DataResult> lstTopTen = new List<DataResult>();
            try
            {
                var word = string.Empty;
                // Loading.
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = web.Load(EnterUrl);

                word = document.DocumentNode.SelectSingleNode("//body").InnerText;
                word = word.Replace("\n", "");

                var split = word.Split(' ');
                var repeatedWordCount = new Dictionary<string, int>();
                for (int i = 0; i < split.Length; i++)
                {
                    if (!String.IsNullOrEmpty(split[i]))
                    {
                        if (repeatedWordCount.ContainsKey(split[i]))
                        {
                            int value = repeatedWordCount[split[i]];
                            repeatedWordCount[split[i]] = value + 1;
                        }
                        else
                        {
                            repeatedWordCount.Add(split[i], 1);
                        }
                    }
                }

                foreach (KeyValuePair<string, int> kvp in repeatedWordCount.OrderByDescending(x => x.Value).Take(10))
                {
                    DataResult data = new DataResult();
                    // Setting Top 10
                    data.Word = kvp.Key;
                    data.Quantity = kvp.Value;

                    // Adding
                    lstTopTen.Add(data);
                }
                //Console.ReadKey();            // Top 10

            }
            catch (Exception ex)
            {
                // Info
                TempData["Message"] = ex.Message.ToString();
            }

            return lstTopTen;
        }

    }
}
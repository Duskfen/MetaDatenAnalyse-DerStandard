using System;
using System.Collections.Generic;
using System.IO;

using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

using Magazine_Structure;
using StringExtension;
using SeleniumExtension;

namespace DerStandard_Anwendung
{
    public static class MyExtension
    {
        public static void ExportToTSV(this Magazine magazine, string path="../", string name="DerStandard.tsv")
        {
            string isempty;
            
            if(path != "../")
            {
                if(path[path.Length-1]!= '\\')
                {
                    path += '\\';
                }
            }
            try
            {
                using (StreamReader sr = new StreamReader(path+name)) { isempty = sr.ReadLine(); } ;
            }
            catch
            {
                isempty = "";
            }
            using(StreamWriter sw = new StreamWriter(path+name, append:true))
            {
                if (isempty == "")
                {
                    sw.WriteLine("date\tresort\tauthor\tarticle\tarticle-cicker\tworkload-index\t");
                }

                foreach (Resort resort in magazine)
                {
                    foreach (Author author in resort.authors)
                    {
                        foreach (Article article in author.articles)
                        {
                            sw.WriteLine(article.PublishDate.ToString() + "\t" + resort.name + "\t" + author.Name + "\t" + article.Title + "\t" + article.ArticleKicker + "\t" + article.Length.ToString());
                        }
                    }
                }
            }
        }

        public static void LogErrorToTSV(this Exception exception, string additional = "")
        {
            using(StreamWriter sw = new StreamWriter("ErrorLog/Errorlog_" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" +  DateTime.Now.Year + ".tsv", append: true))
            {
                sw.WriteLine(DateTime.Now.ToString() + "\t" + exception.Message + "\t" + additional);
            }
        }
    }

    public class DerStandardAnalyse
    {
        string articleKicker;
        string title;
        DateTime date;
        List<string> authorname = new List<string>();
        int workload;

        public Magazine AnalyzeData(Magazine derStandard)
        {
            try 
            {
                FirefoxOptions options = new FirefoxOptions();
                options.SetPreference("dom.push.enabled", false);

                foreach (Resort item in derStandard)
                {
                    try
                    {
                        using (IWebDriver driver = new FirefoxDriver("../", options))
                        {
                            AnalyzeResort(item, driver);
                            driver.Close();
                            driver.Dispose();
                        }
                    }
                    catch (Exception e)
                    {
                        e.LogErrorToTSV("while analyzing resort " + item.name);
                    }
                }
            }
            catch (Exception e)
            {
                e.LogErrorToTSV("generally unknown Error");
            }

            return derStandard;
        }

        void AnalyzeResort(Resort resort, IWebDriver driver)
        {
            string content;
            driver.Url = resort.Url;

            try
            {
                driver.FindElement(By.ClassName("js-privacywall-agree")).Click(); //agreeing cookies

                IWebElement daily = driver.FindElement(By.CssSelector("section[data-id=\"" + resort.Container + "\"]"));

                List<IWebElement> articles = new List<IWebElement>();
                articles.AddRange(daily.FindElements(By.TagName("article")));

                foreach (IWebElement item in articles)
                {
                    try
                    {
                        driver.ClickOpenNewTab(item);

                        try
                        {
                            content = driver.PageSource;
                 
                            ProcessArticle(content);

                            if(driver.WindowHandles.Count >= 2)
                            {
                                driver.Close(); //close this tab
                                driver.SwitchTabs(0);
                            }

                            foreach (string name in authorname)
                            {
                                if (authorname.Count > 5)
                                {
                                    new Exception().LogErrorToTSV("To many Authors -> It's very likely that the extracting from the authorname didn't worked properly so the article" + title.Preview(20) + " was rejected. There were " + authorname.Count + " many authors");
                                    break;
                                }
                                string temp = "";
                                if (name != "" && date != null && workload != 0 && articleKicker != "" && name.Length <= 100)
                                {
                                    temp = name;
                                    temp = temp.ReplaceChars("", '\r', '\n', '\t');
                                    temp = temp.Trim(' ');

                                    if (temp != "") resort.AddArticle(temp, date, workload, title, articleKicker);
                                }
                                else new Exception().LogErrorToTSV("something was wrong with the article data so it was not added. Article: author: "+ temp.Preview(20) + ", date: " + date.ToString() +", workload: "+ workload.ToString() +", title: "+title.Preview(20)+", articleKicker: " + articleKicker.Preview(20));

                            }
                            authorname.Clear();
                        }
                        catch (Exception e)
                        {
                            e.LogErrorToTSV("while trying to get the content of the article: " + item.FindElement(By.TagName("a")).GetAttribute("href"));
                        }
                    }
                    catch (Exception e)
                    {
                        e.LogErrorToTSV("while opening the Article " + item.FindElement(By.TagName("a")).GetAttribute("href"));
                    }
                }
            }
            catch (Exception e)
            {
                e.LogErrorToTSV("while aggreeing the cookies or getting IWebElement article in resort " + resort.name);
            }
        }

        public void ProcessArticle(string content)
        {
            articleKicker = content.FindSubString("<h2 class=\"article-kicker\">", "</h2>");
            title = content.FindSubString("<h1 class=\"article-title\" itemprop=\"headline\">", "</h1>");
            title.ReplaceChars("\\\"", '"', '\''); //for compatibility with neo4j (if the title starts with a " or ' it has to end with one.. so i put in front of every " or ' a \) ;          
            
            string date = content.FindSubString("<p class=\"article-pubdate\">\r\n            <time datetime=\"", "\r\n\">"); 
            date = date.Replace('T', ' ');

            DateTime.TryParse(date, out this.date);

            workload = (content.FindSubString("<div class=\"article-body\">", "<nav class=\"story-tool js-article-tool\">").Length); 

            //multiple cases (the name of the author is not always on the right bzw same position...)
            authorname.AddRange(content.FindSubString("<div class=\"article-origins\">", "<br>").Split(','));

            if(authorname.Count >= 1)
            {
                if (authorname[0] == "")
                {
                    authorname.Clear();
                    authorname.AddRange(content.FindSubStringReversed("<p>", "<aside data-section-type=\"supplemental\" data-type=\"supplemental\">").FindSubStringReversed("(", ")").Split(','));
                    authorname.RemoveAt(authorname.Count - 1); //don't want the day part

                    if (authorname.Count == 0)
                    {
                        authorname.Clear();
                        authorname.AddRange(content.FindSubStringReversed("<p>", "<nav class=\"story-tool js-article-tool\">").FindSubStringReversed("(", ")").Split(','));
                        authorname.RemoveAt(authorname.Count - 1);
                    }
                }
            }
            else
            {
                    authorname.AddRange(content.FindSubStringReversed("<p>", "<aside data-section-type=\"supplemental\" data-type=\"supplemental\">").FindSubStringReversed("(", ")").Split(','));
                    authorname.RemoveAt(authorname.Count - 1); //don't want the day part

                    if (authorname.Count == 0)
                    {
                        authorname.Clear();
                        authorname.AddRange(content.FindSubStringReversed("<p>", "<nav class=\"story-tool js-article-tool\">").FindSubStringReversed("(", ")").Split(','));
                        authorname.RemoveAt(authorname.Count - 1);
                    }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Magazine_Structure
{
    public class Article
    {
        public string Resort { get; private set; }
        public int Length { get; private set; }
        public DateTime PublishDate { get; private set; }

        public string ArticleKicker { get; private set; }
        public string Title { get; private set; }

        public Article(string resort, int length, DateTime publishDate, string title, string articleKicker)
        {
            Resort = resort;
            Length = length;
            PublishDate = publishDate;
            Title = title;
            ArticleKicker = articleKicker;
        }
    }
}

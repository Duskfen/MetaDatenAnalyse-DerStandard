using System;
using System.Collections.Generic;

namespace Magazine_Structure
{
    public class Author
    {
        public List<Article> articles = new List<Article>();
        public string Name { get; private set; }

        public Author(string name)
        {
            Name = name;
        }

        public void AddArticle(Article article)
        {
            articles.Add(article);
        }

    }
}
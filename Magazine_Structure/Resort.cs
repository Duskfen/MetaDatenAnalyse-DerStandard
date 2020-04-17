using System;
using System.Collections.Generic;
using System.Text;

namespace Magazine_Structure
{
    public class Resort
    {
        public List<Author> authors = new List<Author>();
        public string name;
        public string Url;
        public string Container;

        public Resort(string name, string Url)
        {
            this.name = name;
            this.Url = Url;
            this.Container = "s2";
        }
        public Resort(string name, string Url, string container)
        {
            this.name = name;
            this.Url = Url;
            this.Container = container;
        }

        public void AddArticle(string authorname, DateTime publishDate, int length, string title, string articleKicker)
        {
            if(this[authorname].articles.Contains(new Article(name, length, publishDate, title, articleKicker)) == false)
            {
                this[authorname].AddArticle(new Article(name, length, publishDate, title, articleKicker)); //add an article to an author or create a new author and add an article to them
            }
        }

        /// <summary>
        /// A Getter; returns an existing author or creates a new one and returns it
        /// </summary>
        /// <param name="name">Authorname</param>
        /// <returns>the Author which has the indexed name</returns>
        public Author this[string name]
        {
            get
            {
                foreach (Author item in authors)
                {
                    if (item.Name == name)
                    {
                        return item;
                    }
                }
                authors.Add(new Author(name));
                return this[name];
            }
        }
    }
}

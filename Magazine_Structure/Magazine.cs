using System;
using System.Collections.Generic;
using System.Text;

namespace Magazine_Structure
{
    public class Magazine:List<Resort>
    {
        
        //public List<Resort> resorts = new List<Resort>();

        /// <summary>
        /// returns the resort which is named like the indexed string. If there is no Resort named like this, it'll return null;
        /// </summary>
        /// <param name="resortname"></param>
        public Resort this[string resortname]
        {
            get
            {
                resortname = resortname.ToLower();
                foreach (Resort item in this)
                {
                    if (item.name == resortname) return item;
                }
                return null;
            }
        }


        public void Add(string name, string url)
        {
            name = name.ToLower();
            if (this[name] == null) base.Add(new Resort(name, url)); //if this is not a duplicate
            else throw new Exception("This Resort(name) is a duplicate"); //if this is a duplicate
        }
        public void Add(string name, string url, string container)
        {
            name = name.ToLower();
            if (this[name] == null) base.Add(new Resort(name, url, container)); //if this is not a duplicate
            else throw new Exception("This Resort(name) is a duplicate"); //if this is a duplicate
        }

        /// <summary>
        /// better you use Add(string name, string url) instead
        /// </summary>
        /// <param name="resort"></param>
        public new void Add(Resort resort)
        {
            Add(resort.name, resort.Url);
        }

    }
}

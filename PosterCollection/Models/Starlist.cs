using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterCollection.Models
{
    public class Star
    {
        public int id;
        public string title;
        public string imagepath;
        public string posterpath;
        public string comment;
        public int type;
        public Star(int id,string title,string imagepath,string posterpath,string comment, int type)
        {
            this.id = id;
            this.title = title;
            this.imagepath = imagepath;
            this.posterpath = posterpath;
            this.comment = comment;
            this.type = type;
        }
    }
}

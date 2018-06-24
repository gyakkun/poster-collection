using System.Collections.Generic;

namespace PosterCollection.Models
{
    public class Backdrop
    {
        public double aspect_ratio { get; set; }
        public string file_path { get; set; }
        public int height { get; set; }
        public string iso_639_1 { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
        public int width { get; set; }
    }

    public class Poster
    {
        public double aspect_ratio { get; set; }
        public string file_path { get; set; }
        public int height { get; set; }
        public string iso_639_1 { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
        public int width { get; set; }
    }

    public class Pictures
    {
        public int id { get; set; }
        public List<Backdrop> backdrops { get; set; }
        public List<Poster> posters { get; set; }
    }
}

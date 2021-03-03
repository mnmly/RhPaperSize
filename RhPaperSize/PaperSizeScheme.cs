using System;
using System.Collections.Generic;

namespace MNML
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Dimension
    {
        public List<int> mm { get; set; }
        public List<int> points { get; set; }
        public List<double> inches { get; set; }
    }

    public class Format
    {
        public string name { get; set; }
        public Dimension size { get; set; }
    }
    

    public class Paper
    {
        public string name { get; set; }
        public List<Format> formats { get; set; }
    }
}

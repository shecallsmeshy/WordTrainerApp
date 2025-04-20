using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace WordTrainerApp.Models
{
    public class WordEntry
    {
        public string ForeignWord { get; set; }
        public string Translation { get; set; }
        public string Category { get; set; }
    }
}


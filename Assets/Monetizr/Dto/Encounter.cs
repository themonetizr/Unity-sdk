using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Monetizr.Dto
{
    public class Encounter
    {
        public string trigger_type { get; set; }
        public string completion_status { get; set; }
        public string trigger_tag { get; set; }
        public string level_name { get; set; }
        public string difficulty_level_name { get; set; }
        public string difficulty_estimation { get; set; }
    }
}

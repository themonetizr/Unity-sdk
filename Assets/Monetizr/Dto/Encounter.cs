using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Monetizr.Dto
{
    [Serializable]
    public class Encounter
    {
        public string trigger_type ;
        public string completion_status ;
        public string trigger_tag ;
        public string level_name ;
        public string difficulty_level_name ;
        public string difficulty_estimation ;
    }
}

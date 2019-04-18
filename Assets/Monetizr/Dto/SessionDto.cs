using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Monetizr.Dto
{
    public class SessionDto
    {
        public string device_identifier { get; set; }
        public DateTime session_start { get; set; }
        public DateTime session_end { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monetizr.Dto
{
    [Serializable]
    public class SessionDto
    {
        public string device_identifier ;
        public DateTime session_start ;
        public DateTime session_end ;
    }
}

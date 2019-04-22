using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Monetizr.Dto
{
    [Serializable]
    public class SessionDto
    {
        public string device_identifier ;
        public DateTime session_start ;
        public DateTime session_end ;
    }
}

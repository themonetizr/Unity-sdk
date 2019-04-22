using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Monetizr.Dto
{
    [Serializable]
    public class DeviceData
    {
        public string device_name ;
        public string os_version ;
        public string region ;
        public string language ;
        public string device_identifier ;
    }
}

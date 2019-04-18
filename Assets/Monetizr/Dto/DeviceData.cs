using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Monetizr.Dto
{

    public class DeviceData
    {
        public string device_name { get; set; }
        public string os_version { get; set; }
        public string region { get; set; }
        public string language { get; set; }
        public string device_identifier { get; set; }
    }
}

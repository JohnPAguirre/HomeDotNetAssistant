using System;
using System.Collections.Generic;
using System.Text;

namespace AutomaticSwitchTimer.ResponseDTO
{
    public class ServiceCallDTO
    {
        public int id { get; set; }
        public string type { get; set; }
        public string domain { get; set; }
        public string service { get; set; }
        public ServiceData service_data { get; set; }
    }
    public class ServiceData
    {
        public string entity_id { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskKiller.Models
{
    public class Service
    {
        public string ServiceName { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public string PID { get; set; }
        public string MachineName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GageStatsAgent.Resources
{
    public class NLDISettings
    {
        public string baseurl { get; set; }
        public Dictionary<string, string> resources { get; set; }
    }
}
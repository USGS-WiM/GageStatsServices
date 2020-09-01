using System;
using System.Collections.Generic;
using WIM.Resources;
using WIM.Hypermedia;
using System.ComponentModel.DataAnnotations.Schema;

namespace GageStatsDB.Resources
{
    public partial class Agency : IHypermedia {[NotMapped] public List<WIM.Resources.Link> Links { get; set; } }
    public partial class Characteristic : IHypermedia {[NotMapped] public List<WIM.Resources.Link> Links { get; set; } }
    public partial class Station : IHypermedia {[NotMapped] public List<WIM.Resources.Link> Links { get; set; } }
    public partial class StationType : IHypermedia {[NotMapped] public List<WIM.Resources.Link> Links { get; set; } }
    public partial class Statistic : IHypermedia {[NotMapped] public List<WIM.Resources.Link> Links { get; set; } }
}

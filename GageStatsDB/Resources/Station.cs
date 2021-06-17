//------------------------------------------------------------------------------
//----- Resource ---------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   Simple Plain Old Class Object (POCO) 
//
//discussion:   POCO's arn't derived from special base classed nor do they return any special types for their properties.
//              
//
//     

using NpgsqlTypes;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using SharedDB.Resources;

namespace GageStatsDB.Resources
{
    public partial class Station
    {
        [Required][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public int AgencyID { get; set; }
        [Required]
        public string Name { get; set; }     
        public bool? IsRegulated { get; set; }
        [Required]
        public int StationTypeID { get; set; }
        [Required]
        public Geometry Location { get; set; }
        public string LocationSource { get; set; }
        public int? RegionID { get; set; }
        public ICollection<Statistic> Statistics { get; set; }
        public ICollection<Characteristic> Characteristics { get; set; }
        public Agency Agency { get; set; }
        public StationType StationType { get; set; }
        public Region Region { get; set; }
        [NotMapped]
        public virtual string? Direction { get; set; }
    }
}

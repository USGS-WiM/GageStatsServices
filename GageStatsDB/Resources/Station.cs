//------------------------------------------------------------------------------
//----- Resource ---------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WiM - USGS

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
using System.Collections.Generic;

namespace GageStatsDB.Resources
{
    public partial class Station
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string AgencyID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public bool IsRegulated { get; set; }
        [Required]
        public int StationTypeID { get; set; }
        [Required]
        public Point Location { get; set; }

        public List<Statistic> Statistic { get; set; }

        public Agency Agency { get; set; }

        public StationType StationType { get; set; }
    }
}

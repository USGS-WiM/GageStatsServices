//------------------------------------------------------------------------------
//----- Resource ---------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2019 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   Simple Plain Old Class Object (POCO) 
//
//discussion:   POCO's arn't derived from special base classed nor do they return any special types for their properties.
//              
//
//   

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SharedDB.Resources;

namespace GageStatsDB.Resources
{
    public partial class Characteristic
    {
        [Required][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public int StationID { get; set; }
        [Required]
        public int VariableTypeID { get; set; }
        [Required]
        public int UnitTypeID { get; set; }
        public int? CitationID { get; set; }
        [Required]
        public double Value { get; set; }
        public string Comments { get; set; }

        public virtual VariableType VariableType { get; set; }
        public virtual UnitType UnitType { get; set; }
        public virtual Citation Citation { get; set; }
    }
}
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

using SharedDB.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GageStatsDB.Resources
{
    public partial class Statistic
    {
        [Required][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public int StatisticGroupTypeID  { get; set; }
        [Required]
        public int RegressionTypeID { get; set; }
        [Required]
        public int StationID { get; set; }
        [Required]
        public double Value { get; set; }
        [Required]
        public int UnitTypeID { get; set; }
        public string Comments { get; set; }
        public double? YearsofRecord { get; set; }
        public int? CitationID { get; set; }
        public int? PredictionIntervalID { get; set; }

        public ICollection<StatisticError> StatisticErrors { get; set; }
        public virtual Citation Citation { get; set; }
        public virtual UnitType UnitType { get; set; }
        public virtual StatisticGroupType StatisticGroupType { get; set; }
        public virtual RegressionType RegressionType { get; set; }
        public virtual Station Station { get; set; }
        public virtual PredictionInterval PredictionInterval { get; set; }
    }
}
﻿//------------------------------------------------------------------------------
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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GageStatsDB.Resources
{
    public partial class StatisticErrors
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int UnitTypeID { get; set; }
        [Required]
        public int ErrorTypeID { get; set; }
        [Required]
        public double Value { get; set; }

        public Statistic Statistic { get; set; }


    }
}

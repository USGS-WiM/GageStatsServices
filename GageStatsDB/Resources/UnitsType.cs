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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GageStatsDB.Resources
{
    public partial class UnitsType
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public int Unit { get; set; }

        public string Abbr { get; set; }

        [Required]
        public int UnitSystemTypeID { get; set; }

    }
}

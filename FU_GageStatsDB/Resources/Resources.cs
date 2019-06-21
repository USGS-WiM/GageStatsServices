using GageStatsDB.Resources;
using NetTopologySuite.Geometries;
using SharedDB.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace FU_GageStatsDB.Resources
{
    public class FU_Station:Station {
        public string Agency_cd { get; set; }
        public string StationTypeCode { get; set; }
       
        //Statistics
        public string StatisticDefType { get; set; }
        public string StatisticCode { get; set; }
        public double StatisticValue { get; set; }
        public Double? StatisticYears { get; set; }
        public double? StatisticError { get; set; }
        public double? StatisticVariance { get; set; }
        public double? StatisticLowerCI { get; set; }
        public double? StatisticUpperCI { get; set; }
        public string StatisticComments { get; set; }
        public string StatisticUnitAbbr { get; set; }
        public string Citation { get; set; }
        public string CitationURL { get; set; }



        public static FU_Station FromDataReader(System.Data.IDataReader r)
        {
            return new FU_Station()
            {
                Name = r.GetDataType<string>("Name"),
                Code = r.GetDataType<string>("Code"),
                Location = new Point(r.GetDataType<double>("Longitude",-99.9), r.GetDataType<double>("Latitude",-99.9)),
                Agency_cd = r.GetDataType<string>("Agency_cd"),
                StationTypeCode = r.GetDataType<string>("StationTypeCode"),
                //StatisticValue = r.GetDataType<double>("StatisticValue"),
                //StatisticYears = r.GetDataType<double?>("YearsRec"),
                //StatisticError = r.GetDataType<double?>("StdError"),
                //StatisticVariance = r.GetDataType<double?>("Variance"),
                //StatisticLowerCI = r.GetDataType<double?>("LowerCI"),
                //StatisticUpperCI = r.GetDataType<double?>("UpperCI"),
                //StatisticComments = r.GetDataType<string>("StatStartDate"), 
                //StatisticDefType = r.GetDataType<string>("StatisticDefType"),
                //StatisticCode = r.GetDataType<string>("StatLabelCode"),
                //StatisticUnitAbbr = r.GetDataType<string>("UnitAbbr"),
                //Citation = r.GetDataType<string>("Citation"),
                //CitationURL = r.GetDataType<string>("CitationURL")  
            };

        }
    }
    public class GageStatsStatisticGroupType : StatisticGroupType
    {
        public static GageStatsStatisticGroupType FromDataReader(System.Data.IDataReader r)
        {
            return new GageStatsStatisticGroupType()
            {
                ID = Convert.ToInt32(r["ID"]),
                Code = r["Code"].ToString(),
                Name = r["Name"].ToString(),
            };

        }
    }
    public class GageStatsStationType : StationType
    {
        public static GageStatsStationType FromDataReader(System.Data.IDataReader r)
        {
            return new GageStatsStationType()
            {
                ID = Convert.ToInt32(r["ID"]),                
                Code = r["Code"].ToString(),
                Name = r["Name"].ToString()
            };

        }
    }
    public class GageStatsVariableType : VariableType
    {
        public static GageStatsVariableType FromDataReader(System.Data.IDataReader r)
        {
            return new GageStatsVariableType()
            {
                ID = Convert.ToInt32(r["ID"]),
                Code = r["Code"].ToString(),
                Name = r["Name"].ToString(),
                Description = r["Description"].ToString()
            };

        }
    }
    public class GagesStatsUnitType : UnitType
    {
        public static GagesStatsUnitType FromDataReader(System.Data.IDataReader r)
        {
            return new GagesStatsUnitType()
            {
                ID = Convert.ToInt32(r["ID"]),
                Abbreviation = r["Abbreviation"].ToString(),
                Name = r["Name"].ToString(),
                UnitSystemTypeID = Convert.ToInt32(r["UnitSystemTypeID"])
            };

        }
    }
    public class GageStatsRegressionType : RegressionType
    {
        public static GageStatsRegressionType FromDataReader(System.Data.IDataReader r)
        {
            return new GageStatsRegressionType()
            {
                ID = Convert.ToInt32(r["ID"]),
                Code = r["Code"].ToString(),
                Name = r["Name"].ToString(),
                Description = r["Description"].ToString()
            };
        }
    }
    public class FUString
    {
        public string Value { get; set; }

        public static FUString FromDataReader(System.Data.IDataReader r)
        {
            return new FUString()
            {
                Value = r[0] is DBNull ? "" : Convert.ToString(r[0])
            };

        }


    }
    public class FUInt
    {
        public Int32 Value { get; set; }

        public static FUInt FromDataReader(System.Data.IDataReader r)
        {
            return new FUInt()
            {
                Value = r[0] is DBNull ? -1 : Convert.ToInt32(r[0])
            };

        }


    }
}

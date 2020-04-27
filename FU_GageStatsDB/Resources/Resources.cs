using GageStatsDB.Resources;
using NetTopologySuite.Geometries;
using SharedDB.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace FU_GageStatsDB.Resources
{
    public interface FUArgs {
        object[] ToArgs();
    }
    public class FU_Station:Station {
        public string Agency_cd { get; set; }
        public string StationTypeCode { get; set; }    

        public static FU_Station FromDataReader(System.Data.IDataReader r)
        {
            return new FU_Station()
            {
                Name = r.GetDataType<string>("Name"),
                Code = r.GetDataType<string>("Code"),
                Location = new Point(r.GetDataType<double>("Longitude",-99.9), r.GetDataType<double>("Latitude",-99.9)),
                Agency_cd = r.GetDataType<string>("Agency_cd"),
                StationTypeCode = r.GetDataType<string>("StationTypeCode"),

            };

        }
    }
    public class FU_Citation:Citation {
        
        public static FU_Citation FromDataReader(System.Data.IDataReader r)
        {
            return new FU_Citation()
            {   
                Title = splitTitle(r.GetDataType<string>("Citation", ""), true),
                Author = splitTitle(r.GetDataType<string>("Citation", ""), false),
                CitationURL = r.GetDataType<string>("CitationURL")
            };

        }
        private static string splitTitle(string authortitle, bool getTitle = false)
        {
            try
            {
                Int32 splitlocation = authortitle.IndexOfAny("0123456789".ToCharArray());
                if (!getTitle)
                    //auther
                    return authortitle.Substring(0, splitlocation - 1).Trim();
                else
                    //title
                    return authortitle.Substring(splitlocation).Trim();
            }
            catch (Exception)
            {
                return authortitle;
            }
        }
    }
    public class FU_Statistics {
        //Statistics
        public string StatisticDefType { get; set; }
        public string StatisticCode { get; set; }
        public string StatisticTypeCode { get; set; }
        public double StatisticValue { get; set; }
        public Double? StatisticYears { get; set; }
        public double? StatisticError { get; set; }
        public double? StatisticVariance { get; set; }
        public double? StatisticLowerCI { get; set; }
        public double? StatisticUpperCI { get; set; }
        public DateTime? StatisticStartDate { get; set; }
        public DateTime? StatisticEndDate { get; set; }
        public string StatisticRemarks { get; set; }
        public string StatisticUnitAbbr { get; set; }
        public bool StatisticIsPreferred { get; set; }
        public FU_Citation Citation { get; set; }


        public static FU_Statistics FromDataReader(System.Data.IDataReader r)
        {
            return new FU_Statistics()
            {
                StatisticValue = r.GetDataType<double>("StatisticValue"),
                StatisticYears = r.GetDataType<double?>("YearsRec"),
                StatisticError = r.GetDataType<double?>("StdError"),
                StatisticVariance = r.GetDataType<double?>("Variance"),
                StatisticLowerCI = r.GetDataType<double?>("LowerCI"),
                StatisticUpperCI = r.GetDataType<double?>("UpperCI"),
                StatisticStartDate = r.GetDataType<DateTime?>("StatStartDate"),
                StatisticEndDate = r.GetDataType<DateTime?>("StatEndDate"),
                StatisticRemarks = r.GetDataType<string>("StatisticRemarks"),
                StatisticDefType = r.GetDataType<string>("StatisticDefType"),
                StatisticCode = r.GetDataType<string>("StatLabelCode"),
                StatisticTypeCode = r.GetDataType<string>("StatisticTypeCode"),
                StatisticUnitAbbr = r.GetDataType<string>("UnitAbbr"),
                StatisticIsPreferred = r.GetDataType<bool>("IsPreferred"),
                Citation = FU_Citation.FromDataReader(r)
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
    public class GageStatsAgency : Agency
    {
        public static GageStatsAgency FromDataReader(System.Data.IDataReader r)
        {
            return new GageStatsAgency()
            {
                ID = r.GetDataType<Int32>("ID"),
                Code = r.GetDataType<string>("Code"),
                Name = r.GetDataType<string>("Name"),
                Description = r.GetDataType<string>("Description")
            };
        }
    }
    public class GageStatsCitations : Citation
    {
        public static GageStatsCitations FromDataReader(System.Data.IDataReader r)
        {
            return new GageStatsCitations()
            {
                ID = r.GetDataType<Int32>("ID"),
                Title = r.GetDataType<string>("Title"),
                Author = r.GetDataType<string>("Author"),
                CitationURL = r.GetDataType<string>("CitationURL")
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

//------------------------------------------------------------------------------
//----- postgresqldbOps -------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2015 WiM - USGS

//    authors:  Jeremy K. Newson USGS Wisconsin Internet Mapping
//             
// 
//   purpose: Manage databases, provides retrieval/creation/update/deletion
//          
//discussion:
//

#region "Comments"
//02.09.2015 jkn - Created
#endregion

#region "Imports"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.Data.Odbc;
using Npgsql;
using WIM.Utilities;
using WIM.Utilities.Extensions;
using FU_GageStatsDB.Resources;
using GageStatsDB.Resources;
#endregion

namespace FU_GageStatsDB
{
    public class GageStatsDbOps : dbOps
    {
        #region "Fields"
        public ConnectionType connectionType { get; private set; }
        #endregion
        #region Constructors
        public GageStatsDbOps(string pSQLconnstring, ConnectionType pConnectionType)
            : base()
        {
            this.connectionType = pConnectionType;
            init(pSQLconnstring);
        }
        #endregion        
        #region "Methods"
        public IEnumerable<T> GetItems<T>(SQLType type, params object[] args)
        {
            string sql = string.Empty;
            try
            {
                sql = string.Format(getSQL(type), args);
                return base.GetItems<T>(sql);
            }
            catch (Exception ex)
            {
                this.sm(ex.Message);
                throw ex;
            }
        }
        public bool Update(SQLType type, Int32 pkID, Object[] args)
        {

            string sql = string.Empty;
            try
            {
                sql = String.Format(getSQL(type), pkID, args);
                return base.Update(sql);
            }
            catch (Exception ex)
            {
                this.sm(ex.Message);
                return false;
            }
        }
        public Int32 AddItem(SQLType type, Object[] args)
        {
            string sql = string.Empty;
            try
            {
                args = args.Select(a => a == null ? "null" : a).ToArray();
                sql = String.Format(getSQL(type), args);
                return base.AddItem(sql);

            }
            catch (Exception ex)
            {
                this.sm(ex.Message);
                return -1;
            }
            finally
            {
                this.CloseConnection();
            }
        }
        public void AddItems(SQLType type, IEnumerable<object[]> items, object[] args)
        {
            List<string> sql = new List<string>();
            try
            {
                args = args.Select(a => a == null ? "null" : a).ToArray();
                items = items.Select(i => i.Select(a => a == null ? "null" : a).ToArray());
                foreach (var item in items)
                {
                    sql.Add(String.Format(getSQL(type), item.Concat(args).ToArray()));
                }


                var sqltopush = string.Join("\n", sql);
                ExecuteSql(sqltopush);

            }
            catch (Exception ex)
            {
                this.sm(ex.Message);
                throw ex;
            }
        }
        public void AddItems(IEnumerable<Statistic> items)
        {
            List<string> sql = new List<string>();
            try
            {
                /*
                 WITH predictioninterval as (Select Null as "ID"),
                    stats as (INSERT INTO "gagestats"."Statistics"("StatisticGroupTypeID","RegressionTypeID","StationID","Value","UnitTypeID","Comments","YearsofRecord","CitationID","PredictionIntervalID") 
                                                    SELECT 21,503,1,0,44,'',Null,122,CAST(p."ID" as INT) FROM predictioninterval as p RETURNING "ID")
                    INSERT INTO "gagestats"."StatisticError"("StatisticID","ErrorTypeID","Value") SELECT stats."ID", 1,123.5 FROM stats;
																	 
                 */
                foreach (var item in items)
                {
                    
                    string predIntervalsql = item.PredictionInterval != null? 
                        String.Format(getSQL(SQLType.e_predictioninterval), new object[] { item.PredictionInterval.LowerConfidenceInterval.HasValue? item.PredictionInterval.LowerConfidenceInterval.Value.ToString():"Null",
                                                                                           item.PredictionInterval.UpperConfidenceInterval.HasValue? item.PredictionInterval.UpperConfidenceInterval.Value.ToString():"Null",
                                                                                           item.PredictionInterval.Variance.HasValue? item.PredictionInterval.Variance.Value.ToString():"Null" }) : @"Select Null as ""ID""";
                    //""StatisticGroupTypeID"",""RegressionTypeID"",""StationID"",""Value"",""UnitTypeID"",""Comments"",""YearsOfRecord"",""CitationID"",""PredictionIntervalID""
                    string stats = String.Format(getSQL(SQLType.e_statistics), new object[]{ item.StatisticGroupTypeID, item.RegressionTypeID, item.StationID, item.Value, item.UnitTypeID, item.Comments, item.YearsofRecord.HasValue? item.YearsofRecord.Value.ToString() :"Null", item.CitationID.HasValue? item.CitationID.Value.ToString():"Null", item.IsPreferred });
                    //""StatisticID"",""ErrorTypeID"",""Value""

                    sql.Add($"WITH predictioninterval as ({predIntervalsql})");

                    if (item.StatisticErrors == null)
                        sql.Add(stats);
                    else
                        sql.Add(String.Format(getSQL(SQLType.e_statisticnerror), new object[] {stats, item.StatisticErrors.FirstOrDefault().ErrorTypeID, item.StatisticErrors.FirstOrDefault().Value }));

                    
                    sql.Add(";");
                    
                }

                var sqltopush = string.Join("\n", sql);
                ExecuteSql(sqltopush);

            }
            catch (Exception ex)
            {
                this.sm(ex.Message);
                throw ex;
            }
        }
        public bool ResetTables()
        {
            string sql = string.Empty;
            try
            {
                sql += @"TRUNCATE TABLE ""gagestats"".""Stations"" RESTART IDENTITY CASCADE;
                         TRUNCATE TABLE ""gagestats"".""Citations"" RESTART IDENTITY CASCADE;";

                ExecuteSql(sql);

                return true;
            }
            catch (Exception ex)
            {
                this.sm(ex.Message);
                return false;
                throw ex;
            }
        }
        #endregion
        #region "Helper Methods"
        protected override DbCommand getCommand(string sql)
        {
            switch (this.connectionType)
            {
                case ConnectionType.e_access:
                    return new OdbcCommand(sql, (OdbcConnection)this.connection);
                case ConnectionType.e_postgresql:
                    return new NpgsqlCommand(sql, (NpgsqlConnection)this.connection);
                //case ConnectionType.e_mysql:
                //return new MySqlCommand(sql, (MySqlConnection)this.connection);
                default:
                    return null;
            }
        }
        private string getSQL(SQLType type)
        {
            switch (this.connectionType)
            {
                case ConnectionType.e_access:
                    return getAccessSql(type);
                case ConnectionType.e_postgresql:
                case ConnectionType.e_mysql:
                    return getSql(type);
                default:
                    return "";
            }
        }
        private string getAccessSql(SQLType type)
        {
            string results = string.Empty;
            switch (type)
            {
                //station, stationtype, and agency
                case SQLType.e_station:
                    //results = @"SELECT TOP {0} *
                    //            FROM
                    //            (SELECT TOP {1}
                    //                s.StationName as Name, s.StaID as Code, s.Latitude, s.Longitude, s.Agency_cd, s.StationTypeCode,
                    //                stat.StatisticValue, stat.YearsRec, stat.StdError, stat.Variance, stat.LowerCI, stat.UpperCI, stat.StatStartDate, stat.StatEndDate,       
                    //                statType.DefType as StatisticDefType, statlab.Statlabel as StatLabelCode, u.EnglishAbbrev as UnitAbbr,                            
                    //                ds.Citation, ds.CitationURL
                    //            FROM (((((Station s
                    //            LEFT JOIN Statistic stat ON (stat.StaID = s.StaID))                               
                    //            LEFT JOIN StatLabel statlab ON (statlab.StatisticLabelID = stat.StatisticLabelID))
                    //            LEFT JOIN StatType statType ON (statType.StatisticTypeCode = statlab.StatisticTypeCode))
                    //            LEFT JOIN Units u ON (statlab.UnitID = u.UnitID))
                    //            LEFT JOIN DataSource ds ON (ds.DataSourceID = stat.DataSourceID))
                    //            WHERE IsNumeric(stat.StatisticValue))
                    //            ;     ";
                    results = @"SELECT s.StationName as Name, s.StaID as Code, s.Latitude, s.Longitude, s.Agency_cd, s.StationTypeCode
                                FROM Station s
                                WHERE s.Latitude Is Not Null AND s.Longitude Is Not Null AND
                                    s.StaID In 
                                      (
                                        SELECT TOP {0} A.StaID
                                        FROM [
                                               SELECT TOP {1} Station.*
                                               FROM Station
                                               ORDER BY Station.StaID DESC
                                             ]. AS A
                                        ORDER BY A.StaID ASC
                                      )                                      
                                ORDER BY s.StaID DESC;    ";
                    break;

                //statistic and Characteristic
                case SQLType.e_stationCount:
                    results = @"SELECT COUNT(*) AS Expr1 FROM Station s";
                    break;
                case SQLType.e_agency:
                    results = @"SELECT DISTINCT s.Agency_cd FROM Station s
                                    WHERE s.Latitude Is Not Null AND s.Longitude Is Not Null; ";
                    break;

                case SQLType.e_statistic_data:
                    results = @"SELECT s.StatisticValue, s.YearsRec, s.StdError, s.Variance, s.LowerCI, s.UpperCI, s.StatStartDate, s.StatEndDate, s.StatisticRemarks,
                                s.IsPreferred, ds.Citation, ds.CitationURL,
                                st.DefType as StatisticDefType, 
                                st.StatisticTypeCode,
                                sl.StatLabel as StatLabelCode,
                                u.EnglishAbbrev as UnitAbbr
                                FROM ((((Statistic s
                                LEFT JOIN StatLabel sl ON (s.StatisticLabelID = sl.StatisticLabelID))
                                LEFT JOIN DataSource ds ON (ds.DataSourceID = s.DataSourceID))
                                LEFT JOIN Units u ON (sl.UnitID = u.UnitID))
                                LEFT JOIN StatType st ON (sl.statisticTypeID = st.StatisticTypeID))                         
                                WHERE s.StaID = '{0}' AND IsNumeric(s.StatisticValue) AND sl.StatisticTypeCode <> 'D' ;";
                    break;

                case SQLType.e_citation:
                    results = @"SELECT DISTINCT ds.Citation, ds.CitationURL
                                FROM ((Statistic s
                                LEFT JOIN StatLabel sl ON (s.StatisticLabelID = sl.StatisticLabelID))
                                LEFT JOIN DataSource ds ON (ds.DataSourceID = s.DataSourceID))                        
                                WHERE IsNumeric(s.StatisticValue) AND sl.StatisticTypeCode <> 'D' ;";
                    break;
                case SQLType.e_regressiontype:
                    results = @"SELECT DISTINCT (0-1) as ID, sl.StatLabel as Code, sl.Definition as Description, sl.StatisticLabel as Name
                                FROM (Statistic s
                                LEFT JOIN StatLabel sl on (s.StatisticLabelID = sl.StatisticLabelID))
                                LEFT JOIN StatType st on (sl.statisticTypeID = st.StatisticTypeID)
                                WHERE st.DefType = 'FS';";
                    break;
                case SQLType.e_unittype:
                    results = @"SELECT DISTINCT MetricAbbrev FROM Units UNION SELECT EnglishAbbrev FROM Units";
                    break;
                case SQLType.e_variabletype:
                    //select all variables used in equations and report.
                    results = @"SELECT DISTINCT (0-1) as ID, sl.StatLabel as Code, sl.Definition as Description, sl.StatisticLabel as Name
                                FROM (Statistic s
                                LEFT JOIN StatLabel sl on (s.StatisticLabelID = sl.StatisticLabelID))
                                LEFT JOIN StatType st on (sl.statisticTypeID = st.StatisticTypeID)
                                WHERE st.DefType = 'BC';";
                    break;
                case SQLType.e_statisticgrouptype:
                    results = @"SELECT DISTINCT (0-1) as ID, st.StatisticTypeCode as Code, st.StatisticType as Name 
                                FROM StatType st WHERE st.DefType ='FS'";
                    break;
                case SQLType.e_stationtype:
                    results = @"SELECT DISTINCT (0-1) as ID, st.StationTypeCode as Code, st.StationType as Name
                                FROM StationType st;";
                    break;
                default:
                    sm("invalid sqltype");
                    break;
            }

            return results;
        }
        private string getSql(SQLType type)
        {
            string results = string.Empty;
            switch (type)
            {
                case SQLType.e_station:
                    //item.Code, agencyID, item.Name,item.IsRegulated, stationType, item.Location }
                    results = @"INSERT INTO ""gagestats"".""Stations""(""Code"",""AgencyID"",""Name"", ""IsRegulated"", ""StationTypeID"", ""Location"") 
                                    VALUES('{0}',{1},'{2}',{3},{4}, ST_SetSRID(ST_GeomFromText('{5}'),4236));";
                    break;
                case SQLType.e_postcitation:
                    results = @"INSERT INTO ""gagestats"".""Citations""(""Title"",""Author"",""CitationURL"") VALUES('{0}','{1}','{2}');";
                    break;                
               
                case SQLType.e_predictioninterval:
                    results = @"INSERT INTO ""gagestats"".""PredictionInterval""(""LowerConfidenceInterval"",""UpperConfidenceInterval"",""Variance"") 
                                VALUES({0},{1},{2}) RETURNING ""ID""";
                    break;               
                
                case SQLType.e_statistics:
                    results = @"INSERT INTO ""gagestats"".""Statistics""(""StatisticGroupTypeID"",""RegressionTypeID"",""StationID"",""Value"",""UnitTypeID"",""Comments"",""YearsofRecord"",""CitationID"",""IsPreferred"",""PredictionIntervalID"") 
                                SELECT {0},{1},{2},{3},{4},'{5}',{6},{7},{8},CAST(p.""ID"" as INT) FROM predictioninterval as p
                                RETURNING ""ID""";

                    break;
                case SQLType.e_statisticnerror:
                    results = @",stats as ({0}) INSERT INTO ""gagestats"".""StatisticError""(""StatisticID"", ""ErrorTypeID"", ""Value"") 
                                    SELECT stats.""ID"", {1},{2} FROM stats";
                    break;

                case SQLType.e_characteristics:
                    results = @"INSERT INTO ""gagestats"".""Characteristics""(""VariableTypeID"",""UnitTypeID"",""CitationID"",""Value"",""Comments"",""StationID"") VALUES({0},{1},{2},{3},'{4}',{5});";
                    break;

                case SQLType.e_getstatisticgroups:
                    results = @"SELECT * FROM ""gagestats"".""StatisticGroupType_view""";
                    break;
                case SQLType.e_getvariabletypes:
                    results = @"SELECT * FROM ""gagestats"".""VariableType_view""";
                    break;
                case SQLType.e_getunittypes:
                    results = @"SELECT * FROM ""gagestats"".""UnitType_view""";
                    break;
                case SQLType.e_getregressiontypes:
                    results = @"SELECT * FROM ""gagestats"".""RegressionType_view""";
                    break;
                case SQLType.e_stationtype:
                    results = @"SELECT * FROM ""gagestats"".""StationTypes""";
                    break;
                case SQLType.e_agency:
                    results = @"SELECT * FROM ""gagestats"".""Agencies""";
                    break;
                case SQLType.e_citation:
                    results = @"SELECT * FROM ""gagestats"".""Citations""";
                    break;
                default:
                    break;
            }
            return results;
        }

        protected void init(string connectionString)
        {
            switch (connectionType)
            {
                case ConnectionType.e_access:
                    this.connection = new OdbcConnection(connectionString);
                    break;
                case ConnectionType.e_postgresql:
                    this.connection = new NpgsqlConnection(connectionString);
                    break;
                case ConnectionType.e_mysql:
                    //this.connection = new MySqlConnection(connectionString);
                    break;
                default:
                    break;
            }

        }
        #endregion
        #region "Enumerated Constants"
        public enum SQLType
        {
            e_station,
            e_stationCount,
            e_agency,
            e_statistic_data,
            e_predictioninterval,
            e_statisticnerror,
            e_characteristics,
            e_statistics,
            e_stationtype,
            e_citation,

            e_postcitation,

            e_getstatisticgroups,
            e_statisticgrouptype,
            e_getvariabletypes,
            e_variabletype,
            e_getunittypes,
            e_unittype,
            e_getregressiontypes,
            e_regressiontype,
            e_errortype            
        }
        public enum ConnectionType
        {
            e_access,
            e_postgresql,
            e_mysql
        }
        #endregion

    }
}
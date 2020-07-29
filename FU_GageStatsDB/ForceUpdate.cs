//------------------------------------------------------------------------------
//----- ForceUpdate -------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2019 WiM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//             
// 
//   purpose: Forces and update to Streamstats from an access SSDB
//          
//discussion: 
//Access connection using odbc https://mrojas.ghost.io/msaccess-in-dotnetcore/
// must download and install https://www.microsoft.com/en-us/download/details.aspx?id=13255 for some reason, even if you have access installed.

#region "Comments"
//10.31.2016 jkn - Created
#endregion

#region "Imports"
using FU_GageStatsDB.Resources;
using GageStatsDB.Resources;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SharedDB.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
#endregion
namespace FU_GageStatsDB
{
    public class ForceUpdate
    {
        #region properties
        private List<string> _message = new List<string>();
        public List<string> Messages
        {
            get { return _message; }
        }
        public List<StatisticGroupType> statisticGroupTypeList { get; private set; }
        public List<VariableType> variableTypeList { get; private set; }
        public List<UnitType> unittypeList { get; private set; }
        public List<RegressionType> regressionTypeList { get; private set; }
        public List<StationType> stationTypeList { get; private set; }
        public List<Agency> agencies { get; private set; }

        private string SSDBConnectionstring;
        private string GagesStatsDBConnectionstring;
        
        #endregion
        #region Constructors
        public ForceUpdate(string dbusername, string dbpassword, string accessdb)
        {
            SSDBConnectionstring = string.Format(@"Driver={{Microsoft Access Driver (*.mdb)}};dbq={0}", accessdb);
            GagesStatsDBConnectionstring = string.Format("Server=test.c69uuui2tzs0.us-east-1.rds.amazonaws.com; database={0}; UID={1}; password={2}", "StatsDB", dbusername, dbpassword);

            init();
        }
        #endregion

        #region Methods
        public bool VerifyLists()
        {
            List<string> DBUnitAbbr = this.unittypeList.Select(k => k.Abbreviation.Trim()).ToList();
            List<string> DBVariableList = this.variableTypeList.Select(vt => vt.Code.Trim().ToUpper()).ToList();
            List<string> DBStatisticGroupList = this.statisticGroupTypeList.Select(vt => vt.Code.Trim().ToUpper()).ToList();
            List<string> DBregressionList = this.regressionTypeList.Select(vt => vt.Code.Trim().ToUpper()).ToList();
            List<string> DBstationTypeList = this.stationTypeList.Select(vt => vt.Code.Trim().ToUpper()).ToList();
            List<string> DBAgencyList = this.agencies.Select(a => a.Code.Trim().ToUpper()).ToList();
           

            List<string> ssdbUnitAbbr = null;
            List<GageStatsVariableType> ssdbDBVariableList = null;
            List<GageStatsStatisticGroupType> ssdbStatisticGroupList = null;
            List<GageStatsRegressionType> ssdbRegressionList = null;
            List<GageStatsStationType> ssdbStationTypeList = null;
            List<string> ssAgencyList = null;

            using (var ssdb = new GageStatsDbOps(SSDBConnectionstring, GageStatsDbOps.ConnectionType.e_access))
            {
                ssdbUnitAbbr = ssdb.GetItems<FUString>(GageStatsDbOps.SQLType.e_unittype).Select(f => f.Value.Trim()).ToList();
                ssdbDBVariableList = ssdb.GetItems<GageStatsVariableType>(GageStatsDbOps.SQLType.e_variabletype).ToList();
                ssdbStatisticGroupList = ssdb.GetItems<GageStatsStatisticGroupType>(GageStatsDbOps.SQLType.e_statisticgrouptype).ToList();
                ssdbRegressionList = ssdb.GetItems<GageStatsRegressionType>(GageStatsDbOps.SQLType.e_regressiontype).ToList();
                ssdbStationTypeList = ssdb.GetItems<GageStatsStationType>(GageStatsDbOps.SQLType.e_stationtype).ToList();
                ssAgencyList = ssdb.GetItems<FUString>(GageStatsDbOps.SQLType.e_agency).Select(e=>e.Value).ToList();
            }//end using

            var diffUnits = ssdbUnitAbbr.Except(DBUnitAbbr).ToList();
            var diffVariable = ssdbDBVariableList.Where(v=>!DBVariableList.Contains(v.Code.Trim().ToUpper())).ToList();
            var diffSG = ssdbStatisticGroupList.Where(sg=>!DBStatisticGroupList.Contains(sg.Code.Trim().ToUpper())).ToList();
            var diffRegList = ssdbRegressionList.Where(r => !DBregressionList.Contains(r.Code.Trim().ToUpper())).ToList();
            var diffStationtypeList = ssdbStationTypeList.Where(r => !DBstationTypeList.Contains(r.Code.Trim().ToUpper())).ToList();
            var diffAgencies = ssAgencyList.Where(r => !DBAgencyList.Contains(r.Trim().ToUpper())).ToList();

            if (diffVariable.Count > 0) createUpdateList(diffVariable);
            if (diffRegList.Count > 0) createUpdateList(diffRegList);
            if (diffSG.Count > 0) createUpdateList(diffSG);
            if (diffStationtypeList.Count > 0) createUpdateList(diffStationtypeList);
            //if (diffAgencies.Count > 0) createUpdateList(diffAgencies);

            return diffUnits.Count < 2 && diffVariable.Count < 1 && diffSG.Count < 1 && diffRegList.Count < 1 && diffStationtypeList.Count <1;
        }
        public void Load() {
            try
            {
                sm("Starting migration " + DateTime.Today.ToShortDateString());
                using (var ssdb = new GageStatsDbOps(SSDBConnectionstring, GageStatsDbOps.ConnectionType.e_access))
                {
                    using (var gsDBOps = new GageStatsDbOps(GagesStatsDBConnectionstring, GageStatsDbOps.ConnectionType.e_postgresql))
                    {
                        // COMMENT OUT if rerunning script
                        gsDBOps.ResetTables();
                
                        bool DBcontainsMoreRecords = true;
                        var stationcount = ssdb.GetItems<FUInt>(GageStatsDbOps.SQLType.e_stationCount).FirstOrDefault().Value;

                        sm("Uploading Citations");
                        //citations // COMMENT OUT next line if rerunning script so citations don't duplicate
                        gsDBOps.AddItems(GageStatsDbOps.SQLType.e_postcitation, ssdb.GetItems<FU_Citation>(GageStatsDbOps.SQLType.e_citation).Select(c=> new object[] { c.Title, c.Author, c.CitationURL }),new object[] { });
                        List<GageStatsCitations> citationlist = gsDBOps.GetItems<GageStatsCitations>(GageStatsDbOps.SQLType.e_citation, new object[] { }).ToList();

                        Int32 limit = 1000;
                        Int32 offset = 0;
                        Int32 currentcount = 0;
                        sm("Uploading Stations");
                        while (DBcontainsMoreRecords)
                        {
                            sm($"LIMIT: {limit} Offset: {offset} ");
                            if ((stationcount - offset) < limit)
                            {
                                limit = (stationcount - offset);
                                DBcontainsMoreRecords = false;
                            }//endif
                             //#warning  TODO Impove method with threading and parallelizm
                             // improvements https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.parallel.foreach?view=netcore-2.2
                             //https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.parallel.for?view=netcore-2.2
                            foreach (var item in ssdb.GetItems<FU_Station>(GageStatsDbOps.SQLType.e_station, limit, limit + offset))
                            {
                                currentcount++;
                                //Processing Station 06934600 15613 / 36683 - comma in stats value
                                //Processing Station 06485500 17172/36683 - text in stats year 73 (123)
                                //Processing Station 06479640 17225 / 36683 - text in stats year 29 (33)
                                //Processing Station 06479515 17230/36683 - text in stats year 29 (30)
                                //Processing Station 06479438 17238 / 36683 - text in stats year 29 (121)
                                //Processing Station 06473500 17305 / 36683 - text in stats year 29 (55)
                                //Processing Station 06445685 17456 / 36683 - TextReader in stats year (54)

                                //Processing Station 01656600 32323/36683
                                //if (currentcount < 28788) continue;
                                sm($"Processing Station {item.Code} {currentcount}/{stationcount}");
                                if (string.IsNullOrEmpty(item.Name)) item.Name = "Undefined in Database";
                                //POST Station
                                var agency = this.agencies.FirstOrDefault(e => String.Equals(e.Code, item.Agency_cd, StringComparison.OrdinalIgnoreCase))?? this.agencies.FirstOrDefault(e=>string.Equals(e.Name, "Undefined"));
                                var stationType = this.stationTypeList.FirstOrDefault(e => String.Equals(e.Code, item.StationTypeCode))?? this.stationTypeList.FirstOrDefault(st=>string.Equals(st.Name, "Undefined"));

                                var existingStations = gsDBOps.GetItems<GageStatsStations>(GageStatsDbOps.SQLType.e_getstations, new object[] { });
                                // if station already exists (useful when running function to update citations)
                                var currentStation = existingStations.FirstOrDefault(s => s.Code == item.Code);
                                if (currentStation != null)
                                {
                                    item.ID = currentStation.ID;
                                } else
                                {
                                    item.ID = gsDBOps.AddItem(GageStatsDbOps.SQLType.e_station, new object[] { item.Code, agency.ID, item.Name.Replace("'", " "), item.IsRegulated, stationType.ID, item.Location.AsText() });
                                }
                                if (item.ID < 1)
                                {
                                    sm($"99999999 Error pushing station {item.Code} 99999999");
                                    continue;
                                }
                                //get stats and characteristics to push
                                List<FU_Statistics> statistics = ssdb.GetItems<FU_Statistics>(GageStatsDbOps.SQLType.e_statistic_data, item.Code).ToList();

                                // writing this in in case a citation fails, can use the next two lines to update any null citation IDs and skip adding chars/stats
                                //updateCitationIDs(gsDBOps, item.ID, statistics, citationlist);
                                //continue;

                                //charactersitics
                                gsDBOps.AddItems(GageStatsDbOps.SQLType.e_characteristics, statistics.Where(s => String.Equals(s.StatisticDefType, "BC", StringComparison.OrdinalIgnoreCase))
                                                                        .Select(c => new object[] {
                                                                            this.variableTypeList.FirstOrDefault(v=>String.Equals(v.Code,c.StatisticCode)).ID,
                                                                            this.unittypeList.FirstOrDefault(u=> string.Equals(u.Abbreviation, c.StatisticUnitAbbr)).ID,
                                                                            citationlist.FirstOrDefault(s=>string.Equals(s.Title,c.Citation.Title,StringComparison.OrdinalIgnoreCase)&& string.Equals(s.Author,c.Citation.Author, StringComparison.OrdinalIgnoreCase) && (s.CitationURL == "null" || string.Equals(s.CitationURL,c.Citation.CitationURL, StringComparison.OrdinalIgnoreCase)))?.ID,
                                                                            c.StatisticValue,
                                                                            c.StatisticRemarks
                                                                        }).ToList(), new object[] { item.ID });


                                //Statistics

                                gsDBOps.AddItems(statistics.Where(s => String.Equals(s.StatisticDefType, "FS", StringComparison.OrdinalIgnoreCase))
                                                                        .Select(c => new Statistic
                                                                        {
                                                                            StationID = item.ID,
                                                                            CitationID = citationlist.FirstOrDefault(s => string.Equals(s.Title, c.Citation.Title, StringComparison.OrdinalIgnoreCase) && string.Equals(s.Author, c.Citation.Author, StringComparison.OrdinalIgnoreCase) && string.Equals(s.CitationURL, c.Citation.CitationURL, StringComparison.OrdinalIgnoreCase))?.ID,
                                                                            Comments = createComment(c.StatisticStartDate, c.StatisticEndDate, c.StatisticRemarks),
                                                                            RegressionTypeID = this.regressionTypeList.FirstOrDefault(v => String.Equals(v.Code, c.StatisticCode)).ID,
                                                                            StatisticGroupTypeID = this.statisticGroupTypeList.FirstOrDefault(v => String.Equals(v.Code, c.StatisticTypeCode)).ID,
                                                                            UnitTypeID = this.unittypeList.FirstOrDefault(u => string.Equals(u.Abbreviation, c.StatisticUnitAbbr)).ID,
                                                                            Value = c.StatisticValue,
                                                                            YearsofRecord = c.StatisticYears,
                                                                            IsPreferred = c.StatisticIsPreferred,
                                                                            PredictionInterval = (c.StatisticLowerCI.HasValue || c.StatisticUpperCI.HasValue || c.StatisticVariance.HasValue) ? new PredictionInterval()
                                                                            {
                                                                                LowerConfidenceInterval = c.StatisticLowerCI,
                                                                                UpperConfidenceInterval = c.StatisticUpperCI,
                                                                                Variance = c.StatisticVariance
                                                                            } : null,
                                                                            StatisticErrors = c.StatisticError.HasValue ? new List<StatisticError>(){ new StatisticError() {
                                                                                ErrorTypeID = 1,
                                                                                Value = c.StatisticError.Value
                                                                            } } : null
                                                                        }).ToList());
                            }//next station
                            //increment
                            offset = offset + 1000;
                        }//DO
                    }//end using
                }//end using
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
        #region Helper Methods
        private string createComment(DateTime? startDate, DateTime? endDate, String Comments)
        {
            List<string> stringBuilder = new List<string>();
            if(startDate.HasValue && endDate.HasValue) stringBuilder.Add($"Statistic Date Range {startDate.Value.ToShortDateString()} - {endDate.Value.ToShortDateString()} ");
            if (!string.IsNullOrEmpty(Comments)) stringBuilder.Add($"Other Remarks {Comments} ");

            return string.Join("\n", stringBuilder);
        }
        private void createUpdateList<T>(List<T> diffList)
        {
            string tableName = "";
            List<string> updateList = new List<string>();
            string insertStmnt = @"INSERT INTO {0} VALUES ({1});";

            switch (typeof(T).Name)
            {
                case "GageStatsVariableType":
                    tableName = @"""shared"".""VariableType""(""Name"",""Code"",""Description"")";
                    updateList = diffList.Cast<GageStatsVariableType>()
                        .Select(t => 
                            String.Format(insertStmnt, tableName, String.Join(',', new List<string>() {$"'{t.Name}'", $"'{t.Code}'", $"'{t.Description}'" }))).ToList();
                    break;
                case "GageStatsStatisticGroupType":
                    tableName = @"""shared"".""StatisticGroupType""(""Name"",""Code"")";
                    updateList = diffList.Cast<GageStatsStatisticGroupType>()
                        .Select(t =>
                            String.Format(insertStmnt, tableName, String.Join(',', new List<string>() { $"'{t.Name}'", $"'{t.Code}'" }))).ToList();
                    break;
                case "GageStatsRegressionType":
                    tableName = @"""shared"".""RegressionType""(""Name"",""Code"",""Description"")";
                    updateList = diffList.Cast<GageStatsRegressionType>()
                        .Select(t =>
                            String.Format(insertStmnt, tableName, String.Join(',', new List<string>() { $"'{t.Name}'", $"'{t.Code}'",$"'{t.Description}'" }))).ToList();
                    break;

                default:
                    return;
            }
           

            using (TextWriter tw = new StreamWriter(typeof(T).Name+".sql"))
            {
                foreach (var s in updateList)
                    tw.WriteLine(s);
            }

        }

        private void updateCitationIDs(GageStatsDbOps gsDBOps, Int32 stationID, List<FU_Statistics> statistics, List<GageStatsCitations> citationlist)
        {
            List<GageStatsStatistic> gsStatistics = gsDBOps.GetItems<GageStatsStatistic>(GageStatsDbOps.SQLType.e_getstatistics, new object[] { stationID }).ToList();
            List<GageStatsCharacteristic> gsChars = gsDBOps.GetItems<GageStatsCharacteristic>(GageStatsDbOps.SQLType.e_getcharacteristics, new object[] { stationID }).ToList();
            if (gsStatistics.Count < 1 && gsChars.Count < 1) return;
            foreach (var stat in gsStatistics)
            {
                var regType = this.regressionTypeList.FirstOrDefault(rt => rt.ID == stat.RegressionTypeID);
                var statType = this.statisticGroupTypeList.FirstOrDefault(sg => sg.ID == stat.StatisticGroupTypeID);
                var unitType = this.unittypeList.FirstOrDefault(ut => ut.ID == stat.UnitTypeID);
                var ssdbStat = statistics.FirstOrDefault(s => String.Equals(s.StatisticDefType, "FS", StringComparison.OrdinalIgnoreCase) && String.Equals(s.StatisticCode, regType.Code) && s.StatisticValue == stat.Value &&
                    String.Equals(s.StatisticUnitAbbr, unitType.Abbreviation) && String.Equals(s.StatisticTypeCode, statType.Code));
                if (ssdbStat != null)
                {
                    var cit = citationlist.FirstOrDefault(s => string.Equals(s.Title, ssdbStat.Citation.Title, StringComparison.OrdinalIgnoreCase));
                    if (cit != null)
                    {
                        stat.CitationID = cit.ID;
                        var updatedStat = gsDBOps.Update(GageStatsDbOps.SQLType.e_updatestatistic, stat.ID, new object[] { cit.ID, stat.ID });
                    }
                }

            }
            foreach (var stat in gsChars)
            {
                var varType = this.variableTypeList.FirstOrDefault(vt => vt.ID == stat.VariableTypeID);
                var unitType = this.unittypeList.FirstOrDefault(ut => ut.ID == stat.UnitTypeID);
                var ssdbStat = statistics.FirstOrDefault(s => String.Equals(s.StatisticDefType, "BC", StringComparison.OrdinalIgnoreCase) && String.Equals(s.StatisticCode, varType.Code) && s.StatisticValue == stat.Value &&
                    String.Equals(s.StatisticUnitAbbr, unitType.Abbreviation));
                if (ssdbStat != null)
                {
                    var cit = citationlist.FirstOrDefault(s => string.Equals(s.Title, ssdbStat.Citation.Title, StringComparison.OrdinalIgnoreCase));
                    if (cit != null)
                    {
                        stat.CitationID = cit.ID;
                        var updatedChar = gsDBOps.Update(GageStatsDbOps.SQLType.e_updatecharacteristic, stat.ID, new object[] { cit.ID, stat.ID });
                    }
                }
            }
        }
        
        private void init()
        {
            using (var GageStatsDBOps = new GageStatsDbOps(GagesStatsDBConnectionstring, GageStatsDbOps.ConnectionType.e_postgresql))
            {
                statisticGroupTypeList = GageStatsDBOps.GetItems<GageStatsStatisticGroupType>(GageStatsDbOps.SQLType.e_getstatisticgroups).ToList<StatisticGroupType>();
                variableTypeList = GageStatsDBOps.GetItems<GageStatsVariableType>(GageStatsDbOps.SQLType.e_getvariabletypes).ToList<VariableType>();
                unittypeList = GageStatsDBOps.GetItems<GagesStatsUnitType>(GageStatsDbOps.SQLType.e_getunittypes).ToList<UnitType>();
                regressionTypeList = GageStatsDBOps.GetItems<GageStatsRegressionType>(GageStatsDbOps.SQLType.e_getregressiontypes).ToList<RegressionType>();
                stationTypeList = GageStatsDBOps.GetItems<GageStatsStationType>(GageStatsDbOps.SQLType.e_stationtype).ToList<StationType>();
                agencies = GageStatsDBOps.GetItems<GageStatsAgency>(GageStatsDbOps.SQLType.e_agency).ToList<Agency>();
            }//end using
        }

        private void sm(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
            Console.WriteLine(msg);
            this._message.Add(msg);
        }
        #endregion


    }
}

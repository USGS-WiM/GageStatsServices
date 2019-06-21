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


        private string SSDBConnectionstring;
        private string GagesStatsDBConnectionstring;
        
        #endregion
        #region Constructors
        public ForceUpdate(string dbusername, string dbpassword, string accessdb)
        {
            SSDBConnectionstring = string.Format(@"Driver={{Microsoft Access Driver (*.mdb, *.accdb)}};dbq={0}", accessdb);
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

            List<string> ssdbUnitAbbr = null;
            List<GageStatsVariableType> ssdbDBVariableList = null;
            List<GageStatsStatisticGroupType> ssdbStatisticGroupList = null;
            List<GageStatsRegressionType> ssdbRegressionList = null;
            List<GageStatsStationType> ssdbStationTypeList = null;

            using (var ssdb = new GageStatsDbOps(SSDBConnectionstring, GageStatsDbOps.ConnectionType.e_access))
            {
                ssdbUnitAbbr = ssdb.GetItems<FUString>(GageStatsDbOps.SQLType.e_unittype).Select(f => f.Value.Trim()).ToList();
                ssdbDBVariableList = ssdb.GetItems<GageStatsVariableType>(GageStatsDbOps.SQLType.e_variabletype).ToList();
                ssdbStatisticGroupList = ssdb.GetItems<GageStatsStatisticGroupType>(GageStatsDbOps.SQLType.e_statisticgrouptype).ToList();
                ssdbRegressionList = ssdb.GetItems<GageStatsRegressionType>(GageStatsDbOps.SQLType.e_regressiontype).ToList();
                ssdbStationTypeList = ssdb.GetItems<GageStatsStationType>(GageStatsDbOps.SQLType.e_stationtype).ToList();
            }//end using

            var diffUnits = ssdbUnitAbbr.Except(DBUnitAbbr).ToList();
            var diffVariable = ssdbDBVariableList.Where(v=>!DBVariableList.Contains(v.Code.Trim().ToUpper())).ToList();
            var diffSG = ssdbStatisticGroupList.Where(sg=>!DBStatisticGroupList.Contains(sg.Code.Trim().ToUpper())).ToList();
            var diffRegList = ssdbRegressionList.Where(r => !DBregressionList.Contains(r.Code.Trim().ToUpper())).ToList();
            var diffStationtypeList = ssdbStationTypeList.Where(r => !DBstationTypeList.Contains(r.Code.Trim().ToUpper())).ToList();

            if (diffVariable.Count > 0) createUpdateList(diffVariable);
            if (diffRegList.Count > 0) createUpdateList(diffRegList);
            if (diffSG.Count > 0) createUpdateList(diffSG);
            if (diffStationtypeList.Count > 0) createUpdateList(diffStationtypeList);

            return diffUnits.Count < 2 && diffVariable.Count < 1 && diffSG.Count < 1 && diffRegList.Count < 1 && diffStationtypeList.Count <1 ;
        }
        public void Load() {
            try
            {
                sm("Starting migration " + DateTime.Today.ToShortDateString());
                using (var ssdb = new GageStatsDbOps(SSDBConnectionstring, GageStatsDbOps.ConnectionType.e_access))
                {
                    //this.GageStatsDBOps.ResetTables();
                    //read station, stationtype, Agency, characteristics, statistics (and errors/predictininterval), citations, 
                    bool DBcontainsMoreRecords = true;
                    var stationcount = ssdb.GetItems<FUInt>(GageStatsDbOps.SQLType.e_stationCount).FirstOrDefault().Value;
                    Int32 limit = 1000;
                    Int32 offset = 0;

                    while (DBcontainsMoreRecords)
                    {
                        sm($"LIMIT: {limit} Offset: {offset} ");
                        if ((stationcount - offset) < limit)
                        {
                            limit = (stationcount - offset);
                            DBcontainsMoreRecords = false;
                        }//endif

                        var stationlist = ssdb.GetItems<FU_Station>(GageStatsDbOps.SQLType.e_station, limit, limit+offset);

                        //foreach (var item in stationlist)
                        //{
                        //    Console.WriteLine(item.Code);


                        //    //Post all citations

                        //    //post all characteristics
                        //}//next station
                        //increment
                        offset = offset + 1000;
                    }//DO

                 }//end using
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        #endregion
        #region Helper Methods
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
        private void init()
        {
            using (var GageStatsDBOps = new GageStatsDbOps(GagesStatsDBConnectionstring, GageStatsDbOps.ConnectionType.e_postgresql))
            {
                statisticGroupTypeList = GageStatsDBOps.GetItems<GageStatsStatisticGroupType>(GageStatsDbOps.SQLType.e_getstatisticgroups).ToList<StatisticGroupType>();
                variableTypeList = GageStatsDBOps.GetItems<GageStatsVariableType>(GageStatsDbOps.SQLType.e_getvariabletypes).ToList<VariableType>();
                unittypeList = GageStatsDBOps.GetItems<GagesStatsUnitType>(GageStatsDbOps.SQLType.e_getunittypes).ToList<UnitType>();
                regressionTypeList = GageStatsDBOps.GetItems<GageStatsRegressionType>(GageStatsDbOps.SQLType.e_getregressiontypes).ToList<RegressionType>();
                stationTypeList = GageStatsDBOps.GetItems<GageStatsStationType>(GageStatsDbOps.SQLType.e_stationtype).ToList<StationType>();
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

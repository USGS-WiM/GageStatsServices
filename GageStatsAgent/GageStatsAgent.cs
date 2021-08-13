//------------------------------------------------------------------------------
//----- ServiceAgent -------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   The service agent is responsible for initiating the service call, 
//              capturing the data that's returned and forwarding the data back to 
//              the requestor.
//
//discussion:   
//
// 

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using WIM.Resources;
using WIM.Security.Authentication;
using GageStatsDB.Resources;
using System.Threading.Tasks;
using System.Linq;
using WIM.Utilities;
using GageStatsDB;
using SharedDB.Resources;
using Microsoft.EntityFrameworkCore;

namespace GageStatsAgent
{
    public interface IGageStatsAgent : IAuthenticationAgent
    {
        //Agency
        IQueryable<Agency> GetAgencies(List<string> regionList = null, List<string> stationTypeList = null, List<string> regressionTypeList = null, List<string> variableTypeList = null, List<string> statisticGroupList = null, string filterText = null);
        Task<Agency> GetAgency(Int32 ID);
        Task<Agency> Add(Agency item);
        Task<IEnumerable<Agency>> Add(List<Agency> items);
        Task<Agency> Update(Int32 pkId, Agency item);
        Task DeleteAgency(Int32 id);

        //Characteristic
        IQueryable<Characteristic> GetCharacteristics(string stationIDorcode = null, List<string> citationList = null, List<string> statisticGroupList = null);
        Task<Characteristic> GetCharacteristic(Int32 ID);
        Task<Characteristic> Add(Characteristic item);
        Task<IEnumerable<Characteristic>> Add(List<Characteristic> items);
        Task<Characteristic> Update(Int32 pkId, Characteristic item);
        Task DeleteCharacteristic(Int32 id);

        //Citation
        IQueryable<Citation> GetCitations();
        Task<Citation> GetCitation(Int32 ID);
        Task<Citation> Add(Citation item);
        Task<IEnumerable<Citation>> Add(List<Citation> items);
        Task<Citation> Update(Int32 pkId, Citation item);
        Task DeleteCitation(Int32 id);

        //Roles
        IQueryable<string> GetRoles();

        //Station
        IQueryable<Station> GetStations(List<string> regionList = null, List<string> stationTypeList = null, List<string> agencyList = null, List<string> regressionTypeList = null, List<string> variableTypeList = null, List<string> statisticGroupList = null, bool includeStats = false, string filterText = null, List<string> stationCodeList = null);
        Task<Station> GetStation(string identifier);
        IQueryable<Station> GetNearest(double lat, double lon, double radius, bool includeStats);
        Task<Station> Add(Station item);
        Task<IEnumerable<Station>> Add(List<Station> items);
        Task<Station> Update(Int32 pkId, Station item);
        Task DeleteStation(Int32 id);
        IQueryable<Station> GetStationsWithinBounds(double xmin, double ymin, double xmax, double ymax, bool includeStats);

        //StationType
        IQueryable<StationType> GetStationTypes(List<string> regionList = null, List<string> agencyList = null, List<string> regressionTypeList = null, List<string> variableTypeList = null, List<string> statisticGroupList = null, string filterText = null);
        Task<StationType> GetStationType(Int32 ID);
        Task<StationType> Add(StationType item);
        Task<IEnumerable<StationType>> Add(List<StationType> items);
        Task<StationType> Update(Int32 pkId, StationType item);
        Task DeleteStationType(Int32 id);

        //Statistic
        IQueryable<Statistic> GetStatistics(string stationIDOrCode = null, List<string> citationList = null, List<string> statisticGroupList = null);
        Task<Statistic> GetStatistic(Int32 ID);
        Task<Statistic> Add(Statistic item);
        Task<IEnumerable<Statistic>> Add(List<Statistic> items);
        Task<Statistic> Update(Int32 pkId, Statistic item);
        Task DeleteStatistic(Int32 id);
        void TriggerStatisticPreferred(Int32 statID, Int32 regTypeID, Int32 stationID);

        //Manager
        IQueryable<Manager> GetUsers();
        Manager GetUser(Int32 ID);

        //Regions
        IQueryable<Region> GetRegions(List<string> stationTypeList = null, List<string> agencyList = null, List<string> regressionTypeList = null, List<string> variableTypeList = null, List<string> statisticGroupList = null, string filterText = null);
        Task<Region> GetRegion(Int32 ID);
        Region GetRegionByIDOrCode(string identifier);
        IQueryable<Region> GetManagerRegions(int managerID);

        //Readonly (Shared Views) methods
        IQueryable<ErrorType> GetErrors();
        Task<ErrorType> GetError(Int32 ID);
        IQueryable<StatisticGroupType> GetStatisticGroups(List<string> defTypeList = null, List<string> regionList = null, List<string> stationTypeList = null, List<string> agencyList = null, List<string> regressionTypeList = null, List<string> variableTypeList = null, string filterText = null);
        Task<RegressionType> GetRegression(Int32 ID);
        IQueryable<RegressionType> GetRegressions(List<string> regionList = null, List<string> stationTypeList = null, List<string> agencyList = null, List<string> variableTypeList = null, List<string> statisticGroupList = null, string filterText = null);
        Task<StatisticGroupType> GetStatisticGroup(Int32 ID);
        IQueryable<UnitType> GetUnits();
        Task<UnitType> GetUnit(Int32 ID);
        IQueryable<UnitSystemType> GetUnitSystems();
        Task<UnitSystemType> GetUnitSystem(Int32 ID);
        IQueryable<VariableType> GetVariables(List<string> regionList = null, List<string> stationTypeList = null, List<string> agencyList = null, List<string> regressionTypeList = null, List<string> statisticGroupList = null, string filterText = null);
        Task<VariableType> GetVariable(Int32 ID);
    }

    public class GageStatsAgent : DBAgentBase, IGageStatsAgent
    {
        #region Properties
        private readonly IDictionary<Object, Object> _messages;
        #endregion
        #region Constructor
        public GageStatsAgent(GageStatsDBContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            this._messages = httpContextAccessor.HttpContext.Items;
            this.context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        #endregion
        #region Methods    
        #region Agency
        public IQueryable<Agency> GetAgencies(List<string> regionList = null, List<string> stationTypeList = null, List<string> regressionTypeList = null, List<string> variableTypeList = null, List<string> statisticGroupList = null, string filterText = null)
        {
            // if no filters
            if (!regionList.Any() && !stationTypeList.Any() && !regressionTypeList.Any() && !variableTypeList.Any() && !statisticGroupList.Any() && filterText == null) return this.Select<Agency>().OrderBy(a => a.ID);

            // filter by other elements to get all available agencies for that selection
            var stations = this.GetStations(regionList, stationTypeList, null, regressionTypeList, variableTypeList, statisticGroupList, false, filterText);
            return stations.Select(s => s.Agency).Distinct();
        }
        public Task<Agency> GetAgency(int ID)
        {
            return Find<Agency>(ID);
        }
        public Task<Agency> Add(Agency item)
        {
            return Add<Agency>(item);
        }
        public Task<IEnumerable<Agency>> Add(List<Agency> items)
        {
            return Add<Agency>(items);
        }
        public Task<Agency> Update(int pkId, Agency item)
        {
            return this.Update<Agency>(pkId, item);
        }
        public Task DeleteAgency(int id)
        {
            return this.Delete<Agency>(id);
        }
        #endregion
        #region Characteristic
        public IQueryable<Characteristic> GetCharacteristics(string stationIDOrCode = null, List<string> citationList = null, List<string> statisticGroupList = null)
        {
            IQueryable<Characteristic> query = Select<Characteristic>().Include(c => c.VariableType).Include(c => c.UnitType).Include(c => c.Citation);
            if (stationIDOrCode != null)
                query = query.Where(c => c.Station.Code == stationIDOrCode || c.Station.ID.ToString() == stationIDOrCode);
            if (citationList != null && citationList.Any())
                query = query.Where(c => citationList.Contains(c.CitationID.ToString()));
            if (statisticGroupList != null && statisticGroupList.Any())
                query = query.Where(c => statisticGroupList.Contains(c.VariableType.StatisticGroupTypeID.ToString().Trim()) || statisticGroupList.Contains(c.VariableType.StatisticGroupType.Code.ToLower()));
            return query;
        }
        public Task<Characteristic> GetCharacteristic(int ID)
        {
            return GetCharacteristics().FirstOrDefaultAsync(c => c.ID == ID);
        }
        public Task<Characteristic> Add(Characteristic item)
        {
            return Add<Characteristic>(item);
        }
        public Task<IEnumerable<Characteristic>> Add(List<Characteristic> items)
        {
            return Add<Characteristic>(items);
        }
        public Task<Characteristic> Update(int pkId, Characteristic item)
        {
            return Update<Characteristic>(pkId, item);
        }
        public Task DeleteCharacteristic(int id)
        {
            return this.Delete<Characteristic>(id);
        }
        #endregion
        #region Citation
        public IQueryable<Citation> GetCitations()
        {
            return Select<Citation>();
        }
        public Task<Citation> GetCitation(int ID)
        {
            return Find<Citation>(ID);
        }
        public Task<Citation> Add(Citation item)
        {
            return Add<Citation>(item);
        }
        public Task<IEnumerable<Citation>> Add(List<Citation> items)
        {
            return Add<Citation>(items);
        }
        public Task<Citation> Update(int pkId, Citation item)
        {
            return Update<Citation>(pkId, item);
        }
        public Task DeleteCitation(int id)
        {
            return Delete<Citation>(id);
        }
        #endregion
        #region Roles
        public IQueryable<String> GetRoles()
        {
            return Role.ToList().AsQueryable();
        }

        #endregion 
        #region Station
        public IQueryable<Station> GetStations(List<string> regionList = null, List<string> stationTypeList = null, List<string> agencyList = null, List<string> regressionTypeList = null, List<string> variableTypeList = null, List<string> statisticGroupList = null, bool includeStats = false, string filterText = null, List<string> stationCodeList = null)
        {
            IQueryable<Station> query = this.Select<Station>();
            if (includeStats) query = query.Include(s => s.Statistics).Include("Statistics.StatisticGroupType").Include(s => s.Characteristics).Include("Characteristics.VariableType");
            // if filters, apply them before returning query
            if (regionList != null && regionList.Any())
                query = query.Where(st => regionList.Contains(st.RegionID.ToString()) || regionList.Contains(st.Region.Code.ToLower()));
            if (stationTypeList != null && stationTypeList.Any())
                query = query.Where(st => stationTypeList.Contains(st.StationTypeID.ToString()) || stationTypeList.Contains(st.StationType.Code.ToLower()));
            if (agencyList != null && agencyList.Any())
                query = query.Where(st => agencyList.Contains(st.AgencyID.ToString()) || agencyList.Contains(st.Agency.Code.ToLower()));
            if (regressionTypeList != null && regressionTypeList.Any())
            {
                query = query.Where(st => st.Statistics.Any(s => regressionTypeList.Contains(s.RegressionTypeID.ToString()) || regressionTypeList.Contains(s.RegressionType.Code.ToLower())));
            }
            if (variableTypeList != null && variableTypeList.Any())
            {
                query = query.Where(st => st.Characteristics.Any(c => variableTypeList.Contains(c.VariableTypeID.ToString()) || variableTypeList.Contains(c.VariableType.Code.ToLower())));
            }
            if (statisticGroupList != null && statisticGroupList.Any())
            {
                query = query.Where(st => st.Statistics.Any(s => statisticGroupList.Contains(s.StatisticGroupTypeID.ToString()) || statisticGroupList.Contains(s.StatisticGroupType.Code.ToLower()) || 
                    st.Characteristics.Any(c => statisticGroupList.Contains(c.VariableType.StatisticGroupTypeID.ToString()) || statisticGroupList.Contains(c.VariableType.StatisticGroupType.Code.ToLower()))));
            }
            if (filterText != null)
            {
                query = query.Where(st => st.Name.ToUpper().Contains(filterText.ToUpper()) || st.Code.ToUpper().Contains(filterText.ToUpper()));
            }
            if (stationCodeList != null && stationCodeList.Any())
            {
                query = query.Where(st => stationCodeList.Contains(st.Code.ToUpper()));
            }
            return query.Include(s => s.StationType).OrderBy(s => s.ID);
        }
        public Task<Station> GetStation(string identifier)
        {
            return GetStations().Include("Agency").Include("StationType").Include("Characteristics.Citation").Include("Characteristics.VariableType").Include("Characteristics.UnitType")
                .Include("Statistics.PredictionInterval").Include("Statistics.StatisticErrors").Include("Statistics.StatisticErrors.ErrorType").Include("Statistics.RegressionType").Include("Statistics.UnitType")
                .Include("Statistics.Citation").Include("Statistics.StatisticGroupType").Include("Region").FirstOrDefaultAsync(s => s.Code == identifier || s.ID.ToString() == identifier);
        }
        public IQueryable<Station> GetNearest(double lat, double lon, double radius, bool includeStats)
        {
            var radius_m = radius * 1000; //GageStatsDB searches in meters by default, user has specified km
            var query = String.Format(getSQLStatement(sqltypeenum.stationsbyradius), lat, lon, radius_m);
            if (includeStats) return FromSQL<Station>(query).Include(s => s.StationType).Include(s => s.Statistics).Include("Statistics.StatisticGroupType").Include(s => s.Characteristics).Include("Characteristics.VariableType");
            return FromSQL<Station>(query).Include(s => s.StationType);
        }
        public IQueryable<Station> GetStationsWithinBounds(double xmin, double ymin, double xmax, double ymax, bool includeStats)
        {
            var query = String.Format(getSQLStatement(sqltypeenum.stationsbyboundingbox), xmin, ymin, xmax, ymax);
            if (includeStats) return FromSQL<Station>(query).Include(s => s.StationType).Include(s => s.Statistics).Include("Statistics.StatisticGroupType").Include(s => s.Characteristics).Include("Characteristics.VariableType");
            return FromSQL<Station>(query).Include(s => s.StationType);
        }
        public Task<Station> Add(Station item)
        {
            return Add<Station>(item);
        }
        public Task<IEnumerable<Station>> Add(List<Station> items)
        {
            return Add<Station>(items);
        }
        public Task<Station> Update(int pkId, Station item)
        {
            return Update<Station>(pkId, item);
        }
        public Task DeleteStation(int id)
        {
            return Delete<Station>(id);
        }
        #endregion
        #region StationType
        public IQueryable<StationType> GetStationTypes(List<string> regionList = null, List<string> agencyList = null, List<string> regressionTypeList = null, List<string> variableTypeList = null, List<string> statisticGroupList = null, string filterText = null)
        {
            // if no filters
            if (!regionList.Any() && !agencyList.Any() && !regressionTypeList.Any() && !variableTypeList.Any() && !statisticGroupList.Any() && filterText == null) return Select<StationType>().OrderBy(st => st.ID);
            // filter by other elements to get all available agencies for that selection
            var stations = this.GetStations(regionList, null, agencyList, regressionTypeList, variableTypeList, statisticGroupList, false, filterText);
            return stations.Select(s => s.StationType).Distinct().OrderBy(st => st.ID);
        }
        public Task<StationType> GetStationType(int ID)
        {
            return Find<StationType>(ID);
        }
        public Task<StationType> Add(StationType item)
        {
            return Add<StationType>(item);
        }
        public Task<IEnumerable<StationType>> Add(List<StationType> items)
        {
            return Add<StationType>(items);
        }
        public Task<StationType> Update(int pkId, StationType item)
        {
            return Update<StationType>(pkId, item);
        }
        public Task DeleteStationType(int id)
        {
            return Delete<StationType>(id);
        }
        #endregion
        #region Statistic
        public IQueryable<Statistic> GetStatistics(string stationIDOrCode = null, List<string> citationList = null, List<string> statisticGroupList = null)
        {
            IQueryable<Statistic> query = Select<Statistic>().Include(s => s.PredictionInterval).Include("StatisticErrors.ErrorType")
                    .Include(s => s.RegressionType).Include(s => s.StatisticGroupType);
            if (stationIDOrCode != null)
                query = query.Where(s => s.Station.Code == stationIDOrCode || s.Station.ID.ToString() == stationIDOrCode);
            if (citationList != null && citationList.Any())
                query = query.Where(s => citationList.Contains(s.CitationID.ToString()));
            if (statisticGroupList != null && statisticGroupList.Any())
                query = query.Where(s => statisticGroupList.Contains(s.StatisticGroupTypeID.ToString().Trim()) || statisticGroupList.Contains(s.StatisticGroupType.Code.ToLower()));
            return query;
        }
        public Task<Statistic> GetStatistic(int ID)
        {
            return GetStatistics().FirstOrDefaultAsync(s => s.ID == ID);
        }
        public Task<Statistic> Add(Statistic item)
        {
            return Add<Statistic>(item);
        }
        public Task<IEnumerable<Statistic>> Add(List<Statistic> items)
        {
            return Add<Statistic>(items);
        }
        public Task<Statistic> Update(int pkId, Statistic item)
        {
            return Update<Statistic>(pkId, item);
        }
        public Task DeleteStatistic(int id)
        {
            return Delete<Statistic>(id);
        }
        public void TriggerStatisticPreferred(Int32 statID, Int32 regTypeID, Int32 stationID)
        {
            // change all statistics with x reg type ID within the station to not preferred
            var statistics = Select<Statistic>().Where(s => s.ID != statID && s.StationID == stationID && s.RegressionTypeID == regTypeID && s.IsPreferred).ToList();
            statistics.ForEach(s =>
            {
                s.IsPreferred = false;
                Update<Statistic>(s.ID, s);
            });
            return;
        }
        #endregion
        #region Manager
        public IQueryable<Manager> GetUsers()
        {
            return Select<Manager>();
        }
        public Manager GetUser(int ID)
        {
            return Select<Manager>().Include(m => m.RegionManagers).FirstOrDefault(u => u.ID == ID);
        }
        public IUser GetUserByUsername(string username)
        {
            return Select<Manager>().Include(m => m.RegionManagers).FirstOrDefault(r => string.Equals(r.Username.ToLower(), username.ToLower()));
        }
        public IUser GetUserByID(int id)
        {
            return Select<Manager>().FirstOrDefault(u => u.ID == id);
        }

        public IUser AuthenticateUser(string username, string password)
        {
            //this is where one authenticates the username/password before passing back user
            try
            {
                var user = (Manager)GetUserByUsername(username);
                if (user == null || !WIM.Security.Cryptography.VerifyPassword(password, user.Salt, user.Password))
                {
                    return null;
                }
                return user;

            }
            catch (Exception ex)
            {
                sm("Error authenticaticating user ", MessageType.error);
                return null;
            }
        }
        public Task<Manager> Add(Manager item)
        {
            return Add<Manager>(item);
        }
        public Task<IEnumerable<Manager>> Add(List<Manager> items)
        {
            return Add<Manager>(items);
        }
        public Task<Manager> Update(int pkId, Manager item)
        {
            return Update<Manager>(pkId, item);
        }
        public Task DeleteUser(int id)
        {
            return Delete<Manager>(id);
        }
        #endregion
        #region Region
        public Region GetRegionByIDOrCode(string identifier)
        {
            try
            {

                return Select<Region>().FirstOrDefault(e => String.Equals(e.ID.ToString().Trim().ToLower(),
                                                        identifier.Trim().ToLower()) || String.Equals(e.Code.Trim().ToLower(),
                                                        identifier.Trim().ToLower()));
            }
            catch (Exception ex)
            {
                sm("Error finding region " + ex.Message, WIM.Resources.MessageType.error);
                return null;
            }


        }
        public IQueryable<Region> GetRegions(List<string> stationTypeList = null, List<string> agencyList = null, List<string> regressionTypeList = null, List<string> variableTypeList = null, List<string> statisticGroupList = null, string filterText = null)
        {
            // if no filters
            if (!stationTypeList.Any() && !agencyList.Any() && !regressionTypeList.Any() && !variableTypeList.Any() && !statisticGroupList.Any() && filterText == null) return this.Select<Region>().OrderBy(r => r.ID);

            // filter by other elements to get all available regions for that selection
            var stations = this.GetStations(null, stationTypeList, agencyList, regressionTypeList, variableTypeList, statisticGroupList, false, filterText);
            return stations.Select(s => s.Region).Distinct().OrderBy(r => r.ID);
        }
        public Task<Region> GetRegion(int ID)
        {
            return this.Find<Region>(ID);
        }
        public IQueryable<Region> GetManagerRegions(int managerID)
        {
            return Select<RegionManager>().Where(rm => rm.ManagerID == managerID)
                                .Include("Region").Select(rm => rm.Region);
        }
        #endregion
        #region ReadOnly
        public IQueryable<ErrorType> GetErrors()
        {
            return this.Select<ErrorType>();
        }
        public Task<ErrorType> GetError(Int32 ID)
        {
            return this.Find<ErrorType>(ID);
        }
        public ErrorType GetErrorByCode(string code)
        {
            return this.Select<ErrorType>().FirstOrDefault(r => string.Equals(r.Code.ToLower(), code.ToLower()));
        }
        public RegressionType GetRegressionByCode(string code)
        {
            return this.Select<RegressionType>().FirstOrDefault(r => string.Equals(r.Code.ToLower(), code.ToLower()));
        }
        public IQueryable<RegressionType> GetRegressions(List<string> regionList = null, List<string> stationTypeList = null, List<string> agencyList = null, List<string> variableTypeList = null, List<string> statisticGroupList = null, string filterText = null)
        {
            // if no filters
            if (!regionList.Any() && !stationTypeList.Any() && !agencyList.Any() && !variableTypeList.Any() && !statisticGroupList.Any() && filterText == null) return this.Select<RegressionType>().OrderBy(rt => rt.ID);
            // filter by other elements to get all available agencies for that selection
            var stations = this.GetStations(regionList, stationTypeList, agencyList, null, variableTypeList, statisticGroupList, false, filterText);
            return stations.SelectMany(s => s.Statistics).Select(st => st.RegressionType).Distinct().OrderBy(rt => rt.ID);
        }
        public Task<RegressionType> GetRegression(Int32 ID)
        {
            return this.Find<RegressionType>(ID);
        }
        public StatisticGroupType GetStatisticGroupByCode(string code)
        {
            return this.Select<StatisticGroupType>().FirstOrDefault(r => string.Equals(r.Code.ToLower(), code.ToLower()));
        }
        public IQueryable<StatisticGroupType> GetStatisticGroups(List<string> defTypeList = null, List<string> regionList = null, List<string> stationTypeList = null, List<string> agencyList = null, List<string> regressionTypeList = null, List<string> variableTypeList = null, string filterText = null)
        {
            // if any filters
            if (regionList.Any() || stationTypeList.Any() || agencyList.Any() || regressionTypeList.Any() || variableTypeList.Any() || filterText != null)
            {
                // filter by other elements to get all available agencies for that selection
                var stations = this.GetStations(regionList, stationTypeList, agencyList, regressionTypeList, variableTypeList, null, true, filterText);
                return stations.SelectMany(s => s.Statistics).Select(s => s.StatisticGroupType)
                    .Union(stations.SelectMany(s => s.Characteristics).Select(c => c.VariableType.StatisticGroupType)).Distinct()
                    .OrderBy(sg => sg.ID);
            }

            var query = this.Select<StatisticGroupType>();
            if (defTypeList != null && defTypeList.Count > 0)
            {
                query = query.Where(sg => defTypeList.Contains(sg.DefType.ToLower())).OrderBy(st => st.ID);
            }
            return query.OrderBy(sg => sg.ID);
        }
        public Task<StatisticGroupType> GetStatisticGroup(Int32 ID)
        {
            return this.Find<StatisticGroupType>(ID);
        }
        public IQueryable<UnitType> GetUnits()
        {
            return this.Select<UnitType>().OrderBy(ut => ut.ID);
        }
        public Task<UnitType> GetUnit(Int32 ID)
        {
            return this.Find<UnitType>(ID);
        }
        public UnitType GetUnitByAbbreviation(string abbr)
        {
            return this.Select<UnitType>().FirstOrDefault(r => string.Equals(r.Abbreviation.ToLower(), abbr.ToLower()));
        }
        public IQueryable<UnitSystemType> GetUnitSystems()
        {
            return this.Select<UnitSystemType>();
        }
        public Task<UnitSystemType> GetUnitSystem(Int32 ID)
        {
            return this.Find<UnitSystemType>(ID);
        }
        public IQueryable<VariableType> GetVariables(List<string> regionList = null, List<string> stationTypeList = null, List<string> agencyList = null, List<string> regressionTypeList = null, List<string> statisticGroupList = null, string filterText = null)
        {
            // TODO: reconsider this, the problem is, if a user is making this request from the gagestats page, we want them filtered by what variables are used in the gages filtered
            // BUT, if we're using it in other places, like when adding a new characteristic to a gage, we might want to filter the available variables by the statistic group instead
            // we might want to create a parameter called "gagestatisticgroups", "statisticgroupsbygage" or something like that?
            IQueryable<VariableType> query = this.Select<VariableType>().Include(vt => vt.MetricUnitType).Include(vt => vt.EnglishUnitType).Include(vt => vt.StatisticGroupType);
            // if any filters other than statisticgroups, filter by variables used in the filtered list of stations
            if (regionList.Any() || stationTypeList.Any() || agencyList.Any() || regressionTypeList.Any() || filterText != null)
            {
                // filter by other elements to get all available agencies for that selection
                var stations = this.GetStations(regionList, stationTypeList, agencyList, regressionTypeList, null, statisticGroupList, false, filterText);
                query = stations.SelectMany(s => s.Characteristics).Select(c => c.VariableType).Distinct().Include(vt => vt.MetricUnitType).Include(vt => vt.EnglishUnitType).Include(vt => vt.StatisticGroupType);
            } else if (statisticGroupList.Any())
            {
                // if only statistic group given, filter variables by the assigned statistic groups
                query = query.Where(vt => statisticGroupList.Contains(vt.StatisticGroupTypeID.ToString().Trim()) || statisticGroupList.Contains(vt.StatisticGroupType.Code.ToLower()));
            }
            return query.OrderBy(vt => vt.ID);
        }
        public Task<VariableType> GetVariable(Int32 ID)
        {
            return this.Find<VariableType>(ID);
        }
        public VariableType GetVariableByCode(string code)
        {
            return this.Select<VariableType>().FirstOrDefault(r => string.Equals(r.Code.ToLower(), code.ToLower()));
        }
        #endregion
        #endregion
        #region HELPER METHODS
        private string getSQLStatement(sqltypeenum type)
        {
            string sql = string.Empty;
            switch (type)
            {
                case sqltypeenum.stationsbyradius:
                    return @"SELECT * FROM gagestats.""Stations"" as st 
                                      where ST_Contains(st_transform(ST_Buffer(st_geomfromtext('Point({1} {0})',4326)::geography, {2})::geometry, 4326), st.""Location"")";
                case sqltypeenum.stationsbyboundingbox:
                    return @"select * from gagestats.""Stations"" where ""Location"" @ ST_MakeEnvelope({0}, {1}, {2}, {3}, 4326)";
                default:
                    throw new Exception("No sql for table " + type);
            }
        }
        private Task Delete<T>(Int32 id) where T : class, new()
        {
            var entity = base.Find<T>(id).Result;
            if (entity == null) return new Task(null);
            return base.Delete<T>(entity);
        }
        protected override void sm(string msg, MessageType type = MessageType.info)
        {
            sm(new Message() { msg = msg, type = type });
        }
        private void sm(Message msg)
        {
            //wim_msgs comes from WIM.Standard/blob/staging/Services/Middleware/X-Messages.cs
            //see services.startup for defined key (default is wim_msgs)
            if (!this._messages.ContainsKey("wim_msgs"))
                this._messages["wim_msgs"] = new List<Message>();

            ((List<Message>)this._messages["wim_msgs"]).Add(msg);
        }

        #endregion
        private enum sqltypeenum
        {
            stationsbyradius,
            stationsbyboundingbox
        }
    }
}
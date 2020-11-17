﻿//------------------------------------------------------------------------------
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
        IQueryable<Agency> GetAgencies(List<string> regionList = null, List<string> stationTypeList = null, List<string> regressionTypeList = null, List<string> variableTypeList = null, List<string> statisticGroupList = null);
        Task<Agency> GetAgency(Int32 ID);
        Task<Agency> Add(Agency item);
        Task<IEnumerable<Agency>> Add(List<Agency> items);
        Task<Agency> Update(Int32 pkId, Agency item);
        Task DeleteAgency(Int32 id);

        //Characteristic
        IQueryable<Characteristic> GetCharacteristics(string stationIDorcode = null);
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
        IQueryable<Station> GetStations(List<string> regionList = null, List<string> stationTypeList = null, List<string> agencyList = null, List<string> regressionTypeList = null, List<string> variableTypeList = null, List<string> statisticGroupList = null, bool includeStats = false);
        Task<Station> GetStation(string identifier);
        IQueryable<Station> GetNearest(double lat, double lon, double radius);
        Task<Station> Add(Station item);
        Task<IEnumerable<Station>> Add(List<Station> items);
        Task<Station> Update(Int32 pkId, Station item);
        Task DeleteStation(Int32 id);

        //StationType
        IQueryable<StationType> GetStationTypes();
        Task<StationType> GetStationType(Int32 ID);
        Task<StationType> Add(StationType item);
        Task<IEnumerable<StationType>> Add(List<StationType> items);
        Task<StationType> Update(Int32 pkId, StationType item);
        Task DeleteStationType(Int32 id);

        //Statistic
        IQueryable<Statistic> GetStatistics(string stationIDOrCode = null);
        Task<Statistic> GetStatistic(Int32 ID);
        Task<Statistic> Add(Statistic item);
        Task<IEnumerable<Statistic>> Add(List<Statistic> items);
        Task<Statistic> Update(Int32 pkId, Statistic item);
        Task DeleteStatistic(Int32 id);

        //Manager
        IQueryable<Manager> GetUsers();
        Manager GetUser(Int32 ID);

        //Regions
        IQueryable<Region> GetRegions();
        Task<Region> GetRegion(Int32 ID);
        Region GetRegionByIDOrCode(string identifier);
        IQueryable<Region> GetManagerRegions(int managerID);

        //Readonly (Shared Views) methods
        IQueryable<ErrorType> GetErrors();
        Task<ErrorType> GetError(Int32 ID);
        IQueryable<StatisticGroupType> GetStatisticGroups(List<string> defTypeList = null);
        Task<RegressionType> GetRegression(Int32 ID);
        IQueryable<RegressionType> GetRegressions();
        Task<StatisticGroupType> GetStatisticGroup(Int32 ID);
        IQueryable<UnitType> GetUnits();
        Task<UnitType> GetUnit(Int32 ID);
        IQueryable<UnitSystemType> GetUnitSystems();
        Task<UnitSystemType> GetUnitSystem(Int32 ID);
        IQueryable<VariableType> GetVariables(List<string> statisticGroupList = null);
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
        public IQueryable<Agency> GetAgencies(List<string> regionList = null, List<string> stationTypeList = null, List<string> regressionTypeList = null, List<string> variableTypeList = null, List<string> statisticGroupList = null)
        {
            // filter by other elements, so basically need to filter stations by those items and see which agencies are used in that list
            var stations = this.GetStations(regionList, null, regressionTypeList, variableTypeList, statisticGroupList);
            return Select<Agency>().Where(a => stations.Any(s => s.AgencyID == a.ID));
            // st.Statistics.Any(s => regressionTypeList.Contains(s.RegressionTypeID.ToString()) || regressionTypeList.Contains(s.RegressionType.Code.ToLower())))
            // query.Where(st => agencyList.Contains(st.AgencyID.ToString()) || agencyList.Contains(st.Agency.Code.ToLower()));
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
        public IQueryable<Characteristic> GetCharacteristics(string stationIDOrCode = null)
        {
            IQueryable<Characteristic> query = Select<Characteristic>().Include(c => c.VariableType).Include(c => c.UnitType).Include(c => c.Citation);
            if (stationIDOrCode != null)
            {
                query = query.Where(c => c.Station.Code == stationIDOrCode || c.Station.ID.ToString() == stationIDOrCode);
            }
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
        public IQueryable<Station> GetStations(List<string> regionList = null, List<string> stationTypeList = null, List<string> agencyList = null, List<string> regressionTypeList = null, List<string> variableTypeList = null, List<string> statisticGroupList = null, bool includeStats = false)
        {
            IQueryable<Station> query = this.Select<Station>();
            if (includeStats) query = query.Include(s => s.Statistics).Include(s => s.Characteristics);
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
                query = query.Where(st => st.Statistics.Any(s => statisticGroupList.Contains(s.StatisticGroupTypeID.ToString()) || statisticGroupList.Contains(s.StatisticGroupType.Code.ToLower())));
            }
            return query;
        }
        public Task<Station> GetStation(string identifier)
        {
            return GetStations().Include("Agency").Include("StationType").Include("Characteristics.Citation").Include("Characteristics.VariableType").Include("Characteristics.UnitType")
                .Include("Statistics.PredictionInterval").Include("Statistics.StatisticErrors").Include("Statistics.StatisticErrors.ErrorType").Include("Statistics.RegressionType").Include("Statistics.UnitType")
                .Include("Statistics.Citation").Include("Statistics.StatisticGroupType").Include("Region").FirstOrDefaultAsync(s => s.Code == identifier || s.ID.ToString() == identifier);
        }
        public IQueryable<Station> GetNearest(double lat, double lon, double radius)
        {
            var radius_m = radius * 1000; //GageStatsDB searches in meters by default, user has specified km
            var query = String.Format(getSQLStatement(sqltypeenum.stationsbyradius), lat, lon, radius_m);
            return FromSQL<Station>(query);
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
        public IQueryable<StationType> GetStationTypes()
        {
            return Select<StationType>();
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
        public IQueryable<Statistic> GetStatistics(string stationIDOrCode = null)
        {
            IQueryable<Statistic> query = Select<Statistic>().Include(s => s.PredictionInterval).Include("StatisticErrors.ErrorType")
                    .Include(s => s.RegressionType).Include(s => s.StatisticGroupType);
            if (stationIDOrCode != null)
            {
                query = query.Where(s => s.Station.Code == stationIDOrCode || s.Station.ID.ToString() == stationIDOrCode);
            }
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
        public IQueryable<Region> GetRegions()
        {
            return this.Select<Region>();
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
        public IQueryable<RegressionType> GetRegressions()
        {
            return this.Select<RegressionType>().OrderBy(rt => rt.ID);
        }
        public Task<RegressionType> GetRegression(Int32 ID)
        {
            return this.Find<RegressionType>(ID);
        }
        public StatisticGroupType GetStatisticGroupByCode(string code)
        {
            return this.Select<StatisticGroupType>().FirstOrDefault(r => string.Equals(r.Code.ToLower(), code.ToLower()));
        }
        public IQueryable<StatisticGroupType> GetStatisticGroups(List<string> defTypeList = null)
        {
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
        public IQueryable<VariableType> GetVariables(List<string> statisticGroupList = null)
        {
            IQueryable<VariableType> query = this.Select<VariableType>().Include(vt => vt.MetricUnitType).Include(vt => vt.EnglishUnitType).Include(vt => vt.StatisticGroupType);
            if (statisticGroupList != null && statisticGroupList.Count > 0)
            {
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
            stationsbyradius
        }
    }
}
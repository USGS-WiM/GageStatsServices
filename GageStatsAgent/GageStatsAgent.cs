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
using NetTopologySuite.Geometries;
using NetTopologySuite;

namespace GageStatsAgent
{
    public interface IGageStatsAgent:IAuthenticationAgent
    {
        //Agency
        IQueryable<Agency> GetAgencies();
        Task<Agency> GetAgency(Int32 ID);
        Task<Agency> Add(Agency item);
        Task<IEnumerable<Agency>> Add(List<Agency> items);
        Task<Agency> Update(Int32 pkId, Agency item);
        Task DeleteAgency(Int32 id);

        //Characteristic
        IQueryable<Characteristic> GetCharacteristics();
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
        IQueryable<Station> GetStations(List<string> stationTypeList = null, List<string> agencyList = null);
        Task<Station> GetStation(string identifier);
        IQueryable<Station> GetNearest(double lat, double lon, double radius);
        //Task<Station> GetNearestStations(string identifier, double radius);
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
        IQueryable<Statistic> GetStatistics();
        Task<Statistic> GetStatistic(Int32 ID);
        Task<Statistic> Add(Statistic item);
        Task<IEnumerable<Statistic>> Add(List<Statistic> items);
        Task<Statistic> Update(Int32 pkId, Statistic item);
        Task DeleteStatistic(Int32 id);

        //User
        IQueryable<User> GetUsers();
        Task<User> GetUser(Int32 ID);
        Task<User> Add(User item);
        Task<IEnumerable<User>> Add(List<User> items);
        Task<User> Update(Int32 pkId, User item);
        Task DeleteUser(Int32 id);

        //Readonly (Shared Views) methods
        IQueryable<ErrorType> GetErrors();
        Task<ErrorType> GetError(Int32 ID);
        IQueryable<StatisticGroupType> GetStatisticGroups();
        Task<RegressionType> GetRegression(Int32 ID);
        IQueryable<RegressionType> GetRegressions();
        Task<StatisticGroupType> GetStatisticGroup(Int32 ID);
        IQueryable<UnitType> GetUnits();
        Task<UnitType> GetUnit(Int32 ID);
        IQueryable<UnitSystemType> GetUnitSystems();
        Task<UnitSystemType> GetUnitSystem(Int32 ID);
        IQueryable<VariableType> GetVariables();
        Task<VariableType> GetVariable(Int32 ID);
    }

    public class GageStatsAgent :DBAgentBase, IGageStatsAgent
    {
        #region Properties
        private readonly IDictionary<Object, Object> _messages;
        #endregion
        #region Constructor
        public GageStatsAgent(GageStatsDBContext context, IHttpContextAccessor httpContextAccessor) :base(context)
        {
            this._messages = httpContextAccessor.HttpContext.Items;
        }
        #endregion
        #region Methods    
        #region Agency
        public IQueryable<Agency> GetAgencies()
        {
            return Select<Agency>();
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
        public IQueryable<Characteristic> GetCharacteristics()
        {
            return Select<Characteristic>().Include(c=>c.VariableType).Include(c=>c.UnitType);
        }
        public Task<Characteristic> GetCharacteristic(int ID)
        {
            return GetCharacteristics().FirstOrDefaultAsync(c=>c.ID == ID);
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
        public IQueryable<Station> GetStations(List<string> stationTypeList = null, List<string> agencyList = null)
        {
            var query = this.Select<Station>();
            // if filters, apply them before returning query
            if (stationTypeList != null && stationTypeList.Any())
                query = query.Where(st => stationTypeList.Contains(st.StationTypeID.ToString()) || stationTypeList.Contains(st.StationType.Code.ToLower()));
            if (agencyList != null && agencyList.Any())
                query = query.Where(st => agencyList.Contains(st.AgencyID.ToString()) || agencyList.Contains(st.Agency.Code.ToLower()));
            return query;
        }
        public Task<Station> GetStation(string identifier)
        {
             return GetStations().Include("Characteristics.Citation").Include("Statistics.PredictionInterval").Include("Statistics.StatisticErrors")
                .Include("Statistics.Citation").FirstOrDefaultAsync(s => s.Code == identifier || s.ID.ToString() == identifier);
        }
        public IQueryable<Station> GetNearest(double lat, double lon, double radius)
        {
            GeometryFactory Geography = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
            var coordinate = new Coordinate();
            coordinate.Y = lat;
            coordinate.X = lon;
            var point = Geography.CreatePoint(coordinate);
            var query = this.Select<Station>().Where(x => x.Location.Within(point.Buffer(radius)));
            return query; //.Select(
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
        public IQueryable<Statistic> GetStatistics()
        {
            return Select<Statistic>().Include(s=>s.PredictionInterval).Include("StatisticErrors.ErrorType")
                .Include(s=>s.RegressionType).Include(s=>s.StatisticGroupType);
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
        #region User
        public IQueryable<User> GetUsers()
        {
            return Select<User>();
        }
        public Task<User> GetUser(int ID)
        {
            return Find<User>(ID);
        }
        public IUser GetUserByUsername(string username)
        {
            return Select<User>().FirstOrDefault(r => string.Equals(r.Username.ToLower(), username.ToLower()));
            throw new NotImplementedException();
        }
        public IUser GetUserByID(int id)
        {
            return new User() { FirstName = "Jeremy", Role = Role.Admin, Username = "me-here", Password = "yellow", ID = 1, Salt="yes please" };
        }

        public IUser AuthenticateUser(string username, string password)
        {
            //this is where one authenticates the username/password before passing back user
            var user = (User)GetUserByUsername(username);
            if (user == null || !WIM.Security.Cryptography.VerifyPassword(password, user.Salt, user.Password))
            {
                return null;
            }
            return new User() { FirstName = "Jeremy", Role = Role.Admin, Username = username, Password = password, ID = 1 };
        }
        public Task<User> Add(User item)
        {
            return Add<User>(item);
        }
        public Task<IEnumerable<User>> Add(List<User> items)
        {
            return Add<User>(items);
        }
        public Task<User> Update(int pkId, User item)
        {
            return Update<User>(pkId, item);
        }
        public Task DeleteUser(int id)
        {
            return Delete<User>(id);
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
                return this.Select<RegressionType>();
        }
        public Task<RegressionType> GetRegression(Int32 ID)
        {
            return this.Find<RegressionType>(ID);
        }
        public StatisticGroupType GetStatisticGroupByCode(string code)
        {
            return this.Select<StatisticGroupType>().FirstOrDefault(r => string.Equals(r.Code.ToLower(), code.ToLower()));
        }
        public IQueryable<StatisticGroupType> GetStatisticGroups()
        {
            return this.Select<StatisticGroupType>();
        }
       public Task<StatisticGroupType> GetStatisticGroup(Int32 ID)
        {
            return this.Find<StatisticGroupType>(ID);
        }
        public IQueryable<UnitType> GetUnits()
        {
            return this.Select<UnitType>();
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
        public IQueryable<VariableType> GetVariables()
        {
            return this.Select<VariableType>();
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
    }

}
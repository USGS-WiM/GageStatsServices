//------------------------------------------------------------------------------
//----- HttpController ---------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2019 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   Handles resources through the HTTP uniform interface.
//
//discussion:   Controllers are objects which handle all interaction with resources. 
//              
//
// 

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using SharedDB.Resources;
using Shared.Controllers;
using GageStatsAgent;
using System.Threading.Tasks;
using System.Collections.Generic;
using SharedAgent;
using System.Linq;
using NetTopologySuite.Geometries;
using WIM.Exceptions.Services;
using WIM.Services.Attributes;
using WIM.Security.Authorization;
using System.Security.Claims;

namespace GageStatsServices.Controllers
{
    [Route("[controller]")]
    [APIDescription(type = DescriptionType.e_link, Description = "/Docs/StatisticGroups/summary.md")]
    public class StatisticGroupsController : StatisticGroupsControllerBase
    {
        protected IGageStatsAgent agent;
        public StatisticGroupsController(IGageStatsAgent sa, ISharedAgent shared_sa) : base(shared_sa)
        {
            this.agent = sa;
        }

        #region METHOD
        [HttpGet(Name ="Statistic Groups")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/StatisticGroups/Get.md")]
        public async Task<IActionResult> Get([FromQuery] string defTypes = "", [FromQuery] string regions = "", [FromQuery] string stationTypes = "", [FromQuery] string agencies = "", [FromQuery] string regressionTypes = "",
            [FromQuery] string variableTypes = "", [FromQuery] string filterText = null)
        {
            IQueryable<StatisticGroupType> entities = null;
            try
            {
                List<string> defTypeList = parse(defTypes);
                List<string> regionList = parse(regions);
                List<string> stationTypeList = parse(stationTypes);
                List<string> agencyList = parse(agencies);
                List<string> regressionTypeList = parse(regressionTypes);
                List<string> variableTypeList = parse(variableTypes);

                entities = agent.GetStatisticGroups(defTypeList, regionList, stationTypeList, agencyList, regressionTypeList, variableTypeList, filterText);

                sm($"statistic group count {entities.Count()}");
                return Ok(entities);

            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("{id}", Name ="Statistic Group")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/StatisticGroups/GetDistinct.md")]
        public override async Task<IActionResult> Get(int id)
        {
            try
            {
                if(id<0) return new BadRequestResult(); // This returns HTTP 404

                return Ok(await agent.GetStatisticGroup(id));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }
        #endregion
        #region Helper Methods
        #endregion
    }
}

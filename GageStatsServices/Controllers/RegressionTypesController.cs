﻿//------------------------------------------------------------------------------
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
using WIM.Services.Attributes;
using WIM.Security.Authorization;

namespace GageStatsServices.Controllers
{
    [Route("[controller]")]
    [APIDescription(type = DescriptionType.e_link, Description = "/Docs/RegressionTypes/summary.md")]
    public class RegressionTypesController : RegressionTypesControllerBase
    {
        protected IGageStatsAgent agent;
        public RegressionTypesController(IGageStatsAgent sa, ISharedAgent shared_sa) : base(shared_sa)
        {
            this.agent = sa;
        }

        #region METHOD
        [HttpGet(Name = "Regression Types")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/RegressionTypes/Get.md")]
        public async Task<IActionResult> GetRegressionTypes([FromQuery] string regions = "", [FromQuery] string stationTypes = "", [FromQuery] string agencies = "",
            [FromQuery] string variableTypes = "", [FromQuery] string statisticGroups = "", [FromQuery] string filterText = null, [FromQuery] bool? isRegulated = null)
        {
            IQueryable<RegressionType> entities = null;
            try
            {
                List<string> regionList = parse(regions);
                List<string> stationTypeList = parse(stationTypes);
                List<string> agencyList = parse(agencies);
                List<string> variableTypeList = parse(variableTypes);
                List<string> statisticGroupList = parse(statisticGroups);

                entities = agent.GetRegressions(regionList, stationTypeList, agencyList, variableTypeList, statisticGroupList, filterText, isRegulated);

                sm($"regressiontype count {entities.Count()}");
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("{id}", Name ="Regression Type")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/RegressionTypes/GetDistinct.md")]
        public override async Task<IActionResult> Get(int id)
        {
            try
            {
                if(id<0) return new BadRequestResult(); // This returns HTTP 404

                return Ok(agent.GetRegression(id));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }
        #endregion
    }
}

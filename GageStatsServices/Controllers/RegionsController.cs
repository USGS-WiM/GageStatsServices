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
using GageStatsDB.Resources;
using GageStatsAgent;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WIM.Services.Attributes;
using WIM.Security.Authorization;
using SharedDB.Resources;
using SharedAgent;
using Shared.Controllers;

namespace GageStatsServices.Controllers
{
    [Route("[controller]")]
    [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Regions/summary.md")]
    public class RegionsController: RegionsControllerBase
    {
        protected IGageStatsAgent agent;
        protected ISharedAgent shared;
        public RegionsController(IGageStatsAgent sa, ISharedAgent shared_sa) : base(shared_sa)
        {
            this.agent = sa;
            this.shared = shared_sa;
        }
        #region METHODS
        [HttpGet(Name = "Regions")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Regions/Get.md")]
        public async Task<IActionResult> Get([FromQuery] string stationTypes = "", [FromQuery] string agencies = "", [FromQuery] string regressionTypes = "", [FromQuery] string variableTypes = "", [FromQuery] string statisticGroups = "", [FromQuery] string filterText = null, [FromQuery] bool? isRegulated = null)
        {
            IQueryable<Region> entities = null;
            try
            {
                List<string> stationTypeList = parse(stationTypes);
                List<string> agencyList = parse(agencies);
                List<string> regressionTypeList = parse(regressionTypes);
                List<string> variableTypeList = parse(variableTypes);
                List<string> statisticGroupList = parse(statisticGroups);

                entities = agent.GetRegions(stationTypeList, agencyList, regressionTypeList, variableTypeList, statisticGroupList, filterText, isRegulated);
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }

        }

        [HttpGet("{id}", Name = "Region")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Regions/GetDistinct.md")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                if (String.IsNullOrEmpty(id)) return new BadRequestResult();
                var item = agent.GetRegionByIDOrCode(id);
                if (item == null) return new BadRequestObjectResult(new Error(errorEnum.e_badRequest));
                return Ok(item);
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost(Name = "Add Region")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Regions/Add.md")]
        public async Task<IActionResult> Post([FromBody]Region entity)
        {
            try
            {
                if (!isValid(entity)) return new BadRequestResult();
                return Ok(await shared.Add(entity));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost("[action]", Name = "Region Batch Upload")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Regions/Batch.md")]
        public async Task<IActionResult> Batch([FromBody]List<Region> entities)
        {
            try
            {
                entities.ForEach(e => e.ID = 0);
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");

                return Ok(await shared.Add(entities));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPut("{id}", Name = "Edit Region")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Regions/Edit.md")]
        public async Task<IActionResult> Put(int id, [FromBody]Region entity)
        {
            try
            {
                if (id < 1 || !isValid(entity)) return new BadRequestResult();
                return Ok(await shared.Update(id, entity));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }

        }

        [HttpDelete("{id}", Name = "Delete Region")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Regions/Delete.md")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {

                if (id < 1) return new BadRequestResult();
                await shared.DeleteRegion(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }
        #endregion
    }
}

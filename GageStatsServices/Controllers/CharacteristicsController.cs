﻿//------------------------------------------------------------------------------
//----- HttpController ---------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   Handles resources through the HTTP uniform interface.
//
//discussion:   Controllers are objects which handle all interaction with resources. 
//              
//
// 

using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using GageStatsAgent;
using System.Threading.Tasks;
using System.Collections.Generic;
using WIM.Resources;
using WIM.Services.Attributes;
using Microsoft.AspNetCore.Authorization;
using WIM.Security.Authentication;
using GageStatsDB.Resources;
using WIM.Security.Authorization;
using WIM.Exceptions.Services;

namespace GageStatsServices.Controllers
{
    [Route("[controller]")]
    [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Characteristics/summary.md")]
    public class CharacteristicsController : WIM.Services.Controllers.ControllerBase
    {
        public IGageStatsAgent agent { get; set; }
        public CharacteristicsController(IGageStatsAgent agent ) : base()
        {
            this.agent = agent;
        }
        #region METHODS
        [HttpGet(Name = "Characteristics")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Characteristics/Get.md")]
        public async Task<IActionResult> Get([FromQuery] string stationIDOrCode, [FromQuery] string citations = "", [FromQuery] string statisticGroups = "", [FromQuery] int page = 1, [FromQuery] int pageCount = 50)
        {
            try
            {
                List<string> citationList = parse(citations);
                List<string> statisticGroupList = parse(statisticGroups);

                IQueryable<Characteristic> entities = agent.GetCharacteristics(stationIDOrCode, citationList, statisticGroupList);

                // get number of items to skip for pagination
                var skip = (page - 1) * pageCount;
                sm("Returning page " + page + " of " + (entities.Count() / pageCount + 1) + ".");
                sm("Total Count: " + entities.Count());
                entities = entities.Skip(skip).Take(pageCount);

                return Ok(entities);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}", Name = "Characteristic")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Characteristics/GetDistinct.md")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id < 0) return new BadRequestResult(); // This returns HTTP 404
                return Ok(await agent.GetCharacteristic(id));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost(Name = "Add Characteristic")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Characteristics/Add.md")]
        public async Task<IActionResult> Post([FromBody]Characteristic entity)
        {
            try
            {
                if (!isValid(entity)) return new BadRequestResult(); // This returns HTTP 404
                return Ok(await agent.Add(entity));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost("[action]", Name = "Characteristic Batch Upload")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Characteristics/Batch.md")]
        public async Task<IActionResult> Batch([FromBody]List<Characteristic> entities)
        {
            try
            {

                entities.ForEach(e => e.ID = 0);
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");
                return Ok(await agent.Add(entities));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPut("{id}", Name = "Edit Characteristic")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Characteristics/Edit.md")]
        public async Task<IActionResult> Put(int id, [FromBody]Characteristic entity)
        {
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestResult(); // This returns HTTP 404
                return Ok(await agent.Update(id, entity));

            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }

        }

        [HttpDelete("{id}", Name = "Delete Characteristic")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Characteristics/Delete.md")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await agent.DeleteCharacteristic(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }
        #endregion
        #region HELPER METHODS

        #endregion
    }
}

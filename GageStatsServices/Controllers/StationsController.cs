//------------------------------------------------------------------------------
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
using GageStatsAgent;
using System.Threading.Tasks;
using System.Collections.Generic;
using WIM.Resources;
using WIM.Services.Attributes;
using Microsoft.AspNetCore.Authorization;
using WIM.Security.Authorization;
using GageStatsDB.Resources;
using WIM.Exceptions.Services;
using System.Linq;

namespace GageStatsServices.Controllers
{
    [Route("[controller]")]
    [APIDescription(type = DescriptionType.e_link, Description = "/Docs/GageStats/summary.md")]
    public class StationsController : WIM.Services.Controllers.ControllerBase
    {
        public IGageStatsAgent agent { get; set; }
        public StationsController(IGageStatsAgent agent ) : base()
        {
            this.agent = agent;
        }
        #region METHODS
        [HttpGet(Name = "Stations")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Stations/Get.md")]
        public async Task<IActionResult> Get([FromQuery] string stationTypes = "", [FromQuery] string agencies = "", [FromQuery] int page = 1, [FromQuery] int pageCount = 50)
        {
            try
            {
                // Do we want/need a region filter?
                List<string> stationTypeList = parse(stationTypes);
                List<string> agencyList = parse(agencies);

                IQueryable<Station> entities = agent.GetStations(stationTypeList, agencyList).OrderBy(s => s.ID);

                // get number of items to skip for pagination
                var skip = (page - 1) * pageCount;
                sm("Returning page " + page + " of " + (entities.Count() / pageCount + 1) + ".");
                return Ok(entities.Skip(skip).Take(pageCount));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{idOrCode}", Name = "Station")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Stations/GetDistinct.md")]
        public async Task<IActionResult> Get(string idOrCode)
        {
            try
            {
                var entity = await agent.GetStation(idOrCode);
                if (entity != null)
                {
                    return Ok(entity);
                } else
                {
                    throw new BadRequestException("Station not found");
                }
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("Nearest", Name = "Nearest Station")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Stations/GetDistinct.md")]
        public async Task<IActionResult> Nearest([FromQuery]string idOrCode, [FromQuery]double radius)
        {
            try
            {
                //var entity = await agent.GetNearestStations(idOrCode, radius);
                IQueryable<Station> entities = agent.GetNearest(idOrCode, radius).OrderBy(s => s.ID);

                return Ok(entities);
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost(Name = "Add Station")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Stations/Add.md")]
        public async Task<IActionResult> Post([FromBody]Station entity)
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

        [HttpPost("[action]", Name = "Station Batch Upload")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Stations/Batch.md")]
        public async Task<IActionResult> Batch([FromBody]List<Station> entities)
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

        [HttpPut("{id}", Name = "Edit Station")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Stations/Edit.md")]
        public async Task<IActionResult> Put(int id, [FromBody]Station entity)
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

        [HttpDelete("{id}", Name = "Delete Station")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Stations/Delete.md")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await agent.DeleteStation(id);
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

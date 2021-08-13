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
using System.Linq;
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
    [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Statistics/summary.md")]
    public class StatisticsController : WIM.Services.Controllers.ControllerBase
    {
        public IGageStatsAgent agent { get; set; }
        public StatisticsController(IGageStatsAgent agent ) : base()
        {
            this.agent = agent;
        }
        #region METHODS
        [HttpGet(Name = "Statistics")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Statistics/Get.md")]
        public async Task<IActionResult> Get([FromQuery] string stationIDOrCode, [FromQuery] string citations = "", [FromQuery] string statisticGroups = "", [FromQuery] int page = 1, [FromQuery] int pageCount = 50)
        {
            try
            {
                List<string> citationList = parse(citations);
                List<string> statisticGroupList = parse(statisticGroups);

                IQueryable<Statistic> entities = agent.GetStatistics(stationIDOrCode, citationList, statisticGroupList);

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

        [HttpGet("{id}", Name = "Statistic")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Statistics/GetDistinct.md")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id < 0) return new BadRequestResult(); // This returns HTTP 404
                return Ok(await agent.GetStatistic(id));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost(Name = "Add Statistic")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Statistics/Add.md")]
        public async Task<IActionResult> Post([FromBody]Statistic entity)
        {
            try
            {
                if (!isValid(entity)) return new BadRequestResult(); // This returns HTTP 404
                // if preferred, mark others at this station as not preferred
                entity = await agent.Add(entity);
                if (entity.IsPreferred) agent.TriggerStatisticPreferred(entity.ID, entity.RegressionTypeID, entity.StationID); // didn't work for some reason
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost("[action]", Name = "Statistic Batch Upload")]
        [Authorize(Policy = "AdminOnly")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Statistics/Batch.md")]
        public async Task<IActionResult> Batch([FromBody]List<Statistic> entities)
        {
            try
            {

                entities.ForEach(e => e.ID = 0);
                // if preferred, mark others at this station as not preferred
                if (!isValid(entities)) return new BadRequestObjectResult("Object is invalid");
                entities = (await agent.Add(entities)).ToList();
                entities.ForEach(e =>
                {
                    if (e.IsPreferred) agent.TriggerStatisticPreferred(e.ID, e.RegressionTypeID, e.StationID);
                });
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPut("{id}", Name = "Edit Statistic")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Statistics/Edit.md")]
        public async Task<IActionResult> Put(int id, [FromBody]Statistic entity)
        {
            try
            {
                if (id < 0 || !isValid(entity)) return new BadRequestResult(); // This returns HTTP 404
                // if preferred, mark others at this station as not preferred
                entity = await agent.Update(id, entity);
                if (entity.IsPreferred) agent.TriggerStatisticPreferred(entity.ID, entity.RegressionTypeID, entity.StationID);
                return Ok(await agent.Update(id, entity));

            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpDelete("{id}", Name = "Delete Statistic")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Statistics/Delete.md")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await agent.DeleteStatistic(id);
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

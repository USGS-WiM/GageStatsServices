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

namespace GageStatsServices.Controllers
{
    [Route("[controller]")]
    [APIDescription(type = DescriptionType.e_link, Description = "/Docs/GageStats/summary.md")]
    public class CitationsController : WIM.Services.Controllers.ControllerBase
    {
        public IGageStatsAgent agent { get; set; }
        public CitationsController(IGageStatsAgent agent ) : base()
        {
            this.agent = agent;
        }
        #region METHODS
        [HttpGet(Name = "Citations")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Citations/Get.md")]
        public async Task<IActionResult> Get()
        {
            try
            {
                
                return Ok(agent.GetCitations());
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}", Name = "Citation")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Citations/GetDistinct.md")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id < 0) return new BadRequestResult(); // This returns HTTP 404
                return Ok(await agent.GetCitation(id));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost(Name = "Add Citation")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Citations/Add.md")]
        public async Task<IActionResult> Post([FromBody]Citation entity)
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

        [HttpPost("[action]", Name = "Citation Batch Upload")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Citations/Batch.md")]
        public async Task<IActionResult> Batch([FromBody]List<Citation> entities)
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

        [HttpPut("{id}", Name = "Edit Citation")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Citations/Edit.md")]
        public async Task<IActionResult> Put(int id, [FromBody]Citation entity)
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

        [HttpDelete("{id}", Name = "Delete Citation")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Citations/Delete.md")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await agent.DeleteCitation(id);
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

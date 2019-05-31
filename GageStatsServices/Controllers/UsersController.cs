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
using WIM.Security.Authentication;
using GageStatsDB.Resources;
using GageStatsServices.Resources;
using WIM.Security.Authorization;
using WIM.Services.Security.Authentication.JWTBearer;
using Microsoft.Extensions.Options;

namespace GageStatsServices.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [APIDescription(type = DescriptionType.e_link, Description = "/Docs/GageStats/summary.md")]
    public class UsersController : JwtBearerAuthenticationBase
    {
        //Overrides base property
        public new IGageStatsAgent agent => (IGageStatsAgent)base.agent; 
        public UsersController(IGageStatsAgent agent, IOptions<JwtBearerSettings> jwtsettings ) : 
            base(agent,jwtsettings.Value.SecretKey)
        {

        }
        #region METHODS
        [HttpGet(Name = "Users")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Users/Get.md")]
        public async Task<IActionResult> Get()
        {
            try
            {                
                return Ok(agent.GetUsers());
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}", Name = "User")]
        [Authorize(Policy = Policy.Managed)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Users/GetDistinct.md")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id < 0) return new BadRequestResult(); // This returns HTTP 404
                return Ok(await agent.GetUser(id));
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpPost(Name = "Add User")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Users/Add.md")]
        public async Task<IActionResult> Post([FromBody]User entity)
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

        [HttpPost("[action]", Name = "User Batch Upload")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Users/Batch.md")]
        public async Task<IActionResult> Batch([FromBody]List<User> entities)
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

        [HttpPut("{id}", Name = "Edit User")]
        [Authorize(Policy = Policy.Managed)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Users/Edit.md")]
        public async Task<IActionResult> Put(int id, [FromBody]User entity)
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

        [HttpDelete("{id}", Name = "Delete User")]
        [Authorize(Policy = Policy.AdminOnly)]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Users/Delete.md")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await agent.DeleteUser(id);
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

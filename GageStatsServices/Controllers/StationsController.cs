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
using System.IO;
using GageStatsAgent.Resources;
using Microsoft.Extensions.Options;
using GageStatsAgent.ServiceAgents;
using GageStatsServices.Filters;

namespace GageStatsServices.Controllers
{
    [Route("[controller]")]
    [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Stations/summary.md")]
    public class StationsController : WIM.Services.Controllers.ControllerBase
    {
        public IGageStatsAgent agent { get; set; }
        private NLDISettings NLDIsettings { get; set; }
        private NavigationSettings Navigationsettings { get; set; }
        public StationsController(IGageStatsAgent agent, IOptions<NLDISettings> nldisettings, IOptions<NavigationSettings> navsettings) : base()
        {
            NLDIsettings = nldisettings.Value;
            Navigationsettings = navsettings.Value;
            this.agent = agent;
        }
        #region METHODS
        [HttpGet(Name = "Stations")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Stations/Get.md")]
        public async Task<IActionResult> Get([FromQuery] string regions = "", [FromQuery] string stationTypes = "", [FromQuery] string agencies = "", [FromQuery] string regressionTypes = "", [FromQuery] string variableTypes = "",
            [FromQuery] string statisticGroups = "", [FromQuery] string filterText = null, [FromQuery] int page = 1, [FromQuery] int pageCount = 50, [FromQuery] bool includeStats = false, [FromQuery] bool geojson = false)
        {
            try
            {
                List<string> regionList = parse(regions);
                List<string> stationTypeList = parse(stationTypes);
                List<string> agencyList = parse(agencies);
                List<string> regressionTypeList = parse(regressionTypes);
                List<string> variableTypeList = parse(variableTypes);
                List<string> statisticGroupList = parse(statisticGroups);

                IQueryable<Station> entities = agent.GetStations(regionList, stationTypeList, agencyList, regressionTypeList, variableTypeList, statisticGroupList, includeStats, filterText);

                // get number of items to skip for pagination
                var skip = (page - 1) * pageCount;
                sm("Returning page " + page + " of " + (entities.Count() / pageCount + 1) + ".");
                sm("Total Count: " + entities.Count());
                entities = entities.Skip(skip).Take(pageCount);

                if (geojson) return Ok(GeojsonFormatter.ToGeojson(entities)); // return as geojson

                return Ok(entities);
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
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Stations/GetNearest.md")]
        public async Task<IActionResult> Nearest([FromQuery]double lat, [FromQuery]double lon, [FromQuery]double radius)
        {
            try
            {
                IQueryable<Station> gages = agent.GetNearest(lat, lon, radius);

                return Ok(gages);
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex);
            }
        }

        [HttpGet("Network", Name = "Nearest Stations on Network")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Stations/GetNearestOnNetwork.md")]
        public async Task<IActionResult> Network([FromQuery] double lat, [FromQuery] double lon, [FromQuery] double distance, [FromQuery] int page = 1, [FromQuery] int pageCount = 50)
        {
            try
            {
                //var nav_sa = new NavigationServiceAgent(this.Navsettings);
                var nldi_sa = new NLDIServiceAgent(this.NLDIsettings, this.Navigationsettings);
                var isOk = await nldi_sa.ReadNLDIAsync(lat, lon, distance);

                if (!isOk) throw new Exception("Failed to retrieve NLDI data");
                return Ok(nldi_sa.getStations());
            }
            catch (Exception ex)
            {                
                return await HandleExceptionAsync(ex);
            }            
        }

        [HttpGet("Bounds", Name = "Stations Within Bounding Box")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Stations/GetWithinBounds.md")]
        public async Task<IActionResult> WithinBounds([FromQuery] double xmin, [FromQuery] double ymin, [FromQuery] double xmax, [FromQuery] double ymax, [FromQuery] bool geojson = false)
        {
            try
            {
                IEnumerable<Station> gages = agent.GetStationsWithinBounds(xmin, ymin, xmax, ymax).AsEnumerable();
                if (geojson) return Ok(GeojsonFormatter.ToGeojson(gages));

                return Ok(gages);
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
                var existingStation = await agent.GetStation(id.ToString());
                if (existingStation.Statistics.Count() > 0 || existingStation.Characteristics.Count() > 0) throw new Exception("Statistics or characteristics are assigned to this station."); 
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

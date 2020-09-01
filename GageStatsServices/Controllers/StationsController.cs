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
using ChoETL;
using System.IO;

namespace GageStatsServices.Controllers
{
    [Route("[controller]")]
    [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Stations/summary.md")]
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
        public async Task<IActionResult> Get([FromQuery] string stationTypes = "", [FromQuery] string agencies = "", [FromQuery] string regressionTypes = "", [FromQuery] string variableTypes = "",
            [FromQuery] string statisticGroups = "", [FromQuery] string filterText = null, [FromQuery] int page = 1, [FromQuery] int pageCount = 50, [FromQuery] bool includeStats = false)
        {
            try
            {
                List<string> stationTypeList = parse(stationTypes);
                List<string> agencyList = parse(agencies);
                List<string> regressionTypeList = parse(regressionTypes);
                List<string> variableTypeList = parse(variableTypes);
                List<string> statisticGroupList = parse(statisticGroups);

                IQueryable<Station> entities = agent.GetStations(stationTypeList, agencyList, regressionTypeList, variableTypeList, statisticGroupList, includeStats);

                if (filterText != null)
                {
                    entities = entities.Where(s => s.Name.ToUpper().Contains(filterText.ToUpper()) || s.Code.ToUpper().Contains(filterText.ToUpper()));
                }

                entities = entities.OrderBy(s => s.ID);

                // get number of items to skip for pagination
                var skip = (page - 1) * pageCount;
                sm("Returning page " + page + " of " + (entities.Count() / pageCount + 1) + ".");
                sm("Total Count: " + entities.Count());
                return Ok(entities.Skip(skip).Take(pageCount));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{idOrCode}", Name = "Station")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/Stations/GetDistinct.md")]
        public async Task<IActionResult> Get(string idOrCode, [FromQuery]bool csv = false)
        {
            try
            {
                var entity = await agent.GetStation(idOrCode);

        //        if (entity != null && csv == true)
        //        {
        //            public void GenerateReport<T>(List<T> rows) where T : class
        //            {
        //            }
        //            var items = new List<Station>();
        //            var myObj = entity;
                    
        //            for (int i = 0; i < 100; i++)
        //            {
        //                items.Add(myObj);
        //            }
        //            var output = GenerateReport<Station>(items);
        //            Console.Write(output);
        //        }
        //        public string GenerateReport<T>(List<T> items) where T : class
        //        {
        //            var output = “”;
        //            var delimiter = ‘;’;
        //            var properties = typeof(T).GetProperties()
        //             .Where(n =>
        //             n.PropertyType == typeof(string)
        //             || n.PropertyType == typeof(bool)
        //             || n.PropertyType == typeof(char)
        //             || n.PropertyType == typeof(byte)
        //             || n.PropertyType == typeof(decimal)
        //             || n.PropertyType == typeof(int)
        //             || n.PropertyType == typeof(DateTime)
        //             || n.PropertyType == typeof(DateTime?));
        //            using (var sw = new StringWriter())
        //            {
        //                var header = properties
        //                .Select(n => n.Name)
        //                .Aggregate((a, b) => a + delimiter + b);
        //                sw.WriteLine(header);
        //                foreach (var item in items)
        //                {
        //                    var row = properties
        //                    .Select(n => n.GetValue(item, null))
        //                    .Select(n => n == null ? “null” : n.ToString()).Aggregate((a, b) => a + delimiter + b);
        //                    sw.WriteLine(row);
        //                }
        //                output = sw.ToString();
        //            }
        //    return Ok(output);
        //}
        //        else
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
        public async Task<IActionResult> Nearest([FromQuery]double lat, [FromQuery]double lon, [FromQuery]double radius, [FromQuery] int page = 1, [FromQuery] int pageCount = 50)
        {
            try
            {
                IQueryable<Station> gages = agent.GetNearest(lat, lon, radius);

                // get number of items to skip for pagination
                var skip = (page - 1) * pageCount;
                sm("Returning page " + page + " of " + (gages.Count() / pageCount + 1) + ".");
                return Ok(gages.Skip(skip).Take(pageCount));
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

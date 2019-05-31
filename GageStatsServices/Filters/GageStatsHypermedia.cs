//------------------------------------------------------------------------------
//----- NavigationHypermedia ---------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   Intersects the pipeline after
//
//discussion:   Controllers are objects which handle all interaction with resources. 
//              
//
// 
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIM.Hypermedia;
using WIM.Resources;
using WIM.Services.Filters;

namespace GageStatsServices.Filters
{
    public class GageStatsHypermedia : HypermediaBase
    {
        protected override List<Link> GetEnumeratedHypermedia(IHypermedia entity)
        {
            List<Link> results = null;
            switch (entity.GetType().Name)
            {
                case "GageStats":
                    results = new List<WIM.Resources.Link>();
                    //results.Add(Hyperlinks.Generate(BaseURI, "Citations", UrlHelper.RouteUrl("Region_Citations") + $"?regressionregions={String.Join(",", ((Scenario)entity).RegressionRegions.Select(r => r.ID))}", WIM.Resources.refType.GET));

                    break;

                default:
                    break;
            }

            return results;

        }

        protected override List<Link> GetReflectedHypermedia(IHypermedia entity)
        {
            List<Link> results = null;
            switch (entity.GetType().Name)
            {
                case "GageStats":
                    results = new List<WIM.Resources.Link>();
                   //results.Add(Hyperlinks.Generate(BaseURI, "Citations", UrlHelper.RouteUrl("Citations") + $"?regressionregions={String.Join(",", ((Scenario)entity).RegressionRegions.Select(r => r.ID))}", WIM.Resources.refType.GET));
                    break;                
                default:
                    break;
            }

            return results;
        }
    }
}

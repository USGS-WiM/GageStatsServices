//------------------------------------------------------------------------------
//----- GeojsonFormatter ---------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2020 WIM - USGS

//    authors:  Katrin E Jacobsen USGS Web Informatics and Mapping
//              
//  
//   purpose:   Formats response into Geojson FeatureCollection
//              
//
//
using System.Collections.Generic;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System.Reflection;

namespace GageStatsServices.Filters
{
    public class GeojsonFormatter
    {
        public static FeatureCollection ToGeojson(IEnumerable<object> entity)
        {

            if (entity == null) return null;

            var fc = new FeatureCollection();
            foreach (object item in entity)
            {
                var feat = new Feature();
                feat.Attributes = new AttributesTable();
                PropertyInfo[] arrayPropertyInfos = item.GetType().GetProperties();

                foreach (PropertyInfo property in arrayPropertyInfos)
                {
                    if (property.PropertyType.Name == "Geometry")
                    {
                        feat.Geometry = (Geometry)property.GetValue(item);
                    } else if (property.GetValue(item) != null)
                    {
                        feat.Attributes.Add(property.Name, property.GetValue(item));
                    }
                }
                fc.Add(feat);
            }
            return fc;
        }
    }
}

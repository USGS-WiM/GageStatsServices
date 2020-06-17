### About
This API is intended to document the available service endpoints and provide limited example usage to assist developers in building outside programs that leverage the same data sources in an effort to minimize duplicative efforts across the USGS and other agencies.

### Status
The Gage Statistics database contains information for gaging stations around the United States, including descriptive information and measured characteristics and statistics for those stations. The characteristics and statistics are used to develop and publish regression equations for estimating streamflow statistics at ungaged locations, which are stored in the National Streamflow Statistics (NSS) database, and can be viewed using the [NSS Services](https://test.streamstats.usgs.gov/docs/nssservices/), or the [NSS client](https://test.streamstats.usgs.gov/nss/). The NSS program is used in a variety of water-resources and emergency planning, management and regulatory purposes, and for designing of structures; such as bridges and culverts, and is used to estimate streamflow statistics in the [StreamStats client](https://streamstats.usgs.gov/ss/).
### Getting Started
The Gage Statistic Services perform multiple procedures which include database and geospatial queries and spatial operations to compile and create simple responses with hypermedia enabled links to related resources. The responses are intended to be directly consumed by custom client applications and used to more fully decouple the client-service relationship by providing directional hypermedia links within the resource response objects.
### Using the API
The URL and an example response can be obtained by accessing one of the resources and URI endpoints located on the sidebar (or selecting the menu button located on the bottom of the screen, if viewing on a mobile device). 

<img alt="sidebarImage.jpg" 
src="data:image/png;base64,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" />


The sidebar displays the available service resources, accompanying HTTP method and URI endpoints. Selecting a URL endpoint will display additional resource documentation information related to the selected endpoint, such as the description of the resource, the service URL, and any URL parameters, the parameter name, value type, a description of what the parameter represents, and/or whether the parameter is required or optional.

A REST query URL test tool is also available that builds an example URL based on the user-provided input parameter values and a simple query tool that provides an example of the requested response.

### Common Status Codes
Common HTTP status codes returned by the services

| Code &nbsp; &nbsp; &nbsp;   |When     
| ------- |---------
| `200`   | Status  `OK`.
| `400`   | Status `Bad Request`. The request data cannot be read.
| `404`   | Status `Not Found`. The resource is not available.
| `500`   | Status `Internal Server Error`. Please contact the administrator. 

### Example
Web service requests can be performed using most HTTP client libraries. The following illustrates a typical http request/response performed by a client application.

```
GET /GageStatsServices/agencies HTTP/1.1
Host: streamstats.usgs.gov
Accept: application/json
```
```
HTTP/1.1 200 OK

[
    {
        "id": 1,
        "name": "United States Geological Survey",
        "description": "United States Geological Survey",
        "code": "USGS"
    },
    {
        "id": 2,
        "name": "United States Department of Agriculture Research Service",
        "description": "United States Department of Agriculture Research Service",
        "code": "USARS"
    },
    {
        "id": 3,
        "name": "Undefined",
        "description": "Undefined",
        "code": "undef"
    }
]
```

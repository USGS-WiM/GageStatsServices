### About
This API is intended to document the available service endpoints and provide limited example usage to assist developers in building outside programs that leverage the same data sources in an effort to minimize duplicative efforts across the USGS and other agencies.

### Status
The GageStats database contains information for gaging stations around the United States, including descriptive information and measured characteristics and statistics for those stations. The characteristics and statistics are used to develop and publish regression equations for estimating streamflow statistics at ungaged locations, which are stored in the National Streamflow Statistics (NSS) database, and can be viewed using the [NSS Services](https://test.streamstats.usgs.gov/docs/nssservices/), or the [NSS client](https://test.streamstats.usgs.gov/nss/). The NSS program is used in a variety of water-resources and emergency planning, management and regulatory purposes, and for designing of structures; such as bridges and culverts, and is used to estimate streamflow statistics in the [StreamStats client](https://streamstats.usgs.gov/ss/).
### Getting Started
The GageStats services perform multiple procedures which include database and geospatial queries and spatial operations to compile and create simple responses with hypermedia enabled links to related resources. The responses are intended to be directly consumed by custom client applications and used to more fully decouple the client-service relationship by providing directional hypermedia links within the resource response objects.
### Using the API
The URL and an example response can be obtained by accessing one of the resources and URI endpoints located on the sidebar (or selecting the menu button located on the bottom of the screen, if viewing on a mobile device). 

<img alt="sidebarImage.jpg" 
src="data:image/png;base64,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" />


The sidebar displays the available service resources, accompanying HTTP method and URI endpoints. Selecting a URL endpoint will display additional resource documentation information related to the selected endpoint, such as the description of the resource, the service URL, any URL Parameters, parameter name, value type, a description of what the parameter represents, and/or whether the parameter is required or optional.

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

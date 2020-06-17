### Statistic batch upload
<span style="color:red">Requires Authentication</span>  
Provides the ability to batch upload statistic resources.

### Request Example
The REST URL section below displays the example url and the body/payload of the request used to simulate a response.

```
POST /gagestatsservices/statistics/batch HTTP/1.1
Host: streamstats.usgs.gov
Accept: application/json
content-type: application/json;charset=UTF-8
content-length: 576

[{
    "statisticGroupTypeID": 1,
    "regressionTypeID": 1,
    "stationID": 1,
    "value": 1,
	"unitTypeID": 1,
    "comments": "example statistic 1",
    "isPreferred": true,
    "yearsofRecord": 20
},
{
    "statisticGroupTypeID": 1,
    "regressionTypeID": 2,
    "stationID": 1,
    "value": 2,
	"unitTypeID": 2,
    "comments": "example statistic 2",
    "isPreferred": true,
    "yearsofRecord": 20
},
{
    "statisticGroupTypeID": 1,
    "regressionTypeID": 3,
    "stationID": 1,
    "value": 3,
	"unitTypeID": 3,
    "comments": "example statistic 3",
    "isPreferred": true,
    "yearsofRecord": 20
}]
```

```
HTTP/1.1 200 OK
[{
	"id": 25,
	"statisticGroupTypeID": 1,
    "regressionTypeID": 1,
    "stationID": 1,
    "value": 1,
	"unitTypeID": 1,
    "comments": "example statistic 1",
    "isPreferred": true,
    "yearsofRecord": 20
},
{
	"id":52,
	"statisticGroupTypeID": 1,
    "regressionTypeID": 2,
    "stationID": 1,
    "value": 2,
	"unitTypeID": 2,
    "comments": "example statistic 2",
    "isPreferred": true,
    "yearsofRecord": 20
},
{
	"id":53,
    "statisticGroupTypeID": 1,
    "regressionTypeID": 3,
    "stationID": 1,
    "value": 3,
	"unitTypeID": 3,
    "comments": "example statistic 3",
    "isPreferred": true,
    "yearsofRecord": 20
}]
```
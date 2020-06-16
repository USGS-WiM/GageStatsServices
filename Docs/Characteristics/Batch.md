## Characteristic batch upload
<span style="color:red">Requires Authentication</span>  
Provides the ability to batch upload characteristic resources.

#### Request Example
The REST URL section below displays the example url and the body/payload of the request used to simulate a response.

```
POST /gagestatsservices/characteristics/batch HTTP/1.1
Host: streamstats.usgs.gov
Accept: application/json
content-type: application/json;charset=UTF-8
content-length: 576

[{
    "stationID": 1,
    "variableTypeID": 1,
    "unitTypeID": 1,
	"value": 1,
    "comments": "example comment 1"
},
{
    "stationID": 1,
    "variableTypeID": 2,
    "unitTypeID": 2,
	"value": 2,
    "comments": "example comment 2"
},
{
    "stationID": 1,
    "variableTypeID": 3,
    "unitTypeID": 3,
	"value": 3,
    "comments": "example comment 3"
}]
```

```
HTTP/1.1 200 OK
[{
	"id": 100,
	"stationID": 1,
    "variableTypeID": 1,
    "unitTypeID": 1,
	"value": 1,
    "comments": "example comment 1"
},
{
    "id": 101,
	"stationID": 1,
    "variableTypeID": 2,
    "unitTypeID": 2,
	"value": 2,
    "comments": "example comment 2"
},
{
    "id": 102,
	"stationID": 1,
    "variableTypeID": 3,
    "unitTypeID": 3,
	"value": 3,
    "comments": "example comment 3"
}]
```
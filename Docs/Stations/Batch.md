### Station batch upload
<span style="color:red">Requires Administrator Authentication</span>    
Provides the ability to batch upload station resources.

### Request Example
The REST URL section below displays the example url and the body/payload of the request used to simulate a response.

```
POST /gagestatsservices/stations/batch HTTP/1.1
Host: streamstats.usgs.gov
Accept: application/json
content-type: application/json;charset=UTF-8
content-length: 576

[
	{
        "code":"11111111",
        "agencyID": 1,
        "name":"example station 1 at example location, ex. river",
        "stationTypeID":"1",
        "location": {
            "type": "Point",
            "coordinates": [
				11.111,
				-99.999
			]
        },
		"locationSource":"example source",
        "regionID": 1
	},
	{
        "code":"11111112",
        "agencyID": 1,
        "name":"example station 2 at example location, ex. river",
        "stationTypeID":"1",
        "location": {
            "type": "Point",
            "coordinates": [
                11.111,
				-99.999
            ]
        },
		"locationSource":"example source",
        "regionID": 1
	}
]
```

```
HTTP/1.1 200 OK
[
	{
        "id": 1,
        "code":"11111111",
        "agencyID": 1,
        "name":"example station 1 at example location, ex. river",
        "stationTypeID":"1",
        "location": {
            "type": "Point",
            "coordinates": [
                11.111,
				-99.999
            ]
        },
        "locationSource":"example source",
        "regionID": 1
	},
	{
        "id": 2,
        "code":"11111112",
        "agencyID": 1,
        "name":"example station 2 at example location, ex. river",
        "stationTypeID":"1",
        "location": {
            "type": "Point",
            "coordinates": [
                11.111,
				-99.999
            ]
        },
        "locationSource":"example source",
        "regionID": 1
	}
]
```
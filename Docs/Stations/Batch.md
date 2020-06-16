## Station batch upload
<span style="color:red">Requires Administrators Authentication</span>    
Provides the ability to batch upload station resources.

#### Request Example
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
                "0": -99.999,
                "1": 44.444
            ]
        }
	},
	{
        "code":"11111112",
        "agencyID": 1,
        "name":"example station 2 at example location, ex. river",
        "stationTypeID":"1",
        "location": {
            "type": "Point",
            "coordinates": [
                "0": -99.999,
                "1": 44.445
            ]
        }
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
                "0": -99.999,
                "1": 44.444
            ]
        }
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
                "0": -99.999,
                "1": 44.445
            ]
        }
	}
]
```
### Station Type batch upload
<span style="color:red">Requires Administrator Authentication</span>  
Provides the ability to batch upload station type resources.

### Request Example
The REST URL section below displays the example url and the body/payload of the request used to simulate a response.

```
POST /gagestatsservices/stationtypes/batch HTTP/1.1
Host: streamstats.usgs.gov
Accept: application/json
content-type: application/json;charset=UTF-8
content-length: 576

[{
    "name":"Example Station Type 1",
    "description": "Example description 1",
    "code": "ST1"
},
{
    "name":"Example Station Type 2",
    "description": "Example description 2",
    "code": "ST2"
},
{
    "name":"Example Station Type 3",
    "description": "Example description 3",
    "code": "ST3"
}]
```

```
HTTP/1.1 200 OK
[{
	"id":51,
    "name":"Example Station Type 1",
    "description": "Example description 1",
    "code": "ST1"
},
{
	"id":52,
    "name":"Example Station Type 2",
    "description": "Example description 2",
    "code": "ST2"
},
{
	"id":53,
    "name":"Example Station Type 3",
    "description": "Example description 3",
    "code": "ST3"
}]
```
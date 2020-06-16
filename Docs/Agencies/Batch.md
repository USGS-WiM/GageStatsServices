## Agency batch upload
<span style="color:red">Requires Administrators Authentication</span>   
Provides the ability to batch upload agency resources.

#### Request Example
The REST URL section below displays the example url and the body/payload of the request used to simulate a response.

```
POST /gagestatsservices/agencies/batch HTTP/1.1
Host: streamstats.usgs.gov
Accept: application/json
content-type: application/json;charset=UTF-8
content-length: 576

[{
    "name":"Agency 1",
    "description":"Description of agency 1",
    "code":"UniqueCode1"
},
{
    "name":"Agency 2",
    "description":"Description of agency 2",
    "code":"UniqueCode2"
},
{
    "name":"Agency 3",
    "description":"Description of agency 3",
    "code":"UniqueCode3"
}]
```

```
HTTP/1.1 200 OK
[{
	"id":5,
    "name":"Agency 1",
    "description":"Description of agency 1",
    "code":"UniqueCode1"
},
{
	"id":6,
    "name":"Agency 2",
    "description":"Description of agency 2",
    "code":"UniqueCode2"
},
{
	"id":7,
    "name":"Agency 3",
    "description":"Description of agency 3",
    "code":"UniqueCode3"
}]
```
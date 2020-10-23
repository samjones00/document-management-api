![CI Pipeline](https://github.com/samjones00/document-management-api-example/workflows/.NET%20Core/badge.svg)

# document-management-api-example

https://lucid.app/documents/view/80b791ef-6597-4280-adef-fe815bbe0f2c

 ## Requirements
* Docker
* Azure Functions Core Tools (Optional) https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=windows%2Ccsharp%2Cbash
* Dot net core 3.1

### Setting up

#### Azure Storage Emulator  (requires 1.5gb disk space)
Run ```docker run -p 10000:10000 -p 10001:10001 -p 10002:10002 microsoft/azure-storage-emulator```

#### Azure Cosmos Emulator
Azure Cosmos Emulator in Docker container (requires 10gb disk space)
Run `docker-compose up` from `.\`

Or 

download the Cosmos Emulator application:
https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator-release-notes

Once Cosmos emulator is running, it can be accessed from:
https://localhost:8081/_explorer/index.html

## Endpoints

| Action | Request type | Url |
|--------|--------------|-----|
| Upload | POST | /api/upload | 
| List   | GET  | /api/list/{sort property}/{sort direction}| 
| Download   | GET  | /api/download/{filename}|
| Delete   | DELETE  | /api/delete/{filename}| 

## Example requests

#### Upload
`POST /api/upload`
```json
{
    "filename":"example13.pdf",
    "data":"JVBERi0xL..."
}
```
#### Statuses
201 Created, 400 Bad Request, 500 Internal Server error

#### Requirements (editable in [settings.json](./DocumentManager.Api/settings.json))
* 5MB Upload limit
* Only application/pdf files are allowed

#### List
`GET /api/list`
`GET /api/list/{property}`
`GET /api/list/{property}/{sortDirection}`

#### Statuses
200 Success, 500 Internal Server error

Available properties: DateCreated, Filename, ContentType, Bytes
Available sort directions: Ascending, Descending

A list of documents will be listed from Cosmos, `property` and `sortDirection` are optional and will order by DateCreated Ascending by deault.

Example response:
```json
{
EXAMPLE HERE
}
```

#### Download
`GET /api/download/{filename}`

#### Statuses
200 Success, 404 Not found, 500 Internal Server error

Example response:
A memory stream will be returned, if you use postman there is the option to save the response as a file.

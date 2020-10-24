# Document Management API

![CI Pipeline](https://github.com/samjones00/document-management-api-example/workflows/.NET%20Core/badge.svg)

![Diagram](https://github.com/samjones00/document-management-api-example/blob/main/Documentation/Diagram.jpeg?raw=true "Diagram")
https://lucid.app/documents/view/80b791ef-6597-4280-adef-fe815bbe0f2c

The API is writen using Azure Functions V3, I chose to do it this way as it hands over the responsibility of scaling and availability to Azure while lowering running costs. I'm adding a document to Cosmos DB for each document and the files themselves are saved to Azure blob storage.

I realise that I could have used Azure Storage Tables for the documents, or just listed the documents from blob storage directly but I wanted to use a more "real world" approach.

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

### Running the API
To start the API you can either open the solution in Visual Studio and run the `DocumentManager.Api` project.

Alternatively, if you have the Azure Functions Core Tools installed then you can open a terminal window and run `func start` from .\DocumentManager.Api

You should see the following output
```
[01:34:30 INF] Initializing function HTTP routes
Mapped function route 'api/delete/{filename}' [delete] to 'Delete'
Mapped function route 'api/download/{filename}' [get] to 'Download'
Mapped function route 'api/list/{sortProperty?}/{sortDirection?}' [get] to 'List'
Mapped function route 'api/Upload' [post] to 'Upload'

[01:34:30 INF] Host initialized (127ms)

Functions:

        Delete: [DELETE] http://localhost:7071/api/delete/{filename}

        Download: [GET] http://localhost:7071/api/download/{filename}

        List: [GET] http://localhost:7071/api/list/{sortProperty?}/{sortDirection?}

        Upload: [POST] http://localhost:7071/api/Upload

        CreateDocument: blobTrigger

For detailed output, run func with --verbose flag.
[01:34:30 INF] Host started (174ms)
[01:34:30 INF] Job host started
Hosting environment: Production
Content root path: D:\Projects\Mine\document-management-api-example\DocumentManager.Api\bin\output
Now listening on: http://0.0.0.0:7071
Application started. Press Ctrl+C to shut down.
```

## Endpoints

| Method | Request type | Url |
|--------|--------------|-----|
| Upload | POST | http://localhost:7071/api/upload | 
| List   | GET  | http://localhost:7071/api/list/{sort property}/{sort direction}| 
| Download   | GET  | http://localhost:7071/api/download/{filename}|
| Delete   | DELETE  | http://localhost:7071/api/delete/{filename}| 
-----------------

### Upload
`POST /api/upload` \
Example request:
```json
{
    "filename": "example.pdf",
    "data": "JVBERi0xLj..."
}
```
Example response:
```json
{
    "filename": "example.pdf",
    "size": 13264
}
```
The data property should be filled with the base64 encoded file data, there are sites to help with this step such as https://www.browserling.com/tools/file-to-base64

#### Statuses
201 Created, 400 Bad Request, 500 Internal Server error

#### Requirements (editable in [settings.json](./DocumentManager.Api/settings.json))
* 5MB Upload limit
* Only application/pdf files are allowed

-------------

### List
`GET /api/list` \
`GET /api/list/{property}` \
`GET /api/list/{property}/{sortDirection}` \

#### Statuses
200 Success, 500 Internal Server error

Available properties: DateCreated, Filename, ContentType, Bytes
Available sort directions: Ascending, Descending

A list of documents will be listed from Cosmos, `property` and `sortDirection` are optional and will order by DateCreated Ascending by default.

Example response:
```json
[
    {
        "id": "b584d8e5-07d7-431d-a407-12ad92af1fcb",
        "filename": "example.pdf",
        "dateCreated": "2020-10-24T19:09:37.4478874Z",
        "bytes": 13264,
        "contentType": "application/pdf"
    },
    {
        "id": "b584d8e5-07d7-431d-a407-12ad92af1fcb",
        "filename": "example2.pdf",
        "dateCreated": "2020-10-24T19:09:37.4478874Z",
        "bytes": 253264,
        "contentType": "application/pdf"
    }
]
```
---------------

### Download
`GET /api/download/{filename}`

#### Statuses
200 Success, 404 Not found, 500 Internal Server error

Example response:
A memory stream will be returned, if you use postman there is the option to save the response as a file.

------------

### Delete
`DELETE /api/delete/{filename}`

#### Statuses
200 Success, 404 Not found, 500 Internal Server error

# Fidelizador C# + RestSharp example

This repository contains an example of how to use the API with C#, using the [RestSharp](https://github.com/restsharp/RestSharp) package.

## List of methods used:

* `GET /oauth/v2/token` for the authentication process
* `GET /1.0/list.json` get a list of existing contact lists
* `POST /1.0/list.json` to create a new list
* `POST /1.0/list/{id}/import.json` to import a csv file into an specific list
* `POST /1.0/campaign.json` to create a list
* `POST /1.0/campaign/{id}/schedule.json` to schedule an existing campaign


See more api details at [postman doc](https://documenter.getpostman.com/view/5320495/Tzz5tyZ2), using the `C# - RestSharp` language option.

## Configure credentials

To test this example, replace the following variable values on `Program.cs`:
```csharp
    string client_id = "CLIENT_ID";
    string client_secret = "CLIENT_SCRET";
    string client_slug = "SLUG";
```

## How to run on Linux

If .net is installed as one of the options given by [microsoft](https://docs.microsoft.com/en-us/dotnet/core/install/linux), then use:

```bash
$ dotnet run
```

## Tested On:

* Ubuntu 20.04 with dotnet 6.0

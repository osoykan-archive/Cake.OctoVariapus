# Cake.OctoVariapus [![Build status](https://ci.appveyor.com/api/projects/status/5q1klj3re9ebpuj3?svg=true)](https://ci.appveyor.com/project/osoykan/cake-octovariapus) [![NuGet version](https://badge.fury.io/nu/Cake.OctoVariapus.svg)](https://badge.fury.io/nu/Cake.OctoVariapus) 
![Cake](https://raw.githubusercontent.com/cake-contrib/graphics/a5cf0f881c390650144b2243ae551d5b9f836196/png/cake-contrib-medium.png)  

Cake integration for Octopus variable management

```csharp
#addin nuget:?package=Cake.OctoVariapus
```
## Usage

### Passing Collection

```csharp
OctoImportVariables(octopusUrl,
                octoProjectName,
                octoApiKey,
                new List<OctoVariable>
                {
                    new OctoVariable
                    {
                        Name = "ConnectionString",
                        IsSensitive = false,
                        IsEditable = true,
                        Value = "DataSource:localhost25",
                        Scopes = new List<OctoScope>
                        {
                            new OctoScope
                            {
                                Name = "Environment",
                                Values = new List<string> { "Development", "Stage" }
                            },
                            new OctoScope
                            {
                                Name = "Role",
                                Values = new List<string> { "Development" }
                            }
                        }
                    }
                })
```
### With Json File

```json
[
  {
    "Name": "ProductApiEndPoint",
    "Value": "http://www.developmentProductApi.com",
    "IsSensitive": false,
    "IsEditable": true,
    "Scopes": [
      {
        "Name": "Environment",
        "Values": [
          "Development"
        ]
      }
    ]
  },
  {
    "Name": "ProductApiEndPoint",
    "Value": "http://www.stageProductApi.com",
    "IsSensitive": false,
    "IsEditable": true,
    "Scopes": [
      {
        "Name": "Environment",
        "Values": [
          "Stage"
        ]
      }
    ]
  },
  {
    "Name": "ProductApiEndPoint",
    "Value": "http://www.productionProductApi.com",
    "IsSensitive": false,
    "IsEditable": true,
    "Scopes": [
      {
        "Name": "Environment",
        "Values": [
          "Production"
        ]
      }
    ]
  },
  {
    "Name": "ConnectionIsSafe",
    "Value": "true",
    "IsSensitive": false,
    "IsEditable": true,
    "Scopes": [
      {
        "Name": "Environment",
        "Values": [
          "Development",
          "Stage",
          "Production"
        ]
      },
      {
        "Name": "Role",
        "Values": [
          "Development"
        ]
      },
      {
        "Name": "Machine",
        "Values": [
          "Oguzhan's Machine Group"
        ]
      },
      {
        "Name": "Step",
        "Values": [
          "1. Initial"
        ]
      }
    ]
  }
]
```
Execution:

```csharp
OctoImportVariables(octopusUrl,
                octoProjectName,
                octoApiKey,
                "./variables.json")
```

# Cake.OctoVariapus [![Build status](https://ci.appveyor.com/api/projects/status/5q1klj3re9ebpuj3?svg=true)](https://ci.appveyor.com/project/osoykan/cake-octovariapus)

Cake integration for Octopus variable management

```csharp
#addin nuget:?package=Cake.OctoVariapus
```
## Usage

### Passing Collection

```csharp
ImportVariables(octopusUrl,
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
                        Scope = new OctoScope
                        {
                            Name = "Environment",
                            Values = new List<string> { "Development", "Stage" }
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
    "Scope": {
      "Name": "Environment",
      "Values": [
        "Development"
      ]
    },
    "IsEditable": true
  },
  {
    "Name": "ProductApiEndPoint",
    "Value": "http://www.stageProductApi.com",
    "IsSensitive": false,
    "Scope": {
      "Name": "Environment",
      "Values": [
        "Stage"
      ]
    },
    "IsEditable": true
  },
  {
    "Name": "ProductApiEndPoint",
    "Value": "http://www.productionProductApi.com",
    "IsSensitive": false,
    "Scope": {
      "Name": "Environment",
      "Values": [
        "Production"
      ]
    },
    "IsEditable": true
  },
  {
    "Name": "ConnectionIsSfae",
    "Value": "true",
    "IsSensitive": false,
    "Scope": {
      "Name": "Environment",
      "Values": [
        "Development",
        "Stage",
        "Production"
      ]
    },
    "IsEditable": true
  }
]
```

```csharp
ImportVariables(octopusUrl,
                octoProjectName,
                octoApiKey,
                "variables.json")
```

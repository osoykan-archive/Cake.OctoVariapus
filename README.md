# Cake.OctoVariapus [![Build status](https://ci.appveyor.com/api/projects/status/5q1klj3re9ebpuj3?svg=true)](https://ci.appveyor.com/project/osoykan/cake-octovariapus)

Cake integration for Octopus variable management

```csharp
#addin nuget:?package=Cake.OctoVariapus
```
## Usage
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

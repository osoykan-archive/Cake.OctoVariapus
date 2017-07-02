# Cake.OctoVariapus

Cake integration for Octopus variable management

```csharp
#addin nuget:?package=Cake.OctoVariapus
```

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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

using Newtonsoft.Json;

using Octopus.Client;
using Octopus.Client.Model;

namespace Cake.OctoVariapus
{
    /// <summary>
    ///     Octopus variable management
    /// </summary>
    [CakeAliasCategory("OctoVariableImport")]
    public static class OctoVariableImportAlias
    {
        /// <summary>
        ///     Imports the variables.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="octopusServerEndpoint">The octopus server endpoint.</param>
        /// <param name="octopusProjectName">Name of the octopus project.</param>
        /// <param name="octopusApiKey">The octopus API key.</param>
        /// <param name="variables">The variables.</param>
        /// <param name="clearAllNonSensitiveExistingVariables">Clear all nonsensitive variables before adding variables.</param>
        [CakeMethodAlias]
        public static void OctoImportVariables(this ICakeContext context,
            string octopusServerEndpoint,
            string octopusProjectName,
            string octopusApiKey,
            IEnumerable<OctoVariable> variables,
            bool clearAllNonSensitiveExistingVariables = false)
        {
            try
            {
                IOctopusAsyncClient client = new OctopusClientFactory().CreateAsyncClient(new OctopusServerEndpoint(octopusServerEndpoint, octopusApiKey)).Result;
                var octopus = new OctopusAsyncRepository(client);

                ProjectResource project = octopus.Projects.FindByName(octopusProjectName).Result;

                VariableSetResource variableSet = octopus.VariableSets.Get(project.Link("Variables")).Result;

                if (clearAllNonSensitiveExistingVariables)
                {
                    context.Log.Information($"Deleting all nonsensitive variables...");

                    List<VariableResource> sensitiveVariables = variableSet.Variables.Where(variable => variable.IsSensitive).ToList();

                    variableSet.Variables.Clear();

                    sensitiveVariables.ForEach(sensitiveVariable => { variableSet.Variables.Add(sensitiveVariable); });

                    context.Log.Information($"Deleting operation finished.");
                }

                foreach (OctoVariable variable in variables)
                {
                    var newVariable = new VariableResource
                    {
                        Name = variable.Name,
                        Value = variable.Value,
                        IsSensitive = variable.IsSensitive,
                        Type = variable.IsSensitive ? VariableType.Sensitive : VariableType.String,
                        IsEditable = variable.IsEditable,
                        Scope = CreateScopeSpesification(variable, variableSet)
                    };

                    string scopeNames = CreateScopeInformationsForLogging(variable);

                    VariableResource existingVariable = variableSet.Variables.FirstOrDefault(x => x.Name == variable.Name && x.Scope.Equals(newVariable.Scope));
                    if (existingVariable != null)
                    {
                        context.Log.Information($"Variable: ({variable.Name}), Scopes:({scopeNames}) already exists in octopus, trying to update...");

                        variableSet.AddOrUpdateVariableValue(existingVariable.Name, newVariable.Value, newVariable.Scope, newVariable.IsSensitive);

                        context.Log.Information($"Variable: ({variable.Name}), Scopes:({scopeNames}) updated successfully...");
                    }
                    else
                    {
                        context.Log.Information($"New Variable: ({variable.Name}), Scopes:({scopeNames}) detected, trying to add...");

                        variableSet.Variables.Add(newVariable);

                        context.Log.Information($"New Variable: ({variable.Name}), Scopes:({scopeNames}) added successfully...");
                    }
                }

                octopus.VariableSets.Modify(variableSet).Wait();
                octopus.VariableSets.Refresh(variableSet).Wait();

                context.Log.Information($"Variables are all successfully set.");
            }
            catch (Exception exception)
            {
                throw new CakeException(exception.Message, exception.InnerException);
            }
        }

        private static string CreateScopeInformationsForLogging(OctoVariable variable)
        {
            List<string> scopeInformation = variable.Scopes
                                                    .SelectMany(x => x.Values, (scope, c) => $"{scope.Name}({string.Join(", ", scope.Values)})")
                                                    .Distinct()
                                                    .ToList();

            return string.Join(", ", scopeInformation);
        }

        /// <summary>
        ///     Imports the variables from a json file.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="octopusServerEndpoint">The octopus server endpoint.</param>
        /// <param name="octopusProjectName">Name of the octopus project.</param>
        /// <param name="octopusApiKey">The octopus API key.</param>
        /// <param name="jsonVariableFilePath">The json variable file path.</param>
        /// <param name="clearAllNonSensitiveExistingVariables"></param>
        [CakeMethodAlias]
        public static void OctoImportVariables(this ICakeContext context,
            string octopusServerEndpoint,
            string octopusProjectName,
            string octopusApiKey,
            FilePath jsonVariableFilePath,
            bool clearAllNonSensitiveExistingVariables = false)
        {
            string jsonString = File.ReadAllText(jsonVariableFilePath.FullPath);
            var variables = JsonConvert.DeserializeObject<List<OctoVariable>>(jsonString);

            context.OctoImportVariables(octopusServerEndpoint, octopusProjectName, octopusApiKey, variables);
        }

        private static ScopeSpecification CreateScopeSpesification(OctoVariable variable, VariableSetResource variableSet)
        {
            var scopeSpecifiaciton = new ScopeSpecification();

            variable.Scopes.ForEach(scope =>
            {
                ScopeField scopeName = FindScopeName(scope);

                List<ReferenceDataItem> referenceDataItems = FindScopeValue(scopeName, variableSet);

                List<string> scopeValues = referenceDataItems.Join(scope.Values,
                    refDataItem => refDataItem.Name,
                    selectedScope => selectedScope,
                    (item, s) => item.Id)
                                                             .ToList();

                if (!scopeValues.Any()) throw new CakeException($"({string.Join(",", scope.Values)}) value(s) can not be found on ({scope.Name}) scope.");

                var value = new ScopeValue(scopeValues.First(), scopeValues.Skip(1).ToArray());

                scopeSpecifiaciton.Add(scopeName, value);
            });

            return scopeSpecifiaciton;
        }

        private static List<ReferenceDataItem> FindScopeValue(ScopeField scopeField, VariableSetResource variableSet)
        {
            List<ReferenceDataItem> referenceDataItem;
            switch (scopeField)
            {
                case ScopeField.Environment:
                    referenceDataItem = variableSet.ScopeValues.Environments;
                    break;
                case ScopeField.Role:
                    referenceDataItem = variableSet.ScopeValues.Roles;
                    break;
                case ScopeField.Machine:
                    referenceDataItem = variableSet.ScopeValues.Machines;
                    break;
                case ScopeField.Action:
                    referenceDataItem = variableSet.ScopeValues.Actions;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scopeField));
            }

            return referenceDataItem;
        }

        private static ScopeField FindScopeName(OctoScope variableScope)
        {
            ScopeField scopeField;
            switch (variableScope.Name)
            {
                case "Environment":
                    scopeField = ScopeField.Environment;
                    break;
                case "Role":
                    scopeField = ScopeField.Role;
                    break;
                case "Machine":
                    scopeField = ScopeField.Machine;
                    break;
                case "Step":
                    scopeField = ScopeField.Action;
                    break;
                default:
                    throw new ArgumentException($"There is no proper ScopeField for ({variableScope.Name}) variable import operation.");
            }

            return scopeField;
        }
    }
}

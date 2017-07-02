using System;
using System.Collections.Generic;
using System.Linq;

using Cake.Core;
using Cake.Core.Annotations;

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
        [CakeMethodAlias]
        public static void ImportVariables(this ICakeContext context,
            string octopusServerEndpoint,
            string octopusProjectName,
            string octopusApiKey,
            IEnumerable<OctoVariable> variables)
        {
            try
            {
                IOctopusAsyncClient client = new OctopusClientFactory().CreateAsyncClient(new OctopusServerEndpoint(octopusServerEndpoint, octopusApiKey)).Result;
                var octopus = new OctopusAsyncRepository(client);

                ProjectResource project = octopus.Projects.FindByName(octopusProjectName).Result;

                VariableSetResource variableSet = octopus.VariableSets.Get(project.Link("Variables")).Result;

                foreach (OctoVariable variable in variables)
                {
                    var newVariable = new VariableResource
                    {
                        Name = variable.Name,
                        Value = variable.Value,
                        IsSensitive = variable.IsSensitive,
                        Type = variable.IsSensitive ? VariableType.Sensitive : VariableType.String,
                        Scope = CreateScopeSpesification(variable, variableSet),
                        IsEditable = variable.IsEditable
                    };

                    VariableResource existingVariable = variableSet.Variables.FirstOrDefault(x => x.Name == variable.Name);
                    if (existingVariable != null)
                    {
                        variableSet.AddOrUpdateVariableValue(existingVariable.Name, newVariable.Value, newVariable.Scope, newVariable.IsSensitive);
                    }
                    else
                    {
                        variableSet.Variables.Add(newVariable);
                    }
                }

                octopus.VariableSets.Modify(variableSet).Wait();
            }
            catch (Exception exception)
            {
                throw new CakeException(exception.Message, exception.InnerException);
            }
        }

        private static ScopeSpecification CreateScopeSpesification(OctoVariable variable, VariableSetResource variableSet)
        {
            ScopeField scopeName = FindScopeName(variable.Scope);

            List<ReferenceDataItem> referenceDataItems = FindScopeValue(scopeName, variableSet);

            List<string> scopeValues = referenceDataItems.Join(variable.Scope.Values,
                                                             refDataItem => refDataItem.Name,
                                                             selectedScope => selectedScope,
                                                             (item, s) => item.Id)
                                                         .ToList();

            var value = new ScopeValue(scopeValues.First(), scopeValues.Skip(1).ToArray());

            return new ScopeSpecification { { scopeName, value } };
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
                    referenceDataItem = variableSet.ScopeValues.Channels;
                    break;
                case ScopeField.TargetRole:
                    referenceDataItem = variableSet.ScopeValues.Machines;
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
                    scopeField = ScopeField.Action;
                    break;
                case "TargetRole":
                    scopeField = ScopeField.Channel;
                    break;
                default:
                    throw new ArgumentException("There is no proper ScopeField for this variable import operation.");
            }

            return scopeField;
        }
    }
}

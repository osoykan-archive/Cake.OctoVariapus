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
        public static async void ImportVariables(this ICakeContext context,
            string octopusServerEndpoint,
            string octopusProjectName,
            string octopusApiKey,
            IEnumerable<OctoVariable> variables)
        {
            IOctopusAsyncClient client = await new OctopusClientFactory().CreateAsyncClient(new OctopusServerEndpoint(octopusServerEndpoint, octopusApiKey));
            var octopus = new OctopusAsyncRepository(client);

            // Find the project that owns the variables we want to edit
            ProjectResource project = await octopus.Projects.FindByName(octopusProjectName);

            // Get the variables for editing
            VariableSetResource variableSet = await octopus.VariableSets.Get(project.Link("Variables"));

            foreach (OctoVariable variable in variables)
            {
                variableSet.Variables.Add(new VariableResource
                {
                    Name = variable.Name,
                    Value = variable.Value,
                    IsSensitive = variable.IsSensitive,
                    Type = variable.IsSensitive ? VariableType.Sensitive : VariableType.String,
                    Scope = CreateScopeSpesification(variable),
                    IsEditable = true
                });
            }

            // Add a new variable

            // Save the variables
            await octopus.VariableSets.Modify(variableSet);
        }

        private static ScopeSpecification CreateScopeSpesification(OctoVariable variable)
        {
            return new ScopeSpecification
            {
                {
                    FindScopeField(variable.Scope),
                    new ScopeValue(variable.Scope.ScopeValues.First(),
                        variable.Scope.ScopeValues.Skip(1).ToArray()
                    )
                }
            };
        }

        private static ScopeField FindScopeField(OctoScope variableScope)
        {
            var scopeField = ScopeField.Environment;
            switch (variableScope.ScopeName)
            {
                case "Environment":
                    scopeField = ScopeField.Environment;
                    break;
                case "Action":
                    scopeField = ScopeField.Action;
                    break;
                case "Channel":
                    scopeField = ScopeField.Channel;
                    break;
                case "Machine":
                    scopeField = ScopeField.Machine;
                    break;
            }

            return scopeField;
        }
    }
}

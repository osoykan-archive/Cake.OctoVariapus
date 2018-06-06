using System.Collections.Generic;

using FluentAssertions;

using HttpMock;

using Xunit;

namespace Cake.OctoVariapus.Tests
{
    public class CakeOctoVariapus_Tests
    {
        private const string OctopusUrl = "http://local.octopus.com";

        private const string OctoProjectName = "Cake.OctoVariapus";

        private const string OctoApiKey = "API-OW0PLJT3JLSZXFCRBE0EKQ7KU9I";

        [Fact]
        public void clear_nonsensitive_variables_should_work()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var octoVaribleImportAlias = new CakeOctoVariableImportAliasFixture(0);

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            HttpMockRepository.At("http://localhost/api/variables/variableset-Projects-1");
            octoVaribleImportAlias.CakeContext.OctoImportVariables(OctopusUrl,
                                                                   OctoProjectName,
                                                                   OctoApiKey,
                                                                   new List<OctoVariable>
                                                                   {
                                                                       new OctoVariable
                                                                       {
                                                                           Name = "Username",
                                                                           IsSensitive = false,
                                                                           IsEditable = true,
                                                                           Value = "user",
                                                                           Scopes = new List<OctoScope>
                                                                                    {
                                                                                        new OctoScope
                                                                                        {
                                                                                            Name = "Environment",
                                                                                            Values = new List<string> { "Development", "Stage" }
                                                                                        }
                                                                                    }
                                                                       },
                                                                       new OctoVariable
                                                                       {
                                                                           Name = "Password",
                                                                           IsSensitive = true,
                                                                           IsEditable = true,
                                                                           Value = "123456",
                                                                           Scopes = new List<OctoScope>
                                                                                    {
                                                                                        new OctoScope
                                                                                        {
                                                                                            Name = "Environment",
                                                                                            Values = new List<string> { "Development", "Stage" }
                                                                                        }
                                                                                    }
                                                                       }
                                                                   }, true);

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            octoVaribleImportAlias.GetCakeLog.Messages.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void import_from_a_json_file_should_work()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var octoVaribleImportAlias = new CakeOctoVariableImportAliasFixture(0);

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            octoVaribleImportAlias.CakeContext.OctoImportVariables(OctopusUrl, OctoProjectName, OctoApiKey, "variables.json");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            octoVaribleImportAlias.GetCakeLog.Messages.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void import_with_handwritten_list_should_work()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var octoVaribleImportAlias = new CakeOctoVariableImportAliasFixture(0);

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            HttpMockRepository.At("http://localhost/api/variables/variableset-Projects-1");
            octoVaribleImportAlias.CakeContext.OctoImportVariables(OctopusUrl,
                                                                   OctoProjectName,
                                                                   OctoApiKey,
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
                                                                                        }
                                                                                    }
                                                                       }
                                                                   });

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            octoVaribleImportAlias.GetCakeLog.Messages.Count.Should().BeGreaterThan(0);
        }
    }
}

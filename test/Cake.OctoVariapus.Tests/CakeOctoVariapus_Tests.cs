using System;
using System.Collections.Generic;

using Cake.Core;
using Cake.Core.IO;

using FakeItEasy;

using FluentAssertions;

using HttpMock;

using Xunit;

namespace Cake.OctoVariapus.Tests
{
    public class CakeOctoVariapus_Tests
    {
        [Fact]
        public void import_with_handwritten_list_should_work()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var octoVaribleImportAlias = new CakeOctoVariableImportAliasFixture(0);
            const string octopusUrl = "http://local.octopus.com";
            const string octoProjectName = "Cake.OctoVariapus";
            const string octoApiKey = "API-FZNNNTXZK0NWFHLLMYJL4JGFIU";

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            HttpMockRepository.At("http://localhost/api/variables/variableset-Projects-1");
            octoVaribleImportAlias.CakeContext.OctoImportVariables(octopusUrl,
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
                });

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
            const string octopusUrl = "http://local.octopus.com";
            const string octoProjectName = "Cake.OctoVariapus";
            const string octoApiKey = "API-FZNNNTXZK0NWFHLLMYJL4JGFIU";

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------

            octoVaribleImportAlias.CakeContext.OctoImportVariables(octopusUrl,
                octoProjectName,
                octoApiKey,
                "variables.json");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            octoVaribleImportAlias.GetCakeLog.Messages.Count.Should().BeGreaterThan(0);
        }

        private class CakeOctoVariableImportAliasFixture
        {
            public CakeOctoVariableImportAliasFixture(int exitCode)
            {
                GetDirectoryPath = Guid.NewGuid().ToString();

                A.CallTo(() => CakeContext.ProcessRunner).Returns(ProcessRunner);
                A.CallTo(() => CakeContext.FileSystem).Returns(FileSystem);
                A.CallTo(() => CakeContext.Log).Returns(GetCakeLog);
                A.CallTo(() => ProcessRunner.Start(A<FilePath>._, A<ProcessSettings>._)).Returns(Process);
                A.CallTo(() => Process.GetExitCode()).Returns(exitCode);
            }

            public CakeOctoVariableImportAliasFixture()
                : this(0)
            {
            }

            public ICakeContext CakeContext { get; } = A.Fake<ICakeContext>();

            public IFileSystem FileSystem { get; } = A.Fake<IFileSystem>();

            public DirectoryPath GetDirectoryPath { get; }

            public IProcess Process { get; } = A.Fake<IProcess>();

            public IProcessRunner ProcessRunner { get; } = A.Fake<IProcessRunner>();

            public CakeLogFixture GetCakeLog { get; } = new CakeLogFixture();
        }
    }
}

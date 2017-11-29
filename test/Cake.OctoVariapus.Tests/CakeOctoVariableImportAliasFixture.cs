using System;

using Cake.Core;
using Cake.Core.IO;

using FakeItEasy;

namespace Cake.OctoVariapus.Tests
{
    public class CakeOctoVariableImportAliasFixture
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

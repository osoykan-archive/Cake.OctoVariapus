using FluentAssertions;

using Xunit;

namespace Cake.OctoVariapus.Tests
{
    public class CakeOctoVariapus_Tests
    {
        [Fact]
        public void should_work()
        {
            var a = 1;
            a.Should().Be(1);
        }
    }
}

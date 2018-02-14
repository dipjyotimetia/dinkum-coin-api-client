using System;
using Xunit;

namespace DinkumCoin.Api.Client.Tests
{
    public class DinkumClientTests
    {
        [Fact]
        [Trait("Category","Integration")]
        public void SampleTest()
        {
            var client = new DinkumCoinApiClient("http://localhost");
            Assert.NotNull(client);
        }
    }
}

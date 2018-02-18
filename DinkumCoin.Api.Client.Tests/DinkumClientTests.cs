using System;
using DinkumCoin.Api.Client.Http;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace DinkumCoin.Api.Client.Tests
{
    public class DinkumClientTests
    {
        private ITestOutputHelper _output;
        public DinkumClientTests(ITestOutputHelper output){
            _output = output;
        }


        [Fact]
        [Trait("Category","Integration")]
        public void GetAllWallets_ReturnsDictionaryCotainingWallets()
        {
            // Arrange
            var mockClient = new Mock<IHttpClient>();

            var mockResponse = new System.Net.Http.HttpResponseMessage();
            mockResponse.StatusCode = System.Net.HttpStatusCode.OK;
            mockResponse.Content = new System.Net.Http.StringContent("{'123':'Stu 1','456':'wallet 2'}");

            mockClient.Setup(x => x.SendAsync(It.IsAny<System.Net.Http.HttpRequestMessage>())).ReturnsAsync(mockResponse);

            // Act
            var client = new DinkumCoinApiClient("http://localhost",mockClient.Object);
            var response = client.GetAllWallets();

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Assert.Contains(response,(obj) => obj.Key=="123" && obj.Value=="Stu 1");

        }
    }
}

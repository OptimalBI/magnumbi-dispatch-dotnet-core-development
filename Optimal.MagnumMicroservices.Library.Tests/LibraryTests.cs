using System;
using System.Net;
using Xunit;

namespace Optimal.MagnumMicroservices.Library.Tests{
    public class LibraryTests{
        private MagnumMicroservicesClient client;

        public LibraryTests(){
            this.client = new MagnumMicroservicesClient("https://127.0.0.1:6883", "test", "token", false);
        }

        [Fact]
        public void AlwaysPassingTest(){
            Assert.True(true);
        }

        [Fact]
        public async void TestStatusCode(){
            bool res = await this.client.CheckStatus();
            Assert.True(res);
        }

        [Fact]
        public async void TestGetStatusCode(){
            HttpStatusCode code = await this.client.CheckStatusCode();
            Assert.Equal(code, HttpStatusCode.OK);
        }
    }
}
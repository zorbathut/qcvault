using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using QCUtilities;

namespace QCVault.Tests
{
    [TestFixture]
    public class PageValidationTests : LiveServerTests
    {
        [TestCase("/", "text/html; charset=utf-8")]
        [TestCase("/About", "text/html; charset=utf-8")]
        [TestCase("/Error", "text/html; charset=utf-8")]
        [TestCase("/PostList", "text/html; charset=utf-8")]
        [TestCase("/Rss", "application/rss+xml; charset=utf-8")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url,string contentType)
        {
            // Arrange
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.AreEqual(contentType,
                response.Content.Headers.ContentType.ToString());
        }


        [Test]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType_ForPosts()
        {
            var client = factory.CreateClient();
            
            var des = TestUtil.CreateDeserializer();

            var posts = des.VisiblePosts();
            foreach (var post in posts)
            {
                var response = await client.GetAsync(post.FullURL);
                response.EnsureSuccessStatusCode(); // Status Code 200-299
                Assert.AreEqual("text/html; charset=utf-8",
                    response.Content.Headers.ContentType.ToString());
            }
        }

        [TestCase("/GibberishForever")]
        [TestCase("/post/GibberishForever")]
        public async Task Get_EndpointsReturnFailure_ForInvalidPost(string url)
        {
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert

            
            Assert.AreEqual(null,
                response.Content.Headers.ContentType);
            Assert.AreEqual(response.IsSuccessStatusCode, false);
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.NotFound);
        }
    }
}


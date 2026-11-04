using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;


 async Task SwaggerDocument_IsAvailable()
{
    // Arrange
    using var client = new HttpClient();

    // Act
    var response = await client.GetAsync(
        "http://localhost:5000/swagger/v1/swagger.json");

    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
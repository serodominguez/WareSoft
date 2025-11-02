using Application.Dtos.Request.Categories;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace TestApi.Category
{
    [TestClass]
    public class CategoryApplicationTest
    {
        private static WebApplicationFactory<Program>? _factory = null;
        private static IServiceScopeFactory? _scopeFactory = null;

        [ClassInitialize]
        public static void Initialize(TestContext _testContext)
        {
            _factory = new ApiApplicationFactory();
            _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        [TestMethod]
        public async Task RegisterCategory_WhenSendingNullValuesOrEmpty_ValidationErrors()
        {
            using var scope = _scopeFactory?.CreateScope();
            var context = scope?.ServiceProvider.GetService<ICategoriesService>();

            //Arrange
            var name = "";
            var description = "";
            var expected = ReplyMessage.MESSAGE_VALIDATE;

            //Act
            var resutl = await context!.RegisterCategory(1, new CategoriesRequestDto()
            {
                CATEGORY_NAME = name,
                DESCRIPTION = description
            });
            var current = resutl.Message;

            //Assert
            Assert.AreEqual(expected, current);
        }

        [TestMethod]
        public async Task RegisterCategory_WhenSendingCorrectValues_RegisteredSuccesfully()
        {
            using var scope = _scopeFactory?.CreateScope();
            var context = scope?.ServiceProvider.GetService<ICategoriesService>();

            //Arrange
            var name = "Test";
            var description = "PRUEBA DE TEST";
            var expected = ReplyMessage.MESSAGE_SAVE;

            //Act
            var resutl = await context!.RegisterCategory(1, new CategoriesRequestDto()
            {
                CATEGORY_NAME = name,
                DESCRIPTION = description
            });
            var current = resutl.Message;

            //Assert
            Assert.AreEqual(expected, current);
        }
    }
}

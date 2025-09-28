using CT.Application.Interfaces;
using CT.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace CT.Tests.FeaturesIS.UseCases
{
    public class PasswordHashTests : BaseFixture
    {
        [Fact]
        public void ShouldHashPassword()
        {
            var hasher = _serviceProvider.GetRequiredService<IPasswordHasher>();
            var res = hasher.HashPassword("test123");

            Assert.True(res.Length > 0);
        }

    }
}
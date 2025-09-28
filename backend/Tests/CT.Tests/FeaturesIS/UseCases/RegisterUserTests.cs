using CT.Application.Features.Authentication.Commands;
using CT.Application.Features.Authentication.Queries;
using CT.Application.Interfaces;
using CT.Domain.IdentityServer;
using CT.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace CT.Tests.FeaturesIS.UseCases
{
    public class RegisterUserTests : BaseFixture
    {
        [Fact]
        public async void ShouldRegisterUser()
        {
            var cmd = new BasicRegisterUserCommand(new BasicRegisterUserCommand.BasicRegisterUserCommandRequestModel("Test@ct.uk", "test123"));

            var response = await _mediator.Send(cmd);

            await DeleteBasicUserAsync(response.Model!.UserIdentifier!);

            Assert.True(response.Result! == Application.Abstractions.Enums.OperationResult.Created);
        }

        [Fact]
        public async void ShouldLoginUserWithEmailCaseInsensitive()
        {
            var cmd = new BasicRegisterUserCommand(new BasicRegisterUserCommand.BasicRegisterUserCommandRequestModel("Test@ct.uk", "test123"));

            var response = await _mediator.Send(cmd);

            var q = new BasicLoginUserQuery(new BasicLoginUserQuery.BasicLoginUserQueryRequestModel("test@ct.uk", "test123"));

            var qRes= await _mediator.Send(q);

            await DeleteBasicUserAsync(response.Model!.UserIdentifier!);

            Assert.True(!string.IsNullOrWhiteSpace(qRes!.Model!.UserIdentifier));
        }

        [Fact]
        public async void ShouldReloginWithRefreshToken()
        {
            var cmd = new BasicRegisterUserCommand(new BasicRegisterUserCommand.BasicRegisterUserCommandRequestModel("Test@ct.uk", "test123"));

            var response = await _mediator.Send(cmd);

            var cmdRefresh = new RefreshLoginQuery();

            var responseRefresh = await _mediator.Send(cmdRefresh);

            await DeleteBasicUserAsync(response.Model!.UserIdentifier!);

            Assert.True(responseRefresh!.Model!.Result == Application.Abstractions.Enums.LoginUserResult.Success);
        }

        #region private methods
        private async Task DeleteBasicUserAsync(string userIdentifier)
        {
            var repo = _serviceProvider.GetRequiredService<IIdentityServerRepositoryService>();

            var userId = (await repo.GetIdAsync<User>(x => x.Identifier == userIdentifier)).Value;

            await repo.DeleteHardAsync<UserDetail>(userId);
            await repo.DeleteHardAsync<UserCredential>(userId);
            await repo.DeleteHardAsync<User>(userId);
        }
    #endregion

    }
}
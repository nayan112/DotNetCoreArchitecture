using DotNetCore.AspNetCore;
using DotNetCore.Objects;
using DotNetCoreArchitecture.Application;
using DotNetCoreArchitecture.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetCoreArchitecture.Web
{
    [ApiController]
    [RouteController]
    public class UsersController : BaseController
    {
        public UsersController(IUserApplicationService userApplicationService)
        {
            UserApplicationService = userApplicationService;
        }

        private IUserApplicationService UserApplicationService { get; }

        [HttpPost]
        public async Task<IActionResult> AddAsync(AddUserModel addUserModel)
        {
            return Result(await UserApplicationService.AddAsync(addUserModel));
        }

        [AuthorizeEnum(Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            return Result(await UserApplicationService.DeleteAsync(id));
        }

        [HttpGet("Grid")]
        public async Task<PagedList<UserModel>> GridAsync([FromQuery]PagedListParameters parameters)
        {
            return await UserApplicationService.ListAsync(parameters);
        }

        [HttpPatch("{id}/Inactivate")]
        public async Task InactivateAsync(long id)
        {
            await UserApplicationService.InactivateAsync(id);
        }

        [HttpGet]
        public async Task<IEnumerable<UserModel>> ListAsync()
        {
            return await UserApplicationService.ListAsync();
        }

        [HttpGet("{id}")]
        public async Task<UserModel> SelectAsync(long id)
        {
            return await UserApplicationService.SelectAsync(id);
        }

        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignInAsync(SignInModel signInModel)
        {
            return Result(await UserApplicationService.SignInAsync(signInModel));
        }

        [HttpPost("SignOut")]
        public Task SignOutAsync()
        {
            return UserApplicationService.SignOutAsync(new SignOutModel(UserModel.Id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(UpdateUserModel updateUserModel)
        {
            return Result(await UserApplicationService.UpdateAsync(updateUserModel));
        }
    }
}

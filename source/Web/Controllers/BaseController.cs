using DotNetCore.AspNetCore;
using DotNetCore.Extensions;
using DotNetCore.Objects;
using DotNetCoreArchitecture.Model;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreArchitecture.Web
{
    public abstract class BaseController : ControllerBase
    {
        protected UserModel UserModel => new UserModel
        {
            Id = User.Id(),
            Roles = User.RolesFlag<Roles>()
        };

        protected static DefaultResult Result(IResult result)
        {
            return new DefaultResult(result);
        }
    }
}

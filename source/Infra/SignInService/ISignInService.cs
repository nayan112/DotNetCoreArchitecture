using DotNetCore.Objects;
using DotNetCoreArchitecture.Model;

namespace DotNetCoreArchitecture.Infra
{
    public interface ISignInService
    {
        SignInModel CreateSignIn(SignInModel signInModel);

        TokenModel CreateToken(SignedInModel signedInModel);

        IResult Validate(SignedInModel signedInModel, SignInModel signInModel);
    }
}

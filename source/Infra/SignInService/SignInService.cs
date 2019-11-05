using DotNetCore.Extensions;
using DotNetCore.Objects;
using DotNetCore.Security;
using DotNetCoreArchitecture.CrossCutting.Resources;
using DotNetCoreArchitecture.Model;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace DotNetCoreArchitecture.Infra
{
    public class SignInService : ISignInService
    {
        public SignInService
        (
            IHash hash,
            IJsonWebToken jsonWebToken
        )
        {
            Hash = hash;
            JsonWebToken = jsonWebToken;
        }

        private IHash Hash { get; }

        private IJsonWebToken JsonWebToken { get; }

        public SignInModel CreateSignIn(SignInModel signInModel)
        {
            var salt = Guid.NewGuid().ToString();

            var password = Hash.Create(signInModel.Password, salt);

            return new SignInModel
            {
                Login = signInModel.Login,
                Password = password,
                Salt = salt
            };
        }

        public TokenModel CreateToken(SignedInModel signedInModel)
        {
            var claims = new List<Claim>();

            claims.AddSub(signedInModel.Id.ToString());

            claims.AddRoles(signedInModel.Roles.ToString().Split(", "));

            return new TokenModel(JsonWebToken.Encode(claims));
        }

        public IResult Validate(SignedInModel signedInModel, SignInModel signInModel)
        {
            var errorResult = Result.Error(Texts.LoginPasswordInvalid);

            if (signedInModel == default || signInModel == default) { return errorResult; }

            var password = Hash.Create(signInModel.Password, signedInModel.SignIn.Salt);

            if (signedInModel.SignIn.Password != password) { return errorResult; }

            return Result.Success();
        }
    }
}

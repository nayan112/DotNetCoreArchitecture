using DotNetCore.Objects;
using DotNetCoreArchitecture.CrossCutting.Resources;
using DotNetCoreArchitecture.Database;
using DotNetCoreArchitecture.Domain;
using DotNetCoreArchitecture.Infra;
using DotNetCoreArchitecture.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetCoreArchitecture.Application
{
    public sealed class UserApplicationService : IUserApplicationService
    {
        public UserApplicationService
        (
            ISignInService signInService,
            IUnitOfWork unitOfWork,
            IUserLogApplicationService userLogApplicationService,
            IUserRepository userRepository
        )
        {
            SignInService = signInService;
            UnitOfWork = unitOfWork;
            UserLogApplicationService = userLogApplicationService;
            UserRepository = userRepository;
        }

        private ISignInService SignInService { get; }

        private IUnitOfWork UnitOfWork { get; }

        private IUserLogApplicationService UserLogApplicationService { get; }

        private IUserRepository UserRepository { get; }

        public async Task<IDataResult<long>> AddAsync(AddUserModel addUserModel)
        {
            var validation = new AddUserModelValidator().Validate(addUserModel);

            if (validation.IsError)
            {
                return DataResult<long>.Error(validation.Message);
            }

            addUserModel.SignIn = SignInService.CreateSignIn(addUserModel.SignIn);

            var userEntity = UserFactory.Create(addUserModel);

            userEntity.Add();

            await UserRepository.AddAsync(userEntity);

            await UnitOfWork.SaveChangesAsync();

            return DataResult<long>.Success(userEntity.Id);
        }

        public async Task<IResult> DeleteAsync(long id)
        {
            await UserRepository.DeleteAsync(id);

            await UnitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task InactivateAsync(long id)
        {
            var userEntity = UserFactory.Create(id);

            userEntity.Inactivate();

            await UserRepository.UpdateStatusAsync(userEntity);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<PagedList<UserModel>> ListAsync(PagedListParameters parameters)
        {
            return await UserRepository.ListAsync<UserModel>(parameters);
        }

        public async Task<IEnumerable<UserModel>> ListAsync()
        {
            return await UserRepository.ListAsync<UserModel>();
        }

        public async Task<UserModel> SelectAsync(long id)
        {
            return await UserRepository.SelectAsync<UserModel>(id);
        }

        public async Task<IDataResult<TokenModel>> SignInAsync(SignInModel signInModel)
        {
            var validation = new SignInModelValidator().Validate(signInModel);

            if (validation.IsError)
            {
                return DataResult<TokenModel>.Error(validation.Message);
            }

            var signedInModel = await UserRepository.SignInAsync(signInModel);

            validation = SignInService.Validate(signedInModel, signInModel);

            if (validation.IsError)
            {
                return DataResult<TokenModel>.Error(validation.Message);
            }

            var userLogModel = new UserLogModel(signedInModel.Id, LogType.SignIn);

            await UserLogApplicationService.AddAsync(userLogModel);

            await UnitOfWork.SaveChangesAsync();

            var tokenModel = SignInService.CreateToken(signedInModel);

            return DataResult<TokenModel>.Success(tokenModel);
        }

        public async Task SignOutAsync(SignOutModel signOutModel)
        {
            var userLogModel = new UserLogModel(signOutModel.Id, LogType.SignOut);

            await UserLogApplicationService.AddAsync(userLogModel);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<IResult> UpdateAsync(UpdateUserModel updateUserModel)
        {
            var validation = new UpdateUserModelValidator().Validate(updateUserModel);

            if (validation.IsError)
            {
                return Result.Error(validation.Message);
            }

            var userEntity = await UserRepository.SelectAsync(updateUserModel.Id);

            if (userEntity == default)
            {
                return Result.Success();
            }

            userEntity.ChangeEmail(updateUserModel.Email);

            userEntity.ChangeFullName(updateUserModel.FullName.Name, updateUserModel.FullName.Surname);

            await UserRepository.UpdateAsync(userEntity.Id, userEntity);

            await UnitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
}

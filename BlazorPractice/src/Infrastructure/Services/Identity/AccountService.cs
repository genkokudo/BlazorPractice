﻿using BlazorPractice.Application.Interfaces.Services;
using BlazorPractice.Application.Interfaces.Services.Account;
using BlazorPractice.Application.Requests.Identity;
using BlazorPractice.Infrastructure.Models.Identity;
using BlazorPractice.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPractice.Infrastructure.Services.Identity
{
    /// <summary>
    /// アカウントの操作関係の処理
    /// DB更新やサインインはMicrosoft.AspNetCore.IdentityのUserManager、SignInManagerを使用する
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly UserManager<BlazorHeroUser> _userManager;
        private readonly SignInManager<BlazorHeroUser> _signInManager;
        private readonly IUploadService _uploadService;
        private readonly IStringLocalizer<AccountService> _localizer;

        public AccountService(
            UserManager<BlazorHeroUser> userManager,
            SignInManager<BlazorHeroUser> signInManager,
            IUploadService uploadService,
            IStringLocalizer<AccountService> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _uploadService = uploadService;
            _localizer = localizer;
        }

        /// <summary>
        /// パスワード変更
        /// ChangePasswordRequestはどこから？
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return await Result.FailAsync(_localizer["User Not Found."]);
            }

            var identityResult = await _userManager.ChangePasswordAsync(
                user,
                model.Password,
                model.NewPassword);
            var errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();
            return identityResult.Succeeded ? await Result.SuccessAsync() : await Result.FailAsync(errors);
        }

        /// <summary>
        /// プロフィールを更新する
        /// UpdateProfileRequestはどこから？
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IResult> UpdateProfileAsync(UpdateProfileRequest request, string userId)
        {
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var userWithSamePhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
                if (userWithSamePhoneNumber != null)
                {
                    return await Result.FailAsync(string.Format(_localizer["Phone number {0} is already used."], request.PhoneNumber));
                }
            }

            // DBからユーザ情報のメールアドレスの重複確認
            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail == null || userWithSameEmail.Id == userId)
            {
                // DBから対象ユーザを取得
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return await Result.FailAsync(_localizer["User Not Found."]);
                }

                // 入力された情報で更新する
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.PhoneNumber = request.PhoneNumber;
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (request.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
                }
                var identityResult = await _userManager.UpdateAsync(user);
                var errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();

                // 新しいユーザ情報でサインインし直す
                await _signInManager.RefreshSignInAsync(user);

                return identityResult.Succeeded ? await Result.SuccessAsync() : await Result.FailAsync(errors);
            }
            else
            {
                return await Result.FailAsync(string.Format(_localizer["Email {0} is already used."], request.Email));
            }
        }

        /// <summary>
        /// 写真URLを取得する
        /// </summary>
        /// <param name="userId">対象のユーザID</param>
        /// <returns>URLか、失敗メッセージ</returns>
        public async Task<IResult<string>> GetProfilePictureAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return await Result<string>.FailAsync(_localizer["User Not Found"]);
            }
            return await Result<string>.SuccessAsync(data: user.ProfilePictureDataUrl);
        }

        /// <summary>
        /// 写真のアップロード処理
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return await Result<string>.FailAsync(message: _localizer["User Not Found"]);
            // ファイルを保存して、そのパスを取得
            var filePath = _uploadService.UploadAsync(request);
            user.ProfilePictureDataUrl = filePath;
            var identityResult = await _userManager.UpdateAsync(user);
            var errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();
            return identityResult.Succeeded ? await Result<string>.SuccessAsync(data: filePath) : await Result<string>.FailAsync(errors);
        }
    }
}
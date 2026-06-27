using Core.Exceptions;
using Core.Results;
using Core.Security;
using EduTrack.Application.BusinessRules;
using EduTrack.Application.DTOs.Auth;
using EduTrack.Application.Services.Abstract;
using EduTrack.Domain.Entities;
using EduTrack.Persistence;
using Microsoft.EntityFrameworkCore;


namespace BlogApp.Application.Services.Concrete;

public class AuthService(AppDbContext context, AuthBusinessRules authBusinessRules) : IAuthService
{
    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await context.Users
                   .Include(u => u.UserRoles)
                   .ThenInclude(ur => ur.Role)
                   .FirstOrDefaultAsync(u => u.Email == request.Email);


            authBusinessRules.CheckUserExists(user);
            authBusinessRules.CheckUserPasswordMatch(user!, request.Password);


            var response = new LoginResponse
            {
                Id = user!.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };


            return Result<LoginResponse>.Ok(response);
        }
        catch (BusinessException ex)
        {
            return Result<LoginResponse>.Fail(ex.Message);
        }

    }

    public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            authBusinessRules.CheckUserExistsByEmail(request.Email);

            var result = HashingHelper.CreatePasswordHash(request.Password);

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = result.Hash,
                PasswordSalt = result.Salt,
                IsActive = true, //TODO: email doğrulama eklendiğinde burayı false yapalım
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var response = new RegisterResponse
            {
                UserId = user.Id,
                Email = user.Email,
                IsActive = user.IsActive,
            };

            return Result<RegisterResponse>.Ok(response);

        }
        catch (BusinessException ex)
        {
            return Result<RegisterResponse>.Fail(ex.Message);
        }
    }

}

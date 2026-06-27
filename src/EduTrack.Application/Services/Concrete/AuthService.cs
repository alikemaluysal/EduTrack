using Core.Results;
using Core.Security;
using EduTrack.Application.DTOs.Auth;
using EduTrack.Application.Services.Abstract;
using EduTrack.Domain.Entities;
using EduTrack.Persistence;
using Microsoft.EntityFrameworkCore;


namespace BlogApp.Application.Services.Concrete;

public class AuthService(AppDbContext context) : IAuthService
{
    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await context.Users
                                .Include(u => u.UserRoles)
                                .ThenInclude(ur => ur.Role)
                                .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null)
            return Result<LoginResponse>.Fail("Email adresi veya şifre hatalı.");


        var passwordCorrect = HashingHelper.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt);
        if (!passwordCorrect)
            return Result<LoginResponse>.Fail("Email adresi veya şifre hatalı.");


        var response = new LoginResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            IsActive = user.IsActive,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };


        return Result<LoginResponse>.Ok(response);
    }

    public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        var userExists = context.Users.Any(u => u.Email == request.Email);
        if (userExists)
            return Result<RegisterResponse>.Fail("Bu email ile kayıtlı bir kullanıcı zaten var.");

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


}

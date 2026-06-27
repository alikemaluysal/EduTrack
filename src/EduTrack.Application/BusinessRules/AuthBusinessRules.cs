using Azure.Core;
using Core.Exceptions;
using Core.Results;
using Core.Security;
using EduTrack.Domain.Entities;
using EduTrack.Persistence;


namespace EduTrack.Application.BusinessRules;

public class AuthBusinessRules(AppDbContext context) 
{
    public void CheckUserExists(User? user)
    {
        if (user is null)
           throw new BusinessException("Email adresi veya şifre hatalı.");
    }
    public void CheckUserExistsByEmail(string email)
    {
        var userExists = context.Users.Any(u => u.Email == email);
        if (userExists)
            throw new BusinessException("Bu email ile kayıtlı bir kullanıcı zaten var.");
    }

    public void CheckUserPasswordMatch(User user, string password)
    {
        var passwordCorrect = HashingHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);
        if (!passwordCorrect)
            throw new BusinessException("Email adresi veya şifre hatalı.");
    }
}

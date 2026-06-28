using Core.Results;
using Core.Exceptions;
using EduTrack.Application.BusinessRules;
using EduTrack.Application.DTOs.User;
using EduTrack.Application.Services.Abstract;
using EduTrack.Domain.Entities;
using EduTrack.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduTrack.Application.Services.Concrete;

public class UserService(AppDbContext context, UserBusinessRules userBusinessRules) : IUserService
{
    public async Task<Result<List<UserDto>>> GetAllUsersAsync(string? searchQuery = null, int? roleId = null)
    {
        //TODO: pagination 
        //TODO: dynamic query

        var query = context.Users
            .Include(x => x.UserRoles)
            .ThenInclude(ur => ur.Role)
            .AsQueryable();

        if (roleId is not null)
        {
            query = query.Where(x => x.UserRoles.Any(ur => ur.RoleId == roleId));
        }

        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(x => 
                x.FirstName.Contains(searchQuery) || 
                x.LastName.Contains(searchQuery) || 
                x.Email.Contains(searchQuery));
        }

        var users = await query.ToListAsync();
        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            IsActive = u.IsActive,
            UserRoles = u.UserRoles.Select(ur => new UserRoleDto
            {
                RoleId = ur.RoleId,
                RoleName = ur.Role.Name
            }).ToList()
        }).ToList();

        return Result<List<UserDto>>.Ok(userDtos);
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(Guid id)
    {
        try
        {
            var user = await context.Users
                .Include(x => x.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(x => x.Id == id);

            userBusinessRules.CheckUserExists(user);

            return Result<UserDto>.Ok(new UserDto
            {
                Id = user!.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                UserRoles = user.UserRoles.Select(ur => new UserRoleDto
                {
                    RoleId = ur.RoleId,
                    RoleName = ur.Role.Name
                }).ToList()
            });
        }
        catch (BusinessException ex)
        {
            return Result<UserDto>.Fail(ex.Message);
        }
    }

    public async Task<Result<List<RoleDto>>> GetAllRolesAsync()
    {
        var roles = await context.Roles
            .OrderBy(r => r.Name)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name
            })
            .ToListAsync();

        return Result<List<RoleDto>>.Ok(roles);
    }

    public async Task<Result> UpdateUserAsync(UpdateUserRequest request)
    {
        try
        {
            var user = await context.Users
                .Include(x => x.UserRoles)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            userBusinessRules.CheckUserExists(user);
            await userBusinessRules.CheckEmailIsUniqueForUserAsync(request.Id, request.Email);

            var validRoleIds = await context.Roles
                .Where(r => request.RoleIds.Contains(r.Id))
                .Select(r => r.Id)
                .ToListAsync();

            userBusinessRules.CheckRolesAreValid(validRoleIds, request.RoleIds);

            user!.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            user.Email = request.Email.Trim();
            user.IsActive = request.IsActive;

            var selectedRoleIds = validRoleIds.ToHashSet();
            var rolesToRemove = user.UserRoles
                .Where(ur => !selectedRoleIds.Contains(ur.RoleId))
                .ToList();
            var existingRoleIds = user.UserRoles
                .Select(ur => ur.RoleId)
                .ToHashSet();
            var rolesToAdd = selectedRoleIds
                .Where(roleId => !existingRoleIds.Contains(roleId))
                .Select(roleId => new UserRole
                {
                    UserId = user.Id,
                    RoleId = roleId
                })
                .ToList();

            context.UserRoles.RemoveRange(rolesToRemove);
            await context.UserRoles.AddRangeAsync(rolesToAdd);
            await context.SaveChangesAsync();

            return Result.Ok("User updated successfully.");
        }
        catch (BusinessException ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
    

using Core.Results;
using EduTrack.Application.DTOs.User;
using EduTrack.Application.Services.Abstract;
using EduTrack.Domain.Entities;
using EduTrack.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduTrack.Application.Services.Concrete;

public class UserService(AppDbContext context) : IUserService
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
        var user = await context.Users
            .Include(x => x.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            return Result<UserDto>.Fail("User not found.");
        }

        return Result<UserDto>.Ok(new UserDto
        {
            Id = user.Id,
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
        var user = await context.Users
            .Include(x => x.UserRoles)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (user is null)
        {
            return Result.Fail("User not found.");
        }

        var emailExists = await context.Users.AnyAsync(x => x.Id != request.Id && x.Email == request.Email);
        if (emailExists)
        {
            return Result.Fail("Email is already used by another user.");
        }

        var validRoleIds = await context.Roles
            .Where(r => request.RoleIds.Contains(r.Id))
            .Select(r => r.Id)
            .ToListAsync();

        if (validRoleIds.Count != request.RoleIds.Distinct().Count())
        {
            return Result.Fail("Selected roles are invalid.");
        }

        user.FirstName = request.FirstName.Trim();
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
}
    

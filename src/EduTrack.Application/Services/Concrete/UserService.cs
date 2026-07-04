using Core.Results;
using Core.Exceptions;
using EduTrack.Application.BusinessRules;
using EduTrack.Application.DTOs.User;
using EduTrack.Application.Services.Abstract;
using EduTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using EduTrack.Application.Repositories;

namespace EduTrack.Application.Services.Concrete;

public class UserService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserRoleRepository userRoleRepository,
    UserBusinessRules userBusinessRules) : IUserService
{
    public async Task<Result<List<UserDto>>> GetAllUsersAsync(string? searchQuery = null, int? roleId = null)
    {
        //TODO: pagination 
        //TODO: dynamic query

        var query = userRepository.Query()
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
            var user = await userRepository.GetAsync(
                predicate: x => x.Id == id,
                include: x => x.Include(u => u.UserRoles).ThenInclude(ur => ur.Role));

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
        var roles = await roleRepository.GetListAsync(orderBy: r => r.OrderBy(r => r.Name));

        var result = roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name
        })
        .ToList();

        return Result<List<RoleDto>>.Ok(result);
    }

    public async Task<Result> UpdateUserAsync(UpdateUserRequest request)
    {
        try
        {
            var user = await userRepository.GetAsync(
                x => x.Id == request.Id,
                include: x => x.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                );



            userBusinessRules.CheckUserExists(user);
            await userBusinessRules.CheckEmailIsUniqueForUserAsync(request.Id, request.Email);

            var validRoleIds = await roleRepository.Query()
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

            await userRoleRepository.DeleteRangeAsync(rolesToRemove);
            await userRoleRepository.AddRangeAsync(rolesToAdd);

            return Result.Ok("User updated successfully.");
        }
        catch (BusinessException ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}


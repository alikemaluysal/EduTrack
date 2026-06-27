using Core.Results;
using EduTrack.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduTrack.Application.Services.Abstract;

public interface IUserService
{
    Task<Result<List<UserDto>>> GetAllUsersAsync(string? searchQuery = null, int? roleId = null );
    Task<Result<UserDto>> GetUserByIdAsync(Guid id);
    Task<Result<List<RoleDto>>> GetAllRolesAsync();
    Task<Result> UpdateUserAsync(UpdateUserRequest request);
}

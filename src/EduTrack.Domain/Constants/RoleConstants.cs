using EduTrack.Domain.Entities;

namespace EduTrack.Domain.Constants;

public static class RoleConstants
{
    public static readonly List<Role> BaseRoles =
        [
            new Role(){Id = 1, Name = "Admin"},
            new Role(){Id = 2, Name = "Instructor"},
            new Role(){Id = 3, Name = "User"}
        ];
}


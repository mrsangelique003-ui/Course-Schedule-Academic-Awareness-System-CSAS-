using Microsoft.AspNetCore.Identity;

namespace CourseScheduleSystem.Web.Models;

public class ApplicationRole : IdentityRole
{
    public ApplicationRole() { }
    public ApplicationRole(string roleName) : base(roleName) { }
}

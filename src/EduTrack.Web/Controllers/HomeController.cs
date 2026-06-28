using Core.Results;
using EduTrack.Application.DTOs.Course;
using EduTrack.Application.Services.Abstract;
using EduTrack.Domain.Constants;
using EduTrack.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EduTrack.Web.Controllers;

[Authorize]
public class HomeController(ICourseService courseService) : BaseController
{
    public async Task<IActionResult> Index()
    {
        Result<List<CourseDto>> result;

        var userId = GetCurrentUserId();

        if (User.IsInRole(RoleConstants.Admin))
        {
            result = await courseService.GetAllCoursesAsync();
        }
        else if(User.IsInRole(RoleConstants.Instructor))
        {
            result = await courseService.GetAllCoursesForInstructorAsync(userId);
        }
        else
        {
            result = await courseService.GetAllCoursesForStudentAsync(userId);
        }

        return View(result.Data);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

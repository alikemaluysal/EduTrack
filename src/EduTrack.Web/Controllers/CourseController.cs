using EduTrack.Application.DTOs.Course;
using EduTrack.Application.Services.Abstract;
using EduTrack.Domain.Constants;
using EduTrack.Persistence.Configurations;
using EduTrack.Web.Models.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTrack.Web.Controllers;

public class CourseController(ICourseService courseService) : BaseController
{
    [HttpPost]
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Instructor}")]
    public async Task<IActionResult> Create(CreateCourseViewModel model)
    {
        var request = new CreateCourseRequest
        {
            InstructorId = GetCurrentUserId(),
            Title = model.Title,
            Description = model.Description
        };

        var response = await courseService.CreateCourseAsync(request);

        if (!response.Success)
        {
            ViewBag.ErrorMessage = response.Message;
            return View(model);
        }

        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Join(string code)
    {
        var request = new JoinCourseRequest
        {
            Code = code,
            StudentId = GetCurrentUserId()
        };

        var response = await courseService.JoinCourseAsync(request);

        if (!response.Success)
        {
            ViewBag.ErrorMessage = response.Message;
            return View("Index");
        }

        return RedirectToAction("Index", "Home");
    }
}

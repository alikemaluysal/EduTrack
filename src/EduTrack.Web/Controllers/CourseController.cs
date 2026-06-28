using EduTrack.Application.DTOs.Course;
using EduTrack.Application.Services.Abstract;
using EduTrack.Domain.Constants;
using EduTrack.Persistence.Configurations;
using EduTrack.Web.Models.Course;
using EduTrack.Web.Models.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTrack.Web.Controllers;

public class CourseController(ICourseService courseService) : BaseController
{
    [HttpPost]
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Instructor}")]
    public async Task<IActionResult> Create(HomeViewModel model)
    {

        if (!ModelState.IsValid)
            return RedirectToAction("Index", "Home");


        var request = new CreateCourseRequest
        {
            InstructorId = GetCurrentUserId(),
            Title = model.CreateCourseViewModel.Title,
            Description = model.CreateCourseViewModel.Description
        };

        var response = await courseService.CreateCourseAsync(request);

        if (!response.Success)
        {
            ViewBag.ErrorMessage = response.Message;
            return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Join(HomeViewModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Index", "Home");

        var request = new JoinCourseRequest
        {
            Code = model.JoinCourseViewModel.Code,
            StudentId = GetCurrentUserId()
        };

        var response = await courseService.JoinCourseAsync(request);

        if (!response.Success)
        {
            ViewBag.ErrorMessage = response.Message;
            return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("Index", "Home");
    }
}

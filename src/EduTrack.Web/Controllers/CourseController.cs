using Core.Results;
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

    public async Task<IActionResult> Index()
    {
        Result<List<CourseListDto>> result;

        var userId = GetCurrentUserId();

        result = await courseService.GetUserCoursesAsync(userId);

        var viewModel = new CourseViewModel();
        viewModel.Courses = result.Data;

        return View(viewModel);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Detail(Guid id)
    {

        return View();
    }

    [HttpGet]
    [Authorize]
    public IActionResult Materials(Guid id)
    {

        return View();
    }

    [HttpGet]
    [Authorize]
    public IActionResult Assignments(Guid id)
    {

        return View();
    }

    [HttpGet]
    [Authorize]
    public IActionResult Students(Guid id)
    {

        return View();
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Instructor}")]
    public async Task<IActionResult> Create(CourseViewModel model)
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
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Join(CourseViewModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Index));

        var request = new JoinCourseRequest
        {
            Code = model.JoinCourseViewModel.Code,
            StudentId = GetCurrentUserId()
        };

        var response = await courseService.JoinCourseAsync(request);

        if (!response.Success)
        {
            ViewBag.ErrorMessage = response.Message;
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Index));
    }

}

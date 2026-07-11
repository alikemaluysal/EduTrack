using EduTrack.Application.DTOs.CourseMaterial;
using EduTrack.Application.Repositories;
using EduTrack.Application.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTrack.Web.Controllers;

public class MaterialsController(ICourseMaterialService courseMaterialService) : BaseController
{
    [HttpGet]
    [Authorize]
    public IActionResult Add(Guid id)
    {
        ViewBag.CourseId = id;
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddAsync(Guid id, CreateCourseMaterialRequest model)
    {

        ViewBag.CourseId = id;
        if(!ModelState.IsValid)
            return View(model);

        //TODO: servisi çağır

        var result = await courseMaterialService.AddCourseMaterialAsync(model);

        if (!result.Success)
        {
            ViewBag.ErrorMessage = result.Message;
            return View(model);
        }

        ViewBag.SuccessMessage = "Material başarıyla eklendi.";

        return View();
    }
}

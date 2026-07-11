using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTrack.Web.Controllers;

public class MaterialsController : BaseController
{
    [HttpGet]
    [Authorize]
    public IActionResult Add(Guid id)
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public IActionResult Add(Guid id, string material)
    {

        return View();
    }
}

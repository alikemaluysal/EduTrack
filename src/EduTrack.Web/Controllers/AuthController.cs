using Microsoft.AspNetCore.Mvc;

namespace EduTrack.Web.Controllers;

public class AuthController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

using Microsoft.AspNetCore.Mvc;

public class GroupsController : Controller
{
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
}
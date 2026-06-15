using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[Authorize]
public class SecureController : Controller
{
    public IActionResult Index()
    {
        var claims = User.Claims
            .Select(c => new SecureClaimViewModel(c.Type, c.Value))
            .OrderBy(c => c.Type)
            .ToList();

        return View(claims);
    }
}

public record SecureClaimViewModel(string Type, string Value);
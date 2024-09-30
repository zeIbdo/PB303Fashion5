using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PB303Fashion.Areas.AdminPanel.Controllers
{
    [Authorize]
    [Area("AdminPanel")]
    public class AdminController : Controller
    {

    }
}

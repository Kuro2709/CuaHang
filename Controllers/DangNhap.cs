using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using CuaHang.Models;

namespace CuaHang.Controllers
{
    public class DangNhapController : Controller
    {
        private readonly HttpClient _httpClient;

        public DangNhapController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            var response = await _httpClient.PostAsJsonAsync("Auth/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<JwtTokenResponse>();
                Session["JWToken"] = result.Token;
                return RedirectToAction("Index", "TrangChu");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(loginDto);
        }
    }
}

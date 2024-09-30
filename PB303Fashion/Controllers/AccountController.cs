using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PB303Fashion.DataAccessLayer;
using PB303Fashion.DataAccessLayer.Entities;
using PB303Fashion.Models;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace PB303Fashion.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("index", "home");
    }
    [AllowAnonymous]
    public async Task<IActionResult> Login()
    {
        var vm = new LoginViewModel()
        {
            Schemes = await _signInManager.GetExternalAuthenticationSchemesAsync()
        };
        return View(vm);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View(vm);
        }

        var userByName = await _userManager.FindByNameAsync(vm.Username);
        if (userByName == null)
        {
            ModelState.AddModelError("", "Username or password are incorrect");
            return View(vm);
        }
        var result = await _signInManager.PasswordSignInAsync(userByName, vm.Password, true, true);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Username or password are incorrect");
            return View(vm);
        }
        if (string.IsNullOrWhiteSpace(vm.ReturnUrl))
            return RedirectToAction("Index", "Home");

        return Redirect(vm.ReturnUrl);
    }
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var user = await _userManager.FindByNameAsync(vm.Username);

        if (user != null)
        {
            ModelState.AddModelError("", "User with this name already exists!");

            return View(vm);
        }

        var newUser = new AppUser
        {
            UserName = vm.Username,
            Email = vm.Email
        };
        var result = await _userManager.CreateAsync(newUser, vm.Password);
        if (!result.Succeeded)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
                return View(vm);
            }
        }
        return RedirectToAction("Index", "Home");
    }
    [AllowAnonymous]

    public IActionResult ForgetPassword()
    {
        return View();
    }
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var existingUser = await _userManager.FindByEmailAsync(vm.Email);
        if (existingUser == null)
        {
            ModelState.AddModelError("", "There is not such a user with this email");
            return View(vm);
        }
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
        var resetLink = Url.Action(nameof(ResetPassword), "Account", new { vm.Email, resetToken }, Request.Scheme, Request.Host.ToString());
        string subject = "Reset Password with this token";
        string body = $"<b>Password Reset Token</b><br><hr> <a href=\"{resetLink}\">Recovery Link</a> ";
        try
        {
        SendEmail(vm.Email,subject,body);
            Console.WriteLine("Mail sent");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return View();
    }
    public IActionResult ResetPassword()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm, string email, string resetToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }
        var existingUser =await _userManager.FindByEmailAsync(email);
        if (existingUser == null) return BadRequest();
        var passwordCheck = await _userManager.CheckPasswordAsync(existingUser, vm.Password);
        if (passwordCheck)
        {
            ModelState.AddModelError("", "You cannot use your previous password.");
            return View(vm);
        }
        var result = await _userManager.ResetPasswordAsync(existingUser, resetToken,vm.Password);
        if (result.Succeeded)
            return RedirectToAction(nameof(Login));
         
        foreach(var error in result.Errors)
        {
            ModelState.AddModelError("",error.Description);
        }
        return View(vm);
    } 
    private void SendEmail(string emailid, string subject, string body)
    {
        NetworkCredential credential = new NetworkCredential("imedetzade5@gmail.com", "sdvd tmbx bxwa oozv");
        MailMessage message = new MailMessage();
        message.From = new MailAddress("imedetzade5@gmail.com");
        message.To.Add(new MailAddress(emailid));
        message.Subject = subject;
        message.IsBodyHtml=true;
        message.Body = body;
        using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
        {

            client.UseDefaultCredentials=false;
        client.Credentials = credential;
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.EnableSsl=true;
        //client.Host= "smtp.gmail.com";
        //client.Port = 587;
        client.Send(message);
        }
    }
    public IActionResult ExternalLogin(string provider)
    {
        var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> ExternalLoginCallback(string remoteError = "")
    {
        //Console.WriteLine(returnUrl);
        //if (string.IsNullOrWhiteSpace(returnUrl))
        //    returnUrl = Url.Content("~/");

        var vm = new LoginViewModel()
        {
            Schemes = await _signInManager.GetExternalAuthenticationSchemesAsync()
        };
        if (!string.IsNullOrEmpty(remoteError))
        {
            ModelState.AddModelError("", $"Error from external login provider {remoteError}");
            return View(nameof(Login), vm);
        }
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            ModelState.AddModelError("", $"Error from external login provider {remoteError}");
            return View(nameof(Login), vm);
        }
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);
        if (result.Succeeded) return RedirectToAction("Index", "Home");
        else
        {
            var extMail = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (!string.IsNullOrEmpty(extMail))
            {
                var user = await _userManager.FindByEmailAsync(extMail);
                if (user == null)
                {
                    user = new AppUser()
                    {
                        Email = extMail,
                        UserName = extMail,
                        EmailConfirmed = true
                    };
                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        foreach (var error in createResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(nameof(Login), vm);
                    }
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (!addLoginResult.Succeeded)
                    {
                        foreach (var error in addLoginResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(nameof(Login), vm);
                    }
                }
                await _signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
        }
        ModelState.AddModelError("", "Something went wrong");
        return View(nameof(Login), vm);
    }
}
    


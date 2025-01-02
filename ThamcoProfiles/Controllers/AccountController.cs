using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThamcoProfiles.Data;
using ThamcoProfiles.Models;
using Auth0.AuthenticationApi;
using System.Net.Http.Headers;
using System.Text.Json;
using Auth0.AuthenticationApi.Models;
using ThamcoProfiles.Support;
using Microsoft.AspNetCore.Authorization;
using SQLitePCL;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ThamcoProfiles.Services.ProfileRepo;
using System.Security.AccessControl;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;


namespace ThamcoProfiles.Controllers
{
    public class AccountController : Controller
    {
       // private readonly AccountContext _context;
        private readonly IConfiguration _configuration;

        private readonly IProfileService _profileService;

        private readonly ILogger<AccountController> _logger;

        public AccountController( IConfiguration configuration, IProfileService profileService, ILogger<AccountController> logger)
        {
            
            _configuration = configuration;
            _profileService = profileService;
            _logger = logger;
        }

        // Enable Auth0 login
        public IActionResult Login()
        {
            try
            {
                return Challenge(new AuthenticationProperties { RedirectUri = "/" }, "Auth0");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Login: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
        // Logout logic
        public async Task Logout()
        {
            try
            {
                await HttpContext.SignOutAsync("Auth0");
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                /*await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
                {
                    // Redirect to the home page after logout.
                    RedirectUri = Url.Action("Index", "Home")
                });*/
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Logout: {ex.Message}");
            }

            // Redirect to Home page or Login page after logout
        }

        //profile details
        [Authorize]
        public async Task<IActionResult> Details()
        {

            try{
            
            // Get the Auth0UserId from the logged-in user
            var auth0UserId = Auth0UserHelper.GetAuth0UserId(User);
            var userEmail = Auth0UserHelper.GetEmail(User);

            var user = await _profileService.GetUserByAuth0IdAsync(auth0UserId);

            if(user==null){

                    user = new User
                    {
                        Email = userEmail ?? "",
                        Auth0UserId = auth0UserId,
                        Password = BCrypt.Net.BCrypt.HashPassword("Auth0PasswordSetHere"), // You can handle password reset with Auth0
                    };

                    await _profileService.AddUserAsync(user);
                    await _profileService.SaveChangesAsync();
            }

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
            }
            catch (Exception ex){

                _logger.LogError($"Error in Details: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            

            }
        }
        
        //editting profiles
         [HttpGet]
        public async Task<IActionResult> EditField(string field)
        {


            try{
            var auth0UserId = Auth0UserHelper.GetAuth0UserId(User);

            var user = await _profileService.GetUserByAuth0IdAsync(auth0UserId);
            

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Field = field;
            ViewBag.FieldValue = field switch
            {
                "FirstName" => user.FirstName ?? string.Empty, // Use empty string if null
                "LastName" => user.LastName ?? string.Empty,
                "PhoneNumber" => user.PhoneNumber ?? string.Empty,
                "PaymentAddress" => user.PaymentAddress ?? string.Empty,
                _ => throw new Exception("Invalid field.")
            };

            return View(user);
            }
            catch(Exception ex){
                 _logger.LogError($"Error in EditField: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
           
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditField( string field, string newValue)
        {
            
            var auth0UserId = Auth0UserHelper.GetAuth0UserId(User);

            var user = await _profileService.GetUserByAuth0IdAsync(auth0UserId);
            
            
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                // Local update
                switch (field)
                {
                    case "FirstName":
                        user.FirstName = newValue;
                        break;
                    case "LastName":
                        user.LastName = newValue;
                        break;
                    
                
                    case "PaymentAddress":
                        user.PaymentAddress = newValue;
                        break;
                    case "PhoneNumber":
                        user.PhoneNumber = newValue;
                        break;
                    default:
                        throw new Exception("Invalid field.");

                }
                
                 // Save local changes
                await _profileService.UpdateUser(user);
                await _profileService.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating {field}: {ex.Message}");
                return View(user);
            }

            return RedirectToAction(nameof(Details));
        }

       

         // GET: Account/Delete/5
        public async Task<IActionResult> Delete()
        {
            var auth0UserId = Auth0UserHelper.GetAuth0UserId(User);

            var user = await _profileService.GetUserByAuth0IdAsync(auth0UserId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try {
            var auth0UserId = Auth0UserHelper.GetAuth0UserId(User);

            var user = await _profileService.GetUserByAuth0IdAsync(auth0UserId);

            //the user requests to be deleted and the staff approve 

            /*
           if (user != null)
            {
                _context.User.Remove(user);
            }

            await _context.SaveChangesAsync();
            
            */
            return RedirectToAction(nameof(Logout));
            }
            catch(Exception ex){

                _logger.LogError($"Error deleting user: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
           
            }
        }

        private bool UserExists(int id)
        {
            return _profileService.UserExists(id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieTime.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace MovieTime.Controllers
{
    public class HomeController : Controller
    {

        private DBContext context;
        private PasswordHasher<User> RegisterHasher = new PasswordHasher<User>();
        private PasswordHasher<LoginUser> LoginHasher = new PasswordHasher<LoginUser>();
        
        public HomeController(DBContext dbc)
        {
            context = dbc;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User u)
        {
            if(ModelState.IsValid)
            {
                u.Password = RegisterHasher.HashPassword(u, u.Password);
                context.Users.Add(u);
                context.SaveChanges();
                HttpContext.Session.SetInt32("UserId", u.UserId);
                return Redirect("/success");
            }
            return View("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser l)
        {
            if(ModelState.IsValid)
            {
                User logging_in_user = context.Users.FirstOrDefault(u => u.Email == l.LoginEmail);
                if(logging_in_user != null)
                {
                    var result = LoginHasher.VerifyHashedPassword(l, logging_in_user.Password, l.LoginPassword);
                    if(result == 0)
                    {
                        ModelState.AddModelError("LoginPassword", "Invalid Password");
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("UserId", logging_in_user.UserId);
                        return Redirect("/success");
                    }
                }
                else
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email");
                }
            }
            return View("Index");
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId == null)
            {
                return Redirect("/");
            } 

            List<Movie> Movies = context.Movies
                .Include(m => m.Planner)
                .Include(m => m.AttendingUsers)
                .OrderBy(m => m.StartTime).ToList();
            
            for(int i=0; i<Movies.Count; i++)
            {
                if(Movies[i].StartTime < DateTime.Now)
                {
                    Movies.Remove(Movies[i]);
                }
            }
            ViewBag.Movies = Movies;
            ViewBag.UserId = UserId;
            return View();
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return Redirect("/");
        }

        [HttpGet("movie/new")]
        public IActionResult NewMovie()
        {
            return View();
        }

        [HttpPost("movie")]
        public IActionResult CreateMovie(Movie m)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId == null)
            {
                return Redirect("/");
            } 
            if(ModelState.IsValid)
            {
                m.PlannerId = (int) UserId;
                context.Movies.Add(m);
                context.SaveChanges();
                return Redirect("/success");
            }
            else
            {
                return View("NewMovie", m);
            }
        }

        [HttpGet("edit/{MovieId}")]
        public IActionResult Edit(int MovieId)
        {
            Movie mv = context.Movies.FirstOrDefault(m => m.MovieId == MovieId);
            return View(mv);
        }

        [HttpGet("delete/{MovieId}")]
        public IActionResult Delete(int MovieId)
        {
            Movie m = context.Movies.FirstOrDefault(mv => mv.MovieId == MovieId);
            context.Movies.Remove(m);
            context.SaveChanges();
            return Redirect("/success");
        }

        [HttpGet("view/{MovieId}")]
        public IActionResult ShowMovie(int MovieId)
        {
            Movie m = context.Movies
                .Include(mv => mv.Planner)
                .Include(mv => mv.AttendingUsers)
                .ThenInclude(mv => mv.Joiner)
                .FirstOrDefault(mv => mv.MovieId == MovieId);
            ViewBag.Joins = m.AttendingUsers;
            return View(m);
        }

        [HttpGet("join/{MovieId}")]
        public IActionResult Join(int MovieId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId == null)
            {
                return Redirect("/");
            } 
            Join j = new Join(){
                UserId = (int) UserId,
                MovieId = MovieId
            };
            context.Joins.Add(j);
            context.SaveChanges();
            return Redirect("/success");
        }

        [HttpGet("leave/{MovieId}")]
        public IActionResult Leave(int MovieId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId == null)
            {
                return Redirect("/");
            } 
            Join join = context.Joins
                .Where(j => j.MovieId == MovieId)
                .FirstOrDefault(j => j.UserId == (int) UserId);
            context.Joins.Remove(join);
            context.SaveChanges();
            return Redirect("/success");
        }

        [HttpPost("update/{MovieId}")]
        public IActionResult Update(int MovieId, Movie m)
        {
            if(ModelState.IsValid)
            {
                Movie mv = context.Movies.FirstOrDefault(mov => mov.MovieId == MovieId);
                mv.Title = m.Title;
                mv.Year = m.Year;
                mv.Address = m.Address;
                mv.StartTime = m.StartTime;
                mv.Duration = m.Duration;
                context.SaveChanges();
                return Redirect("/success");
            }
            else
            {
                return View("Edit", m);
            }
        }

    }
}

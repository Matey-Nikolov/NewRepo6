using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Social_Media_v3.Data;
using Social_Media_v3.Models;

namespace Social_Media_v3.Controllers
{
    public class User_v3Controller : Controller
    {
        private readonly SocialMediaDbContext _context;

        public User_v3Controller(SocialMediaDbContext context)
        {
            _context = context;
        }
        
        public  IActionResult AllChats()
        {
            var user = _context.Users;

            return View(user);
        }

        public IActionResult CreateMessage(int? id)
        {
            TempData["id"] = id;

            return View();
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
       public IActionResult CreateMessage(User_v3 objectUser, string from, string messageText, object? id) // To
       {
            id = TempData["id"];


            var user = _context.Users.FirstOrDefault(x => x.UserName == from);

            if (user == null)
            {
                ModelState.AddModelError("From", "You cannot send a message to non-existent user!");
            }
            else if (user.UserName == from)
            {
                ModelState.AddModelError("MessageText", "You cannot send a message to yourself!");
            }

            StringBuilder sbText = new StringBuilder();
            StringBuilder sbFrom = new StringBuilder();

            if (user.MessageText != null)
            {
                sbText.Append(user.MessageText + " \n" + messageText);

            }
            else
            {
                sbText.Append(messageText);
            }

            foreach (var userFind in _context.Users)
            {
                if (userFind.Id == (int)id)
                {
                    if (user.From != null)
                    {
                        sbFrom.Append(user.UserName + ", \n" + userFind.UserName);
                        user.From = sbFrom.ToString();
                        _context.SaveChangesAsync();

                    }
                    else
                    {
                        user.From = userFind.UserName;
                        _context.SaveChangesAsync();
                    }
                }
            }

            user.MessageText = sbText.ToString();
            _context.SaveChangesAsync();

           return RedirectToAction("AllChats");
       }



        //GET 
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User_v3 objectUser, string userName, string password)
        {

            foreach (var item in _context.Users)
            {
                if (item.Password == password && item.UserName == userName)
                {
                    TempData["id"] = item.Id;
                    return RedirectToAction("Login2");
                }
            }

            ModelState.AddModelError("UserName", "Not a full name or non-existent user.");
            ModelState.AddModelError("password", "incorrect password.");

            return View();
        }

        public IActionResult Login2(User_v3 objectUser, string userName, string password)
        {
            object id = TempData["id"];

            if (id != null)
            {
                ViewData["id"] = id;
            }

            IEnumerable<User_v3> objectsUserList = _context.Users;
            objectsUserList = objectsUserList.Where(x => x.Id == (int)id);
            return View("ProfileIndex", objectsUserList);
        }

        public IActionResult ProfileIndex(object? id)
        {
            id = TempData["id"];

            IEnumerable<User_v3> objectsUserList = _context.Users;
            objectsUserList = objectsUserList.Where(x => x.Id == (int)id);

            return View(objectsUserList);
        }


        // GET: User_v3
        public async Task<IActionResult> Index()
        {
              return _context.Users != null ? 
                          View(await _context.Users.ToListAsync()) :
                          Problem("Entity set 'SocialMediaDbContext.Users'  is null.");
        }

        // GET: User_v3/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user_v3 = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user_v3 == null)
            {
                return NotFound();
            }

            return View(user_v3);
        }

        // GET: User_v3/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User_v3/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,Password,Bio,MessageTitle,MessageText")] User_v3 user_v3)
        {
            foreach (var item in _context.Users)
            {
                if (item.UserName == user_v3.UserName && user_v3.Password == item.Password)
                {
                    ModelState.AddModelError("UserName", "Already exists.");
                }
                else if (user_v3.Password == item.Password)
                {
                    ModelState.AddModelError("Password", "Already exists.");
                }
                else if (user_v3.UserName == item.UserName)
                {
                    ModelState.AddModelError("UserName", "Already exists.");
                }

            }


            if (ModelState.IsValid)
            {
                _context.Add(user_v3);
                await _context.SaveChangesAsync();
                TempData["id"] = user_v3.Id;

                return RedirectToAction("ProfileIndex");
            }
            return View(user_v3);
        }

        // GET: User_v3/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }


            var user_v3 = await _context.Users.FindAsync(id);
            if (user_v3 == null)
            {
                return NotFound();
            }

            return View(user_v3);
        }

        // POST: User_v3/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,Password,Bio,MessageTitle,MessageText")] User_v3 user_v3)
        {
            if (id != user_v3.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user_v3);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!User_v3Exists(user_v3.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["id"] = id;

                return RedirectToAction("ProfileIndex");
            }

            TempData["id"] = id;
            return RedirectToAction("ProfileIndex");
        }

        // GET: User_v3/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user_v3 = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user_v3 == null)
            {
                return NotFound();
            }

            return View(user_v3);
        }

        // POST: User_v3/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'SocialMediaDbContext.Users'  is null.");
            }
            var user_v3 = await _context.Users.FindAsync(id);
            if (user_v3 != null)
            {
                _context.Users.Remove(user_v3);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool User_v3Exists(int id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

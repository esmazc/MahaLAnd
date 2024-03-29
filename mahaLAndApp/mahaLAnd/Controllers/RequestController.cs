﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mahaLAnd.Data;
using mahaLAnd.Models;
using Microsoft.AspNetCore.Authorization;

namespace mahaLAnd.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {

        private readonly ApplicationDbContext _context;

        public RequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Request
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Request.Include(r => r.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // POST: Request/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId")] Request request)
        {
            if (ModelState.IsValid)
            {
                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", request.UserId);
            return View(request);
        }

        // POST: Request/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Id, string UserId)
        {
            var profile = _context.Profile.First(p => p.UserId.Equals(UserId));
            var posts = _context.Post.Where(p => p.ProfileId == profile.Id);
            _context.Post.RemoveRange(posts);
            var questions = _context.Question.Where(q => q.UserId.Equals(UserId));
            _context.Question.RemoveRange(questions);
            var notifications = _context.Notification.Where(n => n.UserId.Equals(UserId));
            _context.Notification.RemoveRange(notifications);
            var follow = _context.Follow.Where(f => f.FollowerId.Equals(UserId) || f.FollowingId.Equals(UserId));
            _context.Follow.RemoveRange(follow);
            _context.Profile.Remove(profile);
            var user = await _context.Users.FindAsync(UserId);
            _context.Users.Remove(user);
            var request = _context.Request.Where(r => r.Id == Id);
            _context.Request.RemoveRange(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestExists(int id)
        {
            return _context.Request.Any(e => e.Id == id);
        }
    }
}

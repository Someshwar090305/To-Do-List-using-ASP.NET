using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MyFirstApi.Data;
using MyFirstApi.Models;
using MyFirstApi.DTOs;

namespace MyFirstApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodosController : ControllerBase {
        private readonly TodoDb _db;
        public TodosController(TodoDb db) => _db = db;
        
        // Helper to get the current user's ID
        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> Get() => 
            await _db.Todos.Where(t => t.UserId == GetUserId()).ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Todo>> Create([FromBody] CreateTodoDto dto) {
            var t = new Todo { Name = dto.Name, IsComplete = false, UserId = GetUserId() };
            _db.Todos.Add(t);
            await _db.SaveChangesAsync();
            return Ok(t);
        }

        // --- NEW FEATURES BELOW ---

        [HttpPut("{id}/toggle")]
        public async Task<IActionResult> Toggle(int id) {
            var todo = await _db.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == GetUserId());
            if (todo == null) return NotFound();
            
            todo.IsComplete = !todo.IsComplete;
            await _db.SaveChangesAsync();
            return Ok(todo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) {
            var todo = await _db.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == GetUserId());
            if (todo == null) return NotFound();

            _db.Todos.Remove(todo);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
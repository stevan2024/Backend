using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TodoList.Api.Controllers
{

    [Route("api/[controller]")]
    [EnableCors("AllowAllHeaders")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly ILogger<TodoItemsController> _logger;

        private IToDoRepository _toDoRespository;

        public TodoItemsController(IToDoRepository toDoRespository)
        {
            _toDoRespository = toDoRespository;
        }

        // GET: api/TodoItems
        [EnableCors("AllowAllHeaders")]
        [HttpGet]
        public async Task<IActionResult> GetTodoItems()
        {
            // var results = await _context.TodoItems.Where(x => !x.IsCompleted).ToListAsync();

            var results = await _toDoRespository.GetNonCompletedItemsAsync();


            return Ok(results);
        }

        // GET: api/TodoItems/...
        [EnableCors("AllowAllHeaders")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoItem(Guid id)
        {
            //var result = await _context.TodoItems.FindAsync(id);

            var result = await _toDoRespository.GetToDoItemAsync(id);


            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // PUT: api/TodoItems/...
        [EnableCors("AllowAllHeaders")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(Guid id, TodoItem todoItem)
        {
            try
            {
                if (id != todoItem.Id)
                {
                    return BadRequest();
                }

                await _toDoRespository.UpdateTodoItemAsync(id, todoItem);
            }
            catch(ToDoException todoException)
            {
                if (!string.IsNullOrEmpty(todoException.Message))
                {

                    if(todoException.Message == "ToDoItem Not Found")
                    {
                        return NotFound(todoException);
                    }

                }
            }
            catch (DbUpdateConcurrencyException exception)
            {
                return UnprocessableEntity(exception);
            }
            catch(Exception)
            {
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }



            return NoContent();
        }

        // POST: api/TodoItems 
        [EnableCors("AllowAllHeaders")]
        [HttpPost]
        public async Task<IActionResult> PostTodoItem(TodoItem todoItem)
        {
            if (string.IsNullOrEmpty(todoItem?.Description))
            {
                return BadRequest("Description is required");
            }
            try
            {
                await _toDoRespository.CreateToDoItem(todoItem);
            }
            catch (ToDoException todoException)
            {
                if (!string.IsNullOrEmpty(todoException.Message))
                {

                    if (todoException.Message == "Description already exists")
                    {
                        return BadRequest(todoException);
                    }

                }
            }
            catch (Exception)
            {
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        private bool TodoItemIdExists(Guid id)
        {
            return _context.TodoItems.Any(x => x.Id == id);
        }

        private bool TodoItemDescriptionExists(string description)
        {
            return _context.TodoItems
                   .Any(x => x.Description.ToLowerInvariant() == description.ToLowerInvariant() && !x.IsCompleted);
        }
    }
}

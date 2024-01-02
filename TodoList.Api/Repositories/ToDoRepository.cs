using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Api.Controllers;

namespace TodoList.Api
{
    public class ToDoRepository
    {

        private readonly TodoContext _context;
        private readonly ILogger<TodoItemsController> _logger;

        public ToDoRepository(TodoContext context, ILogger<TodoItemsController> logger)
        {
            _context = context;
            _logger = logger;

        }


        public async Task<List<TodoItem>> GetNonCompletedItemsAsync()
        {

            var results = await _context.TodoItems.Where(x => !x.IsCompleted).ToListAsync();

            return results;
        }

        public async Task<TodoItem> GetToDoItemAsync(Guid id)
        {

            var result = await _context.TodoItems.FindAsync(id);
            return result;
        }


       public async Task UpdateTodoItemAsync(Guid id,TodoItem todoItem)
        {
            _context.Entry(todoItem).State = EntityState.Modified;

           

                if (!TodoItemIdExists(id))
                {
                    throw new ToDoException("ToDoItem Not Found");
                }

                await _context.SaveChangesAsync();
            

        }


        public async Task CreateToDoItem(TodoItem todoItem)
        {
            if (TodoItemDescriptionExists(todoItem.Description))
            {
                throw new ToDoException("Description already exists");
            }

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

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

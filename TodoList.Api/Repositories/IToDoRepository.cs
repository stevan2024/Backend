using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Api.Controllers;

namespace TodoList.Api
{
    public interface IToDoRepository
    {
        public Task<List<TodoItem>> GetNonCompletedItemsAsync();

        public Task<TodoItem> GetToDoItemAsync(Guid id);

        public Task UpdateTodoItemAsync(Guid id, TodoItem todoItem);

        public Task CreateToDoItem(TodoItem todoItem);


    }
}

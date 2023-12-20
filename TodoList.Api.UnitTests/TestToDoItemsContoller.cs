using Xunit;
using Moq;
using TodoList.Api.Controllers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Data.Entity.Infrastructure;
using TestingDemo;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace TodoList.Api.UnitTests
{
    public class ToDoItemsControllerTest
    {
        Mock<TodoContext> mockContext = new Mock<TodoContext>();
        Mock<ILogger<TodoItemsController>> mockController = new Mock<ILogger<TodoItemsController>>();

       
        [Fact]
        public async Task Test_GetToDoItemsAsync()
        { 

            //Arrange
            List<TodoItem> toDoItems = new List<TodoItem> {
                new TodoItem {Id =  Guid.Parse("0b74e8d1-af62-4b12-9951-3e9037864069"),IsCompleted = false,Description = "test1" },
                 new TodoItem{Id =  Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595"),IsCompleted = false,Description = "test2" }
             };

            var toDoItemsQueryable = toDoItems.AsQueryable();

            var mockSet = new Mock<DbSet<TodoItem>>();

            mockSet.As<IDbAsyncEnumerable<TodoItem>>()
               .Setup(m => m.GetAsyncEnumerator())
               .Returns(new TestDbAsyncEnumerator<TodoItem>(toDoItemsQueryable.GetEnumerator()));

            mockSet.As<IQueryable<TodoItem>>()
               .Setup(m => m.Provider)
               .Returns(new TestDbAsyncQueryProvider<TodoItem>(toDoItemsQueryable.Provider));

            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.Expression).Returns(toDoItemsQueryable.Expression);
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.ElementType).Returns(toDoItemsQueryable.ElementType);
            mockSet.As<IQueryable<TodoItem>>().Setup(m => m.GetEnumerator()).Returns(() => toDoItemsQueryable.GetEnumerator());

            var mockLogger = new Mock<ILogger<TodoItemsController>>();


            var mockContext = new Mock<TodoContext>();
            
            mockContext.Setup(c => c.TodoItems).Returns(mockSet.Object);
          

            var service = new TodoItemsController(mockContext.Object, mockLogger.Object);
            //TODO: had problems mocking _context.TodoItems
            //  error -   system.NotSupportedException : Unsupported expression: c => c.TodoItems
            //   Non - overridable members(here: TodoContext.get_TodoItems) may not be used in setup / verification expressions.


            //Act
            var actionResult = await service.GetTodoItems();


            //Assert



        }
    }
}

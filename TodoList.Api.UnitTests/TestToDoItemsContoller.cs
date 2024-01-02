using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TodoList.Api.Controllers;
using Xunit;

namespace TodoList.Api.UnitTests
{
    /// <summary>
    /// Tests the ToDoItemsContoller by mocking out the todoItemsRepository
    /// </summary>
    public class TestToDoItemsContoller
    {

        Mock<IToDoRepository> mockRepository;

        public TestToDoItemsContoller()
        {
            mockRepository = new Mock<IToDoRepository>();
        }

        [Fact]
        public async Task TestGetTodoItemsAsync_Success()
        {

            List<TodoItem> toDoItems = new List<TodoItem> {
                new TodoItem {Id =  Guid.Parse("0b74e8d1-af62-4b12-9951-3e9037864069"),IsCompleted = false,Description = "test1" },
                 new TodoItem{Id =  Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595"),IsCompleted = false,Description = "test2" }
             };

            mockRepository.Setup(m => m.GetNonCompletedItemsAsync()).ReturnsAsync(toDoItems);

            TodoItemsController controller = new TodoItemsController(mockRepository.Object);

            var result = await controller.GetTodoItems();

            Assert.NotNull(result);

            Assert.True(result is OkObjectResult);

            var returnedPayload = (OkObjectResult)result;

            List<TodoItem> returnedToDoItems = ((List<TodoItem>)returnedPayload.Value);

            Assert.True(returnedToDoItems.Count == 2);
            Assert.True(returnedPayload.StatusCode == 200);

        }

        [Fact]
        public async Task TestGetTodoItemAsync_Success()
        {

            TodoItem todoItem = new TodoItem { Id = Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595"), IsCompleted = false, Description = "test2" };
            mockRepository.Setup(m => m.GetToDoItemAsync(It.IsAny<Guid>())).ReturnsAsync(todoItem);


            TodoItemsController controller = new TodoItemsController(mockRepository.Object);


            Guid id = Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595");

            var result = await controller.GetTodoItem(id);

            Assert.NotNull(result);

            Assert.True(result is OkObjectResult);

            var returnedPayload = (OkObjectResult)result;

            TodoItem returnedToDoItem = ((TodoItem)returnedPayload.Value);

            Assert.NotNull(returnedToDoItem);

            Assert.True(returnedToDoItem.Description == "test2");

            Assert.True(returnedPayload.StatusCode == 200);

        }

        [Fact]
        public async Task TestGetTodoItemAsync_Fail_Result_Null()
        {

            TodoItem todoItem = null;
            mockRepository.Setup(m => m.GetToDoItemAsync(It.IsAny<Guid>())).ReturnsAsync(todoItem);

            TodoItemsController controller = new TodoItemsController(mockRepository.Object);

            Guid id = Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595");

            var result = await controller.GetTodoItem(id);


            Assert.True(result is NotFoundResult);

            var returnedPayload = (NotFoundResult)result;

            Assert.True(returnedPayload.StatusCode == 404);

        }


        [Fact]
        public async Task TestPutTodoItem_Success()
        {
            TodoItem todoItem = new TodoItem { Id = Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595"), IsCompleted = false, Description = "test2" };
            Guid id = Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595");

            mockRepository.Setup(m => m.UpdateTodoItemAsync(It.IsAny<Guid>(), It.IsAny<TodoItem>()));

            TodoItemsController controller = new TodoItemsController(mockRepository.Object);
            var result = await controller.PutTodoItem(id, todoItem);

            Assert.True(result is NoContentResult);

        }


        [Fact]
        public async Task TestPutTodoItem_Fail_BadRequest()
        {
            TodoItem todoItem = new TodoItem { Id = Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595"), IsCompleted = false, Description = "test2" };
           
            // use different id
            Guid differentId = Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E597");

            mockRepository.Setup(m => m.UpdateTodoItemAsync(It.IsAny<Guid>(), It.IsAny<TodoItem>()));

            TodoItemsController controller = new TodoItemsController(mockRepository.Object);
            var result = await controller.PutTodoItem(differentId, todoItem);

            Assert.True(result is BadRequestResult);

        }

        [Fact]
        public async Task TestPutTodoItem_Fail_NotFound_ToDoException()
        {
            TodoItem todoItem = new TodoItem { Id = Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595"), IsCompleted = false, Description = "test2" };

            Guid id = Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595");

            mockRepository.Setup(m => m.UpdateTodoItemAsync(It.IsAny<Guid>(), It.IsAny<TodoItem>())).Throws(new ToDoException("ToDoItem Not Found"));

            TodoItemsController controller = new TodoItemsController(mockRepository.Object);
            var result = await controller.PutTodoItem(id, todoItem);

            Assert.True(result is NotFoundObjectResult);

        }

        [Fact]
        public async Task TestPutTodoItem_Fail_DBUpdate_ToDoException()
        {
            TodoItem todoItem = new TodoItem { Id = Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595"), IsCompleted = false, Description = "test2" };

            Guid id = Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595");

            mockRepository.Setup(m => m.UpdateTodoItemAsync(It.IsAny<Guid>(), It.IsAny<TodoItem>())).Throws(new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException());

            TodoItemsController controller = new TodoItemsController(mockRepository.Object);
            var result = await controller.PutTodoItem(id, todoItem);

            Assert.True(result is UnprocessableEntityObjectResult);

        }

        [Fact]
        public async Task TestPutTodoItem_Fail_Exception_ToDoException()
        {
            TodoItem todoItem = new TodoItem { Id = Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595"), IsCompleted = false, Description = "test2" };

            Guid id = Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595");

            mockRepository.Setup(m => m.UpdateTodoItemAsync(It.IsAny<Guid>(), It.IsAny<TodoItem>())).Throws(new Exception());

            TodoItemsController controller = new TodoItemsController(mockRepository.Object);
            var result = await controller.PutTodoItem(id, todoItem);

            Assert.True(result is StatusCodeResult);
            Assert.True(((StatusCodeResult)result).StatusCode == (int)HttpStatusCode.InternalServerError);

        }


        [Fact]
        public async Task TestPostTodoItem_Success()
        {
            TodoItem todoItem = new TodoItem { Id = Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595"), IsCompleted = false, Description = "test2" };
            
            mockRepository.Setup(m => m.CreateToDoItem( It.IsAny<TodoItem>()));

            TodoItemsController controller = new TodoItemsController(mockRepository.Object);
            var result = await controller.PostTodoItem( todoItem);

            Assert.True(result is CreatedAtActionResult);

        }

        [Fact]
        public async Task TestPostTodoItem_Fail_Description_required()
        {
            TodoItem todoItem = new TodoItem { Id = Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595"), IsCompleted = false, Description = "" };

            mockRepository.Setup(m => m.CreateToDoItem(It.IsAny<TodoItem>()));

            TodoItemsController controller = new TodoItemsController(mockRepository.Object);
            var result = await controller.PostTodoItem(todoItem);

            Assert.True(result is BadRequestObjectResult);

        }

        [Fact]
        public async Task TestPostTodoItem_Fail_Description_already_exists()
        {
            TodoItem todoItem = new TodoItem { Id = Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595"), IsCompleted = false, Description = "test2" };

            mockRepository.Setup(m => m.CreateToDoItem(It.IsAny<TodoItem>())).Throws(new ToDoException("Description already exists"));

            TodoItemsController controller = new TodoItemsController(mockRepository.Object);
            var result = await controller.PostTodoItem(todoItem);

            Assert.True(result is BadRequestObjectResult);

        }

        [Fact]
        public async Task TestPostTodoItem_Fail_exception()
        {
            TodoItem todoItem = new TodoItem { Id = Guid.Parse("4E7F7080-C7D6-42E6-BC4A-30EA8C90E595"), IsCompleted = false, Description = "test2" };

            mockRepository.Setup(m => m.CreateToDoItem(It.IsAny<TodoItem>())).Throws(new Exception());

            TodoItemsController controller = new TodoItemsController(mockRepository.Object);
            var result = await controller.PostTodoItem(todoItem);

            Assert.True(result is StatusCodeResult);


            Assert.True(((StatusCodeResult)result).StatusCode == (int)HttpStatusCode.InternalServerError);

        }






    }
}

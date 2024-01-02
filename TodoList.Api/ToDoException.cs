using System;

namespace TodoList.Api
{
    public class ToDoException:Exception
    {

       public ToDoException(string message):base(message) {
        
        
        }

    }
}

namespace ToDoList.Common.Exceptions
{
    public class InvalidActionException : CustomException
    {
        public InvalidActionException(string message) :
            base(400, "Invalid action", message)
        {
        }
    }
}

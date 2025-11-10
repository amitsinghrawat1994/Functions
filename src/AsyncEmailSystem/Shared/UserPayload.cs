namespace Shared;

// The data we receive from the frontend
public class RegistrationRequest
{
    public string Email { get; set; }
    public string FullName { get; set; }
}

// The message we put onto the Queue
public class EmailQueueMessage
{
    public string UserId { get; set; }
    public string EmailTo { get; set; }
    public string UserFullName { get; set; }
    public DateTime RegisteredAt { get; set; }
}

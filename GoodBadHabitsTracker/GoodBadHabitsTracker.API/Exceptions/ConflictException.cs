using Microsoft.AspNetCore.Identity;

namespace GoodBadHabitsTracker.API.Exceptions
{
    public class ConflictException : Exception
    {
        public int StatusCode { get; }
        public ConflictException(string message) : base(message)
        {

        }
    }

}

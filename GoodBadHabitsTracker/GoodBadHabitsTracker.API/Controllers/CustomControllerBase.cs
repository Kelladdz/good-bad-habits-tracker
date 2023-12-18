using Microsoft.AspNetCore.Mvc;

namespace GoodBadHabitsTracker.API.Controllers
{
    [ApiController]
    [Route("API/v{version:apiVersion}")]
    public class CustomControllerBase : ControllerBase
    {
        
    }
}

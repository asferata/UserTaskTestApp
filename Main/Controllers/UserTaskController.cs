using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Models;
using PresentationServices;

namespace Main.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/user-tasks")]
[ApiVersion("1.0")]
public class UserTaskController(IUserTaskPresentationService userTaskPresentationService): ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserTaskDto>>> PageAsync([FromQuery] int offset = 0,
        [FromQuery] int count = int.MaxValue)
    {
        var result = await userTaskPresentationService.PageAsync(offset, count);
        return Ok(result);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> CreateAsync([FromBody] CreateUserTaskDto dto)
    {
        await userTaskPresentationService.CreateAsync(dto);
        return Accepted();
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> CreateAsync([FromRoute] int id, [FromBody] UpdateUserTaskDto dto)
    {
        await userTaskPresentationService.UpdateAsync(id, dto);
        return Accepted();
    }
}
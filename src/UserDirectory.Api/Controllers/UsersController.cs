using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserDirectory.Api.Authorization;
using UserDirectory.Application.Users;

namespace UserDirectory.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(IUserService service) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(CancellationToken cancellationToken)
        => Ok(await service.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await service.GetByIdAsync(id, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    [AllowAnonymous]
    //[Authorize(Policy = ScopeAuthorization.PolicyName)]
    public async Task<ActionResult<UserResponse>> Create(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:int}")]
    [AllowAnonymous]
    //[Authorize(Policy = ScopeAuthorization.PolicyName)]
    public async Task<ActionResult<UserResponse>> Update(
        int id,
        UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await service.UpdateAsync(id, request, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpDelete("{id:int}")]
    //[Authorize(Policy = ScopeAuthorization.PolicyName)]
    [AllowAnonymous]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        => await service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}

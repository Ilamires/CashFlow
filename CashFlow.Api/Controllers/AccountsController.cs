using CashFlow.Api.Contracts.Accounts;
using CashFlow.Api.Extensions;
using CashFlow.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/accounts")]
public class AccountsController(IAccountService accounts) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
    {
        var account = await accounts.CreateAsync(User.GetUserId(), request);
        return Created($"/api/accounts/{account.Id}", account);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await accounts.GetAllAsync(User.GetUserId()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
        => Ok(await accounts.GetByIdAsync(User.GetUserId(), id));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateAccountRequest request)
        => Ok(await accounts.UpdateAsync(User.GetUserId(), id, request));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await accounts.DeleteAsync(User.GetUserId(), id);
        return NoContent();
    }
}
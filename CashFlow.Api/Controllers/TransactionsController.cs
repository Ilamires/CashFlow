using CashFlow.Api.Contracts.Transactions;
using CashFlow.Api.Extensions;
using CashFlow.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/transactions")]
public class TransactionsController(ITransactionService transactions) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request)
    {
        var transaction = await transactions.CreateAsync(User.GetUserId(), request);
        return Created($"/api/transactions/{transaction.Id}", transaction);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TransactionQueryRequest query)
        => Ok(await transactions.GetAllAsync(User.GetUserId(), query));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
        => Ok(await transactions.GetByIdAsync(User.GetUserId(), id));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateTransactionRequest request)
        => Ok(await transactions.UpdateAsync(User.GetUserId(), id, request));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await transactions.DeleteAsync(User.GetUserId(), id);
        return NoContent();
    }
}
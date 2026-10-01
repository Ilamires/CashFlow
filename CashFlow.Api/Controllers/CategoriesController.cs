using CashFlow.Api.Contracts.Categories;
using CashFlow.Api.Extensions;
using CashFlow.Api.Models;
using CashFlow.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/categories")]
public class CategoriesController(ICategoryService categories) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var category = await categories.CreateAsync(User.GetUserId(), request);
        return Created($"/api/categories/{category.Id}", category);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TransactionType? type)
        => Ok(await categories.GetAllAsync(User.GetUserId(), type));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateCategoryRequest request)
        => Ok(await categories.UpdateAsync(User.GetUserId(), id, request));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await categories.DeleteAsync(User.GetUserId(), id);
        return NoContent();
    }
}
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Authorize(Policy = "Sales.Read")]
[Tags("Vendas")]
[Route("api/sales")]
public sealed class SalesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "Sales.Write")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var validator = new SaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var result = await _mediator.Send(ToCommand(request), cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Venda criada com sucesso.",
            Data = result
        });
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<SaleResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] ListSalesCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(new PaginatedResponse<SaleResult>
        {
            Success = true,
            Message = "Vendas recuperadas com sucesso.",
            Data = result.Items,
            CurrentPage = result.CurrentPage,
            TotalPages = result.TotalPages,
            TotalCount = result.TotalCount
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSaleCommand(id), cancellationToken);

        return Ok(new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Venda recuperada com sucesso.",
            Data = result
        });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Sales.Write")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSaleRequest request, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var result = await _mediator.Send(ToCommand(id, request), cancellationToken);

        return Ok(new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Venda atualizada com sucesso.",
            Data = result
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Sales.Delete")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteSaleCommand(id), cancellationToken);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Venda removida com sucesso."
        });
    }

    [HttpPatch("{id}/cancel")]
    [Authorize(Policy = "Sales.Cancel")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelSaleCommand(id), cancellationToken);

        return Ok(new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Venda cancelada com sucesso.",
            Data = result
        });
    }

    [HttpPatch("{id}/items/{itemId}/cancel")]
    [Authorize(Policy = "Sales.Cancel")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelItem([FromRoute] Guid id, [FromRoute] Guid itemId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelSaleItemCommand(id, itemId), cancellationToken);

        return Ok(new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Item cancelado com sucesso.",
            Data = result
        });
    }

    private static CreateSaleCommand ToCommand(CreateSaleRequest request)
    {
        return new CreateSaleCommand
        {
            SaleNumber = request.SaleNumber,
            SaleDate = request.SaleDate,
            CustomerExternalId = request.CustomerExternalId,
            CustomerName = request.CustomerName,
            BranchExternalId = request.BranchExternalId,
            BranchName = request.BranchName,
            Items = request.Items.Select(ToItemCommand).ToList()
        };
    }

    private static UpdateSaleCommand ToCommand(Guid id, UpdateSaleRequest request)
    {
        return new UpdateSaleCommand
        {
            Id = id,
            SaleNumber = request.SaleNumber,
            SaleDate = request.SaleDate,
            CustomerExternalId = request.CustomerExternalId,
            CustomerName = request.CustomerName,
            BranchExternalId = request.BranchExternalId,
            BranchName = request.BranchName,
            Items = request.Items.Select(ToItemCommand).ToList()
        };
    }

    private static SaleItemCommand ToItemCommand(SaleItemRequest request)
    {
        return new SaleItemCommand
        {
            ProductExternalId = request.ProductExternalId,
            ProductName = request.ProductName,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice
        };
    }
}

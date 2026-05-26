using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Route("api/sales")]
[Authorize]
[Tags("Vendas")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "Sales.Write")]
    [SwaggerOperation(Summary = "Cria uma venda", Description = "Cria uma venda com snapshot denormalizado de cliente, filial e produtos. Os descontos sao calculados automaticamente pelo dominio e o evento SaleCreated e registrado no log da aplicacao.")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = new CreateSaleCommand
        {
            SaleNumber = request.SaleNumber,
            SaleDate = request.SaleDate,
            CustomerExternalId = request.CustomerExternalId,
            CustomerName = request.CustomerName,
            BranchExternalId = request.BranchExternalId,
            BranchName = request.BranchName,
            Items = request.Items.Select(item => new SaleItemInput
            {
                ProductExternalId = item.ProductExternalId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            })
        };

        var response = await _mediator.Send(command, cancellationToken);

        return Created($"/api/sales/{response.Id}", new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Venda criada com sucesso.",
            Data = response
        });
    }

    [HttpGet]
    [Authorize(Policy = "Sales.Read")]
    [SwaggerOperation(Summary = "Lista vendas", Description = "Lista vendas com paginacao e filtros opcionais por numero da venda, cliente, filial, status de cancelamento e periodo.")]
    [ProducesResponseType(typeof(PaginatedResponse<SaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List([FromQuery] ListSalesQuery query, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(query, cancellationToken);

        return Ok(new PaginatedResponse<SaleResult>
        {
            Success = true,
            Message = "Vendas recuperadas com sucesso.",
            Data = response.Items,
            CurrentPage = response.CurrentPage,
            TotalPages = response.TotalPages,
            TotalCount = response.TotalCount
        });
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Sales.Read")]
    [SwaggerOperation(Summary = "Busca venda por ID", Description = "Retorna os dados completos da venda, incluindo itens, descontos calculados, total financeiro e status de cancelamento.")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetSaleQuery { Id = id }, cancellationToken);

        return Ok(new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Venda recuperada com sucesso.",
            Data = response
        });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Sales.Write")]
    [SwaggerOperation(Summary = "Atualiza uma venda", Description = "Atualiza os dados da venda e substitui seus itens. Vendas canceladas nao podem ser alteradas. O total e recalculado pelo aggregate e o evento SaleModified e registrado no log.")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSaleRequest request, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = new UpdateSaleCommand
        {
            Id = id,
            SaleNumber = request.SaleNumber,
            SaleDate = request.SaleDate,
            CustomerExternalId = request.CustomerExternalId,
            CustomerName = request.CustomerName,
            BranchExternalId = request.BranchExternalId,
            BranchName = request.BranchName,
            Items = request.Items.Select(item => new SaleItemInput
            {
                ProductExternalId = item.ProductExternalId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            })
        };

        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Venda atualizada com sucesso.",
            Data = response
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Sales.Delete")]
    [SwaggerOperation(Summary = "Remove uma venda", Description = "Remove fisicamente a venda informada. Cancelamento de venda deve ser feito pelo endpoint PATCH /api/sales/{id}/cancel.")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteSaleCommand { Id = id }, cancellationToken);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Venda removida com sucesso."
        });
    }

    [HttpPatch("{id}/cancel")]
    [Authorize(Policy = "Sales.Cancel")]
    [SwaggerOperation(Summary = "Cancela uma venda", Description = "Marca a venda como cancelada. Uma venda cancelada nao permite novas alteracoes de itens e registra o evento SaleCancelled no log.")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CancelSaleCommand { Id = id }, cancellationToken);

        return Ok(new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Venda cancelada com sucesso.",
            Data = response
        });
    }

    [HttpPatch("{id}/items/{itemId}/cancel")]
    [Authorize(Policy = "Sales.Cancel")]
    [SwaggerOperation(Summary = "Cancela item da venda", Description = "Cancela um item especifico da venda, recalcula o total financeiro sem o item cancelado e registra o evento ItemCancelled no log.")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelItem([FromRoute] Guid id, [FromRoute] Guid itemId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CancelSaleItemCommand { SaleId = id, ItemId = itemId }, cancellationToken);

        return Ok(new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Item da venda cancelado com sucesso.",
            Data = response
        });
    }
}

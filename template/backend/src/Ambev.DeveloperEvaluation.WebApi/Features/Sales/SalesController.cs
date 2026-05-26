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
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Route("api/sales")]
[Authorize]
[Tags("Vendas")]
[EnableRateLimiting("SalesPolicy")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "Sales.Write")]
    [SwaggerOperation(OperationId = "Sales_CreateSale", Summary = "Cria uma nova venda.", Description = "Cria uma venda com dados denormalizados de cliente, filial e produtos. A API calcula automaticamente os descontos por quantidade, calcula o total de cada item, calcula o total da venda e registra o evento SaleCreated no log da aplicacao. Regras: 1 a 3 unidades sem desconto; 4 a 9 unidades com 10%; 10 a 20 unidades com 20%; acima de 20 unidades e invalido. Produto duplicado na mesma venda nao e permitido. Desconto manual nao e aceito no request.")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
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
    [SwaggerOperation(OperationId = "Sales_ListSales", Summary = "Lista vendas com paginacao e filtros.", Description = "Retorna uma lista paginada de vendas, permitindo filtros por numero da venda, cliente, filial, status de cancelamento e periodo.")]
    [ProducesResponseType(typeof(PaginatedResponse<SaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List([FromQuery] ListSalesQuery query, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(query, cancellationToken);

        return new OkObjectResult(new PaginatedResponse<SaleResult>
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
    [SwaggerOperation(OperationId = "Sales_GetSaleById", Summary = "Consulta uma venda por identificador.", Description = "Retorna os dados completos de uma venda, incluindo cliente, filial, itens, descontos, totais e status de cancelamento.")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetSaleQuery { Id = id }, cancellationToken);

        return new OkObjectResult(new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Venda recuperada com sucesso.",
            Data = response
        });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Sales.Write")]
    [SwaggerOperation(OperationId = "Sales_UpdateSale", Summary = "Atualiza uma venda existente.", Description = "Atualiza os dados de uma venda e recalcula automaticamente descontos e totais. Vendas canceladas nao podem ser alteradas. Registra o evento SaleModified no log.")]
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

        return new OkObjectResult(new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Venda atualizada com sucesso.",
            Data = response
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Sales.Delete")]
    [SwaggerOperation(OperationId = "Sales_DeleteSale", Summary = "Remove uma venda.", Description = "Remove fisicamente a venda informada. Este comportamento e diferente do cancelamento comercial da venda, que deve ser executado em PATCH /api/sales/{id}/cancel.")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteSaleCommand { Id = id }, cancellationToken);

        return new OkObjectResult(new ApiResponse
        {
            Success = true,
            Message = "Venda removida com sucesso."
        });
    }

    [HttpPatch("{id}/cancel")]
    [Authorize(Policy = "Sales.Cancel")]
    [Tags("Cancelamentos")]
    [SwaggerOperation(OperationId = "Sales_CancelSale", Summary = "Cancela uma venda.", Description = "Marca a venda como cancelada. Apos o cancelamento, a venda nao podera receber alteracoes. Registra o evento SaleCancelled no log.")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CancelSaleCommand { Id = id }, cancellationToken);

        return new OkObjectResult(new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Venda cancelada com sucesso.",
            Data = response
        });
    }

    [HttpPatch("{id}/items/{itemId}/cancel")]
    [Authorize(Policy = "Sales.Cancel")]
    [Tags("Cancelamentos")]
    [SwaggerOperation(OperationId = "Sales_CancelSaleItem", Summary = "Cancela um item da venda.", Description = "Cancela um item especifico da venda, recalcula o total da venda ignorando o item cancelado e registra o evento ItemCancelled no log.")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelItem([FromRoute] Guid id, [FromRoute] Guid itemId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CancelSaleItemCommand { SaleId = id, ItemId = itemId }, cancellationToken);

        return new OkObjectResult(new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Item da venda cancelado com sucesso.",
            Data = response
        });
    }
}

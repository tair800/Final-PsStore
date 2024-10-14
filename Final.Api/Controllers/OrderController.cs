using Final.Application.Exceptions;
using Final.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("place")]
    public async Task<IActionResult> PlaceOrder(string userId)
    {
        try
        {
            var orderId = await _orderService.CreateOrder(userId);
            return Ok(new { Message = "Order placed successfully.", OrderId = orderId });
        }
        catch (CustomExceptions ex)
        {
            return StatusCode(ex.Code, new { Message = ex.Message });
        }
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder(int orderId)
    {
        try
        {
            var order = await _orderService.GetOrder(orderId);
            return Ok(order);
        }
        catch (CustomExceptions ex)
        {
            return StatusCode(ex.Code, new { Message = ex.Message });
        }
    }
}

using kursah_5semestr.Abstractions;
using kursah_5semestr.Contracts;
using kursah_5semestr;
using Microsoft.AspNetCore.Mvc;
using kursah_5semestr.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace kursah_5semestr.Contracts
{

    [Route("/orders")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : ControllerBase
    {
        private IOrdersService _ordersService;
        private IUsersService _usersService;

        public OrdersController(IOrdersService ordersService, IUsersService usersService)
        {
            _ordersService = ordersService;
            _usersService = usersService;
        }

        private OrderOutDto ToDto(Order order)
        {
            var detailsDtos = order.OrderDetails.Select(
                od => new OrderDetailsOutDto(
                    od.ProductId,
                    od.Quantity,
                    od.Price)).ToList();
            return new OrderOutDto(order.Id, order.Date, order.Status, order.Amount, detailsDtos);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var user = Utils.GetAuthenticatedUser(HttpContext, _usersService);
            if (user == null)
            {
                return Unauthorized(new StatusOutDto("error"));
            }
            try
            {
                var order = await _ordersService.CreateOrder(user);
                return Ok(ToDto(order));
            }
            catch (Exception ex)
            {
                return BadRequest(new StatusOutDto("error", ex.Message));
            }
        }

        [HttpGet]
        public async Task<ActionResult<IList<OrderOutDto>>> GetOrders()
        {
            var user = Utils.GetAuthenticatedUser(HttpContext, _usersService);
            if (user == null)
            {
                return Unauthorized(new StatusOutDto("error"));
            }
            var orders = await _ordersService.GetOrdersByUser(user.Id);
            return Ok(orders.Select(ToDto));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<OrderOutDto>> GetOrderById(Guid id)
        {
            var order = await _ordersService.GetOrderById(id);
            if (order == null)
            {
                return NotFound(new StatusOutDto("error", "Order not found"));
            }
            return Ok(ToDto(order));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteOrder(Guid id)
        {
            var order = await _ordersService.GetOrderById(id);
            if (order == null)
            {
                return NotFound(new StatusOutDto("error", "Order not found"));
            }
            if (order.Status != "new")
            {
                return BadRequest(new StatusOutDto("error", $"Cannot delete an order with status '{order.Status}'"));
            }
            var success = await _ordersService.DeleteOrder(id);
            if (!success)
            {
                return NotFound(new StatusOutDto("error", "Order not found"));
            }

            return Ok(new StatusOutDto("ok"));
        }
    }
}
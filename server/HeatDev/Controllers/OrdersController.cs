using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeatDevBLL.Models.DTO;
using HeatDevBLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HeatDev.Controllers
{
    [Produces("application/json")]
    [Route("api/orders")]
    public class OrdersController : Controller
    {
        private readonly IOrderService orderService;

        public OrdersController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        //POST: api/orders
        [HttpPost]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> CreateOrder([FromBody]OrderCreateDTO orderData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await orderService.CreateOrderAsync(User.GetUserId(), orderData);

            return Created($"api/orders/{order.Id}", order);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HeatDev.ViewModels;
using HeatDevBLL.Models;
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
        private readonly IAccountService accountService;

        public OrdersController(IOrderService orderService, IAccountService accountService)
        {
            this.orderService = orderService;
            this.accountService = accountService;
        }

        //POST: api/orders
        /// <summary> Creating order </summary>
        /// <response code="201"> Order created </response>
        /// <response code="400"> refresh token is invalid </response>
        /// <response code="401"> need to authorize </response>
        [HttpPost]
        [Authorize(Roles = "client")]
        [ProducesResponseType(typeof(CreatedOrderVM), 201)]
        public async Task<IActionResult> CreateOrder([FromBody]OrderCreateDTO orderData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await orderService.CreateOrderAsync(User.GetUserId(), orderData);

            return Created($"api/orders/{order.Id}", Mapper.Map<CreatedOrderVM>(order));
        }

        // GET: api/orders/{id}
        /// <summary> Get order details </summary>
        /// <response code="200"> order details </response>
        /// <response code="401"> need to atuhorize </response>
        /// <response code="403"> user is not worker or it's not his order </response>
        /// <response code="404"> order was not found </response>
        [HttpGet("{orderId:int}")]
        [Authorize(Roles = "client, worker")]
        [ProducesResponseType(typeof(OrderDetailVM), 200)]
        public async Task<IActionResult> GetOrderDetail([FromRoute]int orderId)
        {
            var order = await orderService.FindOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            if (User.IsInRole("client") && order.ClientId != User.GetUserId())
            {
                return Forbid();
            }
            
            return Ok(Mapper.Map<OrderDetailVM>(order));
        }

        // PUT: api/orders/{id}/status
        /// <summary> Change order's status </summary>
        /// <response code="200"> status changed successfully </response>
        /// <response code="400"> invalid new status </response>
        /// <response code="401"> need to atuhorize </response>
        /// <response code="403"> user is not worker or it's not his order </response>
        /// <response code="404"> order was not found </response>
        [HttpPut("{orderId:int}/status")]
        [Authorize(Roles = "client, worker")]
        public async Task<IActionResult> ChangeOrderStatus([FromRoute]int orderId, [FromBody]int status) 
        {
            var order = await orderService.FindOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            if (User.IsInRole("client") && order.ClientId != User.GetUserId())
            {
                return Forbid();
            }

            var roleType = User.IsInRole("worker") ? RoleType.Worker : RoleType.Client;
            if (!orderService.ValidateStatuses((OrderStatusBLL)order.StatusId, (OrderStatusBLL)status, roleType))
            {
                return BadRequest();
            };

            await orderService.ChangeOrderStatusAsync(order, (OrderStatusBLL)status);

            return Ok();
        }

        // PUT: api/orders/{id}/price
        /// <summary> Change order's price </summary>
        /// <response code="200"> price changed successfully </response>
        /// <response code="401"> need to atuhorize </response>
        /// <response code="403"> user is not worker </response>
        /// <response code="404"> order was not found </response>
        [HttpPut("{orderId:int}/price")]
        [Authorize(Roles = "worker")]
        public async Task<IActionResult> ChangeOrderPrice([FromRoute] int orderId, [FromBody]double price)
        {
            var order = await orderService.FindOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            await orderService.ChangePriceAsync(order, price);

            return Ok();
        }

        // GET: api/orders
        /// <summary> Get all orders </summary>
        /// <response code="200"> status changed successfully </response>
        /// <response code="401"> need to atuhorize </response>
        /// <response code="403"> user is not worker </response>
        /// <response code="404"> orders were not found </response>
        [HttpGet]
        [Authorize(Roles = "worker")]
        [ProducesResponseType(typeof(IEnumerable<OrderWorkersListVM>), 200)]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await orderService.GetAllOrdersWithClientsAsync();
            if (orders == null || orders?.Count() == 0)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<IEnumerable<OrderWorkersListVM>>(orders));
        }

        // GET: api/orders/my
        /// <summary> Get all client orders</summary>
        /// <response code="200"> list of orders </response>
        /// <response code="401"> need to authorize </response>
        /// <response code="404"> orders were not found </response>
        [HttpGet("my")]
        [Authorize(Roles = "client")]
        [ProducesResponseType(typeof(IEnumerable<ClientOrdersVM>), 200)]
        public async Task<IActionResult> GetPersonalOrders()
        {
            var orders = await orderService.GetClientOrdersAsync(User.GetUserId());
            if (orders == null || orders?.Count() == 0)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<IEnumerable<ClientOrdersVM>>(orders));
        }

        // GET: api/orders/categories
        /// <summary> Get all possible categories </summary>
        /// <response code="200"> list of categories </response>
        /// <response code="404"> categories were not found </response>
        [HttpGet("categories")]
        [ProducesResponseType(typeof(CategoryVM), 200)]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await orderService.GetCategoiresAsync();
            if (categories == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<IEnumerable<CategoryVM>>(categories));
        }
    }
}
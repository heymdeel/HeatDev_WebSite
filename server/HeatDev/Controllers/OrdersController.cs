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

            var orderDetail = Mapper.Map<OrderDetailVM>(order);
            var clientProfile = await accountService.GetUserProfileAsync(order.ClientId);
            orderDetail.ClientProfile = Mapper.Map<UserProfileVM>(clientProfile);

            return Ok(orderDetail);
        }

        [HttpPut("{orderId:int}/status/{status:int}")]
        [Authorize(Roles = "client, worker")]
        public async Task<IActionResult> ChangeOrderStatus([FromRoute]int orderId, [FromRoute]int status) 
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

        [HttpGet("categories")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(OrderCategoryVM), 200)]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await orderService.GetCategoiresAsync();
            if (categories == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<IEnumerable<OrderCategoryVM>>(categories));
        }

    }
}
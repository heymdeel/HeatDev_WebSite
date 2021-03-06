﻿using HeatDevBLL.Models;
using HeatDevBLL.Models.DTO;
using HeatDevBLL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeatDevBLL.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersWithClientsAsync();
        Task<IEnumerable<Order>> GetClientOrdersAsync(int clientId);

        Task<Order> CreateOrderAsync(int userId, OrderCreateDTO orderData);
        Task<Order> FindOrderByIdAsync(int orderId);

        bool ValidateStatuses(OrderStatusBLL oldStatus, OrderStatusBLL newStatus, RoleType roleType);
        Task ChangeOrderStatusAsync(Order order, OrderStatusBLL status);
        Task ChangePriceAsync(Order order, double price);

        Task<IEnumerable<OrderCategory>> GetCategoiresAsync();
    }
}

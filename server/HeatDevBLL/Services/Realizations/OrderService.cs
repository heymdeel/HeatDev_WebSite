﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HeatDevBLL.Models;
using HeatDevBLL.Models.DTO;
using HeatDevBLL.Models.Entities;
using LinqToDB;

namespace HeatDevBLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly double diagnosticPrice = 500;

        public async Task<Order> CreateOrderAsync(int userId, OrderCreateDTO orderData)
        {
            var order = Mapper.Map<Order>(orderData);
            order.BeginingTime = DateTime.Now;
            order.ClientId = userId;
            order.StatusId = (int)OrdersStatuses.Awaiting;
            order.Price = diagnosticPrice;

            using (var db = new DBContext())
            {
                int orderId = await db.InsertWithInt32IdentityAsync(order);

                return await db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            }
        }
    }
}
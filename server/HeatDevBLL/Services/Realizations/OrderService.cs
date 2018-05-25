using System;
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
        private static Dictionary<OrderStatusBLL, List<OrderStatusBLL>> workerPermissions = new Dictionary<OrderStatusBLL, List<OrderStatusBLL>>()
                    {
                        {OrderStatusBLL.Awaiting, new List<OrderStatusBLL>() { OrderStatusBLL.Confirmed, OrderStatusBLL.CanceledByWorker}},
                        {OrderStatusBLL.Confirmed, new List<OrderStatusBLL>() { OrderStatusBLL.Diagnostics, OrderStatusBLL.CanceledByWorker}},
                        {OrderStatusBLL.Diagnostics, new List<OrderStatusBLL>() { OrderStatusBLL.Performing, OrderStatusBLL.CanceledByWorker}},
                        {OrderStatusBLL.Performing, new List<OrderStatusBLL>() { OrderStatusBLL.Completed, OrderStatusBLL.CanceledByWorker}}
                    };

        private static Dictionary<OrderStatusBLL, List<OrderStatusBLL>> clientPermissions = new Dictionary<OrderStatusBLL, List<OrderStatusBLL>>()
                    {
                        {OrderStatusBLL.Awaiting, new List<OrderStatusBLL>() { OrderStatusBLL.CanceledByClient}},
                        {OrderStatusBLL.Confirmed, new List<OrderStatusBLL>() { OrderStatusBLL.CanceledByClient}}
                    };

        public async Task<Order> CreateOrderAsync(int userId, OrderCreateDTO orderData)
        {
            var order = Mapper.Map<Order>(orderData);
            order.BeginningTime = DateTime.Now;
            order.ClientId = userId;
            order.StatusId = (int)OrderStatusBLL.Awaiting;
            order.DiagnosticPrice = diagnosticPrice;

            using (var db = new DBContext())
            {
                int orderId = await db.InsertWithInt32IdentityAsync(order);

                return await db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            }
        }

        public async Task<Order> FindOrderByIdAsync(int orderId)
        {
            using (var db = new DBContext())
            {
                return await db.Orders.LoadWith(o => o.ClientProfile).FirstOrDefaultAsync(o => o.Id == orderId);
            }
        }

        public async Task<IEnumerable<OrderCategory>> GetCategoiresAsync()
        {
            using (var db = new DBContext())
            {
                return await db.OrderCategories.ToListAsync();
            }
        }

        public bool ValidateStatuses(OrderStatusBLL oldStatus, OrderStatusBLL newStatus, RoleType roleType)
        {
            IDictionary<OrderStatusBLL, List<OrderStatusBLL>> permissions;

            switch (roleType)
            {
                case RoleType.Client:
                    permissions = clientPermissions;
                    break;
                case RoleType.Worker:
                    permissions = workerPermissions;
                    break;
                default:
                    permissions = clientPermissions;
                    break;
            }

            if (!permissions.ContainsKey(oldStatus))
            {
                return false;
            }

            if (permissions[oldStatus].Contains(newStatus))
            {
                return true;
            }

            return false;
        }

        public async Task ChangeOrderStatusAsync(Order order, OrderStatusBLL status)
        {
            order.StatusId = (int)status;
            if (status == OrderStatusBLL.CanceledByClient || status == OrderStatusBLL.CanceledByWorker || status == OrderStatusBLL.Completed)
            {
                order.EndTime = DateTime.Now;
            }

            using (var db = new DBContext())
            {
                await db.UpdateAsync(order);
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrdersWithClientsAsync()
        {
            using (var db = new DBContext())
            {
                return await db.Orders.LoadWith(o => o.ClientProfile).ToListAsync();
            }
        }

        public async Task ChangePriceAsync(Order order, double price)
        {
            order.Price = price;
            using (var db = new DBContext())
            {
                await db.UpdateAsync(order);
            }
        }

        public async Task LeaveReviewAsync(int orderId, ReviewCreateDTO reviewData)
        {
            var review = Mapper.Map<Review>(reviewData);
            review.Id = orderId;
            using (var db = new DBContext())
            {
                await db.InsertAsync(review);
            }
        }

        public async Task<bool> OrderHasReviewAsync(int orderId)
        {
            using (var db = new DBContext())
            {
                return await db.Reviews.AnyAsync(r => r.Id == orderId);
            }
        }
    }
}

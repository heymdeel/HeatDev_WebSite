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
        Task<Order> CreateOrderAsync(int userId, OrderCreateDTO orderData);
        Task<Order> FindOrderByIdAsync(int orderId);

        Task<IEnumerable<OrderCategory>> GetCategoiresAsync();
    }
}

using HeatDevBLL.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeatDevBLL.Services
{
    public interface IReviewsService
    {
        Task sendReviewAsync(ReviewCreateDTO reviewData);
        Task<bool> OrderHasReviewAsync(int orderId);
    }
}

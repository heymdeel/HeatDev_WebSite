using AutoMapper;
using HeatDevBLL.Models;
using HeatDevBLL.Models.DTO;
using HeatDevBLL.Models.Entities;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeatDevBLL.Services
{
    public class ReviewsService : IReviewsService
    {
        public async Task sendReviewAsync(ReviewCreateDTO reviewData)
        {
            var review = Mapper.Map<Review>(reviewData);
            Console.WriteLine(review.Id);
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

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            using (var db = new DBContext())
            {
                return await db.Reviews.LoadWith(r => r.Order.ClientProfile).ToListAsync();
            }
        }
    }
}

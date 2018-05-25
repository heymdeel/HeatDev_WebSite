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
    [Route("api/reviews")]
    public class ReviewsController : Controller
    {
        private readonly IReviewsService reviewsService;
        private readonly IOrderService orderService;

        public ReviewsController(IReviewsService reviewsService, IOrderService orderService)
        {
            this.reviewsService = reviewsService;
            this.orderService = orderService;
        }

        //POST: api/reviews
        /// <summary> Sending a review </summary>
        /// <response code="200"> success </response>
        /// <response code="400"> invalid data </response>
        /// <response code="401"> need to authorize </response>
        /// <response code="403"> order doesnt' belong to user </response>
        /// <response code="404"> order was not found </response>
        [HttpPost]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> SendReview([FromBody]ReviewCreateDTO reviewData)
        {
            var order = await orderService.FindOrderByIdAsync(reviewData.Id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.ClientId != User.GetUserId())
            {
                return Forbid();
            }

            if (!ModelState.IsValid || await reviewsService.OrderHasReviewAsync(reviewData.Id))
            {
                return BadRequest(ModelState);
            }

            await reviewsService.sendReviewAsync(reviewData);

            return Ok();
        }
    }
}

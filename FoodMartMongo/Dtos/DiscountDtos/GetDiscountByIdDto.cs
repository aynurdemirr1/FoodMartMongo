﻿namespace FoodMartMongo.Dtos.DiscountDtos
{
    public class GetDiscountByIdDto
    {
        public string DiscountId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string ImageUrl { get; set; }
        public string DiscountRate { get; set; }
    }
}
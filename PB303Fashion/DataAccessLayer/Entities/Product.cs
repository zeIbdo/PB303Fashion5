﻿namespace PB303Fashion.DataAccessLayer.Entities
{
    public class Product : Entity
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId {  get; set; }
        public Category? Category { get; set; }
    }
}

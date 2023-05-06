﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tomi.Domain.Entities
{
    public class Product : BaseEntity
    {
        
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }

}
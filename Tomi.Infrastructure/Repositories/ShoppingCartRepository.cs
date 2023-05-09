﻿using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tomi.Domain.Entities;
using Tomi.Domain.IRepositories;
using Tomi.Infrastructure.Contexts;

namespace Tomi.Infrastructure.Repositories
{
    public class ShoppingCartRepository : BaseRepository<ShoppingCart>, IShoppingCartRepository
	{
		public ShoppingCartRepository(IMongoContext mongoContext, IOptions<MongoDbSettings> mongoDbSettings) : base(mongoContext, mongoDbSettings)
		{
		}
		public async Task<ShoppingCart> GetByUserIdAsync(string id)
		{
			return await _collection.Find(x => x.UserId == id).FirstOrDefaultAsync();
		}
	}
}

using IceCreamRatings.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace IceCreamRatings.Repositories
{
    public class RateRepository
    {
        private readonly IMongoCollection<Rate> _rateCollection;

        public RateRepository(IMongoClient mongoClient)
        {
            _rateCollection = mongoClient
                .GetDatabase("BFYOC")
                .GetCollection<Rate>("Rate");
        }

        /// <summary>
        ///     Adds the specified entities list in repository.
        /// </summary>
        public virtual async Task AddRangeAsync(IEnumerable<Rate> entities, CancellationToken cancellationToken)
        {
            await _rateCollection.InsertManyAsync(entities, cancellationToken: cancellationToken);
        }

        /// <summary>
        ///     Replace the specified entity item in repository.
        /// </summary>
        public virtual async Task UpdateAsync(Rate entity, CancellationToken cancellationToken)
        {
            await _rateCollection.ReplaceOneAsync(x => x.Id!.Equals(entity.Id), entity,
                new ReplaceOptions { IsUpsert = true },
                cancellationToken);
        }

        /// <summary>
        ///     Get single entity by identifier.
        /// </summary>
        public virtual async Task<Rate?> GetSingleOrDefaultAsync(Expression<Func<Rate, bool>> predicate,
            CancellationToken cancellationToken)
        {
            return await _rateCollection.Find(predicate).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        ///     Get all entities in repository.
        /// </summary>
        /// <param name="cancellationToken"> Propagates notification that operations should be canceled.</param>
        public virtual async Task<List<Rate>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _rateCollection.AsQueryable().ToListAsync(cancellationToken);
        }
    }
}
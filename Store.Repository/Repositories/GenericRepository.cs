using Microsoft.EntityFrameworkCore;
using Store.Data.Context;
using Store.Data.Entities;
using Store.Repository.Interfaces;
using Store.Repository.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        private readonly StoreDbContext context;

        public GenericRepository(StoreDbContext context)
        {
            this.context = context;
        }
        public async Task AddAsync(TEntity entity)
           => await context.Set<TEntity>().AddAsync(entity);

        public void Delete(TEntity entity)
        => context.Set<TEntity>().Remove(entity);

        public async Task<IReadOnlyList<TEntity>> GetAllAsync()
         => await context.Set<TEntity>().ToListAsync();

        public async Task<IReadOnlyList<TEntity>> GetAllWithSpecificationAsync(ISpecification<TEntity> specs)
             => await ApplySpecification(specs).ToListAsync();

        public async Task<TEntity> GetByIdAsync(TKey? id)
         => await context.Set<TEntity>().FindAsync(id);

        public async Task<TEntity> GetWithSpecificationByIdAsync(ISpecification<TEntity> specs)
              => await ApplySpecification(specs).FirstOrDefaultAsync();

        public async Task<int> CountSpecificationAsync(ISpecification<TEntity> specs)
              => await ApplySpecification(specs).CountAsync();

        public void Update(TEntity entity)
         => context.Set<TEntity>().Remove(entity);

        private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specs)
            => SpecificationEvaluator<TEntity, TKey>.GetQuery(context.Set<TEntity>(), specs);

    }
}

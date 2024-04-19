using Store.Data.Context;
using Store.Data.Entities;
using Store.Repository.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreDbContext context;
        private Hashtable _repositories;

        public UnitOfWork(StoreDbContext context)
        {
            this.context = context;
        }
        public async Task<int> CompleteAsync()
        => await context.SaveChangesAsync();

        public IGenericRepository<TEntity, Tkey> Repository<TEntity, Tkey>() where TEntity : BaseEntity<Tkey>
        {
            if (_repositories is null)
                _repositories = new Hashtable();

            var entityKey = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(entityKey))
            {
                var repositoryType = typeof(GenericRepository<,>);

                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity) , typeof(Tkey)), context);

                _repositories.Add(entityKey, repositoryInstance);
            }

            return (IGenericRepository<TEntity, Tkey>)_repositories[entityKey];
        }
    }
}

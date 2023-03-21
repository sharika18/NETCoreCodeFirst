using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using DAL.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using DAL.Interfaces;
namespace DAL.Repositories
{
    public class UnitOfWork :IDisposable, IUnitOfWork
    {
        private readonly Context dbContext;

        public ISalesRepository SalesRepository { get; }
        public IProductRepository ProductRepository { get; }
        public ICustomerRepository CustomerRepository { get; }
        public ITerritoriesRepository TerritoriesRepository { get; }
        public UnitOfWork(Context context)
        {
            dbContext = context;

            SalesRepository = new SalesRepository(context);
            ProductRepository = new ProductRepository(context);
            CustomerRepository = new CustomerRepository(context);
            TerritoriesRepository = new TerritoriesRepository(context);
        }

        public void Save()
        {
            dbContext.SaveChanges();
        }

        public Task SaveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbContext.SaveChangesAsync(cancellationToken);
        }

        public IDbContextTransaction StartNewTransaction()
        {
            return dbContext.Database.BeginTransaction();
        }

        public Task<IDbContextTransaction> StartNewTransactionAsync()
        {
            return dbContext.Database.BeginTransactionAsync();
        }

        public Task<int> ExecuteSqlCommandAsync(string sql, object[] parameters, CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbContext.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    dbContext?.Dispose();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}

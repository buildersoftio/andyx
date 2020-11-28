using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic.Books;
using Buildersoft.Andy.X.Logic.Components;
using Buildersoft.Andy.X.Logic.Interfaces.Tenants;
using Buildersoft.Andy.X.Logic.Products;
using Buildersoft.Andy.X.Logic.Readers;
using Buildersoft.Andy.X.Logic.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Services
{
    public class DashboardRestService : IDashboardRestService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly StorageMemoryRepository _storageMemoryRepository;

        public DashboardRestService(ITenantRepository tenantRepository, StorageMemoryRepository storageMemoryRepository)
        {
            _tenantRepository = tenantRepository;
            _storageMemoryRepository = storageMemoryRepository;
        }

        public ICollection<string> GetBookNames(string tenant, string product, string component)
        {
            var bookRepo = new BookMemoryRepository(_storageMemoryRepository.GetBooks(tenant, product, component));
            return bookRepo.GetBooksConcurrent().Keys;
        }

        public List<Book> GetBooks(string tenant, string product, string component)
        {
            var bookRepo = new BookMemoryRepository(_storageMemoryRepository.GetBooks(tenant, product, component));
            return bookRepo.GetAll();
        }

        public ICollection<string> GetComponentNames(string tenant, string product)
        {
            var componentRepo = new ComponentMemoryRepository(_storageMemoryRepository.GetComponents(tenant, product));
            return componentRepo.GetComponentsConcurrent().Keys;
        }

        public List<Component> GetComponents(string tenant, string product)
        {
            var componentRepo = new ComponentMemoryRepository(_storageMemoryRepository.GetComponents(tenant, product));
            return componentRepo.GetAll();
        }

        public ICollection<string> GetProductNames(string tenant)
        {
            var productRepo = new ProductMemoryRepository(_tenantRepository.Get(tenant).Products);
            return productRepo.GetProductsDictionary().Keys;
        }

        public List<Product> GetProducts(string tenant)
        {
            var productRepo = new ProductMemoryRepository(_tenantRepository.Get(tenant).Products);
            return productRepo.GetAll();
        }

        public ICollection<string> GetReaderNames(string tenant, string product, string component, string books)
        {
            var readerRepo = new ReaderMemoryRepository(_storageMemoryRepository.GetReaders(tenant, product, component, books));
            return readerRepo.GetAllReadersConcurrent().Keys;
        }

        public List<Reader> GetReaders(string tenant, string product, string component, string books)
        {
            var readerRepo = new ReaderMemoryRepository(_storageMemoryRepository.GetReaders(tenant, product, component, books));
            return readerRepo.GetAll();
        }

        public ICollection<string> GetTenantNames()
        {
            return _tenantRepository.GetAllAsDictionary().Keys;
        }

        public List<Tenant> GetTenants()
        {
            return _tenantRepository.GetAllAsList();
        }
    }
}

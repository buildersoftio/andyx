using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic.Books;
using Buildersoft.Andy.X.Logic.Components;
using Buildersoft.Andy.X.Logic.Interfaces.Tenants;
using Buildersoft.Andy.X.Logic.Mappers.Dashboard;
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

        public Book GetBookDetails(string tenant, string product, string component, string book)
        {
            try
            {
                var bookInRepo = new BookMemoryRepository(_storageMemoryRepository.GetBooks(tenant, product, component)).Get(book);

                return new Book()
                {
                    Id = bookInRepo.Id,
                    InstanceName = bookInRepo.InstanceName,
                    DataType = bookInRepo.DataType,
                    ModifiedDate = bookInRepo.ModifiedDate,
                    CreatedDate = bookInRepo.CreatedDate
                };
            }
            catch (System.Exception)
            {
                return new Book();
            }
        }

        public ICollection<string> GetBookNames(string tenant, string product, string component)
        {
            try
            {
                var bookRepo = new BookMemoryRepository(_storageMemoryRepository.GetBooks(tenant, product, component));
                return bookRepo.GetBooksConcurrent().Keys;
            }
            catch (System.Exception)
            {
                return null;
            }

        }

        public Component GetComponentDetails(string tenant, string product, string component)
        {
            try
            {
                var componentData = new ComponentMemoryRepository(_storageMemoryRepository.GetComponents(tenant, product)).Get(component);

                return new Component()
                {
                    Id = componentData.Id,
                    Name = componentData.Name,
                    Description = componentData.Description,
                    Status = componentData.Status,
                    ModifiedDate = componentData.ModifiedDate,
                    CreatedDate = componentData.CreatedDate
                };
            }
            catch (System.Exception)
            {
                return new Component();
            }

        }

        public ICollection<string> GetComponentNames(string tenant, string product)
        {
            try
            {
                var componentRepo = new ComponentMemoryRepository(_storageMemoryRepository.GetComponents(tenant, product));
                return componentRepo.GetComponentsConcurrent().Keys;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public Product GetProductDetails(string tenant, string product)
        {
            try
            {
                var productInRepo = new ProductMemoryRepository(_tenantRepository.Get(tenant).Products).Get(product);

                return new Product()
                {
                    Id = productInRepo.Id,
                    Name = productInRepo.Name,
                    Description = productInRepo.Description,
                    Status = productInRepo.Status,
                    ModifiedDate = productInRepo.ModifiedDate,
                    CreatedDate = productInRepo.CreatedDate
                };
            }
            catch (System.Exception)
            {
                return new Product();
            }
 
        }

        public ICollection<string> GetProductNames(string tenant)
        {
            try
            {
                var productRepo = new ProductMemoryRepository(_tenantRepository.Get(tenant).Products);
                return productRepo.GetProductsDictionary().Keys;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public Reader GetReaderDetails(string tenant, string product, string component, string books, string reader)
        {
            try
            {
                var readerInRepo = new ReaderMemoryRepository(_storageMemoryRepository.GetReaders(tenant, product, component, books)).Get(reader);

                return new Reader()
                {
                    Id = readerInRepo.Id,
                    Name = readerInRepo.Name,
                    ReaderTypes = readerInRepo.ReaderTypes,
                    ReaderAs = readerInRepo.ReaderAs,
                    ModifiedDate = readerInRepo.ModifiedDate,
                    CreatedDate = readerInRepo.CreatedDate
                };
            }
            catch (System.Exception)
            {
                return new Reader();
            }
        }

        public ICollection<string> GetReaderNames(string tenant, string product, string component, string books)
        {
            
            try
            {
                var readerRepo = new ReaderMemoryRepository(_storageMemoryRepository.GetReaders(tenant, product, component, books));
                return readerRepo.GetAllReadersConcurrent().Keys;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public Tenant GetTenantDetails(string tenant)
        {
            try
            {
                var tenantInRepo = _tenantRepository.Get(tenant);

                return new Tenant()
                {
                    Id = tenantInRepo.Id,
                    Name = tenantInRepo.Name,
                    Description = tenantInRepo.Description,
                    Status = tenantInRepo.Status,
                    Encryption = new Encryption() { EncryptionStatus = tenantInRepo.GetEncryption().EncryptionStatus },
                    ModifiedDate = tenantInRepo.ModifiedDate,
                    CreatedDate = tenantInRepo.CreatedDate
                };
            }
            catch (System.Exception)
            {
                return new Tenant();
            }
        }

        public ICollection<string> GetTenantNames()
        {
            return _tenantRepository.GetAllAsDictionary().Keys;
        }
    }
}

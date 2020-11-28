using Buildersoft.Andy.X.Data.Model;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic
{
    public class StorageMemoryRepository
    {
        private ConcurrentDictionary<string, Tenant> _tenants;

        public StorageMemoryRepository()
        {
            _tenants = new ConcurrentDictionary<string, Tenant>();
        }

        public void SetTenants(ConcurrentDictionary<string, Tenant> tenants)
        {
            _tenants = tenants;
        }

        public ConcurrentDictionary<string, Tenant> GetTenants()
        {
            return _tenants;
        }

        public ConcurrentDictionary<string, Product> GetProducts(string tenantName)
        {
            try
            {
                return _tenants[tenantName].Products;
            }
            catch (Exception)
            {
                return new ConcurrentDictionary<string, Product>();
            }
        }
        public ConcurrentDictionary<string, Component> GetComponents(string tenantName, string productName)
        {
            try
            {
                return _tenants[tenantName].Products[productName].Components;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public ConcurrentDictionary<string, Book> GetBooks(string tenantName, string productName, string componentName)
        {
            try
            {
                return _tenants[tenantName].Products[productName].Components[componentName].Books;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public ConcurrentDictionary<string, Reader> GetReaders(string tenantName, string productName, string componentName, string bookName)
        {
            try
            {
                return _tenants[tenantName].Products[productName].Components[componentName].Books[bookName].Readers;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public ConcurrentDictionary<string, Message> GetMessages(string tenantName, string productName, string componentName, string bookName)
        {
            try
            {
                return _tenants[tenantName].Products[productName].Components[componentName].Books[bookName].Messages;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

using Buildersoft.Andy.X.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Mappers.Dashboard
{
    public static class ObjectMapper
    {
        public static List<Tenant> Map(this List<Tenant> tenants)
        {
            var result = new List<Tenant>();
            foreach (var tenant in tenants)
            {
                result.Add(new Tenant()
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    Description = tenant.Description,
                    Status = tenant.Status,
                    ModifiedDate = tenant.ModifiedDate,
                    CreatedDate = tenant.CreatedDate
                });
            }

            return result;
        }

        public static List<Product> Map(this List<Product> products)
        {
            var result = new List<Product>();
            foreach (var product in products)
            {
                result.Add(new Product()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Status = product.Status,
                    Description = product.Description,
                    ModifiedDate = product.ModifiedDate,
                    CreatedDate = product.CreatedDate
                });
            }

            return result;
        }

        public static List<Component> Map(this List<Component> components)
        {
            var result = new List<Component>();
            foreach (var component in components)
            {
                result.Add(new Component()
                {
                    Id = component.Id,
                    Name = component.Name,
                    Description = component.Description,
                    Status = component.Status,
                    ModifiedDate = component.ModifiedDate,
                    CreatedDate = component.CreatedDate
                });
            }

            return result;
        }

        public static List<Book> Map(this List<Book> books)
        {
            var result = new List<Book>();
            foreach (var book in books)
            {
                result.Add(new Book()
                {
                    Id = book.Id,
                    InstanceName = book.InstanceName,
                    DataType = book.DataType,
                    ModifiedDate = book.ModifiedDate,
                    CreatedDate = book.CreatedDate
                });
            }

            return result;
        }

        public static List<Reader> Map(this List<Reader> readers)
        {
            var result = new List<Reader>();
            foreach (var reader in readers)
            {
                result.Add(new Reader()
                {
                    Id = reader.Id,
                    Name = reader.Name,
                    ReaderAs = reader.ReaderAs,
                    ReaderTypes = reader.ReaderTypes,
                    ModifiedDate = reader.ModifiedDate,
                    CreatedDate = reader.CreatedDate
                });
            }

            return result;
        }
    }
}

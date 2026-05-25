using PoC.Core.Contracts;
using PoC.Core.Model;

namespace PoC.Core.Repository;

public sealed class InMemoryRepository : IProductRepository
{
    private readonly List<Category> _categories =
    [
        new Category
        {
            Id = 1,
            Name = "Electronics",
            Description = "Electronic devices and accessories"
        },
        new Category
        {
            Id = 2,
            Name = "Books",
            Description = "Printed and digital books"
        },
        new Category
        {
            Id = 3,
            Name = "Home",
            Description = "Home and kitchen products"
        }
    ];

    private readonly List<Product> _products =
    [
        new Product
        {
            Id = 1,
            Name = "Laptop",
            Price = 1200.00m,
            CategoryId = 1
        },
        new Product
        {
            Id = 2,
            Name = "Keyboard",
            Price = 74.45m,
            CategoryId = 1
        },
        new Product
        {
            Id = 3,
            Name = "Book",
            Price = 40.00m,
            CategoryId = 2
        },
        new Product
        {
            Id = 4,
            Name = "Coffee accessories",
            Price = 5.99m,
            CategoryId = 3
        }
    ];

    private readonly Lock _lock = new();

    public IReadOnlyCollection<Product> GetFiltered(ProductFilter productFilter)
    {
        lock (_lock)
        {
            IEnumerable<Product> query = _products;

            if (!string.IsNullOrWhiteSpace(productFilter.Name))
            {
                query = query.Where(product =>
                    product.Name.Contains(productFilter.Name, StringComparison.OrdinalIgnoreCase));
            }

            if (productFilter.CategoryId is not null)
            {
                query = query.Where(product => product.CategoryId == productFilter.CategoryId);
            }

            //if (!string.IsNullOrWhiteSpace(productFilter.CategoryName))
            //{
            //    var matchingCategoryIds = _categories
            //        .Where(category => category.Name.Contains(
            //            productFilter.CategoryName,
            //            StringComparison.OrdinalIgnoreCase))
            //        .Select(category => category.Id)
            //        .ToHashSet();

            //    query = query.Where(product => matchingCategoryIds.Contains(product.CategoryId));
            //}

            if (productFilter.MinPrice is not null)
            {
                query = query.Where(product => product.Price >= productFilter.MinPrice);
            }

            if (productFilter.MaxPrice is not null)
            {
                query = query.Where(product => product.Price <= productFilter.MaxPrice);
            }

            return [.. query.OrderBy(product => product.Id)];
        }
    }

    //public bool CategoryExists(int categoryId) => _categories.Any(category => category.Id == categoryId);

    public Product? Update(int id, UpdateProductRequest request)
    {
        lock (_lock)
        {
            var product = _products.FirstOrDefault(product => product.Id == id);

            if (product is null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                product.Name = request.Name.Trim();
            }

            if (request.Price is not null)
            {
                product.Price = request.Price.Value;
            }

            return product;
        }
    }
}
using PoC.Core.Contracts;
using PoC.Core.Model;

namespace PoC.Core.Repository;

public interface IProductRepository
{
    IReadOnlyCollection<Product> GetFiltered(ProductFilter productFilter);

    Product? Update(int id, UpdateProductRequest request);
}
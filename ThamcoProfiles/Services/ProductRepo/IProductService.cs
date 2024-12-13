using System;

namespace ThamcoProfiles.Services.ProductRepo;

public interface IProductService{

    Task<IEnumerable<ProductDto>> GetProductsAsync();
}
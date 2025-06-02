using FoodMartMongo.Dtos.ProductDtos;

public interface IProductService
{
    Task<List<ResultProductWithCategoryDto>> GetAllProductsWithCategoryAsync();
    Task<List<ResultProductDto>> GetAllProductsAsync(); // ✅ BU EKLENDİ
    Task CreateProductAsync(CreateProductDto createProductDto);
    Task UpdateProductAsync(UpdateProductDto updateProductDto);
    Task DeleteProductAsync(string id);
    Task<GetProductByIdDto> GetProductByIdAsync(string id);
}


using AutoMapper;
using FoodMartMongo.Dtos.ProductDtos;
using FoodMartMongo.Dtos.CategoryDtos;
using FoodMartMongo.Entities;
using FoodMartMongo.Settings;
using MongoDB.Driver;

namespace FoodMartMongo.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMapper _mapper;

        public ProductService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _productCollection = database.GetCollection<Product>(databaseSettings.ProductCollectionName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);

            _mapper = mapper;
        }

        public async Task CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            await _productCollection.InsertOneAsync(product);
        }

        public async Task DeleteProductAsync(string id)
        {
            await _productCollection.DeleteOneAsync(x => x.ProductId == id);
        }

        public async Task<List<ResultProductDto>> GetAllProductsAsync()
        {
            var products = await _productCollection.Find(x => true).ToListAsync();
            return _mapper.Map<List<ResultProductDto>>(products);
        }

        public async Task<List<ResultProductWithCategoryDto>> GetAllProductsWithCategoryAsync()
        {
            var products = await _productCollection.Find(x => true).ToListAsync();
            var result = new List<ResultProductWithCategoryDto>();

            foreach (var product in products)
            {
                var category = await _categoryCollection
                    .Find(x => x.CategoryId == product.CategoryId)
                    .FirstOrDefaultAsync();

                var dto = new ResultProductWithCategoryDto
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Price = product.Price,
                    ProductImageUrl = product.ImageUrl,
                    StockCount = product.StockCount,
                    CategoryId = product.CategoryId,
                    Category = category != null ? new ResultCategoryDto
                    {
                        CategoryId = category.CategoryId,
                        CategoryName = category.CategoryName,
                        IconUrl = category.IconUrl
                    } : null
                };

                result.Add(dto);
            }

            return result;
        }

        public async Task<GetProductByIdDto> GetProductByIdAsync(string id)
        {
            var product = await _productCollection.Find(x => x.ProductId == id).FirstOrDefaultAsync();
            return _mapper.Map<GetProductByIdDto>(product);
        }

        public async Task UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            var updatedProduct = _mapper.Map<Product>(updateProductDto);
            await _productCollection.FindOneAndReplaceAsync(x => x.ProductId == updateProductDto.ProductId, updatedProduct);
        }
    }
}

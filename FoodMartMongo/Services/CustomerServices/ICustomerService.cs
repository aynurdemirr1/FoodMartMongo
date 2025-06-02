using FoodMartMongo.Dtos.CustomerDtos;

namespace FoodMartMongo.Services.CustomerServices
{
    public interface ICustomerService
    {
        Task<List<ResultCustomerDto>> GetAllCustomersAsync();
        Task CreateCustomerAsync(CreateCustomerDto createcustomerDto);
        Task UpdateCustomerAsync(UpdateCustomerDto updatecustomerDto);
        Task DeleteCustomerAsync(string customerId);
        Task<GetCustomerByIdDto> GetCustomerByIdAsync(string customerId);
    }
}

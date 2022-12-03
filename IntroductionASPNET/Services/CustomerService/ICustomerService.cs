namespace IntroductionASPNET.Services.CustomerServices
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetCustomers();
        Task<Result<Customer>> GetCustomersByID(string CustomerNumber);
        string GetCustomerIDValue();
        Task<Result<Customer>> AddCustomer(Customer customer);
        Task<Result<Customer>> UpdateCustomer(Customer customer);

        Task<Result<Customer>> DeleteCustomer(string CustomerNumber);
    }
}

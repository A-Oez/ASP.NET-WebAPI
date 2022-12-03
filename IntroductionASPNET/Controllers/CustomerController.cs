namespace IntroductionASPNET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles="Admin")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<List<Customer>>> GetCustomers()
        {
            var result = await _customerService.GetCustomers();

            if (result is null)
                return BadRequest("No Customers");

            return Ok(result);
        }


        [HttpGet("{CustomerNumber}")] 
        public async Task<ActionResult<Customer>> GetCustomerByID(string CustomerNumber)
        {
            var result = await _customerService.GetCustomersByID(CustomerNumber);
            var customerValue = _customerService.GetCustomerIDValue();
            return result.ToOk(a => customerValue, "Customer","GetID");   
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> AddCustomer(Customer customer)
        {
            var result = await _customerService.AddCustomer(customer);
            return result.ToOk(a => customer.UserId+" "+result.IsSuccess, "Customer","ADD");
        }

        [HttpPut]
        public async Task<ActionResult<Customer>> UpdateCustomer(Customer customer) 
        {
            var result = await _customerService.UpdateCustomer(customer);
            return result.ToOk(a => customer.UserId + " " + result.IsSuccess, "Customer", "ADD");
        }

        [HttpDelete("{CustomerNumber}")] 
        public async Task<ActionResult<Customer>> DeleteCustomerByID(string CustomerNumber)
        {
            var result = await _customerService.DeleteCustomer(CustomerNumber);
            return result.ToOk(a => CustomerNumber + " " + result.IsSuccess, "Customer", "ADD");
        }

    }
}

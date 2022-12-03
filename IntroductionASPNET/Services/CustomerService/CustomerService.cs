namespace IntroductionASPNET.Services.CustomerServices
{
    public class CustomerService : ICustomerService
    {
        private readonly TestDBContext _context;
        private readonly ICacheService _cacheService;
        private readonly IValidator<Customer> _validator;
        private Customer _customer;

        public CustomerService(TestDBContext context, ICacheService cacheService, IValidator<Customer> validator)
        {
            _context = context;
            _cacheService = cacheService;
            _validator = validator;
        }

        public async Task<List<Customer>> GetCustomers()
        {
            //Distributed Cache
            //check cache data
            var cacheData = _cacheService.GetData<IEnumerable<Customer>>("Customer");

            if (cacheData != null && cacheData.Count() > 0)
                return (List<Customer>)cacheData;

            //get value from DB
            var value = await _context.Customers.ToListAsync();

            //set expiry time 
            var expiryTime = DateTimeOffset.Now.AddSeconds(20);
            _cacheService.SetData<IEnumerable<Customer>>("Customer", value, expiryTime);

            return value;
        }

        public async Task<Result<Customer>> GetCustomersByID(string CustomerNumber)
        {
            //Distributed Cache 
            var cacheData = _cacheService.GetData<IEnumerable<Customer>>("Customer");

            var createdCustomer = new Customer();
            createdCustomer.UserId = CustomerNumber;

            //validation
            var validation = await returnValidationError(createdCustomer);
            if (validation.IsSuccess.Equals(false)) return validation;

            if (cacheData is null)
            {
                var customerByID = await _context.Customers.FindAsync(CustomerNumber);

                //get nullDB
                var checkNullDB = returnNullError(customerByID);
                if (checkNullDB.IsSuccess.Equals(false)) return checkNullDB;

                _customer = customerByID;
                return customerByID;
            }

            //get ID from cache
            foreach (var customer in cacheData)
            {
                if (customer.UserId == CustomerNumber)
                {
                    _customer = customer;
                    return customer;
                }
            }
            return null;
        }

        public string GetCustomerIDValue()
        {
            if (_customer is null)
                return null;

            return  "UserID: "      + _customer.UserId      + ";" +
                    "Forename: "    + _customer.Forename    + ";" +
                    "Surename: "    + _customer.Surename    + ";" +
                    "Birthday: "    + _customer.Birthday    + ";" +
                    "City: "        + _customer.City        + ";" +
                    "Street: "      + _customer.Street      + ";" +
                    "HouseNumber: " + _customer.HouseNumber + ";" +
                    "E-Mail: "      + _customer.Email       + ";" +
                    "Telephone: "   + _customer.Telephone   + ";"; ;
        }

        public async Task<Result<Customer>> AddCustomer(Customer customer)
        {
            //validation
            var validation = await returnValidationError(customer);
            if (validation.IsSuccess.Equals(false)) return validation;


            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Result<Customer>> UpdateCustomer(Customer customer)
        {
            var dbCustomer = await _context.Customers.FindAsync(customer.UserId);

            //validation
            var validation = await returnValidationError(customer);
            if (validation.IsSuccess.Equals(false)) return validation;

            //get nullDB
            var checkNullDB = returnNullError(dbCustomer);
            if (checkNullDB.IsSuccess.Equals(false)) return checkNullDB;

            dbCustomer.UserId = customer.UserId;
            dbCustomer.Forename= customer.Forename;
            dbCustomer.Surename = customer.Surename;
            dbCustomer.Birthday = customer.Birthday;
            dbCustomer.City = customer.City;
            dbCustomer.Code = customer.Code;
            dbCustomer.Street = customer.Street;
            dbCustomer.HouseNumber = customer.HouseNumber;
            dbCustomer.Email = customer.Email;
            dbCustomer.Telephone = customer.Telephone;

            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task<Result<Customer>> DeleteCustomer(string CustomerNumber)
        {
            var dbCustomer = await _context.Customers.FindAsync(CustomerNumber);

            var createdCustomer = new Customer();
            createdCustomer.UserId = CustomerNumber;

            //validation
            var validation = await returnValidationError(createdCustomer);
            if (validation.IsSuccess.Equals(false)) return validation;

            //get nullDB
            var checkNullDB = returnNullError(dbCustomer);
            if(checkNullDB.IsSuccess.Equals(false)) return checkNullDB;  

            //Remove from DB
            _context.Customers.Remove(dbCustomer);
            //Remove from Cache
            var cachceData = _cacheService.GetData<IEnumerable<Customer>>("Customer");
            if (cachceData != null)
            {
                foreach (var customer in cachceData)
                {
                    if (customer.UserId == CustomerNumber)
                        _cacheService.RemoveData("Customer");
                }
            }

            await _context.SaveChangesAsync();
            return new Customer();
        }

        //Get Exception Results 
        private async Task<Result<Customer>> returnValidationError(Customer validation)
        {
            var validationResult = await _validator.ValidateAsync(validation);
            if (validationResult.IsValid)
                return null;

            var validationException = new ValidationException(validationResult.Errors);
            return new Result<Customer>(validationException);
        }

        private Result<Customer> returnNullError(Customer checkDB)
        {
            if (checkDB is null)
            {
                var exception = new NullReferenceException("the given ID " + checkDB.UserId.ToString() + " does not exists");
                return new Result<Customer>(exception);
            }

            return null;
        }

    }
}

namespace IntroductionASPNET.Services.OrderServices
{
    public class OrderServices : IOrderService
    {
        private readonly TestDBContext _context;
        private readonly ICacheService _cacheService;
        private readonly IValidator<Order> _validator;
        private Order _order;

        public OrderServices(TestDBContext context, ICacheService cacheService, IValidator<Order> validator)
        {
            _context = context;
            _cacheService = cacheService;
            _validator = validator;
        }

        public async Task<List<Order>> GetOrders()
        {
            //Distributed Cache
            //check cache data
            var cacheData = _cacheService.GetData<IEnumerable<Order>>("Order");

            if (cacheData != null && cacheData.Count() > 0)
                return (List<Order>)cacheData;

            //get value from DB
            var value = await _context.Orders.ToListAsync();

            //set expiry time 
            var expiryTime = DateTimeOffset.Now.AddSeconds(20);
            _cacheService.SetData<IEnumerable<Order>>("Order", value, expiryTime);

            return value;
        }

        public async Task<Result<Order>> GetOrdersByID(string OrderNumber)
        {
            //Distributed Cache 
            var cacheData = _cacheService.GetData<IEnumerable<Order>>("Order");

            var createdOrder = new Order();
            createdOrder.Ordernumber = OrderNumber;

            //check validation
            var validation = await returnValidationError(createdOrder);
            if (validation.IsSuccess.Equals(false)) return validation;

            if (cacheData is null)
            {
                var orderID = await _context.Orders.FindAsync(OrderNumber);
                //check dbNull 
                var checkDBNull = returnNullError(orderID);
                if (checkDBNull.IsSuccess.Equals(false)) return checkDBNull;

                _order = orderID;
                return orderID;
            }

            //get ID from cache
            foreach (var order in cacheData)
            {
                if (order.Ordernumber == OrderNumber)
                {
                    _order = order;
                    return order;
                }
            }
            return null;
        }

        public string GetIDOrderValue()
        {
            if (_order is null)
                return null;

            return "Orderdate: "    + _order.Orderdate        + ";" +
                   "Ordernumber: "  + _order.Ordernumber      + ";" +
                   "UserID: "       + _order.UserId           + ";" +
                   "Article: "      + _order.Article          + ";" ;
        }


        public async Task<Result<Order>> AddOrder(Order order)
        {
            //check validation
            var validation = await returnValidationError(order);
            if (validation.IsSuccess.Equals(false)) return validation;

            //checkForeignKey
            var checkKeys = await checkForeignKey(order);
            if (checkKeys.IsSuccess.Equals(false)) return checkKeys;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Result<Order>> UpdateOrder(Order order)
        {
            var dbOrder = await _context.Orders.FindAsync(order.Ordernumber);

            //check validation
            var validation = await returnValidationError(order);
            if (validation.IsSuccess.Equals(false)) return validation;

            //check dbNull 
            var checkDBNull = returnNullError(dbOrder);
            if (checkDBNull.IsSuccess.Equals(false)) return checkDBNull;

            //checkForeignKey
            var checkKeys = await checkForeignKey(dbOrder);
            if (checkKeys.IsSuccess.Equals(false)) return checkKeys;

            dbOrder.Orderdate = order.Orderdate;
            dbOrder.Ordernumber = order.Ordernumber;
            dbOrder.UserId = order.UserId;
            dbOrder.Article = order.Article;

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Result<Order>> DeleteOrder(string OrderNumber)
        {
            var dbOrder = await _context.Orders.FindAsync(OrderNumber);

            var createdOrder = new Order();
            createdOrder.Ordernumber = OrderNumber;

            //check validation
            var validation = await returnValidationError(createdOrder);
            if (validation.IsSuccess.Equals(false)) return validation;

            //check dbNull 
            var checkDBNull = returnNullError(dbOrder);
            if (checkDBNull.IsSuccess.Equals(false)) return checkDBNull;

            _context.Orders.Remove(dbOrder);

            //Remove from Cache
            var cachceData = _cacheService.GetData<IEnumerable<Order>>("Order");
            if (cachceData != null)
            {
                foreach (var order in cachceData)
                {
                    if (order.Ordernumber == OrderNumber)
                        _cacheService.RemoveData($"Order");
                }
            }

            await _context.SaveChangesAsync();
            return new Order();
        }

        //Get Exception Results 
        private async Task<Result<Order>> returnValidationError(Order validation)
        {
            var validationResult = await _validator.ValidateAsync(validation);
            if (validationResult.IsValid)
                return null;

            var validationException = new ValidationException(validationResult.Errors);
            return new Result<Order>(validationException);
        }

        private Result<Order> returnNullError(Order checkDB)
        {
            if (checkDB is null)
            {
                var exception = new NullReferenceException("the given ID does not exists");
                return new Result<Order>(exception);
            }

            return null;
        }

        private async Task<Result<Order>> checkForeignKey(Order checkKey)
        {
            var keyArticle = await _context.Articles.FindAsync(checkKey.Article);
            var keyCustomer = await _context.Customers.FindAsync(checkKey.UserId);

            if (keyCustomer is null || keyArticle is null)
            {
                var exception = new NullReferenceException("the article/customer does not exist");
                return new Result<Order>(exception);
            }

            return null;
        }
    }
}

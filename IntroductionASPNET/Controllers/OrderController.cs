namespace IntroductionASPNET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles="Admin")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            var result = await _orderService.GetOrders();

            if (result is null)
                return BadRequest("No Orders");

            return Ok(result);
        }


        [HttpGet("{OrderNumber}")] 
        public async Task<ActionResult<Order>> GetOrderByID(string OrderNumber)
        {
            var result = await _orderService.GetOrdersByID(OrderNumber);
            var orderValue = _orderService.GetIDOrderValue();
            return result.ToOk(a => orderValue, "Order","GetID");   
        }

        [HttpPost]
        public async Task<ActionResult<Order>> AddOrder(Order order)
        {
            var result = await _orderService.AddOrder(order);
            return result.ToOk(a => order.Ordernumber+" "+result.IsSuccess, "Order","ADD");
        }

        [HttpPut]
        public async Task<ActionResult<Order>> UpdateOrder(Order order) 
        {
            var result = await _orderService.UpdateOrder(order);
            return result.ToOk(a => order.Ordernumber + " " + result.IsSuccess, "Order", "ADD");
        }

        [HttpDelete("{OrderNumber}")] 
        public async Task<ActionResult<Order>> DeleteOrderByID(string OrderNumber)
        {
            var result = await _orderService.DeleteOrder(OrderNumber);
            return result.ToOk(a => OrderNumber + " " + result.IsSuccess, "Order", "ADD");
        }

    }
}

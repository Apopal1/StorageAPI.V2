using Microsoft.AspNetCore.Mvc;
using StorageManagement.API.Models;
using StorageManagement.API.Repositories;
using System.Threading.Tasks;

namespace StorageManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OutgoingOrdersController : ControllerBase
    {
        private readonly IOutgoingOrderRepository _repository;

        public OutgoingOrdersController(IOutgoingOrderRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _repository.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null)
                return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OutgoingOrder order)
        {
            var createdOrder = await _repository.CreateAsync(order);
            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OutgoingOrder order)
        {
            if (id != order.Id)
                return BadRequest();

            await _repository.UpdateAsync(order);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{orderId}/items")]
        public async Task<IActionResult> AddOrderItem(int orderId, OutgoingOrderItem item)
        {
            if (orderId != item.OutgoingOrderId)
                return BadRequest();

            await _repository.AddOrderItemAsync(item);
            return NoContent();
        }

        [HttpGet("{orderId}/items")]
        public async Task<IActionResult> GetOrderItems(int orderId)
        { 
            var items = await _repository.GetOrderItemsAsync(orderId);
            return Ok(items);
        }
    }
}
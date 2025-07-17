using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StorageManagement.API.Models;
using StorageManagement.API.Repositories;

namespace StorageManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SuperitemsController : ControllerBase
    {
        private readonly ISuperitemRepository _superitemRepo;
        public SuperitemsController(ISuperitemRepository superitemRepo)
        {
            _superitemRepo = superitemRepo;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_superitemRepo.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var superitem = _superitemRepo.GetById(id);
            if (superitem == null) return NotFound();
            return Ok(superitem);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] SuperitemDto dto)
        {
            var superitem = new Superitem
            {
                Name = dto.Name,
                Location = dto.Location,
                Quantity = dto.Quantity
            };
            var id = _superitemRepo.Add(superitem);
            // Add subitems with quantities
            if (dto.SubItems != null)
            {
                foreach (var sub in dto.SubItems)
                {
                    _superitemRepo.AddSubItem(id, sub.StorageItemId, sub.Quantity);
                }
            }
            var created = _superitemRepo.GetById(id);
            return CreatedAtAction(nameof(GetById), new { id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] SuperitemDto dto)
        {
            var superitem = new Superitem
            {
                Id = id,
                Name = dto.Name,
                Location = dto.Location,
                Quantity = dto.Quantity
            };
            _superitemRepo.Update(superitem);
            // Remove all subitems and re-add with quantities
            var existingSubItems = _superitemRepo.GetSubItems(id);
            foreach (var sub in existingSubItems)
            {
                _superitemRepo.RemoveSubItem(id, sub.StorageItemId);
            }
            if (dto.SubItems != null)
            {
                foreach (var sub in dto.SubItems)
                {
                    _superitemRepo.AddSubItem(id, sub.StorageItemId, sub.Quantity);
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            _superitemRepo.Delete(id);
            return NoContent();
        }

        [HttpPost("{id}/subitems/{storageItemId}/{quantity}")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddSubItem(int id, int storageItemId, int quantity)
        {
            _superitemRepo.AddSubItem(id, storageItemId, quantity);
            return NoContent();
        }

        [HttpDelete("{id}/subitems/{storageItemId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult RemoveSubItem(int id, int storageItemId)
        {
            _superitemRepo.RemoveSubItem(id, storageItemId);
            return NoContent();
        }

        [HttpGet("{id}/subitems")]
        public IActionResult GetSubItems(int id)
        {
            var subItems = _superitemRepo.GetSubItems(id);
            return Ok(subItems);
        }

        [HttpGet("{id}/possible")]
        public IActionResult GetPossibleSuperitems(int id)
        {
            var subItems = _superitemRepo.GetSubItems(id);
            if (subItems == null || !subItems.Any())
            {
                return Ok(0); // Or handle as an error if a superitem must have sub-items
            }

            int possibleSuperitems = int.MaxValue;

            foreach (var subItem in subItems)
            {
                var availableQuantity = _superitemRepo.GetStorageItemQuantity(subItem.StorageItemId);
                if (subItem.Quantity == 0) continue; // Avoid division by zero
                
                int canMake = availableQuantity / subItem.Quantity;
                if (canMake < possibleSuperitems)
                {
                    possibleSuperitems = canMake;
                }
            }

            return Ok(possibleSuperitems == int.MaxValue ? 0 : possibleSuperitems);
        }
    }
}
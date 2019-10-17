using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KurirServer;
using KurirServer.Entities;

namespace KurirServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly KurirDbContext _context;

        public AddressesController(KurirDbContext context)
        {
            _context = context;
        }

        // GET: api/Addresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FullAddress>>> GetAddresses()
        {
            return await _context.Addresses.ToListAsync();
        }
        ///GetByUserID/" + UserID
        [Route("/GetByUserID/{UserID}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FullAddress>>> GetAddressesByUserID(int UserID)
        {
            if (_context.Users.Find(UserID) != null)
            {
                try
                {
                    var addresses = await _context.Addresses.Where(a => a.UserID == UserID).ToListAsync();
                    return Ok(addresses);
                }

                catch (Exception ex)
                {
                    return StatusCode(500, "internal server error" + ex.Message + ex.InnerException);

                }
            }
            else
                return NotFound("No user asociated with given parametes.");
        
        }
        // GET: api/Addresses/5

        [HttpGet("{id}")]
        public async Task<ActionResult<FullAddress>> GetFullAddress(int id)
        {
            var fullAddress = await _context.Addresses.FindAsync(id);

            if (fullAddress == null)
            {
                return NotFound();
            }

            return fullAddress;
        }

        // PUT: api/Addresses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFullAddress(int id, FullAddress fullAddress)
        {
            int x = 0;
            DbUpdateConcurrencyException exception=null;
            if (id != fullAddress.FullAddressID)
            {
                return BadRequest();
            }

            _context.Entry(fullAddress).State = EntityState.Modified;

            try
            {

                x = await _context.SaveChangesAsync();
                //
            }
            catch (DbUpdateConcurrencyException ex)
            {
                exception = ex;
                if (!FullAddressExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw; }
            }
            if(x>0)
                return Ok();
            else if(exception!=null)
            return StatusCode(500, "internal server error" + exception.Message + exception.InnerException);
            else
            return NoContent();
        }

        // POST: api/Addresses
        [HttpPost]
        public async Task<ActionResult<FullAddress>> PostFullAddress(FullAddress fullAddress)
        {
            try
            {
                _context.Addresses.Add(fullAddress);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetFullAddress", new { id = fullAddress.FullAddressID }, fullAddress);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "internal server error" + ex.Message + ex.InnerException);

            }
        
        }
        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteFullAddress(int id)
        {
            var fullAddress = await _context.Addresses.FindAsync(id);
            if (fullAddress == null)
            {
                return NotFound("Address with given parametters not found.");
            }
            fullAddress.UserID = 1;
            _context.Addresses.Update(fullAddress);
            //_context.Addresses.Remove(fullAddress);
            await _context.SaveChangesAsync();

            return StatusCode(202,"address successesfully deleted.");
        }

        private bool FullAddressExists(int id)
        {
            return _context.Addresses.Any(e => e.FullAddressID == id);
        }
    }
}

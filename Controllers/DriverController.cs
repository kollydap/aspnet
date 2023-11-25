using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace webutvikling.Controllers;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class DriverController : ControllerBase
{
    private readonly DriverDbContext _context;

    public DriverController(DriverDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Driver>>> GetAllDrivers()
    {
        return await _context.Drivers.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Driver>> GetDriverById(int id)
    {
        var driver = await _context.Drivers.FindAsync(id);

        if (driver == null)
        {
            return NotFound();
        }

        return driver;
    }

    [HttpGet("byname/{name}")]
    public ActionResult<IEnumerable<Driver>> GetDriversByName(string name)
    {
        var drivers = _context.Drivers
            .Where(driver => driver.Name.ToLower().Contains(name.ToLower()))
            .ToList();

        if (drivers == null || drivers.Count == 0)
        {
            return NotFound();
        }

        return drivers;
    }



    [HttpPost]
    public async Task<ActionResult<Driver>> CreateDriver(Driver driver)
    {
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDriverById), new { id = driver.Id }, driver);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDriver(int id, Driver driver)
    {
        if (id != driver.Id)
        {
            return BadRequest();
        }

        _context.Entry(driver).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DriverExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleterDriver(int id)
    {
        var driver = await _context.Drivers.FindAsync(id);

        if (driver == null)
        {
            return NotFound();
        }

        _context.Drivers.Remove(driver);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DriverExists(int id)
    {
        return _context.Drivers.Any(e => e.Id == id);
    }

}
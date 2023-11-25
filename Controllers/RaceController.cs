using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace webutvikling.Controllers;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class RaceController : ControllerBase
{
    private readonly RaceDbContext _context;

    public RaceController(RaceDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Race>>> GetRaces()
    {
        return await _context.Races.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Race>> GetRace(int id)
    {
        var race = await _context.Races.FindAsync(id);

        if (race == null)
        {
            return NotFound();
        }

        return race;
    }

    [HttpPost]
    public async Task<ActionResult<Race>> CreateRace(Race race)
    {
        _context.Races.Add(race);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRace), new { id = race.Id }, race);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutRace(int id, Race race)
    {
        if (id != race.Id)
        {
            return BadRequest();
        }

        _context.Entry(race).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!RaceExists(id))
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
    public async Task<IActionResult> DeleterRace(int id)
    {
        var race = await _context.Races.FindAsync(id);

        if (race == null)
        {
            return NotFound();
        }

        _context.Races.Remove(race);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool RaceExists(int id)
    {
        return _context.Races.Any(e => e.Id == id);
    }

}
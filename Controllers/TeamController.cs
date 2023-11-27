using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace webutvikling.Controllers;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class TeamController : ControllerBase
{
    private readonly TeamDbContext _context;

    public TeamController(TeamDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Team>>> GetAllTeams()
    {
        return await _context.Teams.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Team>> GetTeamById(int id)
    {
        var team = await _context.Teams.FindAsync(id);

        if (team == null)
        {
            return NotFound();
        }

        return team;
    }

    [HttpPost]
    public async Task<ActionResult<Team>> CreateTeam([FromForm] TeamWithImage teamWithImage)
    {
        if (teamWithImage.CarImage == null || teamWithImage.CarImage.Length == 0)
        {
            return BadRequest("Invalid image");
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(teamWithImage.CarImage.FileName);
        var imagePath = Path.Combine("wwwroot/images", fileName);

        using (var stream = new FileStream(imagePath, FileMode.Create))
        {
            await teamWithImage.CarImage.CopyToAsync(stream);
        }

        var team = new Team
        {
            Manufacturer = teamWithImage.Manufacturer,
            Driver1 = teamWithImage.Driver1,
            Driver2 = teamWithImage.Driver2,
            Image = fileName
        };

        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTeamById), new { id = team.Id }, team);
    }

    // [HttpPost]
    // public async Task<ActionResult<Team>> CreateTeam(Team team)
    // {
    //     _context.Teams.Add(team);
    //     await _context.SaveChangesAsync();

    //     return CreatedAtAction(nameof(GetTeamById), new { id = team.Id }, team);
    // }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTeam(int id, Team team)
    {
        if (id != team.Id)
        {
            return BadRequest();
        }

        _context.Entry(team).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TeamExists(id))
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
    public async Task<IActionResult> DeleterTeam(int id)
    {
        var team = await _context.Teams.FindAsync(id);

        if (team == null)
        {
            return NotFound();
        }


        _context.Teams.Remove(team);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TeamExists(int id)
    {
        return _context.Teams.Any(e => e.Id == id);
    }

}
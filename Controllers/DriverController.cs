using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace webutvikling.Controllers;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using StackExchange.Redis;
using Newtonsoft.Json;

[ApiController]
[Route("[controller]")]
public class DriverController : ControllerBase
{
    private readonly DriverDbContext _context;
    private readonly IDatabase _redisDatabase;

    public DriverController(DriverDbContext context)
    {
        _context = context;
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        _redisDatabase = redis.GetDatabase();
    }
    [HttpPost]
    public async Task<ActionResult<Driver>> CreateDriver([FromForm] DriverWithImage driverWithImage)
    {
        if (driverWithImage.Image == null || driverWithImage.Image.Length == 0)
        {
            return BadRequest("Invalid image");
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(driverWithImage.Image.FileName);
        var imagePath = Path.Combine("wwwroot/images", fileName);
        // send image path using a message broker to a python consumer to help convert image to black and white using pillow 
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "image_processor", durable: false, exclusive: false, autoDelete: false, arguments: null);

            string message = imagePath.ToString();
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "image_processor", basicProperties: null, body: body);
            Console.WriteLine($" [x] Sent {message}");
        }



        using (var stream = new FileStream(imagePath, FileMode.Create))
        {
            await driverWithImage.Image.CopyToAsync(stream);
        }


        var driver = new Driver
        {
            Name = driverWithImage.Name,
            Age = driverWithImage.Age,
            Nationality = driverWithImage.Nationality,
            Image = fileName
        };

        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        var driverJson = JsonConvert.SerializeObject(driver);

        // Save the serialized driver object to Redis cache with a key based on the driver's ID
        await _redisDatabase.StringSetAsync($"driver:{driver.Id}", driverJson);

        return CreatedAtAction(nameof(GetDriverById), new { id = driver.Id }, driver);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Driver>>> GetAllDrivers()
    {
        return await _context.Drivers.ToListAsync();
    }

    // [HttpGet("{id}")]
    // public async Task<ActionResult<Driver>> GetDriverById(int id)
    // {
    //     // Check if the driver exists in the cache
    //     string driverJson = await _redisDatabase.StringGetAsync($"driver:{id}");
    //     if (driverJson == null)
    //     {

    //         // If the driver is not found in the cache, return NotFound
    //     }
    //     var driver = await _context.Drivers.FindAsync(id);
    //     if (driver == null)
    //     {
    //         return NotFound();
    //     }

    //     return driver;
    //      Driver driver = JsonConvert.DeserializeObject<Driver>(driverJson);

    // }
    [HttpGet("{id}")]
    public async Task<ActionResult<Driver>> GetDriverById(int id)
    {
       
        // Check if the driver exists in the cache
        string driverJson = await _redisDatabase.StringGetAsync($"driver:{id}");
        Console.WriteLine(driverJson);
        if (driverJson != null)
        {
            // If the driver is found in the cache, deserialize it and return
            Driver driver = JsonConvert.DeserializeObject<Driver>(driverJson);
            Console.WriteLine("found in database");
            return driver;
        }
        else
        {
            // If the driver is not found in the cache, retrieve it from the database
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
            {
                // If the driver is not found in the database, return NotFound
                return NotFound();
            }

            // Serialize the driver object
            driverJson = JsonConvert.SerializeObject(driver);

            // Save the serialized driver object to Redis cache with a key based on the driver's ID
            await _redisDatabase.StringSetAsync($"driver:{id}", driverJson);

            // Return the driver
            return driver;
        }
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



    // [HttpPost]
    // public async Task<ActionResult<Driver>> CreateDriver(Driver driver)
    // {
    //     _context.Drivers.Add(driver);
    //     await _context.SaveChangesAsync();

    //     return CreatedAtAction(nameof(GetDriverById), new { id = driver.Id }, driver);
    // }

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
    public async Task<IActionResult> DeleteDriver(int id)
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
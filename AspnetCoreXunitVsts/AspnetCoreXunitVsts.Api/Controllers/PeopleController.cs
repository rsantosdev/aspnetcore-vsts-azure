using System.Net;
using System.Threading.Tasks;
using AspnetCoreXunitVsts.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspnetCoreXunitVsts.Api.Controllers
{
    [Route("api/people")]
    public class PeopleController : Controller
    {
        private readonly ApiContext _context;

        public PeopleController(ApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPeople()
        {
            var people = await _context.People.ToListAsync();
            return Json(people);
        }

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> GetPerson(int id)
        {
            var person = await _context.People.FirstOrDefaultAsync(x => x.Id == id);

            if (person == null)
            {
                return NotFound();
            }

            return Json(person);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromBody]Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.People.Add(person);
            await _context.SaveChangesAsync();

            return StatusCode((int)HttpStatusCode.Created, person);
        }

        [HttpPut, Route("{id}")]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody]Person person)
        {
            var dbPerson = await _context.People.FirstOrDefaultAsync(p => p.Id == id);
            if (dbPerson == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            dbPerson.Name = person.Name;
            await _context.SaveChangesAsync();

            return Ok(dbPerson);
        }
    }
}

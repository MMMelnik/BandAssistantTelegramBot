using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BandAssistantBot.Models;
using BandAssistantBot.Services;

namespace BandAssistantBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RehearsalsController : ControllerBase
    {
        private readonly RehearsalService _rehearsalService;

        public RehearsalsController(RehearsalService rehearsalService)
        {
            _rehearsalService = rehearsalService;
        }

        [HttpGet]
        public ActionResult<List<Rehearsal>> Get() =>
            _rehearsalService.Get();

        [HttpGet("{id:length(24)}", Name = "GetRehearsal")]
        public ActionResult<Rehearsal> Get(string id)
        {
            var rehearsal = _rehearsalService.Get(id);

            if (rehearsal == null)
            {
                return NotFound();
            }

            return rehearsal;
        }

        [HttpPost]
        public ActionResult<Rehearsal> Create(Rehearsal rehearsal)
        {
            _rehearsalService.Create(rehearsal);

            return CreatedAtRoute("GetRehearsal", new { id = rehearsal.Id }, rehearsal);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Rehearsal rehearsalIn)
        {
            var rehearsal = _rehearsalService.Get(id);

            if (rehearsal == null)
            {
                return NotFound();
            }

            _rehearsalService.Update(id, rehearsalIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var rehearsal = _rehearsalService.Get(id);

            if (rehearsal == null)
            {
                return NotFound();
            }

            _rehearsalService.Remove(rehearsal.Id);

            return NoContent();
        }
    }
}
// izlaboju ieteikumus, tagad var pievienot ari biezumu ka dropdown
// izmantoju gemini, lai sparastu uzdevumu logiku un ka to izdarit, nekad api nebiju taisijis

using Microsoft.AspNetCore.Mvc;
using HabitTracker.Domain;
using HabitTracker.Domain.Repositories;

namespace HabitTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HabitsController : ControllerBase
    {
        private readonly IHabitRepository _repository;
        public HabitsController(IHabitRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Habit>> GetAll() // visu ieradumu saraksts
        {
            return Ok(_repository.GetAllHabits());
        }

        [HttpGet("{id}")]  // iegust pec id
        public ActionResult<Habit> GetById(Guid id)
        {
            var habit = _repository.GetHabitById(id);

            if (habit == null) return NotFound("Ieradums nav atrasts.");

            return Ok(habit);
        }

        [HttpPost] // pievieno jaunu
        public ActionResult AddHabit(Habit habit) // pievienosana
        {
            if (habit.ID == Guid.Empty)
            {
                habit.ID = Guid.NewGuid();
            }

            _repository.AddHabit(habit);

            return CreatedAtAction(nameof(GetById), new { id = habit.ID }, habit);
        }

        [HttpPut("{id}")] //izlabo
        public ActionResult UpdateHabit(Guid id, Habit updatedHabit) // labosana
        {
            var existingHabit = _repository.GetHabitById(id);
            if (existingHabit == null) return NotFound("Ieradums nav atrasts.");

            updatedHabit.ID = id;
            _repository.UpdateHabit(updatedHabit);

            return NoContent();
        }
         
        [HttpDelete("{id}")]
        public ActionResult ArchiveHabit(Guid id) // arhivesana
        {
            var existingHabit = _repository.GetHabitById(id);
            if (existingHabit == null) return NotFound("Ieradums nav atrasts.");

            _repository.DeleteOrArchiveHabit(id);
            return NoContent();
        }

        [HttpPost("{id}/complete")]
        public ActionResult CompleteHabit(Guid id) // atzime ka izpilditu
        {
            var habit = _repository.GetHabitById(id);
            if (habit == null) return NotFound("Ieradums nav atrasts.");

            if (habit.IsArchived) return BadRequest("Arhivētu ieradumu nevar atzīmēt kā izpildītu.");

            if (!habit.IsCompletedToday())
            {
                habit.Record.Add(new HabitTracker.Domain.HabitRecord { CompletionDate = DateTime.Today });
                _repository.UpdateHabit(habit);
            }

            return Ok(habit);
        }
    }
}
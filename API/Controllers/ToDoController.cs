using Common.Contracts;
using Common.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ToDoController : Controller
    {
        private readonly IToDoRepository repo;

        public ToDoController(IToDoRepository repo)
        {
            this.repo = repo;
        }

        /// <summary>
        /// Zwraca listę wszystkich zadań
        /// </summary>
        /// <param name="HideDeleted">Ukryj zadania usunięte</param>
        /// <remarks>!!! Nie dotykać tego w niedziele i święta</remarks>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpGet("Tasks")]
        [Obsolete("Don't use that!!")]
        public async Task<ActionResult<IEnumerable<TodoTask>>> GetTasksAsync(
            [FromQuery] bool? HideDeleted
            )
        {
            try
            {
                if (!HideDeleted.HasValue)
                {
                    throw new ArgumentException(nameof(HideDeleted));

                    //return BadRequest(nameof(HideDeleted));
                }

                var data = await repo.GetTasksAsync(!HideDeleted.Value);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Pobiera ToDoTask o podanym ID z bazy danych 
        /// </summary>
        /// <param name="TaskId">Id ToDoTask</param>
        /// <returns></returns>
        [HttpGet("Task")]
        public async Task<ActionResult> StoreTasksAsync(
            [FromQuery] int TaskId
        )
        {
            try
            {
                var data = await repo.GetTasksAsync(true);

                return Ok(data?.FirstOrDefault(x => x.Id == TaskId));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Zapisuje nowy item ToDoTask do bazy danych
        /// </summary>
        /// <param name="NewTaskItem">Nowy item</param>
        /// <returns></returns>
        [HttpPost("Task")]
        public async Task<ActionResult> StoreTasksAsync(
            [FromBody] TodoTask NewTaskItem
        )
        {
            try
            {
                var licznik = await repo.StoreTaskAsync(NewTaskItem);

                return Created();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}

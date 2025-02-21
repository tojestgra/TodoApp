using Common.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Contracts
{
    public interface IToDoRepository
    {
        /// <summary>
        /// Zapisuje nowy item ToDoTask do bazy danych
        /// </summary>
        /// <param name="show_deleted"></param>
        /// <returns></returns>
        Task<IEnumerable<TodoTask>> GetTasksAsync(bool show_deleted);

        Task<int> StoreTaskAsync(TodoTask task);
    }
}

using API.Context;
using Common.Contracts;
using Common.DTO;
using Microsoft.AspNetCore.Hosting.Server;

namespace API.Repositories
{
    public class ToDoRepository : IToDoRepository
    {
        private readonly DapperContext context;

        public ToDoRepository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<TodoTask>> GetTasksAsync(bool show_deleted)
        {
            string where = " where is_deleted=0";

            if (show_deleted) where = "";


            string query = $"select id, title, description, is_completed, is_deleted from tbl_tasks {where}";

            return await context.QueryAsync<TodoTask>(query);
        }

        public async Task<int> StoreTaskAsync(TodoTask task)
        {
            string q = $"INSERT INTO [dbo].[tbl_tasks]([title],[description],[is_completed]) VALUES(@Title, @Description, @IsCompleted)";

            return await context.ExecuteAsync(q, task);
        }
    }
}

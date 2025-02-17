using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Common.DTO
{
    public class TodoList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<TodoTask> Tasks { get; set; } = new ObservableCollection<TodoTask>();
    }
}

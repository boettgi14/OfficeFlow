using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeFlow
{
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }

        public Task(int id, string name, bool isCompleted, string? description, DateTime? dueDate)
        {
            Id = id;
            Name = name;
            IsCompleted = isCompleted;
            Description = description;
            DueDate = dueDate;
        }

        public override string ToString()
        {
            return $"{Id} {Name} {(IsCompleted ? "(Completed)" : "")} {Description ?? "No Description"} {(DueDate.HasValue ? DueDate.Value.ToShortDateString() : "No Due Date")}";
        }
    }
}
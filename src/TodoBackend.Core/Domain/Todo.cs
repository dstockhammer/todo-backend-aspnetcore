namespace TodoBackend.Core.Domain
{
    public class Todo
    {
        public int Id { get; }
        public string Title { get; private set; }
        public bool Completed { get; private set; }
        public int? Order { get; private set; }

        public Todo(int id, string title, bool completed, int? order)
        {
            Id = id;
            Title = title;
            Completed = completed;
            Order = order;
        }

        public void Update(string title, bool completed, int? order)
        {
            Title = title;
            Completed = completed;
            Order = order;
        }
    }
}
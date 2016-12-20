namespace TodoBackend.Core.Domain
{
    public class Todo : IEntity
    {
        // todo
        private int SequenceId { get; set; }

        public int Id { get; private set; }
        public string Title { get; private set; }
        public bool Completed { get; private set; }
        public int? Order { get; private set; }

        // todo
        private Todo()
        {
        }

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
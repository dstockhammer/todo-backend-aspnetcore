namespace TodoBackend.Api.Views
{
    public sealed class TodoView
    {
        public int Id { get; set; }
        public int? Order { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public bool Completed { get; set; }
    }
}
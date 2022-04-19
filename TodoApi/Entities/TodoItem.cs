namespace TodoApi.Entities
{
    public class TodoItem : IEntity
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }
        public DateTime? Creation { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TodoApi.Entities;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoItemsController : ODataController<TodoItem>
    {
        public TodoItemsController(TodoContext context) : base(context)
        {
        }

        //overrides GetOData
        // GET: api/TodoItems
        //[HttpGet]
        private IQueryable GetTodoItems(int skip, int take, string orderByColumn, bool asc)
        {
            var todoItems = DbSet
                .Where(e => e.IsComplete)
                .Skip(skip)
                .Take(take);

            if(orderByColumn != null)
            {
                todoItems = asc ? todoItems.OrderBy(e => orderByColumn) : todoItems.OrderByDescending(e => orderByColumn);
            }

            todoItems.Select(e => new
                {
                    e.Name,
                    e.IsComplete,
                    e.Creation
                });
            return todoItems;
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await DbSet.FindAsync(id);

            if (todoItem == null)
            {
                var errors = new List<ValidationFailure>();
                errors.Add(new ValidationFailure("prop1", "error1"));

                throw new FluentValidation.ValidationException("TodoItem não encontrado.", errors);
            }

            return todoItem;
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            todoItem.Id = id;
            TodoContext.Update(todoItem);

            try
            {
                await TodoContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    throw new ValidationException("TodoItem não existe.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            DbSet.Add(todoItem);
            await TodoContext.SaveChangesAsync();

            //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            throw new Exception("Ocorreu um erro na aplicação.");//Apenas para testar o ProblemDetails

            var todoItem = await DbSet.FindAsync(id);
            if (todoItem == null)
            {
                throw new ValidationException("TodoItem não encontrado.");
            }

            DbSet.Remove(todoItem);
            await TodoContext.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return DbSet.Any(e => e.Id == id);
        }
    }
}

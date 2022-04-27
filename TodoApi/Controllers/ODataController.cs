using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Query;
using TodoApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Controllers
{
    public abstract class ODataController<TEntity> : ODataController where TEntity : class, IEntity
    {
        protected readonly TodoContext TodoContext;
        protected readonly DbSet<TEntity> DbSet;

        protected ODataController(TodoContext todoContext)
        {
            TodoContext = todoContext;
            DbSet = todoContext.Set<TEntity>();
        }

        [EnableQuery]
        public IEnumerable<TEntity> Get()//Deve ser Get usando EdmModel 
        {
            return DbSet;
        }
    }
}


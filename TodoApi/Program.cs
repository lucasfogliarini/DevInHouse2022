using Hellang.Middleware.ProblemDetails;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using TodoApi.Entities;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder);
var app = builder.Build();
Run(app);

static void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services
        .AddControllers()
        .AddOData(opt =>
        {
            //uso simples
            opt.EnableQueryFeatures(50);
            //o mesmo que: opt.Select().OrderBy().Filter().Expand().SetMaxTop(50).Count();

            //uso avançado:
            //- Contagem (count=true)
            //- Computação (compute=Price mul Quantity)
            //- Prefixo da rota ("odata")
            //- Customizações do EntitySet
            IEdmModel edmModel = GetEdmModel();
            opt.AddRouteComponents("odata", edmModel);
            //opt.AddRouteComponentsODataControllers();
            opt.RouteOptions.EnableControllerNameCaseInsensitive = true;//desabilita o case sensitive do entityset
        });

    builder.Services.AddDbContext<TodoContext>(opt =>
        opt.UseInMemoryDatabase("Todo"));
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddProblemDetails(x =>
    {
        x.MapToStatusCode<ValidationException>(StatusCodes.Status400BadRequest);
        x.IncludeExceptionDetails = (ctx, ex) =>
        {
            return true;
        };
    });
}

static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    //EntitySet name deve ter o prefixo do nome da controller, para 'TodoItemsContoller':
    //- pode ser 'todoitems' se EnableControllerNameCaseInsensitive for true
    //- deve ser 'TodoItems' se EnableControllerNameCaseInsensitive for false (padrão)
    builder.EntitySet<TodoItem>("todoitems");
    return builder.GetEdmModel();
}

static void Run(WebApplication app)
{
    Seed(app);
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseProblemDetails();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}

static void Seed(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var todoContext = scope.ServiceProvider.GetService<TodoContext>();

    for (int i = 1; i <= 10; i++)
    {;
        var todoItem = new TodoItem
        {
            Name = "TodoItem" + i,
            IsComplete = Convert.ToBoolean(Random.Shared.Next(0, 2)),
            Creation = DateTime.Now,
            Price = Random.Shared.Next(50),
            Quantity = Random.Shared.Next(1,5)
        };
        todoContext.Add(todoItem);
    }

    todoContext.SaveChanges();
}

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Reflection;
using TodoApi.Controllers;

namespace Microsoft.AspNetCore.OData
{
    public static class ODataExtensions
    {
        public static void AddRouteComponentsODataControllers(this ODataOptions oDataOptions, string routePrefix = "odata")
        {
            oDataOptions.AddRouteComponents(routePrefix, GetEdmModelODataControllers());
        }

        static IEdmModel GetEdmModelODataControllers()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EnableLowerCamelCase();
            var odataControllers = Assembly.GetExecutingAssembly().GetTypes().Where(e => e.BaseType?.Name == typeof(ODataController<>).Name);
            foreach (var odataController in odataControllers)
            {
                var entityType = odataController.BaseType?.GenericTypeArguments.FirstOrDefault();
                EntitySet(builder, entityType, odataController.Name);
            }
            var edmModel = builder.GetEdmModel();
            return edmModel;
        }

        static void EntitySet(ODataConventionModelBuilder builder, Type? entitySetType, string controllerName)
        {
            var entitySetName = controllerName.Replace("Controller", "");
            typeof(ODataConventionModelBuilder)
                .GetMethod(nameof(ODataConventionModelBuilder.EntitySet))
                .MakeGenericMethod(entitySetType).Invoke(builder, new[] { entitySetName });
        }
    }
}
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq.Expressions;

namespace MiniTools.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        //public static HtmlString DescriptionFor<TModel, TValue>(
        //    this IHtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        //{

        //    var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
        //    var description = metadata.Description;

        //    // fallback! We'll try to show something anyway.
        //    if (String.IsNullOrEmpty(description)) description = String.IsNullOrEmpty(metadata.DisplayName) ? metadata.PropertyName : metadata.DisplayName;

        //    return HtmlString.Create(description);
        //}

        //public static HtmlString DescriptionFor<TModel, TValue>(
        //    this IHtmlHelper<TModel> self, Expression<Func<TModel, TValue>> expression)
        //{
        //    var metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);
        //    var description = metadata.Description;

        //    return HtmlString.Create(string.Format(@"<span>{0}</span>", description));
        //}

        //private static IHtmlContent MetaDataFor<TModel, TValue>(
        //    this IHtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression,
        //    Func<ModelMetadata, string> property)
        //{
        //    if (html == null) throw new ArgumentNullException(nameof(html));
        //    if (expression == null) throw new ArgumentNullException(nameof(expression));


        //    ModelExpressionProvider modelExpressionProvider = 
        //        (ModelExpressionProvider)html.ViewContext.HttpContext.RequestServices.GetService(typeof(IModelExpressionProvider));

        //    var modelExplorer = modelExpressionProvider.CreateModelExpression(html.ViewData, expression);
        //    if (modelExplorer == null) throw new InvalidOperationException($"Failed to get model explorer for {modelExpressionProvider.GetExpressionText(expression)}");

        //    return new HtmlString(property(modelExplorer.Metadata));
        //}

        // https://github.com/dotnet/aspnetcore/blob/main/src/Mvc/Mvc.ViewFeatures/src/Rendering/HtmlHelperDisplayNameExtensions.cs
        // https://coderedirect.com/questions/442355/asp-net-core-3-0-shortname-in-the-display-attribute-dataannotations

        // https://stackoverflow.com/questions/41003192/how-can-i-use-tag-helpers-to-get-a-model-propertys-name-on-an-arbitrary-element

    }
}

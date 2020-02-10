using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace System.Web.Mvc
{
    /// <summary>
    /// Provides useful extension methods for custom markup generation
    /// </summary>
    public static class HtmlInputExtensions
    {

        /// <summary>
        /// Gets the validation attributes.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="excludeParameters">The exclude parameters.</param>
        /// <returns></returns>
        public static string GetValidationAttributes(this HtmlHelper htmlHelper, ModelMetadata metadata, params string[] excludeParameters)
        {
            #region Validate Arguments
            if (htmlHelper == null) throw new ArgumentNullException("htmlHelper");
            if (metadata == null) throw new ArgumentNullException("metadata");
            if (excludeParameters == null) throw new ArgumentNullException("excludeParameters"); 
            #endregion
            var name = metadata.PropertyName;
            var attributes = htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata);
            foreach (var key in excludeParameters)
                attributes.Remove(key);
            return RenderAttributes(attributes);
        }

        /// <summary>
        /// Gets the validation attributes dictionary.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="excludeParameters">The exclude parameters.</param>
        /// <returns></returns>
        public static IDictionary<string, object> GetValidationAttributesDictionary(this HtmlHelper htmlHelper, ModelMetadata metadata, params string[] excludeParameters)
        {
            #region Validate Arguments
            if (htmlHelper == null) throw new ArgumentNullException("htmlHelper");
            if (metadata == null) throw new ArgumentNullException("metadata");
            if (excludeParameters == null) throw new ArgumentNullException("excludeParameters"); 
            #endregion
            var name = metadata.PropertyName;
            var attributes = htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata);
            foreach (var key in excludeParameters)
                attributes.Remove(key);
            return attributes;

        }

        /// <summary>
        /// Gets the validation attributes.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="excludeParameters">The exclude parameters.</param>
        /// <returns></returns>
        public static string GetValidationAttributes<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, params string[] excludeParameters)
        {
            #region Validate Arguments
            if (htmlHelper == null) throw new ArgumentNullException("htmlHelper");
            if (expression == null) throw new ArgumentNullException("expression");
            if (excludeParameters == null) throw new ArgumentNullException("excludeParameters"); 
            #endregion
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            var name = ExpressionHelper.GetExpressionText(expression);
            var attributes = htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata);
            foreach (var key in excludeParameters)
                attributes.Remove(key);
            return RenderAttributes(attributes);

        }

        /// <summary>
        /// Gets the validation attributes.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="excludeParameters">The exclude parameters.</param>
        /// <returns></returns>
        public static IDictionary<string, object> GetValidationAttributesDictionary<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, params string[] excludeParameters)
        {
            #region Validate Arguments
            if (htmlHelper == null) throw new ArgumentNullException("htmlHelper");
            if (expression == null) throw new ArgumentNullException("expression");
            if (excludeParameters == null) throw new ArgumentNullException("excludeParameters");
            #endregion
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            var name = ExpressionHelper.GetExpressionText(expression);
            var attributes = htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata);
            foreach (var key in excludeParameters)
                attributes.Remove(key);
            return attributes;
        }

        private static string RenderAttributes(IDictionary<string, object> attributes)
        {
            var sb = new StringBuilder();
            if (attributes != null)
            {
                foreach (var key in attributes.Keys)
                {
                    string value = attributes[key] as string;
                    if (!string.Equals(key, "id", StringComparison.Ordinal) || !string.IsNullOrEmpty(value))
                    {
                        string str2 = HttpUtility.HtmlAttributeEncode(value);
                        sb.Append(key).Append("=\"").Append(str2).Append('"').Append(' ');
                    }
                }
            }
            return sb.ToString();
        }

    }
}

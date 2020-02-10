using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace System.Web.Mvc
{
    /// <summary>
    /// Helper extensions methods for ValidationGroups
    /// </summary>
    /// <remarks>This code is based heavily on the Mvc3 implementation to maintain as much of the original functionality as possible (everything AFAIK).</remarks>
    public static class ValidationExtensions
    {

        /// <summary>
        /// Validations the summary.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="excludePropertyErrors">if set to <c>true</c> [exclude property errors].</param>
        /// <returns></returns>
        public static MvcHtmlString ValidationSummary(this HtmlHelper htmlHelper, string groupName, bool excludePropertyErrors)
        {
            return htmlHelper.ValidationSummary(groupName, excludePropertyErrors, string.Empty);
        }

        /// <summary>
        /// Validations the summary.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="excludePropertyErrors">if set to <c>true</c> [exclude property errors].</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static MvcHtmlString ValidationSummary(this HtmlHelper htmlHelper, string groupName, bool excludePropertyErrors, string message)
        {
            return htmlHelper.ValidationSummary(groupName, excludePropertyErrors, message, null);
        }

        /// <summary>
        /// Validations the summary.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="excludePropertyErrors">if set to <c>true</c> [exclude property errors].</param>
        /// <param name="message">The message.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public static MvcHtmlString ValidationSummary(this HtmlHelper htmlHelper, string groupName, bool excludePropertyErrors, string message, IDictionary<string, object> htmlAttributes)
        {
            #region Validate Arguments
            if (htmlHelper == null) throw new ArgumentNullException("htmlHelper");
            groupName = groupName ?? string.Empty;
            #endregion

            string[] groupNames = groupName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();

            if (htmlAttributes == null) htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(null);
            htmlAttributes["data-val-valgroup-name"] = groupName;



            object model = htmlHelper.ViewData.Model;
            ModelStateDictionary modelState = htmlHelper.ViewData.ModelState;

            bool canValidate = !modelState.ContainsKey(ValidationGroupsKey)
                || (groupNames.Any() && ((IEnumerable<string>)modelState[ValidationGroupsKey].Value.RawValue).Intersect(groupNames).Any())
                || (!groupNames.Any());
            IEnumerable<ModelState> faulted = canValidate ? GetFaultedModelStates(htmlHelper, groupNames, model) : Enumerable.Empty<ModelState>();
            FormContext formContextForClientValidation = htmlHelper.ViewContext.ClientValidationEnabled ? htmlHelper.ViewContext.FormContext : null;
            if (!canValidate || !faulted.Any())
            {
                if (formContextForClientValidation == null)
                {
                    return null;
                }
            }
            string str = InitializeResponseString(message);

            IEnumerable<ModelState> values = GetValues(htmlHelper, excludePropertyErrors, modelState, faulted);
            TagBuilder builder3 = AddValidationMessages(htmlHelper, values);
            TagBuilder tagBuilder = new TagBuilder("div");
            tagBuilder.MergeAttributes<string, object>(htmlAttributes);
            tagBuilder.AddCssClass("validation-summary");
            tagBuilder.AddCssClass(!faulted.Any() ? HtmlHelper.ValidationSummaryValidCssClassName : HtmlHelper.ValidationSummaryCssClassName);
            tagBuilder.InnerHtml = str + builder3.ToString(TagRenderMode.Normal);
            if (formContextForClientValidation != null)
            {
                if (htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
                {
                    if (!excludePropertyErrors)
                    {
                        tagBuilder.MergeAttribute("data-valmsg-summary", "true");
                    }
                }
                else
                {
                    tagBuilder.GenerateId("validationSummary");
                    formContextForClientValidation.ValidationSummaryId = tagBuilder.Attributes["id"];
                    formContextForClientValidation.ReplaceValidationSummary = !excludePropertyErrors;
                }
            }
            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));

        }

        #region Validation Methods Based (Heavily) on MVC3 Implementation

        private static TagBuilder AddValidationMessages(HtmlHelper htmlHelper, IEnumerable<ModelState> values)
        {
            StringBuilder builder2 = new StringBuilder();
            TagBuilder builder3 = new TagBuilder("ul");
            if (values != null)
            {

                foreach (var msg in values.SelectMany(v => v.Errors.Select(e => GetUserErrorMessageOrDefault(htmlHelper.ViewContext.HttpContext, e, null))
                    .Where(m => !string.IsNullOrEmpty(m)))
                    .Distinct())
                {

                    TagBuilder builder4 = new TagBuilder("li");
                    builder4.SetInnerText(msg);
                    builder2.AppendLine(builder4.ToString(TagRenderMode.Normal));

                }

            }
            if (builder2.Length == 0)
            {
                builder2.AppendLine("<li style=\"display:none\"></li>");
            }
            builder3.InnerHtml = builder2.ToString();
            return builder3;
        }

        private static IEnumerable<ModelState> GetValues(HtmlHelper htmlHelper, bool excludePropertyErrors, ModelStateDictionary modelState, IEnumerable<ModelState> faulted)
        {
            IEnumerable<ModelState> values = null;
            if (excludePropertyErrors)
            {
                ModelState state;
                modelState.TryGetValue(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix, out state);
                if (state != null)
                {
                    values = new ModelState[] { state };
                }
            }
            else
            {
                values = faulted;
            }
            return values;
        }

        private static string InitializeResponseString(string message)
        {
            string str;
            if (!string.IsNullOrEmpty(message))
            {
                TagBuilder builder = new TagBuilder("span");
                builder.SetInnerText(message);
                str = builder.ToString(TagRenderMode.Normal) + Environment.NewLine;
            }
            else
            {
                str = null;
            }
            return str;
        }

        private static IEnumerable<ModelState> GetFaultedModelStates(HtmlHelper htmlHelper, string[] groupNames, object model)
        {
            return GetFaultedModelStates(htmlHelper.ViewData.ModelState, groupNames, model);
        }
        private static IEnumerable<ModelState> GetFaultedModelStates(ModelStateDictionary modelState, string[] groupNames, object model)
        {
            var props = GetPropertyMap(string.Empty, model, new List<object>());
            IEnumerable<string> validatedPropertyNames = props
                .Where(pdm => pdm.Property.Attributes.OfType<ValidationGroupAttribute>()
                    .Where(vg => groupNames.Intersect(vg.GroupNames, StringComparer.OrdinalIgnoreCase).Any()).Any()
                ).Select(pd => pd.Key).ToArray();
            IEnumerable<ModelState> faulted = modelState.Where(ms => validatedPropertyNames.Contains(ms.Key) && !modelState.IsValidField(ms.Key)).Select(ms => ms.Value).ToArray();
            return faulted;
        }

        private class PropertyMap
        {
            public string Key { get; set; }
            public PropertyDescriptor Property { get; set; }
        }

        private static IEnumerable<PropertyMap> GetPropertyMap(string prefix, object model, List<object> parents)
        {
            List<object> pars = new List<object>(parents);
            pars.Add(model);
            var props = TypeDescriptor.GetProperties(model).OfType<PropertyDescriptor>()
                .Select(pd => new PropertyMap { Key = prefix + pd.Name, Property = pd });
            List<PropertyMap> propMaps = new List<PropertyMap>(props);
            foreach (var prop in props.Where(p => p.Property.PropertyType.IsClass || p.Property.PropertyType.IsArray)
                .Where(p => !p.Property.PropertyType.FullName.StartsWith("System.", StringComparison.OrdinalIgnoreCase))
                .Where(p => !p.Property.PropertyType.FullName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase)))
            {
                if (prop.Property.PropertyType.IsArray)
                {
                    Array arr = (Array)prop.Property.GetValue(model);
                    if (arr == null) continue;
                    for (int i = 0; i < arr.Length; i++)
                    {
                        var ele = arr.GetValue(i);
                        if (ele == null) continue;
                        if (pars.Contains(ele)) continue;
                        var elementPrefix = string.Format(CultureInfo.InvariantCulture, "{0}{1}[{2}].", prefix, prop.Property.Name, i);
                        propMaps.AddRange(GetPropertyMap(elementPrefix, ele, pars));
                    }
                }
                else
                {
                    var val = prop.Property.GetValue(model);
                    if (val == null) continue;
                    if (pars.Contains(val)) continue;
                    var elementPrefix = string.Format(CultureInfo.InvariantCulture, "{0}{1}.", prefix, prop.Property.Name);
                    propMaps.AddRange(GetPropertyMap(elementPrefix, val, pars));
                }
            }
            return propMaps;

        }

        /// <summary>
        /// Validations the summary.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="excludePropertyErrors">if set to <c>true</c> [exclude property errors].</param>
        /// <param name="message">The message.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public static MvcHtmlString ValidationSummary(this HtmlHelper htmlHelper, string groupName, bool excludePropertyErrors, string message, object htmlAttributes)
        {
            return htmlHelper.ValidationSummary(groupName, excludePropertyErrors, message, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }


        #region Extracted ValidationSummary Helper Methods

        private static string GetUserErrorMessageOrDefault(HttpContextBase httpContext, ModelError error, ModelState modelState)
        {
            if (!string.IsNullOrEmpty(error.ErrorMessage))
            {
                return error.ErrorMessage;
            }
            if (modelState == null)
            {
                return null;
            }
            string str = (modelState.Value != null) ? modelState.Value.AttemptedValue : null;
            return string.Format(CultureInfo.CurrentCulture, GetInvalidPropertyValueResource(httpContext), new object[] { str });
        }

        private static string GetInvalidPropertyValueResource(HttpContextBase httpContext)
        {
            string str = null;
            if (!string.IsNullOrEmpty(ResourceClassKey) && (httpContext != null))
            {
                str = httpContext.GetGlobalResourceObject(ResourceClassKey, "InvalidPropertyValue", CultureInfo.CurrentUICulture) as string;
            }
            return (str ?? Common_ValueNotValidForProperty);
        }

        #endregion

        #region Extracted Required Methods from MvcResources

        internal static string Common_ValueNotValidForProperty
        {
            get
            {
                return ResourceManager.GetString("Common_ValueNotValidForProperty", CultureInfo.CurrentUICulture);
            }
        }


        /// <summary>
        /// Gets the resource class key.
        /// </summary>
        public static string ResourceClassKey
        {
            get
            {
                return ValidationExtensions.ResourceClassKey;
            }

        }

        private static ResourceManager resourceMan;


        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    ResourceManager manager = new ResourceManager("System.Web.Mvc.Resources.MvcResources", typeof(System.Web.Mvc.Controller).Assembly);
                    resourceMan = manager;
                }
                return resourceMan;
            }
        }

        #endregion

        private static readonly string ValidationGroupsKey = "ValidationGroups_" + Guid.NewGuid().ToString();

        /// <summary>
        /// Determines whether [is group valid] [the specified model state].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modelState">State of the model.</param>
        /// <param name="model">The model.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>
        ///   <c>true</c> if [is group valid] [the specified model state]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsGroupValid<T>(this ModelStateDictionary modelState, T model, string groupName)
        {
            if (modelState == null) throw new ArgumentNullException("modelState");
            if (model == null) throw new ArgumentNullException("model");
            if (string.IsNullOrEmpty(groupName)) return modelState.IsValid;
            return modelState.IsGroupValid(model, groupName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));
        }

        /// <summary>
        /// Determines whether [is group valid] [the specified model state].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modelState">State of the model.</param>
        /// <param name="model">The model.</param>
        /// <param name="groupNames">The group names.</param>
        /// <returns>
        ///   <c>true</c> if [is group valid] [the specified model state]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsGroupValid<T>(this ModelStateDictionary modelState, T model, IEnumerable<string> groupNames)
        {
            if (modelState == null) throw new ArgumentNullException("modelState");
            if (model == null) throw new ArgumentNullException("model");
            if (groupNames == null || !groupNames.Any()) return modelState.IsValid;
            return !GetFaultedModelStates(modelState, groupNames.ToArray(), model).Any();
        }

        private static IEnumerable<PropertyDescriptor> GetProperties(Type valueType, object value)
        {
            IEnumerable<PropertyDescriptor> props = Enumerable.Empty<PropertyDescriptor>();
            if (value == null)
            {
                props = TypeDescriptor.GetProperties(valueType).OfType<PropertyDescriptor>().ToArray();
            }
            else
            {
                props = TypeDescriptor.GetProperties(value).OfType<PropertyDescriptor>().ToArray();
            }
            foreach (var p in props.Where(p => p != null))
            {
                yield return p;
                if (p.PropertyType != null
                    && p.PropertyType.IsClass
                    && !p.PropertyType.Namespace.StartsWith("System", StringComparison.OrdinalIgnoreCase)
                    && !p.PropertyType.Namespace.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var p2 in GetProperties(p.PropertyType, (value == null) ? null : p.GetValue(value)))
                    {
                        yield return p2;
                    }
                }
            }
            yield break;
        }

        #endregion
    }
}

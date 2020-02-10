using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace System.Web.Mvc
{
    /// <summary>
    /// Validation Group Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ValidationGroupAttribute : ValidationAttribute, IClientValidatable
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationGroupAttribute"/> class.
        /// </summary>
        public ValidationGroupAttribute()
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="ValidationGroupAttribute"/> class.
        /// </summary>
        /// <param name="groupName">The group name.</param>
        public ValidationGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string GroupName { get; private set; }

        internal string[] GroupNames
        {
            get
            {
                if (string.IsNullOrEmpty(GroupName)) return new string[0];
                return GroupName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToArray();
            }
        }

        /// <summary>
        /// Determines whether the specified value of the object is valid.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <returns>
        /// true if the specified value is valid; otherwise, false.
        /// </returns>
        public override bool IsValid(object value)
        {
            return true;
        }

        #region IClientValidatable Members

        /// <summary>
        /// When implemented in a class, returns client validation rules for that class.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        /// <param name="context">The controller context.</param>
        /// <returns>
        /// The client validation rules for this validator.
        /// </returns>
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            if (string.IsNullOrEmpty(GroupName)) yield break;
            var rule = new ModelClientValidationRule
            {
                ValidationType = "valgroup"
                ,
                ErrorMessage = string.Empty
            };
            rule.ValidationParameters["name"] = GroupName;
            yield return rule;
        }

        #endregion
    }
}

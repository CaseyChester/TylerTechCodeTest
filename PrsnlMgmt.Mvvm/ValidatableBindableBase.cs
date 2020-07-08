using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PrsnlMgmt.Mvvm
{
    /// <summary>
    /// A base class that can be used by Vew Model classes that need to support
    /// INotifyPropertyChanged AND INotifyDataErrorInfo. 
    /// </summary>
    public class ValidatableBindableBase : BindableBase, INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public bool HasErrors => _errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (!HasErrors) return null;

            if(string.IsNullOrEmpty(propertyName))
            {
                // return all errors
                return _errors.SelectMany(kvp => kvp.Value).ToList();
            }
            else
            {
                if (_errors.ContainsKey(propertyName))
                    return _errors[propertyName];
                else
                    return null;
            }
        }

        /// <summary>
        /// Allows derived classes to directly set/unset validation errors for a property.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="errors">The error messages.</param>
        protected virtual void SetErrors(string propertyName, params string[] errors)
        {
            if (errors != null && errors.Length > 0)
                _errors[propertyName] = errors.ToList();
            else
                _errors.Remove(propertyName);
            RaiseErrorsChanged(propertyName);
        }

        /// <summary>
        /// A method that can be called in property set blocks that will automatically
        /// take care of raising PropertyChanged and performing property validation.
        /// </summary>
        protected override bool SetProperty<T>(ref T currentValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            var changed = base.SetProperty(ref currentValue, newValue, propertyName);
            if (changed) ValidateProperty(propertyName, newValue);
            return changed;
        }

        /// <summary>
        /// Validates all public properties of this instance that are annotated with an
        /// attribute that derives from ValidationAttribute.
        /// </summary>
        protected void ValidateAnnotatedProperties()
        {
            var annotatedPropertyInfos = GetType()
                .GetProperties()
                .Where(p => p.CanRead && p.GetCustomAttributes(typeof(ValidationAttribute), true).Any())
                .Select(p => new { p.Name, Value = p.GetValue(this) })
                .ToList();
            annotatedPropertyInfos.ForEach(pn => ValidateProperty(pn.Name, pn.Value));
        }

        /// <summary>
        /// Forces property validation for the specified property with no assigned value.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void ValidateProperty(string propertyName)
        {
            var results = new List<ValidationResult>();
            ValidationContext ctx = new ValidationContext(this);
            ctx.MemberName = propertyName;
            Validator.TryValidateProperty(null, ctx, results);
            SetErrors(propertyName, results.Select(c => c.ErrorMessage).ToArray());
        }

        private void ValidateProperty<T>(string propertyName, T newValue)
        {
            var results = new List<ValidationResult>();
            ValidationContext ctx = new ValidationContext(this);
            ctx.MemberName = propertyName;
            Validator.TryValidateProperty(newValue, ctx, results);
            SetErrors(propertyName, results.Select(c => c.ErrorMessage).ToArray());
        }

        /// <summary>
        /// Allows derived classes to directly raise the ErrorsChanged event
        /// for a specified property.
        /// </summary>
        protected void RaiseErrorsChanged(string propertyName)
            => OnErrorsChanged(new DataErrorsChangedEventArgs(propertyName));

        /// <summary>
        /// Allows derived classes to customize or handle the ErrorsChanged event.
        /// </summary>
        protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs args) 
            => ErrorsChanged?.Invoke(this, args);
    }
}
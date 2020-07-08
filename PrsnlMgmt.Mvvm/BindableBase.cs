using PrsnlMgmt.Mvvm.Shared;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PrsnlMgmt.Mvvm
{
    /// <summary>
    /// A base class for MVVM objects that need to implement INotifyPropertyChanged.
    /// It provides subclasses with a SetProperty method that takes
    /// care of raising the PropertyChanged event if needed.
    /// </summary>
    public class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
                    => PropertyChanged?.Invoke(this, args);

        protected void RaisePropertyChanged(string propertyName) =>
                    OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Helper method called by subclass property setters that sets
        /// updates the current value to the new value if they are not equal
        /// and then raises the PropertyChanged
        /// </summary>
        /// <typeparam name="T">Supports any type of property</typeparam>
        /// <param name="currentValue">A reference to the current value.</param>
        /// <param name="newValue">The new value supplied to the property setter block.</param>
        /// <param name="propertyName">Compiler supplied property name.</param>
        /// <returns>True if the current value was changed and PropertyChanged event was
        /// raised. Otherwise, False.</returns>
        protected virtual bool SetProperty<T>(ref T currentValue, T newValue,
            [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(currentValue, newValue)) return false;

            currentValue = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }

        public event Action<ErrorViewModel> Error;

        /// <summary>
        /// A helper method that can be called from catch blocks to propagate a
        /// new ErrorViewModel to the parent ViewModel via the Error event.
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <param name="error">An user friendly error message.</param>
        /// <param name="memberName">The source member name that originated the exception</param>
        /// <param name="sourceFilePath">The errant source file path.</param>
        /// <param name="sourceLineNumber">The errant source line number.</param>
        protected void RaiseError(Exception ex, string error, 
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Error?.Invoke(new ErrorViewModel(ex, error, sourceFilePath, sourceLineNumber, memberName, GetType().FullName));
        }
    }
}
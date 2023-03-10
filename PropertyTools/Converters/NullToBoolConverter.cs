// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullToBoolConverter.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Null to bool value converter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PropertyTools
{
    using System;
    using Microsoft.UI.Xaml.Data;
    /// <summary>
    /// Null to bool value converter
    /// </summary>
    [ValueConversion(typeof(object), typeof(bool))]
    public class NullToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullToBoolConverter" /> class.
        /// </summary>
        public NullToBoolConverter()
        {
            this.NullValue = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the source value is <c>null</c>.
        /// </summary>
        public bool NullValue { get; set; }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns <c>null</c>, the valid <c>null</c> value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value == null)
            {
                return this.NullValue;
            }

            if (value is double && double.IsNaN((double)value))
            {
                return this.NullValue;
            }

            return !this.NullValue;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns <c>null</c>, the valid <c>null</c> value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            var b = (bool)value;
            if (b != this.NullValue)
            {
                var ult = Nullable.GetUnderlyingType(targetType);
                if (ult != null)
                {
                    if (ult == typeof(DateTime))
                    {
                        return DateTime.Now;
                    }

                    return Activator.CreateInstance(ult);
                }

                if (targetType == typeof(string))
                {
                    return string.Empty;
                }

                return Activator.CreateInstance(targetType);
            }

            if (targetType == typeof(double))
            {
                return double.NaN;
            }

            return null;
        }
    }
}
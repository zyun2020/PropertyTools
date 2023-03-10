// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleToGridLengthConverter.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Converts double instances to GridLength instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PropertyTools
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;

    /// <summary>
    /// Converts <see cref="double" /> instances to <see cref="GridLength" /> instances.
    /// </summary>
    [ValueConversion(typeof(GridLength), typeof(double))]
    public class DoubleToGridLengthConverter : IValueConverter
    {
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
            return this.ConvertBack(value, targetType, parameter, culture);
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
            if (targetType == typeof(GridLength))
            {
                if (value == null)
                {
                    return GridLength.Auto;
                }

                if (value is double)
                {
                    return new GridLength((double)value);
                }

                return GridLength.Auto;
            }

            if (targetType == typeof(double))
            {
                if (value is GridLength)
                {
                    return ((GridLength)value).Value;
                }

                return double.NaN;
            }

            return null;
        }
    }
}
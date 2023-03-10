// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumToBooleanConverter.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Enum to Boolean converter
//   Usage 'Converter={StaticResource EnumToBooleanConverter}, ConverterParameter={x:Static value...}'
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PropertyTools
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;

    /// <summary>
    /// Enum to Boolean converter
    /// Usage 'Converter={StaticResource EnumToBooleanConverter}, ConverterParameter={x:Static value...}'
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(bool))]
    public class EnumToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the type of the enum.
        /// </summary>
        /// <value>The type of the enum.</value>
        public Type EnumType { get; set; }

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
            if (value == null && parameter == null)
            {
                return true;
            }

            if (value == null || parameter == null)
            {
                return DependencyProperty.UnsetValue;
            }

            string checkValue = value.ToString();
            string targetValue = parameter.ToString();
            return checkValue.Equals(targetValue, StringComparison.OrdinalIgnoreCase);
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
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            try
            {
                bool boolValue = System.Convert.ToBoolean(value);
                if (boolValue)
                {
                    if (parameter == null)
                    {
                        return parameter;
                    }

                    return Enum.Parse(this.EnumType, parameter.ToString());
                }
            }
            catch (ArgumentException)
            {
            }
            catch (FormatException)
            {
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
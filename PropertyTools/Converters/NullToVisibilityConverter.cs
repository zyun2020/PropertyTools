// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullToVisibilityConverter.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Converts Visibility instances to object instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PropertyTools
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;

    /// <summary>
    /// Converts <see cref="Visibility" /> instances to <see cref="object" /> instances.
    /// </summary>
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullToVisibilityConverter" /> class.
        /// </summary>
        public NullToVisibilityConverter()
        {
            this.NullVisibility = Visibility.Collapsed;
            this.NotNullVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Gets or sets the not <c>null</c> visibility.
        /// </summary>
        /// <value>The not <c>null</c> visibility.</value>
        public Visibility NotNullVisibility { get; set; }

        /// <summary>
        /// Gets or sets the <c>null</c> visibility.
        /// </summary>
        /// <value>The <c>null</c> visibility.</value>
        public Visibility NullVisibility { get; set; }

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
                return this.NullVisibility;
            }

            return this.NotNullVisibility;
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
            return null;
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingUtilities.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Provides binding utility extension methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PropertyTools
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Data;
    using System;


    /// <summary>
    /// Provides binding utility extension methods.
    /// </summary>
    public static class BindingUtilities
    {
        /// <summary>
        /// The value to boolean converter.
        /// </summary>
        private static readonly ValueToBooleanConverter ValueToBooleanConverter = new ValueToBooleanConverter();

        /// <summary>
        /// Binds the IsEnabled property of the specified element to the specified property and value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="path">The path.</param>
        /// <param name="parameter">The converter parameter (optional).</param>
        /// <param name="bindingSource">The binding source (optional).</param>
        public static void SetIsEnabledBinding(this Control control, string path, object parameter = null, object bindingSource = null)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var binding = new Binding();
            binding.Path = new PropertyPath(path);
            if (bindingSource != null)
            {
                binding.Source = bindingSource;
            }

            if (parameter != null)
            {
                binding.ConverterParameter = parameter;
                binding.Converter = ValueToBooleanConverter;
            }

            control.SetBinding(Control.IsEnabledProperty, binding);
        }

        public static void Path(this Binding binding, string path)
        {
            binding.Path = new PropertyPath(path);
        }

        public static void SetBinding(this FrameworkElement element, DependencyProperty dp, string propertyPath, BindingMode mode = BindingMode.TwoWay)
        {
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyPath);
            binding.Mode = mode;
            element.SetBinding(dp, propertyPath);
        }

        public static void SetBinding(this FrameworkElement element, DependencyProperty dp, string propertyPath, IValueConverter converter, object converterParameter = null)
        {
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyPath);
            binding.Converter = converter;
            binding.ConverterParameter = converterParameter;
            element.SetBinding(dp, propertyPath);
        }
    }
}
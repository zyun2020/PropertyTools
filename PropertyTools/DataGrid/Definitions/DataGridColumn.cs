﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnDefinition.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Defines a column in a DataGrid.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections;

namespace PropertyTools
{
    /// <summary>
    /// Defines a column in a <see cref="DataGrid" />.
    /// </summary>
    public class DataGridColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridColumn" /> class.
        /// </summary>
        public DataGridColumn()
        {
            this.Width = GridLength.Auto;
            this.MaxLength = int.MaxValue;
            this.CanSort = true;
        }

        public double ActualWidth { get; set; } = double.NaN;
        public double MinWidth { get; set; } = double.NaN;
        public double MaxWidth { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the column width.
        /// </summary>
        /// <value>The width.</value>
        public GridLength Width { get; set; }

        /// <summary>
        /// Gets or sets the converter.
        /// </summary>
        /// <value>The converter.</value>
        public IValueConverter Converter { get; set; }

        /// <summary>
        /// Gets or sets the converter culture.
        /// </summary>
        /// <value>The converter culture.</value>
        public string ConverterLanguage { get; set; }

        /// <summary>
        /// Gets or sets the converter parameter.
        /// </summary>
        /// <value>The converter parameter.</value>
        public object ConverterParameter { get; set; }

        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
        public string FormatString { get; set; }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public object Header { get; set; }

        /// <summary>
        /// Gets or sets the tooltip.
        /// </summary>
        /// <value>The tooltip.</value>
        public object Tooltip { get; set; }

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        /// <value>The horizontal alignment.</value>
        public HorizontalAlignment HorizontalAlignment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is editable (for ComboBox).
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the items source (for ComboBox).
        /// </summary>
        /// <value>The items source.</value>
        public IEnumerable ItemsSource { get; set; }

        /// <summary>
        /// Gets or sets the property name of an items source (for ComboBox).
        /// </summary>
        /// <value>The items source property.</value>
        public string ItemsSourceProperty { get; set; }

        /// <summary>
        /// Gets or sets the selected value path (for ComboBox).
        /// </summary>
        /// <value>
        /// The selected value path.
        /// </value>
        public string SelectedValuePath { get; set; }

        /// <summary>
        /// Gets or sets the display member path.
        /// </summary>
        /// <value>
        /// The display member path.
        /// </value>
        public string DisplayMemberPath { get; set; }

        /// <summary>
        /// Gets or sets the max length (for TextBox).
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the name of a property that determines the state of the cell.
        /// </summary>
        /// <value>
        /// The name of the related property.
        /// </value>
        public string IsEnabledByProperty { get; set; }

        /// <summary>
        /// Gets or sets the value that enables the cell.
        /// </summary>
        /// <remarks>This property is used if the <see cref="IsEnabledByProperty"/> property is set.</remarks>
        public object IsEnabledByValue { get; set; }

        /// <summary>
        /// Gets or sets the background brush.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        /// <remarks>If set, this overrides <see cref="BackgroundProperty" />.</remarks>
        public Brush Background { get; set; }

        /// <summary>
        /// Gets or sets the background property.
        /// </summary>
        /// <value>
        /// The background property.
        /// </value>
        public string BackgroundProperty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the items can be sorted by this column/row.
        /// </summary>
        /// <value>
        /// <c>true</c> if items can be sorted; otherwise, <c>false</c>.
        /// </value>
        public bool CanSort { get; set; }
    }
}
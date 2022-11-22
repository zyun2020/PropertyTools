// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RowDefinition.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Defines a row in a DataGrid.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.UI.Xaml;
using System;

namespace PropertyTools
{
    /// <summary>
    /// Defines a row in a <see cref="DataGrid" />.
    /// </summary>
    public class DataGridRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RowDefinition"/> class.
        /// </summary>
        public DataGridRow()
        {
            this.Height = GridLength.Auto;
        }

        public DataGridRow(String header)
        {
            this.Height = GridLength.Auto;
            this.Header = header;    
        }

        public double ActualHeight { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the row height.
        /// </summary>
        /// <value>The height.</value>
        public GridLength Height { get; set; }

        public object Header { get; set; }
    }
}
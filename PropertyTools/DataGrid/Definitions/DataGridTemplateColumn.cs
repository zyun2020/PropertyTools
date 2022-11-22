// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateColumnDefinition.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Defines a template column in a DataGrid.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.UI.Xaml;

namespace PropertyTools
{
    /// <summary>
    /// Defines a template column in a <see cref="DataGrid" />.
    /// </summary>
    public class DataGridTemplateColumn : DataGridColumn
    {
        /// <summary>
        /// Gets or sets the cell template.
        /// </summary>
        /// <value>A data template that contains display controls.</value>
        public DataTemplate CellTemplate { get; set; }

        /// <summary>
        /// Gets or sets the cell template used when editing the cell.
        /// </summary>
        /// <value>A data template that contains edit controls.</value>
        public DataTemplate CellEditingTemplate { get; set; }        
    }
}
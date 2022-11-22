// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateCellDefinition.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Defines a cell that displays the content with data templates.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.UI.Xaml;

namespace PropertyTools
{
    /// <summary>
    /// Defines a cell that displays the content with data templates.
    /// </summary>
    /// <seealso cref="PropertyTools.Wpf.CellDefinition" />
    public class TemplateCellDefinition : CellDefinition
    {
        /// <summary>
        /// Gets or sets the display template.
        /// </summary>
        /// <value>
        /// The data template.
        /// </value>
        public DataTemplate DisplayTemplate { get; set; }

        /// <summary>
        /// Gets or sets the edit template.
        /// </summary>
        /// <value>
        /// The data template.
        /// </value>
        public DataTemplate EditTemplate { get; set; }
    }
}
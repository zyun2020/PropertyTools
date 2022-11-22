using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PropertyTools
{
    internal class CellPresenter
    {  
        private readonly int column;
        private readonly int row;

        public CellPresenter(int row, int column)
        {
            this.row = row;
            this.column = column;
        }

        Size Size {  get; set; }
        FrameworkElement Content { get; set; }
        public int Column => this.column;
        /// <summary>
        /// Gets the row.
        /// </summary>
        public int Row => this.row;
    }

    internal sealed class DataGridPresenter : Panel
    {
        List<CellPresenter> m_cells = new List<CellPresenter>();
        public DataGridPresenter()
        {
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size measureSize = new Size(Double.PositiveInfinity, availableHeight);
          

            return measureSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        { 
            return base.ArrangeOverride(finalSize);
        }
    }
}

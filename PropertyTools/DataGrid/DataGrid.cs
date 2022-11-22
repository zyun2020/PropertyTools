// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataGrid.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Displays enumerable data in a customizable grid.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PropertyTools
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Media;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using Microsoft.UI.Xaml.Markup;
    using Microsoft.UI.Xaml.Input;
    using Windows.Foundation;
    using Microsoft.UI;
    using Windows.System;
    using DispatcherQueuePriority = Microsoft.UI.Dispatching.DispatcherQueuePriority;
 
    /// <summary>
    /// Displays enumerable data in a customizable grid.
    /// </summary>
    [TemplatePart(Name = PartGrid, Type = typeof(Grid))]
    [TemplatePart(Name = PartSheetScrollViewer, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = PartSheetGrid, Type = typeof(Grid))]
    [TemplatePart(Name = PartColumnScrollViewer, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = PartColumnGrid, Type = typeof(Grid))]
    [TemplatePart(Name = PartRowScrollViewer, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = PartRowGrid, Type = typeof(Grid))]
    [TemplatePart(Name = PartSelectionBackground, Type = typeof(Border))]
    [TemplatePart(Name = PartRowSelectionBackground, Type = typeof(Border))]
    [TemplatePart(Name = PartColumnSelectionBackground, Type = typeof(Border))]
    [TemplatePart(Name = PartCurrentBackground, Type = typeof(Border))]
    [TemplatePart(Name = PartSelection, Type = typeof(Border))]
    [TemplatePart(Name = PartAutoFillSelection, Type = typeof(Border))]
    [TemplatePart(Name = PartAutoFillBox, Type = typeof(Border))]
    [TemplatePart(Name = PartTopLeft, Type = typeof(Border))]
    public partial class DataGrid : Control
    {
        private readonly List<DataGridRow> m_rows = new List<DataGridRow>();
        public DataGrid()
        {
            DefaultStyleKey = typeof(DataGrid);
            CollectionView.VectorChanged += CollectionView_VectorChanged;
        }

        /// <summary>
        /// Handles changes to the items collection.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        private void CollectionView_VectorChanged(Windows.Foundation.Collections.IObservableVector<object> sender, 
            Windows.Foundation.Collections.IVectorChangedEventArgs @event)
        {
            if (this.suspendCollectionChangedNotifications)
            {
                return;
            }


            this.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () => {
                if(@event.CollectionChange == Windows.Foundation.Collections.CollectionChange.Reset)
                {
                    Reset();
                }
                else if (@event.CollectionChange == Windows.Foundation.Collections.CollectionChange.ItemInserted)
                {
                    ItemInserted(@event.Index);
                }
                else if (@event.CollectionChange == Windows.Foundation.Collections.CollectionChange.ItemRemoved)
                {
                    ItemRemoved(@event.Index);
                }
                else if (@event.CollectionChange == Windows.Foundation.Collections.CollectionChange.ItemChanged)
                {
                    ItemChanged(@event.Index);
                }
            });
        }

        private void Reset()
        {
            
            Rows.Clear();
            for (var i = 0; i < CollectionView.Count; i++)
            {
                Rows.Add(new DataGridRow(DefaultRowHeader(i)));
            }
        }

        private void ItemInserted(uint index)
        {

        }

        private void ItemRemoved(uint index)
        {

        }

        private void ItemChanged(uint index)
        {

        }

        /// <summary>
        /// Handles changes in the <see cref="ItemsSource" /> property.
        /// </summary>
        private void ItemsSourceChanged()
        {
            if (this.ItemsSource != null)
            {
                CollectionView.Source = this.ItemsSource;

                //重新生成Columns
                if (this.AutoGenerateColumns && this.Columns.Count == 0)
                {
                    this.Operator.AutoGenerateColumns();
                }
            }
            else
            {
                this.CollectionView.Clear();
            }
        }

        /// <summary>
        /// Gets the column definitions.
        /// </summary>
        /// <value>The column definitions.</value>
        public ObservableCollection<DataGridColumn> Columns { get; } = new ObservableCollection<DataGridColumn>();
        public List<DataGridRow> Rows { get; } = new List<DataGridRow>();

        public int RowCount => Rows.Count;
        public int ColumnCount => Columns.Count;

        /// <summary>
        /// Gets the collection view.
        /// </summary>
        /// <value>
        /// The collection view.
        /// </value>
        public AdvancedCollectionView CollectionView { get; } = new AdvancedCollectionView();

        /// <summary>
        /// Gets the operator.
        /// </summary>
        public IDataGridOperator Operator { get; private set; }
 
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see
        /// cref="M:System.Windows.FrameworkElement.ApplyTemplate" /> .
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.sheetScrollViewer = GetTemplateChild(PartSheetScrollViewer) as ScrollViewer;
            this.sheetGrid = GetTemplateChild(PartSheetGrid) as Grid;
            this.columnScrollViewer = GetTemplateChild(PartColumnScrollViewer) as ScrollViewer;
            this.columnGrid = GetTemplateChild(PartColumnGrid) as Grid;
            this.rowScrollViewer = GetTemplateChild(PartRowScrollViewer) as ScrollViewer;
            this.rowGrid = GetTemplateChild(PartRowGrid) as Grid;
            this.rowSelectionBackground = GetTemplateChild(PartRowSelectionBackground) as Border;
            this.columnSelectionBackground = GetTemplateChild(PartColumnSelectionBackground) as Border;
            this.selectionBackground = GetTemplateChild(PartSelectionBackground) as Border;
            this.currentBackground = GetTemplateChild(PartCurrentBackground) as Border;
            this.selection = GetTemplateChild(PartSelection) as Border;
            this.autoFillSelection = GetTemplateChild(PartAutoFillSelection) as Border;
            this.autoFillBox = GetTemplateChild(PartAutoFillBox) as Border;
            this.topLeft = GetTemplateChild(PartTopLeft) as Border;

            if (this.sheetScrollViewer == null)
            {
                throw new Exception(PartSheetScrollViewer + " not found.");
            }

            if (this.rowScrollViewer == null)
            {
                throw new Exception(PartRowScrollViewer + " not found.");
            }

            if (this.columnScrollViewer == null)
            {
                throw new Exception(PartColumnScrollViewer + " not found.");
            }

            if (this.topLeft == null)
            {
                throw new Exception(PartTopLeft + " not found.");
            }

            if (this.autoFillBox == null)
            {
                throw new Exception(PartAutoFillBox + " not found.");
            }

            if (this.columnGrid == null)
            {
                throw new Exception(PartColumnGrid + " not found.");
            }

            if (this.rowGrid == null)
            {
                throw new Exception(PartRowGrid + " not found.");
            }

            if (this.sheetGrid == null)
            {
                throw new Exception(PartSheetGrid + " not found.");
            }

            this.sheetScrollViewer.ViewChanged += this.ScrollViewerScrollChanged;
            this.rowScrollViewer.ViewChanged += this.RowScrollViewerScrollChanged;
            this.columnScrollViewer.ViewChanged += this.ColumnScrollViewerScrollChanged;

            this.columnGrid.PointerPressed += this.ColumnGridPointerPressed;
            this.columnGrid.PointerMoved += this.ColumnGridPointerMoved;
            this.columnGrid.PointerReleased += this.ColumnGridPointerReleased;
            this.rowGrid.PointerPressed += this.RowGridPointerPressed;
            this.rowGrid.PointerMoved += this.RowGridPointerMoved;
            this.rowGrid.PointerReleased += this.RowGridPointerReleased;

            this.topLeft.Tapped += this.TopLeftTapped;
            this.autoFillBox.Tapped += this.AutoFillBoxTapped;
            this.sheetGrid.DoubleTapped += this.SheetGridDoubleTapped;

            this.sheetScrollViewer.SizeChanged += (s, e) => this.UpdateGridSize();
          
            this.sheetScrollViewer.Loaded += (s, e) => this.UpdateGridSize();

            this.autoFiller = new AutoFiller(this.GetCellValue, this.TrySetCellValue);

            this.autoFillToolTip = new ToolTip
            {
                Placement = PlacementMode.Bottom,
                PlacementTarget = this.autoFillSelection
            };

            this.UpdateGridContent();
            this.SelectedCellsChanged();
        }

        /// <summary>
        /// Gets the default row header.
        /// </summary>
        /// <param name="j">The j.</param>
        /// <returns>
        /// The get row header.
        /// </returns>
        private String DefaultRowHeader(int j)
        { 
            return CellRef.ToRowName(j);
        }

        /// <summary>
        /// Refreshes the collection view and updates the grid content, if the ItemsSource is not implementing INotifyCollectionChanged.
        /// </summary>
        private void RefreshIfRequired()
        {
            if (!(this.ItemsSource is INotifyCollectionChanged))
            {
               this.CollectionView.Refresh();
            }
        }
    }
}
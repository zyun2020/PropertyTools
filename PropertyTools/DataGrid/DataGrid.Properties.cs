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
    using Windows.UI;
    using PropertyTools.DataAnnotations;
    using HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment;
    using System.Reflection.Metadata;

    public partial class DataGrid
    {
        /// <summary>
        /// Identifies the <see cref="CustomSort"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomSortProperty = DependencyProperty.Register(
            nameof(CustomSort),
            typeof(IComparer),
            typeof(DataGrid),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CreateColumnHeader"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CreateColumnHeaderProperty = DependencyProperty.Register(
            nameof(CreateColumnHeader),
            typeof(Func<int, object>),
            typeof(DataGrid),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CreateItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CreateItemProperty = DependencyProperty.Register(
            nameof(CreateItem),
            typeof(Func<object>),
            typeof(DataGrid),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="AddItemHeader"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AddItemHeaderProperty = DependencyProperty.Register(
            nameof(AddItemHeader),
            typeof(string),
            typeof(DataGrid),
            new PropertyMetadata("+"));

        /// <summary>
        /// Identifies the <see cref="AlternatingRowsBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AlternatingRowsBackgroundProperty = DependencyProperty.Register(
                nameof(AlternatingRowsBackground),
                typeof(Brush),
                typeof(DataGrid),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="AutoGenerateColumns"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoGenerateColumnsProperty = DependencyProperty.Register(
                nameof(AutoGenerateColumns),
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="AutoInsert"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoInsertProperty = DependencyProperty.Register(
            nameof(AutoInsert),
            typeof(bool),
            typeof(DataGrid),
            new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="CanClear"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanClearProperty = DependencyProperty.Register(
            nameof(CanClear),
            typeof(bool),
            typeof(DataGrid),
            new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="CanDelete"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanDeleteProperty = DependencyProperty.Register(
            nameof(CanDelete),
            typeof(bool),
            typeof(DataGrid),
            new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="CanInsert"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanInsertProperty = DependencyProperty.Register(
            nameof(CanInsert),
            typeof(bool),
            typeof(DataGrid),
            new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="CanResizeColumns"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanResizeColumnsProperty = DependencyProperty.Register(
                nameof(CanResizeColumns),
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="CanResizeRows"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanResizeRowsProperty = DependencyProperty.Register(
                nameof(CanResizeRows),
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="MultiChangeInChangedColumnOnly"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MultiChangeInChangedColumnOnlyProperty = DependencyProperty.Register(
                nameof(MultiChangeInChangedColumnOnly),
                typeof(bool),
                typeof(DataGrid),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="ColumnHeaderHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnHeaderHeightProperty = DependencyProperty.Register(
                nameof(ColumnHeaderHeight),
                typeof(double),
                typeof(DataGrid),
                new PropertyMetadata(20));

        /// <summary>
        /// Identifies the <see cref="SheetContextMenu"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SheetContextMenuProperty = DependencyProperty.Register(
                nameof(SheetContextMenu),
                typeof(MenuFlyout),
                typeof(DataGrid),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ColumnsContextMenu"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnsContextMenuProperty = DependencyProperty.Register(
                nameof(ColumnsContextMenu),
                typeof(MenuFlyout),
                typeof(DataGrid),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ControlFactory"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ControlFactoryProperty = DependencyProperty.Register(
            nameof(ControlFactory),
            typeof(IDataGridControlFactory),
            typeof(DataGrid),
            new PropertyMetadata(new DataGridControlFactory()));

        /// <summary>
        /// Identifies the <see cref="CellDefinitionFactory"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CellDefinitionFactoryProperty = DependencyProperty.Register(
            nameof(CellDefinitionFactory),
            typeof(ICellDefinitionFactory),
            typeof(DataGrid),
            new PropertyMetadata(new CellDefinitionFactory()));

        /// <summary>
        /// Identifies the <see cref="CurrentCell"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentCellProperty = DependencyProperty.Register(
            nameof(CurrentCell),
            typeof(CellRef),
            typeof(DataGrid),
            new PropertyMetadata(
                new CellRef(0, 0), (d, e) => ((DataGrid)d).CurrentCellChanged()));

        /// <summary>
        /// Identifies the <see cref="DefaultColumnWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultColumnWidthProperty = DependencyProperty.Register(
                nameof(DefaultColumnWidth),
                typeof(GridLength),
                typeof(DataGrid),
                new PropertyMetadata(new GridLength(1, GridUnitType.Star)));

        /// <summary>
        /// Identifies the <see cref="DefaultHorizontalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultHorizontalAlignmentProperty = DependencyProperty.Register(
                nameof(DefaultHorizontalAlignment),
                typeof(HorizontalAlignment),
                typeof(DataGrid),
                new PropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// Identifies the <see cref="DefaultRowHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultRowHeightProperty = DependencyProperty.Register(
                nameof(DefaultRowHeight),
                typeof(GridLength),
                typeof(DataGrid),
                new PropertyMetadata(new GridLength(20)));

        /// <summary>
        /// Identifies the <see cref="IsEasyInsertByKeyboardEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEasyInsertByKeyboardEnabledProperty = DependencyProperty.Register(
            nameof(IsEasyInsertByKeyboardEnabled),
            typeof(bool),
            typeof(DataGrid),
            new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsEasyInsertByMouseEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEasyInsertByMouseEnabledProperty = DependencyProperty.Register(
            nameof(IsEasyInsertByMouseEnabled),
            typeof(bool),
            typeof(DataGrid),
            new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="GridLineBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GridLineBrushProperty = DependencyProperty.Register(
            nameof(GridLineBrush),
            typeof(Brush),
            typeof(DataGrid),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 218, 220, 221))));

        /// <summary>
        /// Identifies the <see cref="HeaderBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderBorderBrushProperty = DependencyProperty.Register(
                nameof(HeaderBorderBrush),
                typeof(Brush),
                typeof(DataGrid),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 177, 181, 186))));

        /// <summary>
        /// Identifies the <see cref="InputDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InputDirectionProperty = DependencyProperty.Register(
            nameof(InputDirection),
            typeof(InputDirection),
            typeof(DataGrid),
            new PropertyMetadata(InputDirection.Vertical));

        /// <summary>
        /// Identifies the <see cref="IsAutoFillEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAutoFillEnabledProperty = DependencyProperty.Register(
            nameof(IsAutoFillEnabled),
            typeof(bool),
            typeof(DataGrid),
            new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsMoveAfterEnterEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMoveAfterEnterEnabledProperty = DependencyProperty.Register(
            nameof(IsMoveAfterEnterEnabled),
            typeof(bool),
            typeof(DataGrid),
            new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="ItemHeaderPropertyPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemHeaderPropertyPathProperty = DependencyProperty.Register(
                nameof(ItemHeaderPropertyPath),
                typeof(string),
                typeof(DataGrid),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(IList),
            typeof(DataGrid),
            new PropertyMetadata(null, (d, e) => ((DataGrid)d).ItemsSourceChanged()));

        /// <summary>
        /// Identifies the <see cref="RowHeadersSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowHeadersSourceProperty = DependencyProperty.Register(
            nameof(RowHeadersSource),
            typeof(IList),
            typeof(DataGrid),
            new PropertyMetadata(null, (d, e) => ((DataGrid)d).UpdateGridContent()));

        /// <summary>
        /// Identifies the <see cref="ColumnHeadersSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnHeadersSourceProperty = DependencyProperty.Register(
            nameof(ColumnHeadersSource),
            typeof(IList),
            typeof(DataGrid),
            new PropertyMetadata(null, (d, e) => ((DataGrid)d).UpdateGridContent()));

        /// <summary>
        /// Identifies the <see cref="RowHeadersFormatString"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowHeadersFormatStringProperty = DependencyProperty.Register(
            nameof(RowHeadersFormatString),
            typeof(string),
            typeof(DataGrid),
            new PropertyMetadata(null, (d, e) => ((DataGrid)d).UpdateGridContent()));

        /// <summary>
        /// Identifies the <see cref="ColumnHeadersFormatString"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnHeadersFormatStringProperty = DependencyProperty.Register(
            nameof(ColumnHeadersFormatString),
            typeof(string),
            typeof(DataGrid),
            new PropertyMetadata(null, (d, e) => ((DataGrid)d).UpdateGridContent()));

        /// <summary>
        /// Identifies the <see cref="RowHeaderWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowHeaderWidthProperty = DependencyProperty.Register(
            nameof(RowHeaderWidth),
            typeof(double),
            typeof(DataGrid),
            new PropertyMetadata(40));

        /// <summary>
        /// Identifies the <see cref="RowsContextMenu"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowsContextMenuProperty = DependencyProperty.Register(
                nameof(RowsContextMenu),
                typeof(MenuFlyout),
                typeof(DataGrid),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SelectedItems"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            nameof(SelectedItems),
            typeof(IEnumerable),
            typeof(DataGrid),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SelectionCell"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionCellProperty = DependencyProperty.Register(
            nameof(SelectionCell),
            typeof(CellRef),
            typeof(DataGrid),
            new PropertyMetadata(new CellRef(0, 0), (d, e) => ((DataGrid)d).SelectionCellChanged()));

        /// <summary>
        /// Identifies the <see cref="WrapItems"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WrapItemsProperty = DependencyProperty.Register(
            nameof(WrapItems),
            typeof(bool),
            typeof(DataGrid),
            new PropertyMetadata(false));

        /// <summary>
        /// The auto fill box.
        /// </summary>
        private const string PartAutoFillBox = "PART_AutoFillBox";

        /// <summary>
        /// The auto fill selection.
        /// </summary>
        private const string PartAutoFillSelection = "PART_AutoFillSelection";

        /// <summary>
        /// The column grid.
        /// </summary>
        private const string PartColumnGrid = "PART_ColumnGrid";

        /// <summary>
        /// The column scroll viewer.
        /// </summary>
        private const string PartColumnScrollViewer = "PART_ColumnScrollViewer";

        /// <summary>
        /// The column selection background.
        /// </summary>
        private const string PartColumnSelectionBackground = "PART_ColumnSelectionBackground";

        /// <summary>
        /// The current background.
        /// </summary>
        private const string PartCurrentBackground = "PART_CurrentBackground";

        /// <summary>
        /// The grid.
        /// </summary>
        private const string PartGrid = "PART_Grid";

        /// <summary>
        /// The row grid.
        /// </summary>
        private const string PartRowGrid = "PART_RowGrid";

        /// <summary>
        /// The row scroll viewer.
        /// </summary>
        private const string PartRowScrollViewer = "PART_RowScrollViewer";

        /// <summary>
        /// The row selection background.
        /// </summary>
        private const string PartRowSelectionBackground = "PART_RowSelectionBackground";

        /// <summary>
        /// The selection.
        /// </summary>
        private const string PartSelection = "PART_Selection";

        /// <summary>
        /// The selection background.
        /// </summary>
        private const string PartSelectionBackground = "PART_SelectionBackground";

        /// <summary>
        /// The sheet grid.
        /// </summary>
        private const string PartSheetGrid = "PART_SheetGrid";

        /// <summary>
        /// The sheet scroll viewer.
        /// </summary>
        private const string PartSheetScrollViewer = "PART_SheetScrollViewer";

        /// <summary>
        /// The top left cell.
        /// </summary>
        private const string PartTopLeft = "PART_TopLeft";

        /// <summary>
        /// The cell map.
        /// </summary>
        private readonly Dictionary<int, FrameworkElement> cellMap = new Dictionary<int, FrameworkElement>();

        /// <summary>
        /// The column header map.
        /// </summary>
        private readonly Dictionary<int, FrameworkElement> columnHeaderMap = new Dictionary<int, FrameworkElement>();

        /// <summary>
        /// The row header map.
        /// </summary>
        private readonly Dictionary<int, FrameworkElement> rowHeaderMap = new Dictionary<int, FrameworkElement>();

        /// <summary>
        /// The sort descriptors
        /// </summary>
        //private readonly SortDescriptionCollection sortDescriptions = new SortDescriptionCollection();

        /// <summary>
        /// The sort description markers
        /// </summary>
        private readonly List<FrameworkElement> sortDescriptionMarkers = new List<FrameworkElement>();

        /// <summary>
        /// The auto fill box.
        /// </summary>
        private Border autoFillBox;

        /// <summary>
        /// The auto fill cell.
        /// </summary>
        private CellRef autoFillCell;

        /// <summary>
        /// The auto fill selection.
        /// </summary>
        private Border autoFillSelection;

        /// <summary>
        /// The auto fill tool tip.
        /// </summary>
        private ToolTip autoFillToolTip;

        /// <summary>
        /// The auto filler.
        /// </summary>
        private AutoFiller autoFiller;

        /// <summary>
        /// The index in the sheet grid where new cells can be inserted.
        /// </summary>
        /// <remarks>The selection and auto fill controls should always be at the end of the sheetGrid children collection.</remarks>
        private int cellInsertionIndex;

        /// <summary>
        /// The column grid control.
        /// </summary>
        private Grid columnGrid;

        /// <summary>
        /// The row grid control.
        /// </summary>
        private Grid rowGrid;

        /// <summary>
        /// The column scroll view control.
        /// </summary>
        private ScrollViewer columnScrollViewer;

        /// <summary>
        /// The column selection background control.
        /// </summary>
        private Border columnSelectionBackground;

        /// <summary>
        /// The current background control.
        /// </summary>
        private Border currentBackground;

        /// <summary>
        /// The current editor.
        /// </summary>
        private FrameworkElement currentEditControl;

        /// <summary>
        /// The editing cells.
        /// </summary>
        private IList<CellRef> editingCells;

        /// <summary>
        /// The end pressed.
        /// </summary>
        private bool endPressed;

        /// <summary>
        /// The sheet grid control.
        /// </summary>
        private Grid sheetGrid;

        /// <summary>
        /// The row scroll viewer control.
        /// </summary>
        private ScrollViewer rowScrollViewer;

        /// <summary>
        /// The row selection background control.
        /// </summary>
        private Border rowSelectionBackground;

        /// <summary>
        /// The selection control.
        /// </summary>
        private Border selection;

        /// <summary>
        /// The selection background control.
        /// </summary>
        private Border selectionBackground;

        /// <summary>
        /// The sheet scroll viewer control.
        /// </summary>
        private ScrollViewer sheetScrollViewer;

        /// <summary>
        /// The top/left control.
        /// </summary>
        private Border topLeft;

        /// <summary>
        /// Flag used for collection changed notification suspension.
        /// </summary>
        private bool suspendCollectionChangedNotifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGrid" /> class.
        /// </summary>
     
        /// <summary>
        /// Gets or sets the header used for the add item row/column. Default is "*".
        /// </summary>
        /// <value>The add item header.</value>
        public string AddItemHeader
        {
            get => (string)this.GetValue(AddItemHeaderProperty);
            set => this.SetValue(AddItemHeaderProperty, value);
        }

        /// <summary>
        /// Gets or sets the alternating rows background brush.
        /// </summary>
        /// <value>The alternating rows background.</value>
        public Brush AlternatingRowsBackground
        {
            get => (Brush)this.GetValue(AlternatingRowsBackgroundProperty);
            set => this.SetValue(AlternatingRowsBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the auto fill cell.
        /// </summary>
        /// <value>The auto fill cell.</value>
        public CellRef AutoFillCell
        {
            get => this.autoFillCell;
            set
            {
                this.autoFillCell = (CellRef)CoerceSelectionCell(this, value);
                this.SelectedCellsChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to auto generate columns.
        /// </summary>
        public bool AutoGenerateColumns
        {
            get => (bool)this.GetValue(AutoGenerateColumnsProperty);
            set => this.SetValue(AutoGenerateColumnsProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow automatic insert mode.
        /// </summary>
        public bool AutoInsert
        {
            get => (bool)this.GetValue(AutoInsertProperty);
            set => this.SetValue(AutoInsertProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this grid can clear cells.
        /// </summary>
        /// <value><c>true</c> if this instance can clear; otherwise, <c>false</c> .</value>
        public bool CanClear
        {
            get => (bool)this.GetValue(CanClearProperty);
            set => this.SetValue(CanClearProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can delete.
        /// </summary>
        /// <value><c>true</c> if this instance can delete; otherwise, <c>false</c> .</value>
        public bool CanDelete
        {
            get => (bool)this.GetValue(CanDeleteProperty);
            set => this.SetValue(CanDeleteProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can insert.
        /// </summary>
        /// <value><c>true</c> if this instance can insert; otherwise, <c>false</c> .</value>
        public bool CanInsert
        {
            get => (bool)this.GetValue(CanInsertProperty);
            set => this.SetValue(CanInsertProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can resize columns.
        /// </summary>
        /// <value><c>true</c> if this instance can resize columns; otherwise, <c>false</c> .</value>
        public bool CanResizeColumns
        {
            get => (bool)this.GetValue(CanResizeColumnsProperty);
            set => this.SetValue(CanResizeColumnsProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can resize rows.
        /// </summary>
        /// <value><c>true</c> if this instance can resize rows; otherwise, <c>false</c> .</value>
        public bool CanResizeRows
        {
            get => (bool)this.GetValue(CanResizeRowsProperty);
            set => this.SetValue(CanResizeRowsProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether only cells in the changed column should be changed when changing value for a selection.
        /// </summary>
        /// <value><c>true</c> if only cells in the current column should be changed; otherwise, <c>false</c> .</value>
        public bool MultiChangeInChangedColumnOnly
        {
            get => (bool)this.GetValue(MultiChangeInChangedColumnOnlyProperty);
            set => this.SetValue(MultiChangeInChangedColumnOnlyProperty, value);
        }

        

        /// <summary>
        /// Gets or sets the height of the column headers.
        /// </summary>
        /// <value>The height of the column header.</value>
        public double ColumnHeaderHeight
        {
            get => (double)this.GetValue(ColumnHeaderHeightProperty);
            set => this.SetValue(ColumnHeaderHeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the columns context menu.
        /// </summary>
        /// <value>The columns context menu.</value>
        public MenuFlyout ColumnsContextMenu
        {
            get => (MenuFlyout)this.GetValue(ColumnsContextMenuProperty);
            set => this.SetValue(ColumnsContextMenuProperty, value);
        }

        /// <summary>
        /// Gets or sets the columns context menu.
        /// </summary>
        /// <value>The columns context menu.</value>
        public MenuFlyout SheetContextMenu
        {
            get => (MenuFlyout)this.GetValue(SheetContextMenuProperty);
            set => this.SetValue(SheetContextMenuProperty, value);
        }

        /// <summary>
        /// Gets or sets the control factory.
        /// </summary>
        public IDataGridControlFactory ControlFactory
        {
            get => (IDataGridControlFactory)this.GetValue(ControlFactoryProperty);
            set => this.SetValue(ControlFactoryProperty, value);
        }

        /// <summary>
        /// Gets or sets the cell definition factory.
        /// </summary>
        public ICellDefinitionFactory CellDefinitionFactory
        {
            get => (ICellDefinitionFactory)this.GetValue(CellDefinitionFactoryProperty);
            set => this.SetValue(CellDefinitionFactoryProperty, value);
        }

        /// <summary>
        /// Gets or sets the create item function.
        /// </summary>
        /// <value>The create item.</value>
        public Func<object> CreateItem
        {
            get => (Func<object>)this.GetValue(CreateItemProperty);
            set => this.SetValue(CreateItemProperty, value);
        }

        /// <summary>
        /// Gets or sets the custom sort comparer.
        /// </summary>
        /// <value>The custom sort comparer.</value>
        public IComparer CustomSort
        {
            get => (IComparer)this.GetValue(CustomSortProperty);
            set => this.SetValue(CustomSortProperty, value);
        }

        /// <summary>
        /// Gets or sets the create column header function.
        /// </summary>
        /// <value>The create column header.</value>
        public Func<int, object> CreateColumnHeader
        {
            get => (Func<int, object>)this.GetValue(CreateColumnHeaderProperty);
            set => this.SetValue(CreateColumnHeaderProperty, value);
        }

        /// <summary>
        /// Gets or sets the current cell.
        /// </summary>
        public CellRef CurrentCell
        {
            get => (CellRef)this.GetValue(CurrentCellProperty);
            set => this.SetValue(CurrentCellProperty, value);
        }

        /// <summary>
        /// Gets or sets the default column width.
        /// </summary>
        /// <value>The default width of the column.</value>
        public GridLength DefaultColumnWidth
        {
            get => (GridLength)this.GetValue(DefaultColumnWidthProperty);
            set => this.SetValue(DefaultColumnWidthProperty, value);
        }

        /// <summary>
        /// Gets or sets the default horizontal alignment.
        /// </summary>
        /// <value>The default horizontal alignment.</value>
        public HorizontalAlignment DefaultHorizontalAlignment
        {
            get => (HorizontalAlignment)this.GetValue(DefaultHorizontalAlignmentProperty);
            set => this.SetValue(DefaultHorizontalAlignmentProperty, value);
        }

        /// <summary>
        /// Gets or sets the default height of the row.
        /// </summary>
        /// <value>The default height of the row.</value>
        public GridLength DefaultRowHeight
        {
            get => (GridLength)this.GetValue(DefaultRowHeightProperty);
            set => this.SetValue(DefaultRowHeightProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether easy insert by keyboard (press enter/down/right to add new rows/columns) is enabled.
        /// </summary>
        /// <value><c>true</c> if easy insert is enabled; otherwise, <c>false</c>.</value>
        public bool IsEasyInsertByKeyboardEnabled
        {
            get => (bool)this.GetValue(IsEasyInsertByKeyboardEnabledProperty);
            set => this.SetValue(IsEasyInsertByKeyboardEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether easy insert by mouse (mouse down outside existing rows/columns) is enabled.
        /// </summary>
        /// <value><c>true</c> if easy insert is enabled; otherwise, <c>false</c>.</value>
        public bool IsEasyInsertByMouseEnabled
        {
            get => (bool)this.GetValue(IsEasyInsertByMouseEnabledProperty);
            set => this.SetValue(IsEasyInsertByMouseEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets the grid line brush.
        /// </summary>
        /// <value>The grid line brush.</value>
        public Brush GridLineBrush
        {
            get => (Brush)this.GetValue(GridLineBrushProperty);
            set => this.SetValue(GridLineBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the header border brush.
        /// </summary>
        /// <value>The header border brush.</value>
        public Brush HeaderBorderBrush
        {
            get => (Brush)this.GetValue(HeaderBorderBrushProperty);
            set => this.SetValue(HeaderBorderBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the input direction (the moving direction of the current cell when Enter is pressed).
        /// </summary>
        /// <value>The input direction.</value>
        public InputDirection InputDirection
        {
            get => (InputDirection)this.GetValue(InputDirectionProperty);
            set => this.SetValue(InputDirectionProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the auto fill is enabled or not.
        /// </summary>
        /// <value>If auto fill is enabled, <c>true</c>; otherwise <c>false</c>.</value>
        public bool IsAutoFillEnabled
        {
            get => (bool)this.GetValue(IsAutoFillEnabledProperty);
            set => this.SetValue(IsAutoFillEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current cell will change after the user has pressed Enter.
        /// </summary>
        /// <value>If the feature is enabled, <c>true</c>; otherwise <c>false</c>.</value>
        public bool IsMoveAfterEnterEnabled
        {
            get => (bool)this.GetValue(IsMoveAfterEnterEnabledProperty);
            set => this.SetValue(IsMoveAfterEnterEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets the binding path to the item headers (row or column headers, depending on the <see cref="ItemsInRows" /> property.
        /// </summary>
        public string ItemHeaderPropertyPath
        {
            get => (string)this.GetValue(ItemHeaderPropertyPathProperty);
            set => this.SetValue(ItemHeaderPropertyPathProperty, value);
        }

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        /// <value>The items source.</value>
        public IList ItemsSource
        {
            get => (IList)this.GetValue(ItemsSourceProperty);
            set => this.SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// Gets or sets the row headers source.
        /// </summary>
        /// <value>The row headers source.</value>
        public IList RowHeadersSource
        {
            get => (IList)this.GetValue(RowHeadersSourceProperty);
            set => this.SetValue(RowHeadersSourceProperty, value);
        }

        /// <summary>
        /// Gets or sets the column headers source.
        /// </summary>
        /// <value>The column headers source.</value>
        public IList ColumnHeadersSource
        {
            get => (IList)this.GetValue(ColumnHeadersSourceProperty);
            set => this.SetValue(ColumnHeadersSourceProperty, value);
        }

        /// <summary>
        /// Gets or sets the row headers format string.
        /// </summary>
        /// <value>The row headers format string.</value>
        public string RowHeadersFormatString
        {
            get => (string)this.GetValue(RowHeadersFormatStringProperty);
            set => this.SetValue(RowHeadersFormatStringProperty, value);
        }

        /// <summary>
        /// Gets or sets the column headers format string.
        /// </summary>
        /// <value>The column headers format string.</value>
        public string ColumnHeadersFormatString
        {
            get => (string)this.GetValue(ColumnHeadersFormatStringProperty);
            set => this.SetValue(ColumnHeadersFormatStringProperty, value);
        }

        /// <summary>
        /// Gets or sets the width of the row headers.
        /// </summary>
        /// <value>The width of the row header.</value>
        public double RowHeaderWidth
        {
            get => (double)this.GetValue(RowHeaderWidthProperty);
            set => this.SetValue(RowHeaderWidthProperty, value);
        }

        /// <summary>
        /// Gets or sets the rows context menu.
        /// </summary>
        /// <value>The rows context menu.</value>
        public MenuFlyout RowsContextMenu
        {
            get => (MenuFlyout)this.GetValue(RowsContextMenuProperty);
            set => this.SetValue(RowsContextMenuProperty, value);
        }

        /// <summary>
        /// Gets the selected cells.
        /// </summary>
        /// <value>The selected cells.</value>
        public IEnumerable<CellRef> SelectedCells
        {
            get
            {
                var rowMin = Math.Min(this.CurrentCell.Row, this.SelectionCell.Row);
                var columnMin = Math.Min(this.CurrentCell.Column, this.SelectionCell.Column);
                var rowMax = Math.Max(this.CurrentCell.Row, this.SelectionCell.Row);
                var columnMax = Math.Max(this.CurrentCell.Column, this.SelectionCell.Column);

                for (var i = rowMin; i <= rowMax; i++)
                {
                    for (var j = columnMin; j <= columnMax; j++)
                    {
                        yield return new CellRef(i, j);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected items.
        /// </summary>
        /// <value>The selected items.</value>
        public IEnumerable SelectedItems
        {
            get => (IEnumerable)this.GetValue(SelectedItemsProperty);
            set => this.SetValue(SelectedItemsProperty, value);
        }

        /// <summary>
        /// Gets or sets the cell defining the selection area. The selection area is defined by the CurrentCell and SelectionCell.
        /// </summary>
        public CellRef SelectionCell
        {
            get => (CellRef)this.GetValue(SelectionCellProperty);
            set => this.SetValue(SelectionCellProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to wrap items in the defined columns.
        /// </summary>
        /// <value><c>true</c> if items should be wrapped; otherwise, <c>false</c> .</value>
        public bool WrapItems
        {
            get => (bool)this.GetValue(WrapItemsProperty);
            set => this.SetValue(WrapItemsProperty, value);
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyGrid.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Specifies how the label widths are shared.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PropertyTools
{
    using System;
    using System.Collections;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Media;
    using HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment;

    /// <summary>
    /// Specifies the layout for checkboxes.
    /// </summary>
    public enum CheckBoxLayout
    {
        /// <summary>
        /// Show the header, then the check box without content
        /// </summary>
        Header,

        /// <summary>
        /// Hide the header, show the check box with the display name as content
        /// </summary>
        HideHeader,

        /// <summary>
        /// Collapse the header, show the check box with the display name as content
        /// </summary>
        CollapseHeader
    }

    /// <summary>
    /// Specifies the visibility of the tab strip.
    /// </summary>
    public enum TabVisibility
    {
        /// <summary>
        /// The tabs are visible.
        /// </summary>
        Visible,

        /// <summary>
        /// The tabs are visible if there is more than one tab.
        /// </summary>
        VisibleIfMoreThanOne,

        /// <summary>
        /// The tab strip is collapsed. The contents of the tab pages will be stacked vertically in the control. 
        /// </summary>
        Collapsed
    }
    public partial class PropertyGrid
    {
        /// <summary>
        /// Identifies the <see cref="CategoryControlTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CategoryControlTemplateProperty = DependencyProperty.Register(
            nameof(CategoryControlTemplate),
            typeof(ControlTemplate),
            typeof(PropertyGrid),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CategoryControlType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CategoryControlTypeProperty = DependencyProperty.Register(
            nameof(CategoryControlType),
            typeof(CategoryControlType),
            typeof(PropertyGrid),
            new PropertyMetadata(CategoryControlType.GroupBox, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="CategoryHeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CategoryHeaderTemplateProperty = DependencyProperty.Register(
            nameof(CategoryHeaderTemplate),
            typeof(DataTemplate),
            typeof(PropertyGrid), null);

        /// <summary>
        /// Identifies the <see cref="DescriptionIconAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionIconAlignmentProperty = DependencyProperty.Register(
            nameof(DescriptionIconAlignment),
            typeof(HorizontalAlignment),
            typeof(PropertyGrid),
            new PropertyMetadata(HorizontalAlignment.Right, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="DescriptionIcon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionIconProperty = DependencyProperty.Register(
            nameof(DescriptionIcon), typeof(ImageSource), typeof(PropertyGrid), null);

        /// <summary>
        /// Identifies the <see cref="EnableLabelWidthResizing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableLabelWidthResizingProperty = DependencyProperty.Register(
            nameof(EnableLabelWidthResizing),
            typeof(bool),
            typeof(PropertyGrid),
            new PropertyMetadata(true, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="EnumAsRadioButtonsLimit"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnumAsRadioButtonsLimitProperty = DependencyProperty.Register(
            nameof(EnumAsRadioButtonsLimit),
            typeof(int),
            typeof(PropertyGrid),
            new PropertyMetadata(4, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="Indentation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IndentationProperty = DependencyProperty.Register(
            nameof(Indentation),
            typeof(double),
            typeof(PropertyGrid),
            new PropertyMetadata(16d));

        /// <summary>
        /// Identifies the <see cref="MaximumLabelWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumLabelWidthProperty = DependencyProperty.Register(
            nameof(MaximumLabelWidth),
            typeof(double),
            typeof(PropertyGrid),
            new PropertyMetadata(double.MaxValue));

        /// <summary>
        /// Identifies the <see cref="MinimumLabelWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumLabelWidthProperty = DependencyProperty.Register(
            nameof(MinimumLabelWidth),
            typeof(double),
            typeof(PropertyGrid),
            new PropertyMetadata(70.0));

        /// <summary>
        /// Identifies the <see cref="MoveFocusOnEnter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MoveFocusOnEnterProperty = DependencyProperty.Register(
            nameof(MoveFocusOnEnter),
            typeof(bool),
            typeof(PropertyGrid),
            new PropertyMetadata(false));

        /// <summary> 
        /// Identifies the <see cref="ToolTipDuration"/> dependency property. 
        /// </summary> 
        public static readonly DependencyProperty ToolTipDurationProperty = DependencyProperty.Register(
            nameof(ToolTipDuration),
            typeof(int),
            typeof(PropertyGrid),
            new PropertyMetadata(5000));

        /// <summary>
        /// Identifies the <see cref="ControlFactory"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ControlFactoryProperty = DependencyProperty.Register(
            nameof(ControlFactory),
            typeof(IPropertyGridControlFactory),
            typeof(PropertyGrid),
            new PropertyMetadata(new PropertyGridControlFactory()));

        /// <summary>
        /// Identifies the <see cref="PropertyItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OperatorProperty = DependencyProperty.Register(
            nameof(Operator),
            typeof(IPropertyGridOperator),
            typeof(PropertyGrid),
            new PropertyMetadata(new PropertyGridOperator()));

        /// <summary>
        /// Identifies the <see cref="RequiredAttribute"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RequiredAttributeProperty = DependencyProperty.Register(
            nameof(RequiredAttribute),
            typeof(Type),
            typeof(PropertyGrid),
            new PropertyMetadata(null, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="SelectedObject"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register(
            nameof(SelectedObject),
            typeof(object),
            typeof(PropertyGrid),
            new PropertyMetadata(null, (s, e) => ((PropertyGrid)s).OnSelectedObjectChanged(e)));

        /// <summary>
        /// Identifies the <see cref="SelectedObjects"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedObjectsProperty = DependencyProperty.Register(
            nameof(SelectedObjects),
            typeof(IEnumerable),
            typeof(PropertyGrid),
            new PropertyMetadata(null, (s, e) => ((PropertyGrid)s).SelectedObjectsChanged(e)));

        /// <summary>
        /// Identifies the <see cref="SelectedTabIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedTabIndexProperty = DependencyProperty.Register(
            nameof(SelectedTabIndex),
            typeof(int),
            typeof(PropertyGrid),
            new PropertyMetadata(0, (s, e) => ((PropertyGrid)s).SelectedTabIndexChanged(e)));

        /// <summary>
        /// The selected tab id property.
        /// </summary>
        public static readonly DependencyProperty SelectedTabIdProperty = DependencyProperty.Register(
            nameof(SelectedTabId),
            typeof(string),
            typeof(PropertyGrid),
            new PropertyMetadata(null, (s, e) => ((PropertyGrid)s).SelectedTabChanged(e)));

        /// <summary>
        /// Identifies the <see cref="CheckBoxLayout"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CheckBoxLayoutProperty = DependencyProperty.Register(
            nameof(CheckBoxLayout),
            typeof(CheckBoxLayout),
            typeof(PropertyGrid),
            new PropertyMetadata(CheckBoxLayout.Header, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="ShowDeclaredOnly"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowDeclaredOnlyProperty = DependencyProperty.Register(
            nameof(ShowDeclaredOnly),
            typeof(bool),
            typeof(PropertyGrid),
            new PropertyMetadata(false, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="ShowDescriptionIcons"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowDescriptionIconsProperty = DependencyProperty.Register(
            nameof(ShowDescriptionIcons),
            typeof(bool),
            typeof(PropertyGrid),
            new PropertyMetadata(true, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="ShowReadOnlyProperties"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowReadOnlyPropertiesProperty = DependencyProperty.Register(
            nameof(ShowReadOnlyProperties),
            typeof(bool),
            typeof(PropertyGrid),
            new PropertyMetadata(true, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="TabHeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabHeaderTemplateProperty = DependencyProperty.Register(
            nameof(TabHeaderTemplate),
            typeof(DataTemplate),
            typeof(PropertyGrid), null);

        /// <summary>
        /// Identifies the <see cref="TabPageHeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabPageHeaderTemplateProperty = DependencyProperty.Register(
            nameof(TabPageHeaderTemplate),
            typeof(DataTemplate),
            typeof(PropertyGrid),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TabStripPlacement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabStripPlacementProperty = DependencyProperty.Register(
            nameof(TabStripPlacement),
            typeof(VerticalAlignment),
            typeof(PropertyGrid),
            new PropertyMetadata(VerticalAlignment.Top));

        /// <summary>
        /// Identifies the <see cref="ToolTipTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ToolTipTemplateProperty = DependencyProperty.Register(
            nameof(ToolTipTemplate),
            typeof(DataTemplate),
            typeof(PropertyGrid),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TabVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabVisibilityProperty = DependencyProperty.Register(
            nameof(TabVisibility),
            typeof(TabVisibility),
            typeof(PropertyGrid),
            new PropertyMetadata(TabVisibility.Visible, AppearanceChanged));

        /// <summary>
        /// Identifies the <see cref="ValidationErrorStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidationErrorStyleProperty = DependencyProperty.Register(
            nameof(ValidationErrorStyle),
            typeof(Style),
            typeof(PropertyGrid),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ValidationErrorTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidationErrorTemplateProperty = DependencyProperty.Register(
            nameof(ValidationErrorTemplate),
            typeof(DataTemplate),
            typeof(PropertyGrid),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="VerticalPropertySpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalPropertySpacingProperty = DependencyProperty.Register(
            nameof(VerticalPropertySpacing),
            typeof(int),
            typeof(PropertyGrid),
            new PropertyMetadata(2, AppearanceChanged));

        /// <summary>
        /// The panel part name.
        /// </summary>
        private const string PartPanel = "PART_Panel";

        /// <summary>
        /// The scroll control part name.
        /// </summary>
        private const string PartScrollViewer = "PART_ScrollViewer";

        /// <summary>
        /// The tab control part name.
        /// </summary>
        private const string PartTabs = "PART_Tabs";

        /// <summary>
        /// The value to visibility converter.
        /// </summary>
        private static readonly ValueToVisibilityConverter ValueToVisibilityConverter = new ValueToVisibilityConverter();

        /// <summary>
        /// The value to boolean converter.
        /// </summary>
        private static readonly ValueToBooleanConverter ValueToBooleanConverter = new ValueToBooleanConverter();

        /// <summary>
        /// Converts a list of values to a boolean value. Returns <c>true</c> if all values equal the converter parameter.
        /// </summary>
        //private static readonly AllMultiValueConverter AllMultiValueConverter = new AllMultiValueConverter();

        /// <summary>
        /// The <c>null</c> to boolean converter.
        /// </summary>
        private static readonly NullToBoolConverter NullToBoolConverter = new NullToBoolConverter { NullValue = false };

        /// <summary>
        /// The zero to visibility converter.
        /// </summary>
        private static readonly ZeroToVisibilityConverter ZeroToVisibilityConverter = new ZeroToVisibilityConverter();

        /// <summary>
        /// The current selected object type.
        /// </summary>
        private Type currentSelectedObjectType;

        /// <summary>
        /// The panel control.
        /// </summary>
        private StackPanel panelControl;

        /// <summary>
        /// The scroll viewer.
        /// </summary>
        private ScrollViewer scrollViewer;

        /// <summary>
        /// The tab control.
        /// </summary>
        private TabView tabView;

        /// <summary>
        /// Initializes static members of the <see cref="PropertyGrid" /> class.
        /// </summary>
        public PropertyGrid()
        {
            DefaultStyleKey = typeof(PropertyGrid);
        }

        /// <summary>
        /// Gets or sets the category control template.
        /// </summary>
        /// <value>The category control template.</value>
        public ControlTemplate CategoryControlTemplate
        {
            get
            {
                return (ControlTemplate)this.GetValue(CategoryControlTemplateProperty);
            }

            set
            {
                this.SetValue(CategoryControlTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the type of the category control.
        /// </summary>
        /// <value>The type of the category control.</value>
        public CategoryControlType CategoryControlType
        {
            get
            {
                return (CategoryControlType)this.GetValue(CategoryControlTypeProperty);
            }

            set
            {
                this.SetValue(CategoryControlTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the category header template.
        /// </summary>
        /// <value>The category header template.</value>
        public DataTemplate CategoryHeaderTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(CategoryHeaderTemplateProperty);
            }

            set
            {
                this.SetValue(CategoryHeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets CurrentObject.
        /// </summary>
        public object CurrentObject { get; set; }

        /// <summary>
        /// Gets or sets the description icon.
        /// </summary>
        /// <value>The description icon.</value>
        public ImageSource DescriptionIcon
        {
            get
            {
                return (ImageSource)this.GetValue(DescriptionIconProperty);
            }

            set
            {
                this.SetValue(DescriptionIconProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the alignment for description icons.
        /// </summary>
        /// <value>The description icon alignment.</value>
        public HorizontalAlignment DescriptionIconAlignment
        {
            get
            {
                return (HorizontalAlignment)this.GetValue(DescriptionIconAlignmentProperty);
            }

            set
            {
                this.SetValue(DescriptionIconAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether label column resizing is enabled.
        /// </summary>
        public bool EnableLabelWidthResizing
        {
            get
            {
                return (bool)this.GetValue(EnableLabelWidthResizingProperty);
            }

            set
            {
                this.SetValue(EnableLabelWidthResizingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of values to show for radio buttons lists.
        /// </summary>
        /// <value>The limit.</value>
        public int EnumAsRadioButtonsLimit
        {
            get
            {
                return (int)this.GetValue(EnumAsRadioButtonsLimitProperty);
            }

            set
            {
                this.SetValue(EnumAsRadioButtonsLimitProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the indentation.
        /// </summary>
        /// <value>
        /// The indentation.
        /// </value>
        public double Indentation
        {
            get
            {
                return (double)this.GetValue(IndentationProperty);
            }

            set
            {
                this.SetValue(IndentationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum width of the label.
        /// </summary>
        /// <value>The maximum width of the label.</value>
        public double MaximumLabelWidth
        {
            get
            {
                return (double)this.GetValue(MaximumLabelWidthProperty);
            }

            set
            {
                this.SetValue(MaximumLabelWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum width of the property labels.
        /// </summary>
        /// <value>The minimum width.</value>
        public double MinimumLabelWidth
        {
            get
            {
                return (double)this.GetValue(MinimumLabelWidthProperty);
            }

            set
            {
                this.SetValue(MinimumLabelWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to move focus on unhandled Enter key down events.
        /// </summary>
        /// <value><c>true</c> if move focus on enter; otherwise, <c>false</c> .</value>
        public bool MoveFocusOnEnter
        {
            get
            {
                return (bool)this.GetValue(MoveFocusOnEnterProperty);
            }

            set
            {
                this.SetValue(MoveFocusOnEnterProperty, value);
            }
        }

        /// <summary> 
        /// Gets or sets the duration of the tool tip. 
        /// </summary> 
        /// <value> 
        /// The duration of the tool tip. 
        /// </value> 
        public int ToolTipDuration
        {
            get
            {
                return (int)this.GetValue(ToolTipDurationProperty);
            }

            set
            {
                this.SetValue(ToolTipDurationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the control factory.
        /// </summary>
        public IPropertyGridControlFactory ControlFactory
        {
            get
            {
                return (IPropertyGridControlFactory)this.GetValue(ControlFactoryProperty);
            }

            set
            {
                this.SetValue(ControlFactoryProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        /// <value>The operator.</value>
        public IPropertyGridOperator Operator
        {
            get
            {
                return (IPropertyGridOperator)this.GetValue(OperatorProperty);
            }

            set
            {
                this.SetValue(OperatorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        public object SelectedObject
        {
            get
            {
                return this.GetValue(SelectedObjectProperty);
            }

            set
            {
                this.SetValue(SelectedObjectProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected objects.
        /// </summary>
        /// <value>The selected objects.</value>
        public IEnumerable SelectedObjects
        {
            get
            {
                return (IEnumerable)this.GetValue(SelectedObjectsProperty);
            }

            set
            {
                this.SetValue(SelectedObjectsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the index of the selected tab.
        /// </summary>
        /// <value>The index of the selected tab.</value>
        public int SelectedTabIndex
        {
            get
            {
                return (int)this.GetValue(SelectedTabIndexProperty);
            }

            set
            {
                this.SetValue(SelectedTabIndexProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected tab id.
        /// </summary>
        public string SelectedTabId
        {
            get
            {
                return (string)this.GetValue(SelectedTabIdProperty);
            }

            set
            {
                this.SetValue(SelectedTabIdProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the check box layout.
        /// </summary>
        /// <value>The check box layout.</value>
        public CheckBoxLayout CheckBoxLayout
        {
            get
            {
                return (CheckBoxLayout)this.GetValue(CheckBoxLayoutProperty);
            }

            set
            {
                this.SetValue(CheckBoxLayoutProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show description icons.
        /// </summary>
        public bool ShowDescriptionIcons
        {
            get
            {
                return (bool)this.GetValue(ShowDescriptionIconsProperty);
            }

            set
            {
                this.SetValue(ShowDescriptionIconsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tab header template.
        /// </summary>
        /// <value>The tab header template.</value>
        public DataTemplate TabHeaderTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(TabHeaderTemplateProperty);
            }

            set
            {
                this.SetValue(TabHeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tab page header template.
        /// </summary>
        /// <value>The tab page header template.</value>
        public DataTemplate TabPageHeaderTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(TabPageHeaderTemplateProperty);
            }

            set
            {
                this.SetValue(TabPageHeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tab strip placement.
        /// </summary>
        /// <value>The tab strip placement.</value>
        public VerticalAlignment TabStripPlacement
        {
            get
            {
                return (VerticalAlignment)this.GetValue(TabStripPlacementProperty);
            }

            set
            {
                this.SetValue(TabStripPlacementProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tool tip template.
        /// </summary>
        /// <value>The tool tip template.</value>
        public DataTemplate ToolTipTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(ToolTipTemplateProperty);
            }

            set
            {
                this.SetValue(ToolTipTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the tab visibility state.
        /// </summary>
        /// <value>The tab visibility state.</value>
        public TabVisibility TabVisibility
        {
            get
            {
                return (TabVisibility)this.GetValue(TabVisibilityProperty);
            }

            set
            {
                this.SetValue(TabVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the validation error style.
        /// </summary>
        /// <value>The validation error style.</value>
        public Style ValidationErrorStyle
        {
            get
            {
                return (Style)this.GetValue(ValidationErrorStyleProperty);
            }

            set
            {
                this.SetValue(ValidationErrorStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the validation error template.
        /// </summary>
        /// <value>The validation error template.</value>
        public DataTemplate ValidationErrorTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(ValidationErrorTemplateProperty);
            }

            set
            {
                this.SetValue(ValidationErrorTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the required attribute type.
        /// </summary>
        /// <value>The required attribute type.</value>
        public Type RequiredAttribute
        {
            get
            {
                return (Type)this.GetValue(RequiredAttributeProperty);
            }

            set
            {
                this.SetValue(RequiredAttributeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show declared properties only.
        /// </summary>
        /// <value><c>true</c> if only declared properties should be shown; otherwise, <c>false</c> .</value>
        public bool ShowDeclaredOnly
        {
            get
            {
                return (bool)this.GetValue(ShowDeclaredOnlyProperty);
            }

            set
            {
                this.SetValue(ShowDeclaredOnlyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show read only properties].
        /// </summary>
        /// <value><c>true</c> if read only properties should be shown; otherwise, <c>false</c> .</value>
        public bool ShowReadOnlyProperties
        {
            get
            {
                return (bool)this.GetValue(ShowReadOnlyPropertiesProperty);
            }

            set
            {
                this.SetValue(ShowReadOnlyPropertiesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value of the vertical spacing between property items.
        /// </summary>
        /// <value><c>true</c> if read only properties should be shown; otherwise, <c>false</c> .</value>
        public int VerticalPropertySpacing
        {
            get
            {
                return (int)this.GetValue(VerticalPropertySpacingProperty);
            }

            set
            {
                this.SetValue(VerticalPropertySpacingProperty, value);
            }
        }
    }
}

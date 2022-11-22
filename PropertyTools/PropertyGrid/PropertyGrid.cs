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
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Media;
    using Microsoft.UI.Xaml.Controls.Primitives;
  
    using PropertyTools.DataAnnotations;
    using Microsoft.UI.Xaml.Input;
    using Windows.System;
    using Microsoft.UI;
    using Microsoft.UI.Xaml.Automation;

    /// <summary>
    /// The property control.
    /// </summary>
    [TemplatePart(Name = PartTabs, Type = typeof(TabView))]
    [TemplatePart(Name = PartPanel, Type = typeof(StackPanel))]
    [TemplatePart(Name = PartScrollViewer, Type = typeof(ScrollViewer))]
    public partial class PropertyGrid : Control, IPropertyGridOptions
    {
        /// <summary>
        /// Creates the controls.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="tabs">The tabs.</param>
        public virtual void CreateControls(object instance, IEnumerable<Tab> tabs)
        {
            if (this.tabView == null)
            {
                return;
            }

            this.tabView.TabItems.Clear();
            this.panelControl.Children.Clear();

            this.tabView.DataContext = instance;

            // Set padding to zero - control margin on the tab items instead
            this.tabView.Padding = new Thickness(0);

            this.tabView.Visibility = Visibility.Visible;
            this.scrollViewer.Visibility = Visibility.Collapsed;

            if (tabs == null)
            {
                return;
            }

            foreach (var tab in tabs)
            {
                bool fillTab = tab.Groups.Count == 1 && tab.Groups[0].Properties.Count == 1
                               && tab.Groups[0].Properties[0].FillTab;

                // Create the panel for the tab content
                var tabPanel = new Grid();
                var tabItem = new TabViewItem() { Header = tab, IsClosable = false, Name = tab.Id ?? string.Empty };
               
                this.ControlFactory.UpdateTabForValidationResults(tab, instance);

                if (fillTab)
                {
                    tabItem.Content = tabPanel;
                }
                else
                {
                    tabItem.Content = new ScrollViewer
                    {
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                        Content = tabPanel,
                        IsTabStop = false
                    };
                }

                this.tabView.TabItems.Add(tabItem);

                // set no margin if 'fill tab' and no tab page header
                tabPanel.Margin = new Thickness(fillTab && this.TabPageHeaderTemplate == null ? 0 : 4);

                if (this.TabHeaderTemplate != null)
                {
                    tabItem.Header = tab;
                    tabItem.HeaderTemplate = this.TabHeaderTemplate;
                }

                this.AddTabPageHeader(tab, tabPanel);

                int i = 0;
                foreach (var g in tab.Groups)
                {
                    var groupPanel = this.CreatePropertyPanel(g, tabPanel, i++, fillTab);
                    foreach (var pi in g.Properties)
                    {
                        // create and add the property panel (label, tooltip icon and property control)
                        this.AddPropertyPanel(groupPanel, pi, instance, tab);
                    }
                }
            }

            int index = this.tabView.SelectedIndex;
            if (index >= this.tabView.TabItems.Count || (uint)index == 0xffffffff)
            {
                index = 0;
            }

            if (this.tabView.TabItems.Count > 0)
            {
                this.tabView.SelectedItem = this.tabView.TabItems[index];
            }
        }

        /// <summary>
        /// Creates the controls (not using tab control).
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="tabs">The tab collection.</param>
        public virtual void CreateControlsTabless(object instance, IEnumerable<Tab> tabs)
        {
            if (this.tabView == null)
            {
                return;
            }

            this.tabView.TabItems.Clear();
            this.panelControl.Children.Clear();
            this.tabView.Visibility = Visibility.Collapsed;
            this.scrollViewer.Visibility = Visibility.Visible;

            if (tabs == null)
            {
                return;
            }

            this.panelControl.DataContext = instance;

            foreach (var tab in tabs)
            {
                // Create the panel for the properties
                var panel = new Grid();
                this.panelControl.Children.Add(panel);

                this.AddTabPageHeader(tab, panel);

                // Add the groups
                int i = 0;
                foreach (var g in tab.Groups)
                {
                    var propertyPanel = this.CreatePropertyPanel(g, panel, i++, false);

                    foreach (var pi in g.Properties)
                    {
                        // create and add the property panel (label, tooltip icon and property control)
                        this.AddPropertyPanel(propertyPanel, pi, instance, tab);
                    }
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see
        /// cref="M:System.Windows.FrameworkElement.ApplyTemplate" /> .
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.tabView = GetTemplateChild(PartTabs) as TabView;
            this.panelControl = GetTemplateChild(PartPanel) as StackPanel;
            this.scrollViewer = GetTemplateChild(PartScrollViewer) as ScrollViewer;
            this.UpdateControls();
        }

        /// <summary>
        /// Creates a tool tip.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>
        /// The tool tip.
        /// </returns>
        protected virtual object CreateToolTip(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }

            if (this.ToolTipTemplate == null)
            {
                return content;
            }

            return new ToolTip { ContentTemplate = this.ToolTipTemplate, Content = content };
        }

        /// <summary>
        /// Invoked when an unhandled KeyDown attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains the event data.</param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);
            if (this.MoveFocusOnEnter && e.Key == VirtualKey.Enter)
            {
                var textBox = e.OriginalSource as TextBox;
                if (textBox != null)
                {
                    if (textBox.AcceptsReturn)
                    {
                        return;
                    }

                    var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
                    if (bindingExpression != null)
                    {
                        bindingExpression.UpdateSource();
                    }
                }

                FocusManager.TryMoveFocus(FocusNavigationDirection.Previous);
 
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called when the selected object is changed.
        /// </summary>
        /// <param name="e">The e.</param>
        protected virtual void OnSelectedObjectChanged(DependencyPropertyChangedEventArgs e)
        {
            this.CurrentObject = this.SelectedObject;
            this.UpdateControls();
        }

        /// <summary>
        /// The appearance changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The e.</param>
        private static void AppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PropertyGrid)d).UpdateControls();
        }

        /// <summary>
        /// Creates the content control and property panel.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="tabItems">The tab items.</param>
        /// <param name="index">The index.</param>
        /// <param name="fillTab">Stretch the panel if set to <c>true</c>.</param>
        /// <returns>
        /// The property panel.
        /// </returns>
        private Grid CreatePropertyPanel(Group g, Grid tabItems, int index, bool fillTab)
        {
            var p = new Grid();
            if (fillTab)
            {
                tabItems.Children.Add(p);
                Grid.SetRow(p, tabItems.RowDefinitions.Count);
                tabItems.RowDefinitions.Add(
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }
            else 
            { 
                ContentControl groupContentControl = null;
                switch (this.CategoryControlType)
                {
                    case CategoryControlType.GroupBox:
                        groupContentControl = new GroupBox { Margin = new Thickness(0, 4, 0, 4) };
                        break;
                    case CategoryControlType.Expander:
                        groupContentControl = new Expander { IsExpanded = index == 0 };
                        break;
                    case CategoryControlType.Template:
                        groupContentControl = new HeaderedContent
                        {
                            Template = this.CategoryControlTemplate,
                            IsTabStop = false
                        };
                        break;
                    case CategoryControlType.None:
                        groupContentControl = new ContentControl();
                        break;
                }
            
                if (groupContentControl != null)
                {
                    if (groupContentControl is HeaderedContent headeredContentControl)
                    {
                        if (this.CategoryHeaderTemplate != null)
                        {
                            headeredContentControl.HeaderTemplate = this.CategoryHeaderTemplate;
                            headeredContentControl.Header = g;
                        }
                        else
                        {
                            headeredContentControl.Header = g.Header;
                        }
                    }
                    else if (groupContentControl is GroupBox groupBox)
                    {
                        if (this.CategoryHeaderTemplate != null)
                        {
                            groupBox.HeaderTemplate = this.CategoryHeaderTemplate;
                            groupBox.Header = g;
                        }
                        else
                        {
                            groupBox.Header = g.Header;
                        }
                    }
 
                    groupContentControl.Content = p;
                    tabItems.Children.Add(groupContentControl);
                    Grid.SetRow(groupContentControl, tabItems.RowDefinitions.Count);

                    tabItems.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                }
            }

            //Add Label Column and Control Coulmn
            var labelColumn = new ColumnDefinition
            {
                Width = GridLength.Auto,
                MinWidth = this.MinimumLabelWidth,
                MaxWidth = this.MaximumLabelWidth,
            };

            p.ColumnDefinitions.Add(labelColumn);
            p.ColumnDefinitions.Add(new ColumnDefinition());
            p.RowSpacing = VerticalPropertySpacing;

            return p;
        }

        /// <summary>
        /// Adds the tab page header if TabPageHeaderTemplate is specified.
        /// </summary>
        /// <param name="tab">The tab.</param>
        /// <param name="panel">The tab panel.</param>
        private void AddTabPageHeader(Tab tab, Grid panel)
        {
            if (this.TabPageHeaderTemplate == null)
            {
                return;
            }

            panel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            var hc = new ContentControl
            {
                IsTabStop = false,
                ContentTemplate = this.TabPageHeaderTemplate,
                Content = tab
            };
            panel.Children.Add(hc);
        }

        /// <summary>
        /// Creates and adds the property panel.
        /// </summary>
        /// <param name="panel">The panel where the property panel should be added.</param>
        /// <param name="pi">The property.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="tab">The tab.</param>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private void AddPropertyPanel(Grid propertyPanel, PropertyItem pi, object instance, Tab tab)
        {
            int nRow = propertyPanel.RowDefinitions.Count;
            var rd = new RowDefinition
            {
                Height = pi.FillTab ? new GridLength(1, GridUnitType.Star) : GridLength.Auto
            };
            propertyPanel.RowDefinitions.Add(rd);

            var propertyLabel = this.CreateLabel(pi);
            var propertyControl = this.CreatePropertyControl(pi);
            ContentControl errorControl = null;
            if (propertyControl != null)
            {
                this.ConfigurePropertyControl(pi, propertyControl);

                if (instance is IDataErrorInfo || instance is INotifyDataErrorInfo)
                {
                    PropertyControlFactoryOptions options = new PropertyControlFactoryOptions
                    {
                        ValidationErrorTemplate = this.ValidationErrorTemplate,
                        ValidationErrorStyle = this.ValidationErrorStyle
                    };

                    this.ControlFactory.SetValidationErrorStyle(propertyControl, options);
                    errorControl = this.ControlFactory.CreateErrorControl(pi, instance, tab, options);

                    // Add a row with the error control to the panel
                    // The error control is placed in column 1
                    propertyPanel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    propertyPanel.Children.Add(errorControl);
                    Grid.SetRow(errorControl, 1);
                    Grid.SetColumn(errorControl, 1);
                }

                Grid.SetRow(propertyControl, nRow);
                Grid.SetColumn(propertyControl, 1);
            }
            AddLabel(pi, propertyPanel, ref propertyLabel, propertyControl, errorControl, nRow);

            // add the property control
            if (propertyControl != null)
            {
                propertyPanel.Children.Add(propertyControl);
            }

            this.ConfigureLabel(pi, propertyLabel);
            this.ConfigureControl(pi, propertyControl);
            this.ConfigurePanel(pi, propertyPanel);
        }

        /// <summary>
        /// Configures the panel.
        /// </summary>
        private void ConfigurePanel(PropertyItem pi, Grid propertyPanel)
        {
            if (pi.IsVisibleDescriptor != null)
            {
                var isVisibleBinding = new Binding();
                isVisibleBinding.Path = new PropertyPath(pi.IsVisibleDescriptor.Name);
                isVisibleBinding.ConverterParameter = pi.IsVisibleValue;
                isVisibleBinding.Converter = ValueToVisibilityConverter;
                propertyPanel.SetBinding(VisibilityProperty, isVisibleBinding);
            }

            if (this.EnableLabelWidthResizing && pi.HeaderPlacement == HeaderPlacement.Left)
            {
                propertyPanel.Children.Add(
                    new GridSplitter
                    {
                        Width = 4,
                        Background = new SolidColorBrush(Colors.Transparent),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        IsTabStop = false,
                    });
            }
        }

        /// <summary>
        /// Configures the property control
        /// </summary>
        private void ConfigureControl(PropertyItem pi, FrameworkElement propertyControl)
        {
            // Set the IsEnabled binding of the property control
            if (pi.IsEnabledDescriptor != null && propertyControl != null)
            {
                var isEnabledBinding = new Binding();
                isEnabledBinding.Path = new PropertyPath(pi.IsEnabledDescriptor.Name);
                if (pi.IsEnabledValue != null)
                {
                    isEnabledBinding.ConverterParameter = pi.IsEnabledValue;
                    isEnabledBinding.Converter = ValueToBooleanConverter;
                }

                //var currentBindingExpression = propertyControl.GetBindingExpression(IsEnabledProperty);
                //if (currentBindingExpression != null)
                //{
                //    var multiBinding = new MultiBinding();
                //    multiBinding.Bindings.Add(isEnabledBinding);
                //    multiBinding.Bindings.Add(currentBindingExpression.ParentBinding);
                //    multiBinding.Converter = AllMultiValueConverter;
                //    multiBinding.ConverterParameter = true;
                //    propertyControl.SetBinding(IsEnabledProperty, multiBinding);
                //}
                //else
                {
                    propertyControl.SetBinding(IsEnabledProperty, isEnabledBinding);
                }
            }
        }

        /// <summary>
        /// Configures the label.
        /// </summary>
        private void ConfigureLabel(PropertyItem pi, FrameworkElement propertyLabel)
        {
            if (pi.IsEnabledDescriptor != null && propertyLabel != null)
            {
                var isEnabledBinding = new Binding();
                isEnabledBinding.Path = new PropertyPath(pi.IsEnabledDescriptor.Name);
                if (pi.IsEnabledValue != null)
                {
                    isEnabledBinding.ConverterParameter = pi.IsEnabledValue;
                    isEnabledBinding.Converter = ValueToBooleanConverter;
                }

                propertyLabel.SetBinding(IsEnabledProperty, isEnabledBinding);
            }
        }

        /// <summary>
        /// Adds the label.
        /// </summary>
        private void AddLabel(PropertyItem pi, Grid propertyPanel, ref FrameworkElement propertyLabel, FrameworkElement propertyControl, ContentControl errorControl, int nRow)
        {
            var actualHeaderPlacement = pi.HeaderPlacement;

            if (propertyControl is CheckBox checkBoxPropertyControl)
            {
                if (this.CheckBoxLayout != CheckBoxLayout.Header)
                {
                    checkBoxPropertyControl.Content = propertyLabel;
                    propertyLabel = null;
                }

                if (this.CheckBoxLayout == CheckBoxLayout.CollapseHeader)
                {
                    actualHeaderPlacement = HeaderPlacement.Collapsed;
                }
            }

            switch (actualHeaderPlacement)
            {
                case HeaderPlacement.Hidden:
                    break;

                case HeaderPlacement.Collapsed:
                    {
                        if (propertyControl != null)
                        {
                            Grid.SetColumn(propertyControl, 0);
                            Grid.SetColumnSpan(propertyControl, 2);
                        }

                        break;
                    }

                default:
                    {
                        // create the label panel
                        var labelPanel = new DockPanel();
                        Grid.SetRow(labelPanel, nRow);
                        Grid.SetColumn(labelPanel, 0);

                        if (pi.HeaderPlacement == HeaderPlacement.Left)
                        {
                            DockPanel.SetDock(labelPanel, Dock.Left);
                        }
                        else
                        {
                            // Above
                            if (propertyControl != null)
                            {
                                propertyPanel.RowDefinitions.Add(new RowDefinition());
                                Grid.SetColumnSpan(labelPanel, 2);
                                Grid.SetRow(propertyControl, nRow + 1);
                                Grid.SetColumn(propertyControl, 0);
                                Grid.SetColumnSpan(propertyControl, 2);
                                if (errorControl != null)
                                {
                                    Grid.SetRow(errorControl, nRow + 2);
                                    Grid.SetColumn(errorControl, 0);
                                    Grid.SetColumnSpan(errorControl, 2);
                                }
                            }
                        }

                        propertyPanel.Children.Add(labelPanel);

                        if (propertyLabel != null)
                        {
                            DockPanel.SetDock(propertyLabel, Dock.Left);
                            labelPanel.Children.Add(propertyLabel);
                        }

                        if (this.ShowDescriptionIcons && this.DescriptionIcon != null)
                        {
                            if (!string.IsNullOrWhiteSpace(pi.Description))
                            {
                                var descriptionIconImage = new Image
                                {
                                    Source = this.DescriptionIcon,
                                    Stretch = Stretch.None,
                                    Margin = new Thickness(0, 4, 4, 4),
                                    VerticalAlignment = VerticalAlignment.Top,
                                    HorizontalAlignment = this.DescriptionIconAlignment
                                };

                                // RenderOptions.SetBitmapScalingMode(descriptionIconImage, BitmapScalingMode.NearestNeighbor);
                                labelPanel.Children.Add(descriptionIconImage);
                                if (!string.IsNullOrWhiteSpace(pi.Description))
                                {
                                    ToolTipService.SetToolTip(descriptionIconImage, this.CreateToolTip(pi.Description));
                                }
                            }
                        }
                        else if(!string.IsNullOrWhiteSpace(pi.Description))
                        {
                            ToolTipService.SetToolTip(labelPanel, this.CreateToolTip(pi.Description));
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Configures the property control.
        /// </summary>
        private void ConfigurePropertyControl(PropertyItem pi, FrameworkElement propertyControl)
        {
            if (!double.IsNaN(pi.Width))
            {
                propertyControl.Width = pi.Width;
                propertyControl.HorizontalAlignment = HorizontalAlignment.Left;
            }

            if (!double.IsNaN(pi.Height))
            {
                propertyControl.Height = pi.Height;
            }

            if (!double.IsNaN(pi.MinimumHeight))
            {
                propertyControl.MinHeight = pi.MinimumHeight;
            }

            if (!double.IsNaN(pi.MaximumHeight))
            {
                propertyControl.MaxHeight = pi.MaximumHeight;
            }

            if (pi.IsOptional)
            {
                propertyControl.SetBinding(
                    IsEnabledProperty,
                    pi.OptionalDescriptor != null ? pi.OptionalDescriptor.Name : pi.Descriptor.Name, NullToBoolConverter);
            }

            if (pi.IsEnabledByRadioButton)
            {
                propertyControl.SetBinding(
                    IsEnabledProperty,
                    pi.RadioDescriptor.Name, new EnumToBooleanConverter() { EnumType = pi.RadioDescriptor.PropertyType }, pi.RadioValue);
            }
        }

        /// <summary>
        /// Creates the label control.
        /// </summary>
        /// <param name="pi">The property item.</param>
        /// <returns>
        /// An element.
        /// </returns>
        private FrameworkElement CreateLabel(PropertyItem pi)
        {
            var indentation = pi.IndentationLevel * this.Indentation;

            FrameworkElement propertyLabel = null;
            if (pi.IsOptional)
            {
                var cb = new CheckBox
                {
                    Content = pi.DisplayName,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5 + indentation, 0, 4, 0)
                };

                cb.SetBinding(
                    ToggleButton.IsCheckedProperty,
                    pi.OptionalDescriptor != null ? pi.OptionalDescriptor.Name : pi.Descriptor.Name, NullToBoolConverter);

                var g = new Grid();
                g.Children.Add(cb);
                propertyLabel = g;
            }

            if (pi.IsEnabledByRadioButton)
            {
                var rb = new RadioButton
                {
                    Content = pi.DisplayName,
                    GroupName = pi.RadioDescriptor.Name,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5 + indentation, 0, 4, 0)
                };

                var converter = new EnumToBooleanConverter { EnumType = pi.RadioDescriptor.PropertyType };
                rb.SetBinding(
                    ToggleButton.IsCheckedProperty,
                    pi.RadioDescriptor.Name, converter, pi.RadioValue);

                var g = new Grid();
                g.Children.Add(rb);
                propertyLabel = g;
            }

            if (propertyLabel == null)
            {
                propertyLabel = new TextBlock
                {
                    Text = pi.DisplayName,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(indentation, 0, 4, 0)
                };
            }

            return propertyLabel;
        }

        /// <summary>
        /// Creates the property control.
        /// </summary>
        /// <param name="pi">The property item.</param>
        /// <returns>
        /// An element.
        /// </returns>
        private FrameworkElement CreatePropertyControl(PropertyItem pi)
        {
            var options = new PropertyControlFactoryOptions { EnumAsRadioButtonsLimit = this.EnumAsRadioButtonsLimit };
            var control = this.ControlFactory.CreateControl(pi, options);
            if (control != null)
            {
                control.SetValue(AutomationProperties.AutomationIdProperty, pi.PropertyName);
            }

            return control;
        }

        /// <summary>
        /// Handles changes in SelectedObjects.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private void SelectedObjectsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                if (e.OldValue is INotifyCollectionChanged notifyCollectionChanged)
                {
                    //Mark
                    //CollectionChangedEventManager.RemoveHandler(notifyCollectionChanged, this.OnSelectedObjectsCollectionChanged);
                }
            }

            if (e.NewValue != null)
            {
                if (e.NewValue is INotifyCollectionChanged notifyCollectionChanged)
                {
                    //Mark
                    //CollectionChangedEventManager.AddHandler(notifyCollectionChanged, this.OnSelectedObjectsCollectionChanged);
                }
                else if (e.NewValue is IEnumerable enumerable)
                {
                    this.SetCurrentObjectFromSelectedObjects(enumerable);
                }
            }
            else
            {
                this.CurrentObject = null;
                this.UpdateControls();
            }
        }

        /// <summary>
        /// Called when the selected objects collection is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnSelectedObjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is IEnumerable enumerable)
            {
                this.SetCurrentObjectFromSelectedObjects(enumerable);
            }
        }

        /// <summary>
        /// Set CurrentObject when SelectedObjects is changed.
        /// </summary>
        /// <param name="enumerable">SelectedObjects.</param>
        private void SetCurrentObjectFromSelectedObjects(IEnumerable enumerable)
        {
            var list = enumerable.Cast<object>().ToList();
            if (list.Count == 0)
            {
                this.CurrentObject = null;
            }
            else if (list.Count == 1)
            {
                this.CurrentObject = list[0];
            }
            else
            {
                this.CurrentObject = new ItemsBag(list);
            }

            this.UpdateControls();
        }

        /// <summary>
        /// Updates the controls.
        /// </summary>
        private void UpdateControls()
        {
            if (this.Operator == null)
            {
                return;
            }

            var oldIndex = this.tabView != null ? this.tabView.SelectedIndex : -1;

            var tabs = this.Operator.CreateModel(this.CurrentObject, false, this);
            var tabsArray = tabs != null ? tabs.ToArray() : null;
            var areTabsVisible = this.TabVisibility == TabVisibility.Visible
                                 || (this.TabVisibility == TabVisibility.VisibleIfMoreThanOne && tabsArray != null && tabsArray.Length > 1);
            if (areTabsVisible)
            {
                this.CreateControls(this.CurrentObject, tabsArray);
            }
            else
            {
                this.CreateControlsTabless(this.CurrentObject, tabsArray);
            }

            var newSelectedObjectType = this.CurrentObject != null ? this.CurrentObject.GetType() : null;
            var currentObject = this.CurrentObject as ItemsBag;
            if (currentObject != null)
            {
                newSelectedObjectType = currentObject.BiggestType;
            }

            if (newSelectedObjectType == this.currentSelectedObjectType && this.tabView != null)
            {
                this.tabView.SelectedIndex = oldIndex;
            }

            if (this.tabView != null && this.tabView.SelectedItem is TabViewItem)
            {
                this.SelectedTabId = (this.tabView.SelectedItem as TabViewItem).Name;
            }

            this.currentSelectedObjectType = newSelectedObjectType;

        }

        /// <summary>
        /// Handles changes of the selected tab.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void SelectedTabChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.tabView == null)
            {
                return;
            }

            var tabId = e.NewValue as string;
            if (tabId == null)
            {
                this.tabView.SelectedIndex = 0;
                return;
            }

            var tab = this.tabView.TabItems.Cast<TabViewItem>().FirstOrDefault(t => t.Name == tabId);
            if (tab != null)
            {
                this.tabView.SelectedItem = tab;
            }
        }

        /// <summary>
        /// Handles changes of the selected tab index.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void SelectedTabIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.tabView == null)
            {
                return;
            }

            int tabIndex;
            int.TryParse(e.NewValue.ToString(), out tabIndex);
            if (tabIndex >= 0)
            {
                var tabItems = this.tabView.TabItems.Cast<TabViewItem>().ToArray();
                if (tabIndex < tabItems.Length)
                {
                    this.SelectedTabId = tabItems[tabIndex].Name;
                }
            }
        }
    }
}

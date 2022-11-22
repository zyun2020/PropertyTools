
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace PropertyTools
{
    public partial class HeaderedContent : ContentControl
    {
        private const string PartHeaderPresenter = "HeaderPresenter";

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderedContent"/> class.
        /// </summary>
        public HeaderedContent()
        {
            DefaultStyleKey = typeof(HeaderedContent);
        }

        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(object),
            typeof(HeaderedContent),
            new PropertyMetadata(null, OnHeaderChanged));

        /// <summary>
        /// Identifies the <see cref="HeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate",
            typeof(DataTemplate),
            typeof(HeaderedContent),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the data used for the header of each control.
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the template used to display the content of the control's header.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty SeparatorVisibleProperty = DependencyProperty.Register("IconVisible", typeof(Visibility),
           typeof(HeaderedContent), new PropertyMetadata(Visibility.Visible));
        public Visibility SeparatorVisible
        {
            get { return (Visibility)GetValue(SeparatorVisibleProperty); }
            set { SetValue(SeparatorVisibleProperty, value); }
        }
      
        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SetHeaderVisibility();
        }

        /// <summary>
        /// Called when the <see cref="Header"/> property changes.
        /// </summary>
        /// <param name="oldValue">The old value of the <see cref="Header"/> property.</param>
        /// <param name="newValue">The new value of the <see cref="Header"/> property.</param>
        protected virtual void OnHeaderChanged(object oldValue, object newValue)
        {
        }

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (HeaderedContent)d;
            control.SetHeaderVisibility();
            control.OnHeaderChanged(e.OldValue, e.NewValue);
        }

        private void SetHeaderVisibility()
        {
            if (GetTemplateChild(PartHeaderPresenter) is FrameworkElement headerPresenter)
            {
                if (Header is string headerText)
                {
                    headerPresenter.Visibility = string.IsNullOrEmpty(headerText)
                        ? Visibility.Collapsed
                        : Visibility.Visible;
                }
                else
                {
                    headerPresenter.Visibility = Header != null
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }

                if(headerPresenter.Visibility == Visibility.Collapsed)
                {
                    SeparatorVisible = Visibility.Collapsed;
                }
            }
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextBoxEx.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Represents a TextBox that can update the binding on enter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PropertyTools
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Input;
    using Windows.System;

    /// <summary>
    /// Represents a TextBox that can update the binding on enter.
    /// </summary>
    public class TextBoxEx : TextBox
    {
        /// <summary>
        /// Identifies the <see cref="MoveFocusOnEnter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MoveFocusOnEnterProperty = DependencyProperty.Register(
            nameof(MoveFocusOnEnter),
            typeof(bool),
            typeof(TextBoxEx),
            new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="UpdateBindingOnEnter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpdateBindingOnEnterProperty = DependencyProperty.Register(
            nameof(UpdateBindingOnEnter),
            typeof(bool),
            typeof(TextBoxEx),
            new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="SelectAllOnFocus"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectAllOnFocusProperty = DependencyProperty.Register(
            nameof(SelectAllOnFocus),
            typeof(bool),
            typeof(TextBoxEx),
            new PropertyMetadata(true));

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxEx" /> class.
        /// </summary>
        public TextBoxEx()
        {
            this.GotFocus += TextBoxEx_GotFocus;
        }

        private void TextBoxEx_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.FocusState == FocusState.Keyboard && this.SelectAllOnFocus)
            {
                this.SelectAll();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to select all on focus.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all should be selected; otherwise, <c>false</c>.
        /// </value>
        public bool SelectAllOnFocus
        {
            get { return (bool)this.GetValue(SelectAllOnFocusProperty); }
            set { this.SetValue(SelectAllOnFocusProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether MoveFocusOnEnter.
        /// </summary>
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
        /// Gets or sets a value indicating whether UpdateBindingOnEnter.
        /// </summary>
        public bool UpdateBindingOnEnter
        {
            get
            {
                return (bool)this.GetValue(UpdateBindingOnEnterProperty);
            }

            set
            {
                this.SetValue(UpdateBindingOnEnterProperty, value);
            }
        }

        /// <summary>
        /// The on preview key down.
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnPreviewKeyDown(KeyRoutedEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            switch (e.Key)
            {
                case VirtualKey.Enter:
                    if (!this.AcceptsReturn)
                    {
                        if (this.UpdateBindingOnEnter)
                        {
                            // get the binding to the Text property
                            var b = this.GetBindingExpression(TextProperty);
                            if (b != null)
                            {
                                // update the source (do not update the target)
                                b.UpdateSource();
                            }
                        }

                        if (this.MoveFocusOnEnter)
                        {
                            // Move focus to next element
                            bool shift = false;
                            FocusManager.TryMoveFocus(shift? FocusNavigationDirection.Previous : FocusNavigationDirection.Next);                           
                        }

                        e.Handled = true;
                    }

                    break;
                case VirtualKey.Escape:
                    this.Undo();
                    this.SelectAll();
                    e.Handled = true;
                    break;
            }
        }
    }
}
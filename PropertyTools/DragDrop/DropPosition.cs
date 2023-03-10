// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DropPosition.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Indicates where an item should be dropped.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PropertyTools
{
    /// <summary>
    /// Indicates where an item should be dropped.
    /// </summary>
    public enum DropPosition
    {
        /// <summary>
        /// Add the item to the target item.
        /// </summary>
        Add,

        /// <summary>
        /// Insert the item before the target item.
        /// </summary>
        InsertBefore,

        /// <summary>
        /// Insert the item after the target item.
        /// </summary>
        InsertAfter
    }
}
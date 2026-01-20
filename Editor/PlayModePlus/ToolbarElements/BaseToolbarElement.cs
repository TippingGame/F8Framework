using System;
using UnityEditor;

namespace F8Framework.Core.Editor
{
    /// <summary>
    /// The base contract for any element that can be displayed on the Custom Toolbar.
    /// To create a new element, inherit from this class.
    /// </summary>
    public abstract class BaseToolbarElement : IComparable<BaseToolbarElement>
    {
        /// <summary>
        /// The display name of the element in the settings UI.
        /// </summary>
        protected abstract string Name { get; }

        /// <summary>
        /// The tooltip displayed when hovering over the element in the toolbar.
        /// </summary>
        protected virtual string Tooltip => string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this element is enabled.
        /// If false, the element will not be shown.
        /// </summary>
        protected bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the width of the element in the toolbar, in pixels.
        /// </summary>
        protected float Width { get; set; } = 32;

        #region EventLoop

        /// <summary>
        /// One-time initialization logic for the element.
        /// Called once when the editor starts. Use this to load icons, cache styles or setup width.
        /// </summary>
        public virtual void OnInit()
        {
        }

        /// <summary>
        /// Called every time the play mode state changes (e.g., entering or exiting play mode).
        /// Allows the element to change its behavior dynamically.
        /// </summary>
        public virtual void OnPlayModeStateChanged(PlayModeStateChange state)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Defines how the element should be drawn in the main toolbar.
        /// </summary>
        public abstract void OnDrawInToolbar();

        #endregion

        public int CompareTo(BaseToolbarElement other)
        {
            return string.Compare(this.Name, other.Name, StringComparison.Ordinal);
        }
    }
}
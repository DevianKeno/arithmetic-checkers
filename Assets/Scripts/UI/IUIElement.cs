namespace Damath.UI
{
    /// <summary>
    /// Base interface for Damath UI elements.
    /// </summary>
    public interface IUIElement
    {
        public bool IsVisible { get; set; }   
        public void ToggleVisibility();
        public void SetVisible(bool visible);
    }
}

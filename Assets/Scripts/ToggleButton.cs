using UnityEngine;

namespace Damath
{
    public class ToggleButton : UnityEngine.UI.Toggle
    {
        public void ToggleColor(bool value)
        {
            if (value)
            {
                image.color = new (0.56f, 0.69f, 0.77f, 1f);
            } else
            {
                image.color = Color.white;
            }
        }
    }
}
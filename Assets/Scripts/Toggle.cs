using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.UI;

namespace Damath
{
    [CustomEditor(typeof(Toggle))]
    public class ToggleEditor : UnityEditor.UI.ToggleEditor
    {
        private SerializedProperty Value;
        private SerializedProperty knobRect;
        private SerializedProperty backgroundImage;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            Value = serializedObject.FindProperty("Value");
            knobRect = serializedObject.FindProperty("knobRect");
            backgroundImage = serializedObject.FindProperty("backgroundImage");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.PropertyField(Value);
            EditorGUILayout.PropertyField(knobRect);
            EditorGUILayout.PropertyField(backgroundImage);

            EditorGUILayout.Space();


            serializedObject.ApplyModifiedProperties();
        }
    }

    public class Toggle : UnityEngine.UI.Toggle, IHoverable
    {
        public bool Value = true;
        public float SpeedInSeconds = 0.5f;
        public Color BackgroundColorOff = new(0.37f, 0.37f, 0.37f, 1f);
        public Color BackgroundColorOn = new(0.2f, 0.33f, 0.46f, 1f);
        public Color KnobColor = new(1f, 1f, 1f, 1f);

        public bool IsHovered { get; set; }
        public event Action<PointerEventData> OnMouseEnter;
        public event Action<PointerEventData> OnMouseExit;

        [SerializeField] RectTransform knobRect;
        [SerializeField] Image backgroundImage;

        protected override void Awake()
        {
            knobRect = transform.Find("Knob").GetComponent<RectTransform>();
            backgroundImage = transform.Find("Background").GetComponent<Image>();
        }

        protected override void Start()
        {
            base.Start();
            
            SetValue(Value);
            onValueChanged.AddListener(SetValue);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {           
            base.OnPointerEnter(eventData);

            IsHovered = true;
            OnMouseEnter?.Invoke(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            IsHovered = false;
            OnMouseExit?.Invoke(eventData);
        }

        public bool GetValue()
        {
            return Value;
        }

        public void SetValue(bool value)
        {
            Value = value;
            interactable = false;

            SetInteractableAfter(true, SpeedInSeconds);

            if (Value)
            {
                LeanTween.move(knobRect, new Vector3(35f, 0f, 0f), SpeedInSeconds)
                .setEaseOutExpo();

                LeanTween.value(backgroundImage.gameObject, 0.1f, 1f, SpeedInSeconds)
                .setEaseOutExpo()
                .setOnUpdate( (i) =>
                {
                    backgroundImage.color = Color.Lerp(BackgroundColorOff, BackgroundColorOn, i);
                });
            } else
            {
                LeanTween.move(knobRect, new Vector3(-35f, 0f, 0f), SpeedInSeconds)
                .setEaseOutExpo();

                LeanTween.value(backgroundImage.gameObject, 0.1f, 1f, SpeedInSeconds)
                .setEaseOutExpo()
                .setOnUpdate( (i) =>
                {
                    backgroundImage.color = Color.Lerp(BackgroundColorOn, BackgroundColorOff, i);
                });
            }
        }

        async void SetInteractableAfter(bool value, float delayInSeconds)
        {
            await Task.Delay((int)(delayInSeconds * 1000));
            interactable = value;
        }
    }
}

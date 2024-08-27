using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace _Game.Scripts.UI
{
    /// <summary>
    /// CustomButton handles touch input, allowing for button press and release interactions.
    /// Optionally, it can scale the button down when pressed, with customizable timing.
    /// It requires an EventTrigger component to handle pointer events.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(EventTrigger))]
    public class CustomButton : MonoBehaviour
    {
        [Header("Button Settings")]
        [Tooltip("Should the button scale down when pressed?")]
        [SerializeField] private bool changeScaleOnPressed = true;

        [Header("Scale Settings")]
        [Tooltip("The multiplier to scale down the button when pressed.")]
        [Range(0.5f, 1f)]
        [SerializeField] private float scaleDownMultiplier = 0.85f;

        [Tooltip("The duration of the scale animation in seconds.")]
        [Range(0.1f, 1f)]
        [SerializeField] private float scaleAnimationDuration = 0.2f;

        [HideInInspector]
        public bool ButtonPressed { get; private set; } = false;

        private RectTransform _rectTransform;
        private Vector3 _initialScale;
        private EventTrigger _eventTrigger;

        private void Awake()
        {
            Initialize();
            SetupEventTrigger();
        }

        /// <summary>
        /// Initializes the button's transform and stores its initial scale.
        /// </summary>
        private void Initialize()
        {
            _rectTransform = GetComponent<RectTransform>();
            _initialScale = _rectTransform.localScale;
        }

        /// <summary>
        /// Sets up the EventTrigger component and configures pointer down and up events.
        /// </summary>
        private void SetupEventTrigger()
        {
            _eventTrigger = GetComponent<EventTrigger>();

            RegisterEvent(EventTriggerType.PointerDown, OnButtonDown);
            RegisterEvent(EventTriggerType.PointerUp, OnButtonUp);
        }

        /// <summary>
        /// Registers a callback for a specific EventTriggerType.
        /// </summary>
        /// <param name="eventType">The type of the event to listen for (e.g., PointerDown, PointerUp).</param>
        /// <param name="action">The method to invoke when the event is triggered.</param>
        private void RegisterEvent(EventTriggerType eventType, System.Action action)
        {
            EventTrigger.Entry eventEntry = new EventTrigger.Entry
            {
                eventID = eventType
            };
            eventEntry.callback.AddListener((data) => action.Invoke());
            _eventTrigger.triggers.Add(eventEntry);
        }

        /// <summary>
        /// Handles the button press interaction. Sets ButtonPressed to true and scales the button down if enabled.
        /// </summary>
        public void OnButtonDown()
        {
            ButtonPressed = true;
            if (changeScaleOnPressed)
            {
                // Use DOTween for smooth scaling animation with dynamic duration
                _rectTransform.DOScale(_initialScale * scaleDownMultiplier, scaleAnimationDuration).SetEase(Ease.OutQuad);
            }
        }

        /// <summary>
        /// Handles the button release interaction. Sets ButtonPressed to false and resets the button scale if enabled.
        /// </summary>
        public void OnButtonUp()
        {
            ButtonPressed = false;
            if (changeScaleOnPressed)
            {
                // Use DOTween for smooth scaling back animation with dynamic duration
                _rectTransform.DOScale(_initialScale, scaleAnimationDuration).SetEase(Ease.OutQuad);
            }
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace F8Framework.Core
{
    [AddComponentMenu("F8Framework/UI/ButtonExtension")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class ButtonExtension : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler, IUpdateSelectedHandler,
        IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
    {
        [Serializable]
        public sealed class ButtonEvent : UnityEvent
        {
        }

        private enum DragPassthroughMode
        {
            Disabled = 0,
            AutoDetect = 1,
            ScrollRectOnly = 2
        }

        private enum ScrollRectDragStrategy
        {
            Always = 0,
            MatchScrollRectAxis = 1,
            HorizontalOnly = 2,
            VerticalOnly = 3
        }

        private enum DragReceiver
        {
            None = 0,
            Slider = 1,
            Scrollbar = 2,
            ScrollRect = 3
        }
        
        [SerializeField] private Button _button;
        [SerializeField] private Transform _animationTarget;
        [SerializeField] private ScrollRect _parentScrollRect;
        [SerializeField] private Slider _parentSlider;
        [SerializeField] private Scrollbar _parentScrollbar;
        
        [SerializeField] private bool _forwardDragToParentScrollRect = true;
        [SerializeField] private bool _forwardScrollWheelToParentScrollRect = true;
        [SerializeField] private DragPassthroughMode _dragPassthroughMode = DragPassthroughMode.AutoDetect;
        [SerializeField] private ScrollRectDragStrategy _scrollRectDragStrategy = ScrollRectDragStrategy.Always;
        [SerializeField, Min(0f)] private float _dragStartThreshold = 12f;
        [SerializeField] private bool _cancelClickWhenDragging = false;
        [SerializeField] private bool _cancelLongPressWhenBeginDrag = false;
        
        [SerializeField] private bool _enableClickAnimation = true;
        [SerializeField, Min(0.01f)] private float _clickScaleMultiplier = 0.92f;
        [SerializeField, Min(0f)] private float _clickPressDuration = 0.06f;
        [SerializeField, Min(0f)] private float _clickReleaseDuration = 0.12f;
        [SerializeField] private Ease _clickPressEase = Ease.EaseOutQuad;
        [SerializeField] private Ease _clickReleaseEase = Ease.EaseOutBack;
        
        [SerializeField] private bool _enableSelectAnimation;
        [SerializeField] private Vector3 _selectedScaleMultiplier = new Vector3(1.05f, 1.05f, 1f);
        [SerializeField, Min(0f)] private float _selectAnimationDuration = 0.15f;
        [SerializeField] private Ease _selectEase = Ease.EaseOutBack;
        [SerializeField] private bool _loopSelectAnimation;
        [SerializeField, Min(0f)] private float _deselectAnimationDuration = 0.12f;
        [SerializeField] private Ease _deselectEase = Ease.EaseOutQuad;

        [SerializeField] private bool _enableHoverAnimation;
        [SerializeField] private Vector3 _hoverScaleMultiplier = new Vector3(1.05f, 1.05f, 1f);
        [SerializeField, Min(0f)] private float _hoverEnterDuration = 0.12f;
        [SerializeField] private Ease _hoverEnterEase = Ease.EaseOutQuad;
        [SerializeField, Min(0f)] private float _hoverExitDuration = 0.1f;
        [SerializeField] private Ease _hoverExitEase = Ease.EaseOutQuad;
        
        [SerializeField] private bool _playClickSound = true;
        [SerializeField] private string _clickSoundAssetName;
        [SerializeField] private bool _playHoverSound;
        [SerializeField] private string _hoverSoundAssetName;
        [SerializeField] private bool _playSelectSound;
        [SerializeField] private string _selectSoundAssetName;
        
        [SerializeField] private bool _enableDoubleClick;
        [SerializeField, Min(0.01f)] private float _doubleClickInterval = 0.3f;
        [SerializeField] private ButtonEvent _onDoubleClick = new ButtonEvent();
        
        [SerializeField] private bool _enableLongPress;
        [SerializeField, Min(0.01f)] private float _longPressDuration = 0.5f;
        [SerializeField] private bool _cancelLongPressWhenPointerExit = false;
        [SerializeField] private ButtonEvent _onLongPress = new ButtonEvent();
        
        [SerializeField] private bool _enableSubmitInputEnhancement;
        [SerializeField] private string _submitActionName = "Submit";
        [SerializeField] private bool _enableSubmitLongPress = true;
        [SerializeField] private bool _enableSubmitDoubleClick = true;
        
        [SerializeField] private ButtonEvent _onHoverEnter = new ButtonEvent();
        [SerializeField] private ButtonEvent _onHoverExit = new ButtonEvent();
        [SerializeField] private ButtonEvent _onSelect = new ButtonEvent();
        [SerializeField] private ButtonEvent _onDeselect = new ButtonEvent();

        private readonly object _tweenKey = new object();
        private Vector3 _defaultScale = Vector3.one;
        private bool _hasCachedDefaultScale;
        private bool _isSelected;
        private bool _isHovering;
        private bool _isPointerDown;
        private bool _isLongPressTriggered;
        private bool _ignoreNextClickFeedback;
        private bool _isDragging;
        private bool _isSubmitPressed;
        private bool _isSubmitLongPressTriggered;
        private bool _isSliderHandle;
        private bool _isScrollbarHandle;
        private DragReceiver _activeDragReceiver;
        private float _pointerDownTime = -1f;
        private float _lastClickTime = -10f;
        private float _submitPressTime = -1f;
        private float _lastSubmitTime = -10f;
        private Vector2 _pointerDownPosition;
        private int _pointerId = int.MinValue;

        public ButtonEvent OnDoubleClick => _onDoubleClick;
        public ButtonEvent OnLongPress => _onLongPress;
        public ButtonEvent OnHoverEnterEvent => _onHoverEnter;
        public ButtonEvent OnHoverExitEvent => _onHoverExit;
        public ButtonEvent OnSelectEvent => _onSelect;
        public ButtonEvent OnDeselectEvent => _onDeselect;

        private void Reset()
        {
            _enableClickAnimation = true;
            _playClickSound = true;
            _enableHoverAnimation = false;
            _playHoverSound = false;
            _enableSelectAnimation = false;
            _loopSelectAnimation = false;
            _playSelectSound = false;
            _enableDoubleClick = false;
            _enableLongPress = false;
            _enableSubmitInputEnhancement = false;
            _dragPassthroughMode = DragPassthroughMode.AutoDetect;
            _scrollRectDragStrategy = ScrollRectDragStrategy.Always;
            CacheReferences();
        }

        private void Awake()
        {
            CacheReferences();
            CacheDefaultScale();
        }

        private void OnEnable()
        {
            CacheReferences();
            if (!_hasCachedDefaultScale)
            {
                CacheDefaultScale();
            }

            _isSelected = EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject;
            _isHovering = false;
            ApplyScaleImmediate(GetVisualStateScale());
        }

        private void Update()
        {
            UpdatePointerLongPress();
            UpdateSubmitLongPress();
        }

        private void UpdatePointerLongPress()
        {
            if (!_enableLongPress || !_isPointerDown || _isLongPressTriggered || !CanInteract())
            {
                return;
            }

            if (TryGetCurrentPointerPosition(out Vector2 currentPosition) &&
                (currentPosition - _pointerDownPosition).sqrMagnitude >= _dragStartThreshold * _dragStartThreshold)
            {
                _isDragging = true;
                return;
            }

            if (Time.unscaledTime - _pointerDownTime < _longPressDuration)
            {
                return;
            }

            _isLongPressTriggered = true;
            _ignoreNextClickFeedback = true;
            _onLongPress.Invoke();
        }

        private void UpdateSubmitLongPress()
        {
            if (!_enableSubmitInputEnhancement || !_enableSubmitLongPress || !_enableLongPress || !_isSelected || !CanInteract())
            {
                return;
            }

            if (string.IsNullOrEmpty(_submitActionName))
            {
                return;
            }

            if (!_isSubmitPressed)
            {
                if (IsSubmitActionPressedThisFrame())
                {
                    _isSubmitPressed = true;
                    _isSubmitLongPressTriggered = false;
                    _submitPressTime = Time.unscaledTime;
                }

                return;
            }

            if (IsSubmitActionReleasedThisFrame())
            {
                _isSubmitPressed = false;
                _submitPressTime = -1f;
                _isSubmitLongPressTriggered = false;
                return;
            }

            if (_isSubmitLongPressTriggered || Time.unscaledTime - _submitPressTime < _longPressDuration)
            {
                return;
            }

            _isSubmitLongPressTriggered = true;
            _onLongPress.Invoke();
        }

        private void OnDisable()
        {
            ResetPressState();
            _ignoreNextClickFeedback = false;
            _isSubmitPressed = false;
            _isSubmitLongPressTriggered = false;
            _isHovering = false;
            StopTween();
            ApplyScaleImmediate(GetVisualStateScale());
        }

        private void OnDestroy()
        {
            StopTween();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!CanInteract())
            {
                return;
            }

            _isHovering = true;
            _onHoverEnter.Invoke();
            PlayHoverFeedback();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!CanInteract() || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            _isPointerDown = true;
            _isLongPressTriggered = false;
            _isDragging = false;
            _pointerDownTime = Time.unscaledTime;
            _pointerDownPosition = eventData.position;
            _pointerId = eventData.pointerId;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            ResetPressState();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            bool wasHovering = _isHovering;
            _isHovering = false;

            if (_cancelLongPressWhenPointerExit)
            {
                ResetPressState();
            }

            if (wasHovering)
            {
                _onHoverExit.Invoke();
                PlayHoverExitFeedback();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!CanInteract() || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (_cancelClickWhenDragging && (_isDragging || HasExceededDragThreshold(eventData)))
            {
                _ignoreNextClickFeedback = false;
                return;
            }

            if (_ignoreNextClickFeedback)
            {
                _ignoreNextClickFeedback = false;
                return;
            }

            PlayClickFeedback();
            TryInvokeDoubleClick();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!CanInteract())
            {
                return;
            }

            if (ShouldDeferSubmitFeedback())
            {
                if (!_isSubmitPressed)
                {
                    _isSubmitPressed = true;
                    _isSubmitLongPressTriggered = false;
                    _submitPressTime = Time.unscaledTime;
                }

                return;
            }

            PlayClickFeedback();
            TryInvokeSubmitDoubleClick();
        }

        public void OnUpdateSelected(BaseEventData eventData)
        {
            if (!_enableSubmitInputEnhancement || !CanInteract())
            {
                return;
            }

            if (string.IsNullOrEmpty(_submitActionName) || InputManager.Instance == null)
            {
                return;
            }

            if (!IsSubmitActionReleasedThisFrame())
            {
                return;
            }

            bool shouldInvokeSubmitClick = _isSubmitPressed && !_isSubmitLongPressTriggered;
            _isSubmitPressed = false;
            _submitPressTime = -1f;
            _isSubmitLongPressTriggered = false;

            if (shouldInvokeSubmitClick)
            {
                PlayClickFeedback();
                TryInvokeSubmitDoubleClick();
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            _isSelected = true;
            _onSelect.Invoke();
            PlaySelectFeedback();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            _isSelected = false;
            _onDeselect.Invoke();
            PlayDeselectFeedback();
        }

        public void PlayClickFeedback()
        {
            PlayClickSound();

            if (!_enableClickAnimation || _animationTarget == null)
            {
                return;
            }

            Vector3 restScale = GetVisualStateScale();
            Vector3 pressedScale = Vector3.Scale(restScale, Vector3.one * _clickScaleMultiplier);

            PlayTransientScaleTween(pressedScale, _clickPressDuration, _clickPressEase, () =>
            {
                PlayTransientScaleTween(restScale, _clickReleaseDuration, _clickReleaseEase);
            }, false);
        }

        public void PlaySelectFeedback()
        {
            PlaySelectSound();

            if (!_enableSelectAnimation)
            {
                return;
            }

            if (_loopSelectAnimation)
            {
                PlaySelectLoopAnimation();
                return;
            }

            PlayTransientScaleTween(GetVisualStateScale(), _selectAnimationDuration, _selectEase);
        }

        public void PlayDeselectFeedback()
        {
            StopTween();

            if (!_enableSelectAnimation)
            {
                return;
            }

            PlayTransientScaleTween(GetVisualStateScale(), _deselectAnimationDuration, _deselectEase);
        }

        public void PlayHoverFeedback()
        {
            PlayHoverSound();

            if (!_enableHoverAnimation)
            {
                return;
            }

            PlayTransientScaleTween(GetVisualStateScale(), _hoverEnterDuration, _hoverEnterEase);
        }

        public void PlayHoverExitFeedback()
        {
            if (!_enableHoverAnimation)
            {
                ResumeSelectAnimationIfNeeded();
                return;
            }

            PlayTransientScaleTween(GetVisualStateScale(), _hoverExitDuration, _hoverExitEase);
        }

        private void CacheReferences()
        {
            _button ??= GetComponent<Button>();
            _animationTarget ??= transform;
            if (_parentScrollRect == null)
            {
                _parentScrollRect = GetComponentInParent<ScrollRect>();
            }

            if (_parentSlider == null)
            {
                _parentSlider = GetComponentInParent<Slider>();
            }

            if (_parentScrollbar == null)
            {
                _parentScrollbar = GetComponentInParent<Scrollbar>();
            }

            _isSliderHandle = IsSliderHandle();
            _isScrollbarHandle = IsScrollbarHandle();
        }

        private void CacheDefaultScale()
        {
            if (_animationTarget != null)
            {
                _defaultScale = _animationTarget.localScale;
                _hasCachedDefaultScale = true;
            }
        }

        private bool CanInteract()
        {
            return isActiveAndEnabled && _button != null && _button.IsActive() && _button.IsInteractable();
        }

        private void TryInvokeDoubleClick()
        {
            if (!_enableDoubleClick)
            {
                _lastClickTime = Time.unscaledTime;
                return;
            }

            float now = Time.unscaledTime;
            if (now - _lastClickTime <= _doubleClickInterval)
            {
                _onDoubleClick.Invoke();
                _lastClickTime = -10f;
                return;
            }

            _lastClickTime = now;
        }

        private void TryInvokeSubmitDoubleClick()
        {
            if (!_enableSubmitInputEnhancement || !_enableSubmitDoubleClick || !_enableDoubleClick)
            {
                _lastSubmitTime = Time.unscaledTime;
                return;
            }

            float now = Time.unscaledTime;
            if (now - _lastSubmitTime <= _doubleClickInterval)
            {
                _onDoubleClick.Invoke();
                _lastSubmitTime = -10f;
                return;
            }

            _lastSubmitTime = now;
        }

        private void PlayClickSound()
        {
            if (!_playClickSound || string.IsNullOrEmpty(_clickSoundAssetName))
            {
                return;
            }

            AudioManager.Instance?.PlayBtnClick(_clickSoundAssetName);
        }

        private void PlayHoverSound()
        {
            if (!_playHoverSound || string.IsNullOrEmpty(_hoverSoundAssetName))
            {
                return;
            }

            AudioManager.Instance?.PlayBtnClick(_hoverSoundAssetName);
        }

        private void PlaySelectSound()
        {
            if (!_playSelectSound || string.IsNullOrEmpty(_selectSoundAssetName))
            {
                return;
            }

            AudioManager.Instance?.PlayBtnClick(_selectSoundAssetName);
        }

        private void ResetPressState()
        {
            _isPointerDown = false;
            _pointerDownTime = -1f;
            _isLongPressTriggered = false;
            _isDragging = false;
            _activeDragReceiver = DragReceiver.None;
            _pointerId = int.MinValue;
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            _isDragging = false;
            _activeDragReceiver = DragReceiver.None;

            if (_dragPassthroughMode == DragPassthroughMode.Disabled)
            {
                return;
            }

            if (ShouldRouteToSlider())
            {
                _parentSlider.OnInitializePotentialDrag(eventData);
                return;
            }

            if (ShouldRouteToScrollbar())
            {
                _parentScrollbar.OnInitializePotentialDrag(eventData);
                return;
            }

            if (CanRouteToScrollRect(eventData))
            {
                _parentScrollRect.OnInitializePotentialDrag(eventData);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            if (_cancelClickWhenDragging)
            {
                CancelButtonClick(eventData);
            }

            if (_cancelLongPressWhenBeginDrag)
            {
                ResetPressState();
                _isDragging = true;
                _ignoreNextClickFeedback = false;
            }

            _activeDragReceiver = ResolveDragReceiver(eventData);
            switch (_activeDragReceiver)
            {
                case DragReceiver.Slider:
                    _parentSlider.OnDrag(eventData);
                    return;
                case DragReceiver.Scrollbar:
                    _parentScrollbar.OnBeginDrag(eventData);
                    _parentScrollbar.OnDrag(eventData);
                    return;
                case DragReceiver.ScrollRect:
                    _parentScrollRect.OnBeginDrag(eventData);
                    return;
                default:
                    return;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            _isDragging = true;
            DragReceiver receiver = _activeDragReceiver != DragReceiver.None ? _activeDragReceiver : ResolveDragReceiver(eventData);
            switch (receiver)
            {
                case DragReceiver.Slider:
                    _parentSlider.OnDrag(eventData);
                    return;
                case DragReceiver.Scrollbar:
                    _parentScrollbar.OnDrag(eventData);
                    return;
                case DragReceiver.ScrollRect:
                    _parentScrollRect.OnDrag(eventData);
                    return;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            switch (_activeDragReceiver)
            {
                case DragReceiver.Scrollbar:
                    _parentScrollbar.OnPointerUp(eventData);
                    break;
                case DragReceiver.ScrollRect:
                    _parentScrollRect.OnEndDrag(eventData);
                    break;
            }

            _activeDragReceiver = DragReceiver.None;
            _isDragging = false;
            _isPointerDown = false;
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (!_forwardScrollWheelToParentScrollRect || _parentScrollRect == null)
            {
                return;
            }

            _parentScrollRect.OnScroll(eventData);
        }

        private bool HasExceededDragThreshold(PointerEventData eventData)
        {
            if (_dragStartThreshold <= 0f)
            {
                return false;
            }

            return (eventData.position - _pointerDownPosition).sqrMagnitude >= _dragStartThreshold * _dragStartThreshold;
        }

        private void CancelButtonClick(PointerEventData eventData)
        {
            if (eventData == null)
            {
                return;
            }

            eventData.eligibleForClick = false;
        }

        private bool TryGetCurrentPointerPosition(out Vector2 position)
        {
            position = default;

            if (_pointerId == int.MinValue)
            {
                return false;
            }

#if ENABLE_INPUT_SYSTEM
            if (_pointerId < 0)
            {
                Mouse mouse = Mouse.current;
                if (mouse == null || !mouse.leftButton.isPressed)
                {
                    return false;
                }

                position = mouse.position.ReadValue();
                return true;
            }

            Touchscreen touchscreen = Touchscreen.current;
            if (touchscreen == null)
            {
                return false;
            }

            foreach (var touch in touchscreen.touches)
            {
                if (!touch.press.isPressed || touch.touchId.ReadValue() != _pointerId)
                {
                    continue;
                }

                position = touch.position.ReadValue();
                return true;
            }

            return false;
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (_pointerId < 0)
            {
                if (!Input.GetMouseButton(0))
                {
                    return false;
                }

                position = Input.mousePosition;
                return true;
            }

            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.fingerId != _pointerId)
                {
                    continue;
                }

                position = touch.position;
                return true;
            }

            return false;
#else
            return false;
#endif
        }

        private bool ShouldDeferSubmitFeedback()
        {
            return _enableSubmitInputEnhancement &&
                   _enableSubmitLongPress &&
                   _enableLongPress &&
                   _isSelected &&
                   !string.IsNullOrEmpty(_submitActionName) &&
                   InputManager.Instance != null;
        }

        private bool IsSubmitActionPressedThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            InputAction submitAction = InputManager.Instance?.FindAction(_submitActionName);
            return submitAction != null && submitAction.WasPressedThisFrame();
#elif ENABLE_LEGACY_INPUT_MANAGER
            return InputManager.Instance != null && InputManager.Instance.GetButtonDown(_submitActionName);
#else
            return false;
#endif
        }

        private bool IsSubmitActionReleasedThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            InputAction submitAction = InputManager.Instance?.FindAction(_submitActionName);
            return submitAction != null && submitAction.WasReleasedThisFrame();
#elif ENABLE_LEGACY_INPUT_MANAGER
            return InputManager.Instance != null && InputManager.Instance.GetButtonUp(_submitActionName);
#else
            return false;
#endif
        }

        private DragReceiver ResolveDragReceiver(PointerEventData eventData)
        {
            if (_dragPassthroughMode == DragPassthroughMode.Disabled)
            {
                return DragReceiver.None;
            }

            if (_dragPassthroughMode != DragPassthroughMode.ScrollRectOnly)
            {
                if (ShouldRouteToSlider())
                {
                    return DragReceiver.Slider;
                }

                if (ShouldRouteToScrollbar())
                {
                    return DragReceiver.Scrollbar;
                }
            }

            return CanRouteToScrollRect(eventData) ? DragReceiver.ScrollRect : DragReceiver.None;
        }

        private bool ShouldRouteToSlider()
        {
            return _parentSlider != null && _isSliderHandle;
        }

        private bool ShouldRouteToScrollbar()
        {
            return _parentScrollbar != null && _isScrollbarHandle;
        }

        private bool CanRouteToScrollRect(PointerEventData eventData)
        {
            if (!_forwardDragToParentScrollRect || _parentScrollRect == null)
            {
                return false;
            }

            switch (_scrollRectDragStrategy)
            {
                case ScrollRectDragStrategy.Always:
                    return true;
                case ScrollRectDragStrategy.MatchScrollRectAxis:
                    return MatchesScrollRectAxis(GetDragDelta(eventData));
                case ScrollRectDragStrategy.HorizontalOnly:
                    return Mathf.Abs(GetDragDelta(eventData).x) >= Mathf.Abs(GetDragDelta(eventData).y);
                case ScrollRectDragStrategy.VerticalOnly:
                    return Mathf.Abs(GetDragDelta(eventData).y) >= Mathf.Abs(GetDragDelta(eventData).x);
                default:
                    return true;
            }
        }

        private Vector2 GetDragDelta(PointerEventData eventData)
        {
            Vector2 delta = eventData.delta;
            if (delta.sqrMagnitude > 0f)
            {
                return delta;
            }

            return eventData.position - _pointerDownPosition;
        }

        private bool MatchesScrollRectAxis(Vector2 delta)
        {
            bool horizontalDominant = Mathf.Abs(delta.x) >= Mathf.Abs(delta.y);
            bool verticalDominant = Mathf.Abs(delta.y) >= Mathf.Abs(delta.x);

            if (_parentScrollRect.horizontal && !_parentScrollRect.vertical)
            {
                return horizontalDominant;
            }

            if (_parentScrollRect.vertical && !_parentScrollRect.horizontal)
            {
                return verticalDominant;
            }

            return _parentScrollRect.horizontal || _parentScrollRect.vertical;
        }

        private bool IsSliderHandle()
        {
            if (_parentSlider == null)
            {
                return false;
            }

            RectTransform rectTransform = transform as RectTransform;
            if (rectTransform == null)
            {
                return false;
            }

            RectTransform handleRect = _parentSlider.handleRect;
            return handleRect != null && (handleRect == rectTransform || rectTransform.IsChildOf(handleRect));
        }

        private bool IsScrollbarHandle()
        {
            if (_parentScrollbar == null)
            {
                return false;
            }

            RectTransform rectTransform = transform as RectTransform;
            if (rectTransform == null)
            {
                return false;
            }

            RectTransform handleRect = _parentScrollbar.handleRect;
            return handleRect != null && (handleRect == rectTransform || rectTransform.IsChildOf(handleRect));
        }

        private Vector3 GetVisualStateScale()
        {
            Vector3 scale = GetBaseScaleWithoutSelection();

            if (_enableSelectAnimation && _isSelected)
            {
                scale = Vector3.Scale(scale, _selectedScaleMultiplier);
            }

            if (_enableHoverAnimation && _isHovering)
            {
                scale = Vector3.Scale(scale, _hoverScaleMultiplier);
            }

            return scale;
        }

        private Vector3 GetBaseScaleWithoutSelection()
        {
            return _defaultScale;
        }

        private void ApplyScaleImmediate(Vector3 scale)
        {
            if (_animationTarget == null)
            {
                return;
            }

            _animationTarget.localScale = scale;
        }

        private void PlayTransientScaleTween(Vector3 targetScale, float duration, Ease ease, Action onComplete = null, bool resumeSelectAfterComplete = true)
        {
            if (_animationTarget == null)
            {
                return;
            }

            StopTween();

            if (duration <= 0f || Tween.Instance == null)
            {
                ApplyScaleImmediate(targetScale);
                onComplete?.Invoke();
                if (resumeSelectAfterComplete)
                {
                    ResumeSelectAnimationIfNeeded();
                }
                return;
            }

            Tween.Instance.ScaleTween(_animationTarget, targetScale, duration)
                .SetCustomId(_tweenKey)
                .SetEase(ease)
                .SetOnComplete(() =>
                {
                    onComplete?.Invoke();
                    if (resumeSelectAfterComplete)
                    {
                        ResumeSelectAnimationIfNeeded();
                    }
                });
        }

        private void PlaySelectLoopAnimation()
        {
            if (_animationTarget == null)
            {
                return;
            }

            StopTween();

            Vector3 fromScale = GetBaseScaleWithoutSelection();
            Vector3 toScale = GetVisualStateScale();

            ApplyScaleImmediate(fromScale);

            if (_selectAnimationDuration <= 0f || Tween.Instance == null)
            {
                ApplyScaleImmediate(toScale);
                return;
            }

            Tween.Instance.ScaleTween(_animationTarget, toScale, _selectAnimationDuration)
                .SetCustomId(_tweenKey)
                .SetEase(_selectEase)
                .SetLoopType(LoopType.Yoyo, -1);
        }

        private void ResumeSelectAnimationIfNeeded()
        {
            if (!_enableSelectAnimation || !_loopSelectAnimation || !_isSelected || _isHovering || _animationTarget == null)
            {
                return;
            }

            PlaySelectLoopAnimation();
        }

        private void StopTween()
        {
            Tween.Instance?.CancelTween(_tweenKey);
        }
    }
}

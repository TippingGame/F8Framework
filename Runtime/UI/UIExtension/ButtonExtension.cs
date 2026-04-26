using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        
        [SerializeField] private Button _button;
        [SerializeField] private Transform _animationTarget;
        [SerializeField] private ScrollRect _parentScrollRect;
        
        [SerializeField] private bool _forwardDragToParentScrollRect = true;
        [SerializeField] private bool _forwardScrollWheelToParentScrollRect = true;
        [SerializeField] private bool _autoFindParentScrollRect = true;
        [SerializeField, Min(0f)] private float _dragStartThreshold = 12f;
        [SerializeField] private bool _cancelClickWhenDragging = true;
        [SerializeField] private bool _cancelLongPressWhenBeginDrag = true;
        
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
        [SerializeField] private bool _cancelLongPressWhenPointerExit = true;
        [SerializeField] private ButtonEvent _onLongPress = new ButtonEvent();
        
        [SerializeField] private bool _enableSubmitInputEnhancement;
        [SerializeField] private string _submitActionName = "Submit";
        [SerializeField] private bool _enableSubmitLongPress = true;
        [SerializeField] private bool _enableSubmitDoubleClick = true;
        
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
        private float _pointerDownTime = -1f;
        private float _lastClickTime = -10f;
        private float _submitPressTime = -1f;
        private float _lastSubmitTime = -10f;
        private Vector2 _pointerDownPosition;
        private int _pointerId = int.MinValue;

        public ButtonEvent OnDoubleClick => _onDoubleClick;
        public ButtonEvent OnLongPress => _onLongPress;
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
                if (InputManager.Instance != null && InputManager.Instance.GetButtonDown(_submitActionName))
                {
                    _isSubmitPressed = true;
                    _isSubmitLongPressTriggered = false;
                    _submitPressTime = Time.unscaledTime;
                }

                return;
            }

            if (InputManager.Instance != null && InputManager.Instance.GetButtonUp(_submitActionName))
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

            if (!InputManager.Instance.GetButtonUp(_submitActionName))
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
            if (_autoFindParentScrollRect && _parentScrollRect == null)
            {
                _parentScrollRect = GetComponentInParent<ScrollRect>();
            }
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

            AudioManager.Instance?.PlayUISound(_clickSoundAssetName);
        }

        private void PlayHoverSound()
        {
            if (!_playHoverSound || string.IsNullOrEmpty(_hoverSoundAssetName))
            {
                return;
            }

            AudioManager.Instance?.PlayUISound(_hoverSoundAssetName);
        }

        private void PlaySelectSound()
        {
            if (!_playSelectSound || string.IsNullOrEmpty(_selectSoundAssetName))
            {
                return;
            }

            AudioManager.Instance?.PlayUISound(_selectSoundAssetName);
        }

        private void ResetPressState()
        {
            _isPointerDown = false;
            _pointerDownTime = -1f;
            _isLongPressTriggered = false;
            _isDragging = false;
            _pointerId = int.MinValue;
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            _isDragging = false;
            if (!_forwardDragToParentScrollRect || _parentScrollRect == null)
            {
                return;
            }

            _parentScrollRect.OnInitializePotentialDrag(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_forwardDragToParentScrollRect || _parentScrollRect == null)
            {
                return;
            }

            _isDragging = true;
            if (_cancelLongPressWhenBeginDrag)
            {
                ResetPressState();
                _isDragging = true;
                _ignoreNextClickFeedback = false;
            }

            _parentScrollRect.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_forwardDragToParentScrollRect || _parentScrollRect == null)
            {
                return;
            }

            _isDragging = true;
            _parentScrollRect.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_forwardDragToParentScrollRect && _parentScrollRect != null)
            {
                _parentScrollRect.OnEndDrag(eventData);
            }

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

        private bool TryGetCurrentPointerPosition(out Vector2 position)
        {
            position = default;

            if (_pointerId == int.MinValue)
            {
                return false;
            }

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
            if (_enableHoverAnimation && _isHovering)
            {
                return Vector3.Scale(_defaultScale, _hoverScaleMultiplier);
            }

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

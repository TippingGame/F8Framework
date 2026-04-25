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
        IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler, IUpdateSelectedHandler,
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
        [SerializeField, Min(0f)] private float _deselectAnimationDuration = 0.12f;
        [SerializeField] private Ease _deselectEase = Ease.EaseOutQuad;
        
        [SerializeField] private bool _playClickSound = true;
        [SerializeField] private string _clickSoundAssetName;
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
            _enableSelectAnimation = false;
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
            ApplyScaleImmediate(GetRestScale());
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
            StopTween();
            ApplyScaleImmediate(GetRestScale());
        }

        private void OnDestroy()
        {
            StopTween();
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
            if (_cancelLongPressWhenPointerExit)
            {
                ResetPressState();
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

            Vector3 restScale = GetRestScale();
            Vector3 pressedScale = Vector3.Scale(restScale, Vector3.one * _clickScaleMultiplier);

            PlayScaleTween(pressedScale, _clickPressDuration, _clickPressEase, () =>
            {
                PlayScaleTween(restScale, _clickReleaseDuration, _clickReleaseEase);
            });
        }

        public void PlaySelectFeedback()
        {
            PlaySelectSound();

            if (!_enableSelectAnimation)
            {
                return;
            }

            PlayScaleTween(GetRestScale(), _selectAnimationDuration, _selectEase);
        }

        public void PlayDeselectFeedback()
        {
            if (!_enableSelectAnimation)
            {
                return;
            }

            PlayScaleTween(GetRestScale(), _deselectAnimationDuration, _deselectEase);
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

        private Vector3 GetRestScale()
        {
            if (!_isSelected || !_enableSelectAnimation)
            {
                return _defaultScale;
            }

            return Vector3.Scale(_defaultScale, _selectedScaleMultiplier);
        }

        private void ApplyScaleImmediate(Vector3 scale)
        {
            if (_animationTarget == null)
            {
                return;
            }

            _animationTarget.localScale = scale;
        }

        private void PlayScaleTween(Vector3 targetScale, float duration, Ease ease, Action onComplete = null)
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
                return;
            }

            Tween.Instance.ScaleTween(_animationTarget, targetScale, duration)
                .SetCustomId(_tweenKey)
                .SetEase(ease)
                .SetOnComplete(onComplete);
        }

        private void StopTween()
        {
            Tween.Instance?.CancelTween(_tweenKey);
        }
    }
}

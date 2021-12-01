using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClemCAddons;
[ExecuteInEditMode]
public class ResizableUI : MonoBehaviour
{
    [Header("Awareness")]
    [SerializeField] private bool _useContentSize;
    [Header("Reduction")]
    [SerializeField] private bool _canReduce = true;
    [SerializeField, LabelOverride("Max Reduction %", 0, 100)] private float _maxReduction = 75;
    [Header("Others")]
    [SerializeField] private bool _canLeaveScreen = false;
    [SerializeField, LabelOverride("Detection Margin %", 0, 100)] private float _borderMargin = 5f;
    [SerializeField, LabelOverride("Limit Margin %", 0, 100)] private float _limitMargin = 0f;
    private Vector2 _renderingSize;
    private float _scaleFactor;
    private RectTransform _rect;
    private Canvas _rootCanvas;
    private Vector2 _baseSize;

    public Vector2 DetectionMargin { get => _renderingSize * (_borderMargin / 100) * 0.5f; }
    public Vector2 RenderingMargin { get => _renderingSize * (_limitMargin / 100) * 0.5f; }
    public float MaxReduction { get => _maxReduction/100; }
    public Vector2 BaseSize { get => _baseSize; }
    public Vector2 HalfSize { get => _baseSize / 2; }
    [ExecuteInEditMode]
    void Start()
    {
        if(Application.isPlaying)
        {
            _rect = GetComponent<RectTransform>();
            if (_rect == null)
                Debug.LogError("Must be placed on an UI component");
            _baseSize = _rect.sizeDelta * _scaleFactor;
            _rootCanvas = _rect.FindParentWithComponent(typeof(Canvas)).GetComponent<Canvas>().rootCanvas;
            _renderingSize = _rootCanvas.renderingDisplaySize;
            _scaleFactor = _rootCanvas.scaleFactor;
        }
    }
    void Update()
    {
        if (Application.isPlaying)
        {
            if (_rect == null)
                return;
            if (_useContentSize)
                SetSizeToChildren();
            if (_canReduce)
            {
                float reduce = 1f;
                float reducex = reduce;
                if (_rect.AnchorNeutralPosition(true).x < -HalfSize.x || _rect.AnchorNeutralPosition(true).x > _renderingSize.x + HalfSize.x)
                {
                    reducex = 0;
                }
                else if (_rect.AnchorNeutralPosition(true).x < HalfSize.x + DetectionMargin.x)
                {
                    reducex = 1 - ((DetectionMargin.x + HalfSize.x - _rect.AnchorNeutralPosition(true).x) / ((_rect.AnchorNeutralPosition(true).x - (RenderingMargin.x + HalfSize.x * (MaxReduction / 100))) + (DetectionMargin.x + HalfSize.x - _rect.AnchorNeutralPosition(true).x)));
                    // a/(b+a) allows for a smooth 0-1 scale from a to b based on the proportion of a and b.
                }
                else if (_rect.AnchorNeutralPosition(true).x > _renderingSize.x - HalfSize.x - DetectionMargin.x)
                {
                    reducex = 1 - (((_renderingSize.x - DetectionMargin.x - HalfSize.x) - _rect.AnchorNeutralPosition(true).x) / ((_rect.AnchorNeutralPosition(true).x - (_renderingSize.x - RenderingMargin.x - HalfSize.x * (MaxReduction / 100))) + ((_renderingSize.x - DetectionMargin.x - HalfSize.x) - _rect.AnchorNeutralPosition(true).x)));
                }
                float reducey = reduce;
                if (_rect.AnchorNeutralPosition(true).y < -HalfSize.y || _rect.AnchorNeutralPosition(true).y > _renderingSize.y + HalfSize.y)
                {
                    reducey = 0;
                }
                else if (_rect.AnchorNeutralPosition(true).y < HalfSize.y + DetectionMargin.y)
                {
                    reducey = 1 - ((DetectionMargin.y + HalfSize.y - _rect.AnchorNeutralPosition(true).y) / ((_rect.AnchorNeutralPosition(true).y - (RenderingMargin.y + HalfSize.y * (MaxReduction / 100))) + (DetectionMargin.y + HalfSize.y - _rect.AnchorNeutralPosition(true).y)));
                    // a/(b+a) allows for a smooth 0-1 scale from a to b based on the proportion of a and b.
                }
                else if (_rect.AnchorNeutralPosition(true).y > _renderingSize.y - HalfSize.y - DetectionMargin.y)
                {
                    reducey = 1 - (((_renderingSize.y - DetectionMargin.y - HalfSize.y) - _rect.AnchorNeutralPosition(true).y) / ((_rect.AnchorNeutralPosition(true).y - (_renderingSize.y - RenderingMargin.y - HalfSize.y * (MaxReduction / 100))) + ((_renderingSize.y - DetectionMargin.y - HalfSize.y) - _rect.AnchorNeutralPosition(true).y)));
                }
                reduce = reducex < reducey ? reducex : reducey;
                reduce = reduce * (1 - (MaxReduction / 100)) + (MaxReduction / 100);
                if (reduce < MaxReduction / 100)
                    reduce = MaxReduction / 100;
                _rect.sizeDelta = BaseSize * reduce / _scaleFactor;
            } else
            {
                _rect.sizeDelta = BaseSize;
            }
            if (!_canLeaveScreen)
            {
                float x = 0f;
                float y = 0f;
                if (_rect.AnchorNeutralPosition(true).x < HalfSize.x + RenderingMargin.x - DetectionMargin.x)
                {
                    x = (HalfSize.x - DetectionMargin.x + RenderingMargin.x - _rect.AnchorNeutralPosition(true).x) * (2 + _scaleFactor);
                }
                else if (_rect.AnchorNeutralPosition(true).x > _renderingSize.x - HalfSize.x - RenderingMargin.x + DetectionMargin.x)
                {
                    x = (_renderingSize.x + DetectionMargin.x - HalfSize.x - RenderingMargin.x - _rect.AnchorNeutralPosition(true).x) * (2 + _scaleFactor);
                }
                if (_rect.AnchorNeutralPosition(true).y < HalfSize.y + RenderingMargin.y - DetectionMargin.y)
                {
                    y = (HalfSize.y - DetectionMargin.y + RenderingMargin.y - _rect.AnchorNeutralPosition(true).y) * (2 + _scaleFactor);
                }
                else if (_rect.AnchorNeutralPosition(true).y > _renderingSize.y - HalfSize.y - RenderingMargin.y + DetectionMargin.y)
                {
                    y = (_renderingSize.y + DetectionMargin.y - HalfSize.y - RenderingMargin.y - _rect.AnchorNeutralPosition(true).y) * (2 + _scaleFactor);
                }
                _rect.anchoredPosition = _rect.anchoredPosition.SetX(x).SetY(y);
                // for some reason Vector2.Set() doesn't actually set the variable, so had to resort to a longer
                // way of doing it
            }
        }
    }
    private Vector2 GetChildrenSize(Transform transformR = null)
    {
        if (transformR == null)
            transformR = transform;
        RectTransform children = transformR.GetComponentInChildren<RectTransform>();
        float size_x = 0, size_y = 0;
        foreach (RectTransform child in children)
        {
            if (child.gameObject.activeInHierarchy == true)
            {
                Vector2 scale = child.sizeDelta;
                Vector2 childSize = GetChildrenSize(child);
                size_x = childSize.x.Max(false, true, size_x, scale.x);
                size_y = childSize.y.Max(false, true, size_y, scale.y);
            }
        }
        return new Vector2(size_x, size_y);
    }
    private void SetSizeToChildren()
    {
        _baseSize = GetChildrenSize();
    }
}

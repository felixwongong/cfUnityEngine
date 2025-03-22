using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class AspectRatioFitterElement: VisualElement 
{
    public enum AspectMode
    {
        None,
        WidthControlsHeight,
        HeightControlsWidth,
        FitInContainer,
        EnvelopContainer
    }
    
    private AspectMode _aspectMode = AspectMode.None;
    [UxmlAttribute]
    public AspectMode aspectMode
    {
        get => _aspectMode;
        set
        {
            _aspectMode = value;
            UpdateAspect();
        }
    }

    private float _aspectRatio = 1f;

    [Range(0.1f, 10)]
    [UxmlAttribute]
    public float aspectRatio
    {
        get => _aspectRatio;
        set
        {
            _aspectRatio = value;
            UpdateAspect();
        }
    }

    private VisualElement _container;
    public override VisualElement contentContainer => _container;

    public AspectRatioFitterElement()
    {
        _container = this;
        var container = new VisualElement
        {
            name = "Container"
        };
        Add(container);

        _container = container;
        
        RegisterCallback<GeometryChangedEvent>(UpdateAspectAfterEvent);
        RegisterCallback<AttachToPanelEvent>(UpdateAspectAfterEvent);
    }

    private void ClearPadding()
    {
        var containerStyle = _container.style;
        containerStyle.paddingTop = containerStyle.paddingBottom = containerStyle.paddingLeft = containerStyle.paddingRight = 0;
    }
    
    private void UpdateAspect()
    {
        if (_aspectRatio < 0)
        {
            Debug.LogError("AspectRatioFitter.UpdateAspect: aspect ratio must be greater than 0");
            return;
        }

        if (float.IsNaN(resolvedStyle.width) || float.IsNaN(resolvedStyle.height))
        {
            return;
        }

        ClearPadding();
        style.alignItems = new StyleEnum<Align>(Align.Center);
        
        var currentRatio = resolvedStyle.width / resolvedStyle.height;
        switch (aspectMode)
        {
            case AspectMode.None:
            case AspectMode.WidthControlsHeight:
            case AspectMode.HeightControlsWidth:
                UpdateAspectToAxis(aspectMode);
                break;
            case AspectMode.FitInContainer:
                UpdateAspectToAxis(currentRatio >= _aspectRatio
                    ? AspectMode.HeightControlsWidth
                    : AspectMode.WidthControlsHeight);
                break;
            case AspectMode.EnvelopContainer:
                UpdateAspectToAxis(currentRatio <= _aspectRatio
                    ? AspectMode.HeightControlsWidth
                    : AspectMode.WidthControlsHeight);
                break;
        }
    }
    
    private void UpdateAspectToAxis(AspectMode aspect)
    {
        switch (aspect)
        {
            case AspectMode.None:
            case AspectMode.EnvelopContainer:
            case AspectMode.FitInContainer:
                break;
            case AspectMode.WidthControlsHeight:
                style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                var width = resolvedStyle.width;
                _container.style.width = width;
                _container.style.height = width / _aspectRatio;
                break;
            case AspectMode.HeightControlsWidth:
                style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
                var height = resolvedStyle.height;
                _container.style.height = height;
                _container.style.width = height * _aspectRatio;
                break;
        }
    }

    private void UpdateAspectAfterEvent(EventBase evt)
    {
        var element = evt.target as AspectRatioFitterElement;
        element?.UpdateAspect();
    }
}

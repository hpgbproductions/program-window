using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using Jundroo.SimplePlanes.ModTools.Parts;
using Jundroo.SimplePlanes.ModTools.Parts.Attributes;

/// <summary>
/// A part modifier for SimplePlanes.
/// A part modifier is responsible for attaching a part modifier behaviour script to a game object within a part's hierarchy.
/// </summary>
[Serializable]
public class ProgramWindowPart : Jundroo.SimplePlanes.ModTools.Parts.PartModifier
{
    [SerializeField]
    [DesignerPropertyTextInput(ExtraWidth = 0, Label = "Class Name", Order = 0, SupportsInputDialog = false)]
    private string _windowClassName = "";

    [SerializeField]
    [DesignerPropertyTextInput(ExtraWidth = 0, Label = "Window Title", Order = 10, SupportsInputDialog = false)]
    private string _windowName = "";

    [SerializeField]
    [DesignerPropertyToggleButton(new string[4] { "64", "128", "256", "512" }, Header = "Standard Options", Label = "Max. Size", Order = 50)]
    private int _maxSize = 256;

    [SerializeField]
    [DesignerPropertySlider(Label = "Update Period (frames)", MaxValue = 20, MinValue = 1, NumberOfSteps = 20, Order = 60)]
    private int _updatePeriod = 2;

    [SerializeField]
    [DesignerPropertyToggleButton(AllowFunkyInput = false, Label = "Filter Mode", Order = 70)]
    private FilterMode _filterMode = FilterMode.Point;

    [SerializeField]
    [DesignerPropertyTextInput(ExtraWidth = 0, Label = "Opacity", Order = 110, SupportsInputDialog = false)]
    private byte _opacity = 255;

    [SerializeField]
    [DesignerPropertyToggleButton(AllowFunkyInput = false, Header = "Set Transparent Color", Label = "Active", Order = 200)]
    private bool _tpActive = false;

    [SerializeField]
    [DesignerPropertyTextInput(ExtraWidth = 0, Label = "Threshold", Order = 210, SupportsInputDialog = false)]
    private int _tpThreshold = 10;

    [SerializeField]
    [DesignerPropertyTextInput(ExtraWidth = 0, Label = "Red", Order = 220, SupportsInputDialog = false)]
    private byte _tpColorR = 255;

    [SerializeField]
    [DesignerPropertyTextInput(ExtraWidth = 0, Label = "Green", Order = 230, SupportsInputDialog = false)]
    private byte _tpColorG = 0;

    [SerializeField]
    [DesignerPropertyTextInput(ExtraWidth = 0, Label = "Blue", Order = 240, SupportsInputDialog = false)]
    private byte _tpColorB = 255;

    [SerializeField]
    [DesignerPropertyToggleButton(AllowFunkyInput = false, Header = "Limit Display Rect", Label = "Active", Order = 300)]
    private bool _cropActive = false;

    [SerializeField]
    [DesignerPropertyTextInput(ExtraWidth = 0, Label = "Begin X", Order = 310, SupportsInputDialog = false)]
    private int _cropX1 = 0;

    [SerializeField]
    [DesignerPropertyTextInput(ExtraWidth = 0, Label = "Begin Y", Order = 320, SupportsInputDialog = false)]
    private int _cropY1 = 0;

    [SerializeField]
    [DesignerPropertyTextInput(ExtraWidth = 0, Label = "End X", Order = 330, SupportsInputDialog = false)]
    private int _cropX2 = 256;

    [SerializeField]
    [DesignerPropertyTextInput(ExtraWidth = 0, Label = "End Y", Order = 340, SupportsInputDialog = false)]
    private int _cropY2 = 256;

    public string WindowClassName
    {
        get { return _windowClassName; }
    }

    public string WindowName
    {
        get { return _windowName; }
    }

    public int MaxSize
    {
        get { return _maxSize; }
    }

    public int UpdatePeriod
    {
        get { return _updatePeriod; }
    }

    public FilterMode TextureFilterMode
    {
        get { return _filterMode; }
    }

    public byte Opacity
    {
        get { return _opacity; }
    }

    public bool TpActive
    {
        get { return _tpActive; }
    }

    public int TpThreshold
    {
        get { return _tpThreshold; }
    }

    public byte TpColorR
    {
        get { return _tpColorR; }
    }

    public byte TpColorG
    {
        get { return _tpColorG; }
    }

    public byte TpColorB
    {
        get { return _tpColorB; }
    }

    public bool CropActive
    {
        get { return _cropActive; }
    }

    public int CropBeginX
    {
        get { return _cropX1; }
    }

    public int CropBeginY
    {
        get { return _cropY1; }
    }

    public int CropEndX
    {
        get { return _cropX2; }
    }

    public int CropEndY
    {
        get { return _cropY2; }
    }

    /// <summary>
    /// Called when this part modifiers is being initialized as the part game object is being created.
    /// </summary>
    /// <param name="partRootObject">The root game object that has been created for the part.</param>
    /// <returns>The created part modifier behaviour, or <c>null</c> if it was not created.</returns>
    public override Jundroo.SimplePlanes.ModTools.Parts.PartModifierBehaviour Initialize(UnityEngine.GameObject partRootObject)
    {
        // Attach the behaviour to the part's root object.
        var behaviour = partRootObject.AddComponent<ProgramWindowPartBehaviour>();
        return behaviour;
    }
}
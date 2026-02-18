using UnityEngine;

namespace KinoGlitch {

public sealed partial class AnalogGlitchController
{
    #region Public properties

    [field:SerializeField, Range(0, 1)]
    public float ScanLineJitter { get; set; }

    [field:SerializeField, Range(0, 1)]
    public float VerticalJump { get; set; }

    [field:SerializeField, Range(0, 1)]
    public float HorizontalShake { get; set; }

    [field:SerializeField, Range(0, 1)]
    public float ColorDrift { get; set; }

    #endregion
}

} // namespace KinoGlitch

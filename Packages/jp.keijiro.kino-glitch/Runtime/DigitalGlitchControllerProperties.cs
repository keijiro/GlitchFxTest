using UnityEngine;

namespace KinoGlitch {

public sealed partial class DigitalGlitchController
{
    #region Public properties

    [field:SerializeField, Range(0, 1)]
    public float Intensity { get; set; }

    #endregion
}

} // namespace KinoGlitch

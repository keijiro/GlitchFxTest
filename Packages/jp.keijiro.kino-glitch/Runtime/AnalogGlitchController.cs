using UnityEngine;

namespace KinoGlitch {

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("KinoGlitch/Analog Glitch Controller")]
public sealed partial class AnalogGlitchController : MonoBehaviour
{
    void OnDestroy() => ReleaseResources();

    void OnDisable() => ReleaseResources();
}

} // namespace KinoGlitch

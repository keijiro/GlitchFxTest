using UnityEngine;

namespace KinoGlitch {

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("KinoGlitch/Digital Glitch Controller")]
public sealed partial class DigitalGlitchController : MonoBehaviour
{
    void OnDestroy() => ReleaseResources();

    void OnDisable() => ReleaseResources();
}

} // namespace KinoGlitch

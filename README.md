# KinoGlitch URP

**KinoGlitch URP** is a Unity URP renderer feature package that provides analog
and digital glitch post-processing effects. It reproduces video glitches with
lightweight full-screen passes and camera-attached controllers.

## System requirements

- Unity 6000.0 or later
- Universal Render Pipeline (URP)

## How to install

Install the KinoGlitch URP package (`jp.keijiro.kino-glitch.universal`) from
the "Keijiro" scoped registry in Package Manager. Follow [these instructions]
to add the registry to your project.

[these instructions]:
  https://gist.github.com/keijiro/f8c7e8ff29bfe63d86b888901b82644c

## How to use

1. Add **AnalogGlitchRendererFeature** and/or
   **DigitalGlitchRendererFeature** to your URP Renderer Asset.
2. Add **AnalogGlitchController** and/or **DigitalGlitchController** to a
   Camera.

## Analog Glitch properties

**Scan Line Jitter** adds horizontal jitter along scanlines.

**Vertical Jump** offsets the image vertically with a continuous jump motion.

**Horizontal Shake** adds discontinuous horizontal shake.

**Color Drift** shifts chroma components over time.

**Horizontal Ripple** warps horizontal UVs with a line-dependent ripple.

## Digital Glitch properties

**Intensity** controls the amount of digital corruption and block replacement.

## Performance notes

Analog Glitch skips its pass when all properties are set to zero. By contrast,
Digital Glitch still runs at zero Intensity because it must keep updating its
internal frame history. Disable the component if you want to eliminate its cost
entirely.

# KinoGlitch URP

![GIF](https://github.com/user-attachments/assets/7d6618aa-0415-45f6-a1f1-a069f8ad97ed)

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

You can change the injection point with the `Pass Event` property on either
renderer feature. By default, they run after post processing.

## Analog Glitch properties

**Scan Line Jitter** adds horizontal jitter along scanlines.

![GIF](https://github.com/user-attachments/assets/e1ac0fef-0d53-4728-b3c3-15a6672f8e2b)

**Vertical Jump** offsets the image vertically with a continuous jump motion.

![GIF](https://github.com/user-attachments/assets/664f27a7-97e0-4073-975d-58c6a89a214c)

**Horizontal Shake** adds discontinuous horizontal shake.

![GIF](https://github.com/user-attachments/assets/e5a16274-5dcb-492b-856a-40501d525f15)

**Color Drift** shifts chroma components over time.

![GIF](https://github.com/user-attachments/assets/a0d2ea9d-9b3e-42bd-becc-b5698c022543)

**Horizontal Ripple** warps horizontal UVs with a line-dependent ripple.

![GIF](https://github.com/user-attachments/assets/e38d0a73-6ad2-40e9-b6a1-daa5846a0d60)

## Digital Glitch properties

**Intensity** controls the amount of digital corruption and block replacement.

![GIF](https://github.com/user-attachments/assets/c68ace9e-3ba7-43d3-a39e-63deff42c33d)

## Performance notes

Analog Glitch skips its pass when all properties are set to zero. By contrast,
Digital Glitch still runs at zero Intensity because it must keep updating its
internal frame history. Disable the component if you want to eliminate its cost
entirely.

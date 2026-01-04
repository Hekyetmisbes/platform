# Sprite Atlas Setup Guide

## Overview

Sprite Atlases combine multiple sprite textures into single larger textures, significantly reducing draw calls and improving performance. This guide explains how to set up sprite atlases for this project.

## Why Use Sprite Atlases?

**Benefits:**
- **Reduced Draw Calls:** Major performance boost (potentially 50-80% reduction)
- **Smaller Build Size:** Better compression when sprites are packed together
- **Faster Loading:** Fewer texture files to load
- **Better Memory Usage:** More efficient GPU memory utilization
- **Improved Performance:** Especially important for mobile devices

**Current State:**
- 235 individual image files
- Each sprite potentially causing a separate draw call
- No texture packing optimization

**Target State:**
- 4-6 sprite atlases grouping related sprites
- Significantly fewer draw calls
- Optimized memory usage

---

## Creating Sprite Atlases

### 1. Create Atlas Assets

**Steps:**
1. In Unity Project window, navigate to `Assets/Atlases` (create this folder if it doesn't exist)
2. Right-click in the folder → **Create → 2D → Sprite Atlas**
3. Create the following atlases:
   - `UIAtlas.spriteatlas` - All UI sprites (buttons, icons, panels)
   - `CharacterAtlas.spriteatlas` - Character animation frames
   - `PlatformAtlas.spriteatlas` - Platform tiles and game objects
   - `EnvironmentAtlas.spriteatlas` - Background elements and decorations

### 2. Configure Each Atlas

**For ALL atlases, use these settings:**

#### Master Settings
- **Type:** Master
- **Include in Build:** ✓ Enabled
- **Allow Rotation:** ✓ Enabled (better packing)
- **Tight Packing:** ✓ Enabled (better packing)
- **Padding:** 2 pixels

#### Texture Settings
- **Max Atlas Size:** 2048 x 2048
- **Compression:**
  - **For Desktop:** RGBA32 (high quality)
  - **For Mobile:** ASTC 6x6 or PVRTC 4 bits/pixel (compressed)
- **Filter Mode:** Bilinear
- **sRGB:** ✓ Enabled (for color sprites)

### 3. Add Sprites to Atlases

**Method 1: Drag and Drop**
1. Select the Sprite Atlas in the Project window
2. In the Inspector, find the **Objects for Packing** section
3. Drag entire folders or individual sprites into this section

**Method 2: Folder References (Recommended)**
1. Click the **+** button in **Objects for Packing**
2. Select **Folder** from the dropdown
3. Drag the sprite folder (e.g., `Assets/Sprites/UI`) into the field
4. This automatically includes all sprites in that folder

**Recommended Organization:**

```
UIAtlas.spriteatlas
└── Objects for Packing:
    ├── Assets/Sprites/UI/Buttons
    ├── Assets/Sprites/UI/Icons
    └── Assets/Sprites/UI/Panels

CharacterAtlas.spriteatlas
└── Objects for Packing:
    ├── Assets/Sprites/Characters/Player
    └── Assets/Sprites/Characters/Animations

PlatformAtlas.spriteatlas
└── Objects for Packing:
    ├── Assets/Sprites/Platforms
    └── Assets/Sprites/GameObjects

EnvironmentAtlas.spriteatlas
└── Objects for Packing:
    ├── Assets/Sprites/Backgrounds
    └── Assets/Sprites/Decorations
```

---

## Platform-Specific Optimization

### Mobile (Android/iOS)

**Recommended Compression:**
- **Android:** ASTC 6x6 or ETC2 RGBA8
- **iOS:** ASTC 6x6 or PVRTC 4 bits/pixel

**Max Atlas Size:** 2048x2048 (some older devices may not support 4096)

**Additional Settings:**
- **Override for Android:** Set compression to ASTC
- **Override for iOS:** Set compression to ASTC

### Desktop (Windows/Mac/Linux)

**Recommended Compression:**
- RGBA32 (uncompressed, high quality)
- Or DXT5 for smaller size with minimal quality loss

**Max Atlas Size:** 4096x4096 (desktop GPUs can handle larger)

---

## Verification

### 1. Check Atlas Generation

After setting up atlases:
1. Select a Sprite Atlas in the Project window
2. Click **Pack Preview** button in the Inspector
3. Verify all sprites are packed correctly
4. Check the atlas utilization percentage (aim for 70-90%)

### 2. Test in Game

**Before Atlasing:**
1. Open the Unity Profiler (Window → Analysis → Profiler)
2. Run the game
3. Note the **Batches** and **SetPass Calls** count in the Rendering section

**After Atlasing:**
1. Run the game again with atlases enabled
2. Compare the Batches/SetPass count
3. You should see a significant reduction (50%+ is excellent)

### 3. Verify Sprites Still Work

- Play through all levels
- Check that all UI elements display correctly
- Verify character animations work
- Ensure platforms and environment sprites render properly

---

## Troubleshooting

### Sprites Not Packing

**Problem:** Sprites aren't being included in the atlas

**Solutions:**
1. Ensure sprite **Texture Type** is set to "Sprite (2D and UI)"
2. Check that **Sprite Packer Mode** is not set to "Disabled" in Edit → Project Settings → Editor
3. Make sure sprites are in the correct folder added to the atlas
4. Try clicking **Pack Preview** or **Repack** button

### Atlas Too Large

**Problem:** Atlas exceeds max size (2048x2048)

**Solutions:**
1. Split into multiple atlases by category
2. Reduce individual sprite sizes
3. Remove unused sprites
4. Increase max atlas size (carefully - may not work on older devices)

### Low Packing Efficiency

**Problem:** Atlas utilization is below 60%

**Solutions:**
1. Enable **Tight Packing**
2. Enable **Allow Rotation**
3. Adjust **Padding** (lower padding = better packing, but may cause artifacts)
4. Group similar-sized sprites together

### Sprites Look Blurry

**Problem:** Sprites appear lower quality after atlasing

**Solutions:**
1. Increase atlas **Max Size**
2. Change compression format to higher quality (e.g., RGBA32)
3. Check **Filter Mode** is set correctly (Point for pixel art, Bilinear for smooth sprites)
4. Disable **Generate Mip Maps** if enabled

### Platform-Specific Issues

**Android:**
- Use ASTC compression (widely supported)
- Test on actual devices, not just emulator

**iOS:**
- Use ASTC or PVRTC compression
- Test on actual devices for accurate performance

**Old Devices:**
- Consider reducing max atlas size to 1024 or 2048
- Use more aggressive compression

---

## Performance Monitoring

### Key Metrics to Track

**In Unity Profiler (Rendering Section):**
- **Batches:** Should decrease significantly (target: < 50 per frame)
- **SetPass Calls:** Should decrease (target: < 30 per frame)
- **Vertices:** Should stay roughly the same
- **Triangles:** Should stay roughly the same

**In Memory Profiler:**
- **Texture Memory:** May increase slightly (atlases are larger individual textures)
- **Total Memory:** Should be similar or lower overall

**Frame Rate:**
- Should improve, especially on lower-end devices
- Target: Stable 60 FPS on mobile

### Before/After Comparison

Create a performance benchmark:

```
Before Sprite Atlases:
- Batches: ~150
- SetPass Calls: ~80
- FPS: ~45 (mobile)
- Texture Memory: ~150 MB

After Sprite Atlases (Target):
- Batches: ~40
- SetPass Calls: ~25
- FPS: ~60 (mobile)
- Texture Memory: ~120 MB
```

---

## Best Practices

### DO:
✓ Group sprites by usage pattern (UI together, characters together)
✓ Use folder references to automatically include new sprites
✓ Enable tight packing and rotation for better efficiency
✓ Test on actual devices, especially for mobile
✓ Use platform-specific compression overrides
✓ Keep atlas size at 2048x2048 or lower for mobile
✓ Monitor draw calls and batches after implementation

### DON'T:
✗ Put all sprites in one giant atlas (hard to manage, may exceed size limits)
✗ Mix frequently updated sprites with static sprites in same atlas
✗ Use uncompressed formats (RGBA32) on mobile
✗ Ignore the Pack Preview - always verify packing
✗ Set max size too high (may not work on older devices)
✗ Forget to test on actual hardware

---

## Integration Checklist

- [ ] Created `Assets/Atlases` folder
- [ ] Created 4 sprite atlases (UI, Character, Platform, Environment)
- [ ] Configured atlas settings (Include in Build, Tight Packing, etc.)
- [ ] Added sprite folders to each atlas
- [ ] Clicked "Pack Preview" to verify packing
- [ ] Set platform-specific compression overrides
- [ ] Tested game - all sprites render correctly
- [ ] Profiled performance - confirmed batch/SetPass reduction
- [ ] Tested on mobile device (if targeting mobile)
- [ ] Verified build size reduction
- [ ] Documented atlas utilization percentages
- [ ] Committed changes to version control

---

## Expected Results

After proper sprite atlas implementation:

**Performance Gains:**
- 50-80% reduction in draw calls
- 40-60% reduction in SetPass calls
- 10-30% FPS improvement on mobile
- Smoother gameplay, fewer frame drops

**Memory Gains:**
- 5-15% reduction in texture memory (better compression)
- Faster texture loading
- Reduced build size (5-20% depending on compression)

**Quality:**
- No visual quality loss (with proper settings)
- Sprites render identically to before
- No gameplay changes required

---

## Maintenance

### When Adding New Sprites:

1. **If using folder references:** Just add sprites to the existing folders - they'll be auto-included in the atlas
2. **If using individual sprite references:** Manually add new sprites to the appropriate atlas
3. **Repack:** Click "Pack Preview" to verify new sprites are included
4. **Test:** Run the game to ensure new sprites render correctly

### When Removing Sprites:

1. Delete the sprite files
2. Click "Repack" on affected atlases to rebuild without removed sprites
3. Verify no broken references in scenes

### Periodic Maintenance:

- Review atlas utilization quarterly
- Look for unused sprites to remove
- Reorganize if atlases become too fragmented
- Update compression settings when targeting new platforms

---

## Additional Resources

- [Unity Sprite Atlas Documentation](https://docs.unity3d.com/Manual/class-SpriteAtlas.html)
- [Unity Sprite Packer](https://docs.unity3d.com/Manual/SpritePacker.html)
- [Optimizing Graphics Performance](https://docs.unity3d.com/Manual/OptimizingGraphicsPerformance.html)

---

**Last Updated:** 2026-01-04
**Applies to Unity Version:** 6000.3.2f1
**Status:** Implementation Guide - Ready for Setup

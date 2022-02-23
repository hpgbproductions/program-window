using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using UnityEngine;

public class ProgramWindowPartBehaviour : Jundroo.SimplePlanes.ModTools.Parts.PartModifierBehaviour
{
    private ProgramWindowPart modifier;

    private MeshRenderer mainRenderer;
    private Material rMat;
    private Texture2D rTex;
    private FilterMode rTexFilterMode;
    private int rTexSize;

    private IntPtr WindowPtr;
    private Bitmap WindowBmp;
    private BitmapData WindowBmpData;

    private int toTexSize;
    private byte[] toTexBytes;

    private string WindowClassName;
    private string WindowTitleName;

    private int UpdatePeriod = 2;

    private byte Opacity = 255;

    private bool UseTransparency = false;
    private int TpThreshold = 0;
    private byte TpColorR = 255;
    private byte TpColorG = 0;
    private byte TpColorB = 255;

    private bool UseCrop = false;
    private int CropBeginX = 0;
    private int CropBeginY = 0;
    private int CropEndX = 255;
    private int CropEndY = 255;

    private void Start()
    {
        modifier = (ProgramWindowPart)PartModifier;

        // Only perform setup in the simulation
        if (!ServiceProvider.Instance.GameState.IsInDesigner && ServiceProvider.Instance.GameState.IsInLevel)
        {
            WindowClassName = modifier.WindowClassName;
            if (WindowClassName == string.Empty)
                WindowClassName = null;
            WindowTitleName = modifier.WindowName;
            if (WindowTitleName == string.Empty)
                WindowTitleName = null;
            WindowPtr = WindowUtility.FindWindow(WindowClassName, WindowTitleName);
            if (WindowPtr.ToInt64() == 0)
            {
                // Invalid window value
                Debug.LogError($"{gameObject}: Could not find Window");
            }
            else
            {
                WindowUtility.DebugWindowInfo(WindowPtr);
            }

            if (modifier.UpdatePeriod < 1)
            {
                Debug.LogWarning("UpdatePeriod must be 1 or greater!");
            }
            UpdatePeriod = Mathf.Max(1, modifier.UpdatePeriod);

            if (!Mathf.IsPowerOfTwo(modifier.MaxSize))
            {
                Debug.LogWarning("MaxSize must be a power of two!");
            }
            rTexSize = Mathf.ClosestPowerOfTwo(modifier.MaxSize);

            rTexFilterMode = modifier.TextureFilterMode;

            Opacity = modifier.Opacity;

            UseTransparency = modifier.TpActive;
            TpThreshold = modifier.TpThreshold;
            TpColorR = modifier.TpColorR;
            TpColorG = modifier.TpColorG;
            TpColorB = modifier.TpColorB;

            UseCrop = modifier.CropActive;
            CropBeginX = modifier.CropBeginX;
            CropBeginY = modifier.CropBeginY;
            CropEndX = modifier.CropEndX;
            CropEndY = modifier.CropEndY;

            // Texture setup
            rTex = new Texture2D(rTexSize, rTexSize, TextureFormat.RGBA32, false, true);
            rTex.filterMode = rTexFilterMode;

            // Material setup
            mainRenderer = transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
            rMat = mainRenderer.materials[0];
            rMat.mainTexture = rTex;

            // 4 bytes per pixel
            toTexSize = rTexSize * rTexSize * 4;
            toTexBytes = new byte[toTexSize];
        }
    }

    private void Update()
    {
        if (Time.frameCount % 60 == 0 && WindowPtr.ToInt64() == 0)
        {
            // Try to get the window
            Start();
            return;
        }

        if (Time.frameCount % UpdatePeriod != 0 || 
            ServiceProvider.Instance.GameState.IsInDesigner || 
            ServiceProvider.Instance.GameState.IsPaused ||
            WindowPtr.ToInt64() == 0)
        {
            // Skip generation of window
            return;
        }

        // Modified from https://docs.microsoft.com/en-us/dotnet/api/system.drawing.bitmap.lockbits
        WindowBmp = WindowUtility.PrintWindow(WindowPtr);
        WindowBmpData = WindowBmp.LockBits(new Rectangle(0, 0, WindowBmp.Width, WindowBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        IntPtr bmpStartPtr = WindowBmpData.Scan0;
        int bmpWidth = WindowBmp.Width;
        int bmpHeight = WindowBmp.Height;
        int bmpSize = bmpWidth * bmpHeight * 4;

        // Store the window bitmap in a byte array (BGRA)
        byte[] rawBmpBytes = new byte[bmpSize];
        Marshal.Copy(bmpStartPtr, rawBmpBytes, 0, bmpSize);

        // Process data and populate the texture data array (RGBA)

        // Rows on Unity are inverted from Bitmap
        int datacount = rTexSize * (rTexSize - 1) * 4;    // Target index on texture data
        int xpos = 0;    // Bitmap position
        int ypos = 0;
        for (int i = 0; i < bmpSize; i += 4)
        {
            if (UseTransparency &&
                ColorDifference(rawBmpBytes[i + 2], rawBmpBytes[i + 1], rawBmpBytes[i], TpColorR, TpColorG, TpColorB) <= TpThreshold)
            {
                // Don't draw pixels with transparent color
                toTexBytes[datacount + 3] = 0;
            }
            else if (UseCrop && (xpos < CropBeginX || ypos < CropBeginY || xpos >= CropEndX || ypos >= CropEndY))
            {
                // Don't draw pixels in cropped region
                toTexBytes[datacount + 3] = 0;
            }
            else if (datacount < toTexSize)
            {
                // Only apply the pixel color if it is in the array
                // No need to check for negative datacount as it is handled by break condition
                toTexBytes[datacount] = rawBmpBytes[i + 2];
                toTexBytes[datacount + 1] = rawBmpBytes[i + 1];
                toTexBytes[datacount + 2] = rawBmpBytes[i];
                toTexBytes[datacount + 3] = Opacity;
            }

            // Set the position for the next pixel
            xpos++;
            if (xpos == bmpWidth)
            {
                // Advance to the position of the next line
                datacount -= 4 * (rTexSize + bmpWidth - 1);
                if (datacount < 0)
                {
                    break;
                }
                xpos = 0;
                ypos++;
            }
            else
            {
                datacount += 4;
            }
        }

        // Set texture data
        rTex.SetPixelData(toTexBytes, 0, 0);
        rTex.Apply();

        WindowBmp.UnlockBits(WindowBmpData);
    }

    private int ColorDifference(int r1, int g1, int b1, int r2, int g2, int b2)
    {
        return Math.Abs(r1 - r2) + Math.Abs(g1 - g2) + Math.Abs(b1 - b2);
    }
}
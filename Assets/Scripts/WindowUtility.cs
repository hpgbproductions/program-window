/*
 * Reference:
 * 
 * EnumWindowsProc, GetWindowText(1args), GetClassName(1args), FindWindows
 * https://stackoverflow.com/a/20276701
 * 
 * PrintWindow
 * https://stackoverflow.com/a/911225
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using UnityEngine;

public class WindowUtility : MonoBehaviour
{
    #region Retrieve window information modified from https://stackoverflow.com/a/20276701

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder strText, int maxCount);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    // Delegate to filter which windows to include 
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    /// <summary> Get the text for the window pointed to by hWnd </summary>
    public static string GetWindowText(IntPtr hWnd)
    {
        int size = GetWindowTextLength(hWnd);
        if (size > 0)
        {
            var builder = new StringBuilder(size + 1);
            GetWindowText(hWnd, builder, builder.Capacity);
            return builder.ToString();
        }

        return String.Empty;
    }

    public static string GetClassName(IntPtr hWnd)
    {
        // Window class name size is limited to 256
        var builder = new StringBuilder(256);
        GetClassName(hWnd, builder, builder.Capacity);
        return builder.ToString();
    }

    /// <summary> Find all windows that match the given filter </summary>
    /// <param name="filter"> A delegate that returns true for windows
    ///    that should be returned and false for windows that should
    ///    not be returned </param>
    public static IEnumerable<IntPtr> FindWindows(EnumWindowsProc filter)
    {
        IntPtr found = IntPtr.Zero;
        List<IntPtr> windows = new List<IntPtr>();

        EnumWindows(delegate(IntPtr wnd, IntPtr param)
        {
            if (filter(wnd, param))
            {
                // only add the windows that pass the filter
                windows.Add(wnd);
            }

            // but return true here so that we iterate all windows
            return true;
        }, IntPtr.Zero);

        return windows;
    }

    /// <summary> Find all windows that contain the given title text </summary>
    /// <param name="titleText"> The text that the window title must contain. </param>
    public static IEnumerable<IntPtr> FindWindowsWithText(string titleText)
    {
        return FindWindows(delegate(IntPtr wnd, IntPtr param)
        {
            return GetWindowText(wnd).Contains(titleText);
        });
    }

    /// <summary> Find all windows without filtering </summary>
    public static IEnumerable<IntPtr> FindWindowsAll()
    {
        return FindWindows(delegate (IntPtr wnd, IntPtr param)
        {
            return true;
        });
    }

    #endregion

    #region Capture window contents modified from https://stackoverflow.com/a/911225

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

    public static Bitmap PrintWindow(IntPtr hwnd)
    {
        GetWindowRect(hwnd, out RECT rc);
        Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
        System.Drawing.Graphics gfxBmp = System.Drawing.Graphics.FromImage(bmp);
        IntPtr hdcBitmap = gfxBmp.GetHdc();

        PrintWindow(hwnd, hdcBitmap, 0);

        gfxBmp.ReleaseHdc(hdcBitmap);
        gfxBmp.Dispose();

        return bmp;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public int Width
        {
            get { return Right - Left; }
        }

        public int Height
        {
            get { return Bottom - Top; }
        }
    }

    #endregion

    public static void DebugWindowInfo(IntPtr hWnd)
    {
        string titleName = GetWindowText(hWnd);
        string className = GetClassName(hWnd);
        if (string.IsNullOrEmpty(titleName))
            Debug.Log($"Found Window with null or empty name:\nClass: {className}\nHandle: 0x{hWnd.ToInt64():X8}");
        else
            Debug.Log($"Found Window: {titleName}\nClass: {className}\nHandle: 0x{hWnd.ToInt64():X8}");
    }

    private void Awake()
    {
        ServiceProvider.Instance.DevConsole.RegisterCommand("ListWindows", ListWindows);
        ServiceProvider.Instance.DevConsole.RegisterCommand<string>("ListWindowsWithName", ListWindowsWithName);
    }

    private void OnDestroy()
    {
        ServiceProvider.Instance.DevConsole.UnregisterCommand("ListWindows");
        ServiceProvider.Instance.DevConsole.UnregisterCommand("ListWindowsWithName");
    }

    public void ListWindows()
    {
        IEnumerable<IntPtr> wnds = FindWindowsAll();
        foreach (IntPtr wnd in wnds)
        {
            DebugWindowInfo(wnd);
        }
    }

    public void ListWindowsWithName(string name)
    {
        IEnumerable<IntPtr> wnds = FindWindowsWithText(name);
        foreach (IntPtr wnd in wnds)
        {
            DebugWindowInfo(wnd);
        }
    }
}

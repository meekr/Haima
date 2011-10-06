using UnityEngine;
using System.Runtime.InteropServices;

public class UIBinding
{
    [DllImport("__Internal")]
    private static extern void _ActivateUICalculator();
    public static void ActivateUICalculator()
    {
        // Call plugin only when running on real device
        if (Application.platform != RuntimePlatform.OSXEditor) {
            _ActivateUICalculator();
        }
    }

    [DllImport("__Internal")]
    private static extern void _DeactivateUICalculator();
    public static void DeactivateUICalculator()
    {
        // Call plugin only when running on real device
        if (Application.platform != RuntimePlatform.OSXEditor) {
            _DeactivateUICalculator();
        }
    }
	
	[DllImport("__Internal")]
    private static extern void _ActivateUIConfig();
    public static void ActivateUIConfig()
    {
        // Call plugin only when running on real device
        if (Application.platform != RuntimePlatform.OSXEditor) {
            _ActivateUIConfig();
        }
    }

    [DllImport("__Internal")]
    private static extern void _DeactivateUIConfig();
    public static void DeactivateUIConfig()
    {
        // Call plugin only when running on real device
        if (Application.platform != RuntimePlatform.OSXEditor) {
            _DeactivateUIConfig();
        }
    }
}
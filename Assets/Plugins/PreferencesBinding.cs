using UnityEngine;
using System.Runtime.InteropServices;

public class PreferencesBinding
{
	[DllImport("__Internal")]
    private static extern void _GetRotation (string levelName, float [] rotation);
 
    /// <summary>
    /// Gets the rotation from standard user defaults
    /// </summary>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <param name='rotation'>
    /// Rotation.
    /// </param>
    public static void GetRotation (string levelName, out Quaternion rotation)
    {
        if (Application.platform != RuntimePlatform.OSXEditor) {
            
            float[] rot = {0, 0, 0, 0};
            
            _GetRotation (levelName, rot);
            
            rotation.x = rot[0];
            rotation.y = rot[1];
            rotation.z = rot[2];
            rotation.w = rot[3];
            
        } else {
            
            rotation = Quaternion.identity;
        }
    }
    
    [DllImport("__Internal")]
    private static extern void _SetRotation (string levelName, float [] rotation);
 
    /// <summary>
    /// Sets the rotation in standard user defaults
    /// </summary>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <param name='rotation'>
    /// Rotation.
    /// </param>
    public static void SetRotation (string levelName, Quaternion rotation)
    {
        // Call plugin only when running on real device
        if (Application.platform != RuntimePlatform.OSXEditor) {
            
            float[] rot = {0, 0, 0, 0};
   
            rot[0] = rotation.x;
            rot[1] = rotation.y;
            rot[2] = rotation.z;
            rot[3] = rotation.w;
            
            _SetRotation (levelName, rot);
        }
    }
}
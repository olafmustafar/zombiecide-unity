using UnityEditor;

public class WebGl : EditorWindow
{
    [MenuItem("WebGL/Enable Embedded Resources")]
    public static void EnableEmbeddedResources()
    {
        PlayerSettings.WebGL.useEmbeddedResources = true;
    }
}

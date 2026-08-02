using UnityEditor;
using UnityEngine;
using System.IO;

public class ExtractAnimations : MonoBehaviour
{
    [MenuItem("Assets/Extract Selected Animations", false, 10)]
    private static void ExtractAnimClips()
    {
        foreach (Object obj in Selection.objects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            foreach (Object subAsset in subAssets)
            {
                if (subAsset is AnimationClip clip && !clip.name.StartsWith("__preview__"))
                {
                    AnimationClip newClip = new AnimationClip();
                    EditorUtility.CopySerialized(clip, newClip);

                    // Clean invalid filename characters from the clip name
                    string cleanClipName = SanitizeFileName(clip.name);

                    string directory = Path.GetDirectoryName(assetPath);
                    string newPath = Path.Combine(directory, $"{cleanClipName}_Extracted.anim");

                    AssetDatabase.CreateAsset(newClip, AssetDatabase.GenerateUniqueAssetPath(newPath));
                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Selected animation clips extracted successfully.");
    }

    // Strips invalid OS characters from the clip name
    private static string SanitizeFileName(string name)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        foreach (char c in invalidChars)
        {
            name = name.Replace(c.ToString(), "");
        }
        
        // Also replace colons and slashes explicitly (common in FBX namespaces/mixamo clips)
        name = name.Replace(":", "_")
                   .Replace("/", "_")
                   .Replace("\\", "_");

        return string.IsNullOrWhiteSpace(name) ? "ExtractedAnimation" : name;
    }

    [MenuItem("Assets/Extract Selected Animations", true)]
    private static bool ValidateExtractAnimClips()
    {
        return Selection.activeObject != null;
    }
}
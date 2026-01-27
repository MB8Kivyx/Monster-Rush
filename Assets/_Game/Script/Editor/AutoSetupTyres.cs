using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoSetupTyres
{
    static AutoSetupTyres()
    {
        EditorApplication.delayCall += CheckAndSetup;
    }

    static void CheckAndSetup()
    {
        // Try to find Player by Tag first
        GameObject player = null;
        try { player = GameObject.FindGameObjectWithTag("Player"); } catch {}

        // Fallback: Find by Name
        if (player == null) player = GameObject.Find("Player");
        if (player == null) player = GameObject.Find("Car"); // Common name guess

        if (player != null)
        {
            TyreController script = player.GetComponent<TyreController>();
            if (script == null)
            {
                script = player.AddComponent<TyreController>();
                Debug.Log($"<color=cyan>[Antigravity Setup]: Attached 'TyreController' to object '{player.name}' successfully!</color>");
            }
            
            // NOTE: We rely on the Runtime Auto-Discovery we added to TyreRotation.cs earlier
            // to find the actual wheel children when the game Starts.
        }
    }
}

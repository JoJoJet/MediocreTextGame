using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "VignetteIndexer", menuName = "DataStealing/VignetteIndexer", order = 1)]
public class VignetteIndexer : ScriptableObject
{
    [Header("Search for incomplete:")]
    [SerializeField]
    bool parents;
    [SerializeField]
    bool images, text, questions, branch;

    public Vignette[] DoSearch()
        => AssetDatabase.FindAssets("t:" + typeof(Vignette).Name)
           .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
           .Select(path => AssetDatabase.LoadAssetAtPath<Vignette>(path))
           .Where(v => !v.parentFinished && parents || !v.imageFinished && images || !v.textFinished && text || !v.questionFinished && questions || !v.branchFinished && branch)
           .ToArray();
}

[CustomEditor(typeof(VignetteIndexer))]
public class VignetteIndexerEditor : Editor
{
    private Vignette[] results = null;

    public override void OnInspectorGUI()
    {
        var target = (VignetteIndexer)this.target;

        DrawDefaultInspector();

        if(GUILayout.Button("Search Vignettes")) {
            results = target.DoSearch();
        }

        if(results != null) {
            GUILayout.Label($"{results.Length} results:");

            for(int i = 0; i < results.Length; i++) {
                EditorGUILayout.ObjectField(new GUIContent($"Result #{i+1}"), results[i], typeof(Vignette), false);
            }
        }
    }
}


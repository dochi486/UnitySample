using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using System;

public class ChangeShader : ScriptableWizard
{
    [MenuItem("Tools/ChangeShader", false, -99)]
    private static void Init()
    {
        ChangeShader changeShader = DisplayWizard<ChangeShader>("Change Shader", "Change");

        changeShader.shader = Shader.Find("Unlit/Texture");
    }

    public enum ChangeType
    {
        SelectedOnly,
        All,
    }

    public ChangeType type = ChangeType.All;
    public Shader shader;

    private void OnWizardCreate()
    {
        switch (type)
        {
            case ChangeType.All:
                ChangeShaderAll();
                break;

            case ChangeType.SelectedOnly:
                ChangeShaderSelected();
                break;
        }

        AssetDatabase.SaveAssets();
    }

    private void ChangeShaderSelected()
    {
        foreach (var item in Selection.objects)
        {
            if (item is GameObject == false)
                continue;

            GameObject go = item as GameObject;
            var renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers == null)
                continue;
            foreach (var mat in renderers)
            {
                if (mat.sharedMaterial.shader == shader)
                    continue;

                if (mat.sharedMaterial == null)
                    continue;

                mat.sharedMaterial.shader = shader;
            }
        }
    }

    private void ChangeShaderAll()
    {
        //shader 씬에 있는 모든 렌더러를 찾아서 쉐이더를 변경하자.
        List<GameObject> rootObjects = GetRootGameObject();

        for (int rootIndex = 0; rootIndex < rootObjects.Count; rootIndex++)
        {
            GameObject rootGO = rootObjects[rootIndex];
            Renderer[] renderers = rootGO.GetComponentsInChildren<Renderer>(true);

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].sharedMaterial == null)
                    continue;
                renderers[i].sharedMaterial.shader = shader;
            }
        }
    }

    private List<GameObject> GetRootGameObject()
    {
        List<GameObject> rootObjects = new List<GameObject>();
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.transform.parent == null)
            {
                rootObjects.Add(obj);
            }
        }
        return rootObjects;
    }
}
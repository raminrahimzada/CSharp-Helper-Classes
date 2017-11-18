using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShowHideObject {

    public static void HideObject(this MonoBehaviour behaviour)
    {
        MeshRenderer render = behaviour.gameObject.GetComponentInChildren<MeshRenderer>();

        render.enabled = false;
        behaviour.HideChildren();
    }
    public static void ShowObject(this MonoBehaviour behaviour)
    {
        MeshRenderer render = behaviour.gameObject.GetComponentInChildren<MeshRenderer>();

        render.enabled = true;
        behaviour.ShowChildren();
    }
    static void HideChildren(this MonoBehaviour behaviour)
    {
        Renderer[] lChildRenderers = behaviour.gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer lRenderer in lChildRenderers)
        {
            lRenderer.enabled = false;
        }
    }
    static void ShowChildren(this MonoBehaviour behaviour)
    {
        Renderer[] lChildRenderers = behaviour.gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer lRenderer in lChildRenderers)
        {
            lRenderer.enabled = true;
        }
    }
}

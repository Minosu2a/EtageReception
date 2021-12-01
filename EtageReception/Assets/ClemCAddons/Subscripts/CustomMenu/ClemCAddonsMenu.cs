using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ClemCAddonsMenu
{
    private static GameObject _toRename;
    private static double _renameTime;

    [MenuItem("GameObject/ClemCAddons/Create Container/Inherit All", false, 0)]
    public static void CreateContainerAll(MenuCommand menuCommand)
    {
        var target = Selection.activeGameObject;
        GameObject container = new GameObject(target.name + " Container");
        GameObjectUtility.SetParentAndAlign(container, target.transform.parent.gameObject);
        container.transform.SetSiblingIndex(target.transform.GetSiblingIndex());
        container.transform.localPosition = target.transform.localPosition;
        container.transform.localRotation = target.transform.localRotation;
        target.transform.parent = container.transform;
        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;
        Selection.activeObject = container;
        RenameFile(container);
    }

    [MenuItem("GameObject/ClemCAddons/Create Container/Inherit Position", false, 0)]
    public static void CreateContainerPos(MenuCommand menuCommand)
    {
        var target = Selection.activeGameObject;
        GameObject container = new GameObject(target.name + " Container");
        GameObjectUtility.SetParentAndAlign(container, target.transform.parent.gameObject);
        container.transform.SetSiblingIndex(target.transform.GetSiblingIndex());
        container.transform.localPosition = target.transform.localPosition;
        target.transform.parent = container.transform;
        target.transform.localPosition = Vector3.zero;
        Selection.activeObject = container;
        RenameFile(container);
    }

    [MenuItem("GameObject/ClemCAddons/Create Container/Inherit Rotation", false, 0)]
    public static void CreateContainerRot(MenuCommand menuCommand)
    {
        var target = Selection.activeGameObject;
        GameObject container = new GameObject(target.name + " Container");
        GameObjectUtility.SetParentAndAlign(container, target.transform.parent.gameObject);
        container.transform.SetSiblingIndex(target.transform.GetSiblingIndex());
        container.transform.localRotation = target.transform.localRotation;
        target.transform.parent = container.transform;
        target.transform.localRotation = Quaternion.identity;
        Selection.activeObject = container;
        RenameFile(container);
    }


    [MenuItem("GameObject/ClemCAddons/Create Container/Inherit None", false, 0)]
    public static void CreateContainerNone(MenuCommand menuCommand)
    {
        var target = Selection.activeGameObject;
        GameObject container = new GameObject(target.name + " Container");
        GameObjectUtility.SetParentAndAlign(container, target.transform.parent.gameObject);
        container.transform.SetSiblingIndex(target.transform.GetSiblingIndex());
        container.transform.localPosition = Vector3.zero;
        target.transform.parent = container.transform;
        Selection.activeObject = container;
        RenameFile(container);
    }

    [MenuItem("GameObject/ClemCAddons/Give Container/All", false, 0)]
    public static void GiveContainerAll(MenuCommand menuCommand)
    {
        var target = Selection.activeTransform;
        target.parent.localPosition = target.localPosition;
        target.localPosition = Vector3.zero;
        target.parent.localRotation = target.localRotation;
        target.localRotation = Quaternion.identity;
        target.parent.localScale = target.localScale;
        target.localScale = Vector3.zero;
    }

    [MenuItem("GameObject/ClemCAddons/Give Container/Position", false, 0)]
    public static void GiveContainerPos(MenuCommand menuCommand)
    {
        var target = Selection.activeTransform;
        target.parent.localPosition = target.localPosition;
        target.localPosition = Vector3.zero;
    }

    [MenuItem("GameObject/ClemCAddons/Give Container/Rotation", false, 0)]
    public static void GiveContainerRot(MenuCommand menuCommand)
    {
        var target = Selection.activeTransform;
        target.parent.localRotation = target.localRotation;
        target.localRotation = Quaternion.identity;
    }

    [MenuItem("GameObject/ClemCAddons/Give Container/Scale", false, 0)]
    public static void GiveContainerScale(MenuCommand menuCommand)
    {
        var target = Selection.activeTransform;
        target.parent.localScale = target.localScale;
        target.localScale = Vector3.zero;
    }

    [MenuItem("GameObject/ClemCAddons/Give Container/Position & Rotation", false, 0)]
    public static void GiveContainerPosRot(MenuCommand menuCommand)
    {
        var target = Selection.activeTransform;
        target.parent.localPosition = target.localPosition;
        target.localPosition = Vector3.zero;
        target.parent.localRotation = target.localRotation;
        target.localRotation = Quaternion.identity;
    }
    [MenuItem("GameObject/ClemCAddons/Give Container/World/All", false, 0)]
    public static void GiveContainerWAll(MenuCommand menuCommand)
    {
        var target = Selection.activeTransform;
        target.parent.localPosition = target.localPosition;
        target.localPosition = Vector3.zero;
        target.parent.localRotation = target.localRotation;
        target.localRotation = Quaternion.identity;
        target.parent.localScale = target.localScale;
        target.localScale = Vector3.zero;
    }

    [MenuItem("GameObject/ClemCAddons/Give Container/World/Position", false, 0)]
    public static void GiveContainerWPos(MenuCommand menuCommand)
    {
        var target = Selection.activeTransform;
        target.parent.localPosition = target.localPosition;
        target.localPosition = Vector3.zero;
    }

    [MenuItem("GameObject/ClemCAddons/Give Container/World/Rotation", false, 0)]
    public static void GiveContainerWRot(MenuCommand menuCommand)
    {
        var target = Selection.activeTransform;
        target.parent.localRotation = target.localRotation;
        target.localRotation = Quaternion.identity;
    }

    [MenuItem("GameObject/ClemCAddons/Give Container/World/Scale", false, 0)]
    public static void GiveContainerWScale(MenuCommand menuCommand)
    {
        var target = Selection.activeTransform;
        target.parent.localScale = target.localScale;
        target.localScale = Vector3.zero;
    }

    [MenuItem("GameObject/ClemCAddons/Give Container/World/Position & Rotation", false, 0)]
    public static void GiveContainerWPosRot(MenuCommand menuCommand)
    {
        var target = Selection.activeTransform;
        target.parent.localPosition = target.localPosition;
        target.localPosition = Vector3.zero;
        target.parent.localRotation = target.localRotation;
        target.localRotation = Quaternion.identity;
    }

    private static void RenameFile(GameObject gameObject)
    {
        _toRename = gameObject;
        _renameTime = EditorApplication.timeSinceStartup + 0.2d;
        EditorApplication.update += EngageRenameMode;
    }

    private static void EngageRenameMode()
    {
        if (EditorApplication.timeSinceStartup >= _renameTime)
        {
            EditorApplication.update -= EngageRenameMode;
            var e = new Event { keyCode = KeyCode.F2, type = EventType.KeyDown }; // or Event.KeyboardEvent("f2");
            EditorWindow.focusedWindow.SendEvent(e);
        }
    }

}

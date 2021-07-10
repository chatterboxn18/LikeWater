using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(inspectedType: typeof(SimpleButton))]
[CanEditMultipleObjects]
public class SimpleButtonEditor : SelectableEditor
{
	[MenuItem("GameObject/UI/SimpleButton")]
	public static void CreateSimpleButton(MenuCommand menuCommand)
	{
		CreateSimpleButton(menuCommand.context as GameObject, "button-");
	}

	private static void CreateSimpleButton(GameObject parent, string name)
	{
		var buttonObj = new GameObject(name, typeof(RectTransform));
		GameObjectUtility.SetParentAndAlign(buttonObj, parent);
		Undo.RegisterCreatedObjectUndo(buttonObj, "Create " + buttonObj.name);
		var image = buttonObj.AddComponent<Image>();
		image.color = new Color(1,1,1,0);
		Selection.activeObject = buttonObj;
		
		var contentObj = new GameObject("group-content", typeof(RectTransform));
		GameObjectUtility.SetParentAndAlign(contentObj.gameObject, buttonObj);
		var contentRect = contentObj.GetComponent<RectTransform>();
		contentRect.anchorMin = Vector2.zero;
		contentRect.anchorMax = Vector2.one;
		contentRect.offsetMin = Vector2.zero;
		contentRect.offsetMax = Vector2.zero;
		contentObj.AddComponent<CanvasGroup>();

		var button = buttonObj.AddComponent<SimpleButton>();
		button.targetGraphic = image;
		button.transition = Selectable.Transition.None;
		button.navigation = new Navigation();
		button.canvasGroup = contentObj.GetComponent<CanvasGroup>();
		button.canvasGroup.alpha = 1;
		button.enabled = true;
	}
	
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		serializedObject.Update();
		//EditorGUILayout.PropertyField(m_InteractableProperty);

		EditorGUILayout.PropertyField(serializedObject.FindProperty("canvasGroup"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_isVisible"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("EvtPointerDown"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("EvtPointerUp"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("EvtPointerClick"), true);
		serializedObject.ApplyModifiedProperties();

	}
}

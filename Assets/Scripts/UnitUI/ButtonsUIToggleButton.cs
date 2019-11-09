using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ButtonsUIToggleButton : MonoBehaviour, IButtonSignalHandler
{
    private int CurrentIndex = 0;

    private List<(DragType, Sprite)> ButtonSprites;

    [SerializeField]
    private List<DragType> DragTypes;
    [SerializeField]
    private List<Sprite> Sprites;

    public ButtonsUIToggleButton()
    {
        ButtonSprites = new List<(DragType, Sprite)>();
    }

    protected void Awake()
    {
        Debug.Assert(DragTypes.Count == Sprites.Count);
        for (int i = 0; i < DragTypes.Count; i++)
        {
            ButtonSprites.Add((DragTypes[i], Sprites[i]));
        }
    }

    public void OnClick()
    {
        CurrentIndex++;
        if (CurrentIndex >= ButtonSprites.Count)
            CurrentIndex = 0;
        GetComponent<Image>().sprite = ButtonSprites[CurrentIndex].Item2;
    }

    public void CloseButton()
    {
        this.enabled = false;
    }

    public void OpenButton()
    {
        this.enabled = true;
    }

    public DragType CurrentDragType
    {
        get => ButtonSprites[CurrentIndex].Item1;
    }


#if UNITY_EDITOR
    // somehow this is not working
    /*
    [CustomEditor(typeof(ButtonsUIToggleButton))]
    public class ButtonsUIToggleButtonEditor : Editor
    {
        bool folding = true;

        public override void OnInspectorGUI()
        {
            var instance = target as ButtonsUIToggleButton;
            var serialized = new SerializedObject(instance);
            serialized.Update();
            serialized.ApplyModifiedProperties();

            List<(DragType, Sprite)> list = instance.ButtonSprites;
            int len = list.Count;

            if (folding = EditorGUILayout.Foldout(folding, "ButtonElements"))
            {
                for (int i = 0; i < len; ++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(i.ToString());
                    DragType item1 = (DragType)EditorGUILayout.EnumFlagsField(list[i].Item1);
                    Sprite item2 = EditorGUILayout.ObjectField(list[i].Item2, typeof(Sprite), true) as Sprite;
                    list[i] = (item1, item2);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Add");
                DragType newItem1 = (DragType)EditorGUILayout.EnumFlagsField(DragType.NORMAL);
                Sprite newItem2 = EditorGUILayout.ObjectField(null, typeof(Sprite), true) as Sprite;
                EditorGUILayout.EndHorizontal();
                if (newItem2 != null) // newItem1 is non-null.
                    list.Add((newItem1, newItem2));
            }

            if (GUI.changed)
            {
                Debug.Log("GUI Changed");
                Debug.Log(instance.ButtonSprites.Count);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            }

            serialized.Update();
            serialized.ApplyModifiedProperties();
        }
    }*/
#endif
}
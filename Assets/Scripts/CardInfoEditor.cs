using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardInfo))]
public class CardInfoEditor : Editor
{
    SerializedProperty CardMerges;
    SerializedProperty CardOutputs;


    void OnEnable()
    {
        // Assuming "yourIntField" is the int field you want to fill
        CardMerges = serializedObject.FindProperty("CardInput");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw the default inspector fields
        DrawDefaultInspector();


        // Handle drag-and-drop events
        HandleDragAndDrop();

        serializedObject.ApplyModifiedProperties();
    }

    void HandleDragAndDrop()
    { 
        Event currentEvent = Event.current;

        if (currentEvent.type == EventType.DragUpdated || currentEvent.type == EventType.DragPerform)
        {

            if (DragAndDrop.objectReferences.Length > 0 && DragAndDrop.objectReferences[0] is CardInfo)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (currentEvent.type == EventType.DragPerform)
                {



                    CardInfo draggedCard = DragAndDrop.objectReferences[0] as CardInfo;


                    CardMerges.arraySize++;
                    SerializedProperty cardToMerge = CardMerges.GetArrayElementAtIndex(CardMerges.arraySize - 1);
                    cardToMerge.stringValue = draggedCard.Name;

                    serializedObject.ApplyModifiedProperties();
                }

        // Consume the event
        currentEvent.Use();
            }
        }
    }

}

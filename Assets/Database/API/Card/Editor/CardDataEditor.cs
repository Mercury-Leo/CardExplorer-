using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Database.API.Card.Extra;
using UnityEditor;
using UnityEngine;

namespace Database.API.Card.Editor
{
    [CustomEditor(typeof(CardData))]
    [CanEditMultipleObjects]
    public class CardDataEditor : UnityEditor.Editor
    {
        private class ExtraDataIntKeyAndName
        {
            public readonly int SelectionKey;
            public readonly string Name;
            
            public ExtraDataIntKeyAndName(int selectionKey, string name)
            {
                SelectionKey = selectionKey;
                Name = name;
            }
        }
        
        private SerializedProperty m_ArtProperty;
        private CardData m_CardData;
        
        private Dictionary<Type, ExtraDataIntKeyAndName> m_AvailableExtraDataTypes;
        
        private static int s_SelectedDataType;
        
        private void OnEnable()
        {
            m_CardData = serializedObject.targetObject as CardData;
            m_ArtProperty = serializedObject.FindProperty("m_Art");
            GetExtraDataTypes();
        }
        
        public override void OnInspectorGUI()
        {
            ShowExtraDataSelection();
            base.OnInspectorGUI();
            ShowCardGraphic();
            
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
        
        private void GetExtraDataTypes()
        {
            m_AvailableExtraDataTypes = new Dictionary<Type, ExtraDataIntKeyAndName>();
            int i = 0;
            Type[] extraDataTypes = typeof(ExtraData).Assembly.GetTypes();
            foreach (Type extraDataType in extraDataTypes)
            {
                if (typeof(ExtraData).IsAssignableFrom(extraDataType) 
                    && !extraDataType.IsInterface 
                    && !extraDataType.IsAbstract
                    && extraDataType != typeof(ExtraData))
                {
                    m_AvailableExtraDataTypes.Add(
                        extraDataType, 
                        new ExtraDataIntKeyAndName(i++, extraDataType.Name.Replace("ExtraData", string.Empty)));
                }
            }

            if (m_CardData.ExtraData == null)
            {
                // Setting some default extra data to begin with.
                m_CardData.SetExtraData(new SpellExtraData());
            }
            
            if (m_CardData.ExtraData != null)
            {
                s_SelectedDataType = m_AvailableExtraDataTypes[m_CardData.ExtraData.GetType()].SelectionKey;
            }
        }
        
        private void ShowExtraDataSelection()
        {
            string[] possibleSelections = new string[m_AvailableExtraDataTypes.Count];
            foreach (var keyValuePair in m_AvailableExtraDataTypes)
            {
                possibleSelections[keyValuePair.Value.SelectionKey] = keyValuePair.Value.Name;
            }
            
            GUILayout.Space(5);
            int selection = EditorGUILayout.Popup(s_SelectedDataType, possibleSelections);
            GUILayout.Space(5);
            
            if (selection != s_SelectedDataType)
            {
                s_SelectedDataType = selection;

                Type selectedType = m_AvailableExtraDataTypes.
                    First(x => x.Value.SelectionKey == selection).Key;
                ConstructorInfo constructor = (selectedType.GetConstructor(Type.EmptyTypes));
                m_CardData.SetExtraData(constructor?.Invoke(null) as ExtraData);
            }
        }
        
        private void ShowCardGraphic()
        {
            if (m_ArtProperty.objectReferenceValue != null)
            {
                GUILayout.Space(5);
                GUILayout.Label("Artwork Preview:");
                GUILayout.Label((m_ArtProperty.objectReferenceValue as Sprite)?.texture,
                    GUILayout.MaxHeight(200),
                    GUILayout.MaxWidth(200));
            }
        }
    }
}
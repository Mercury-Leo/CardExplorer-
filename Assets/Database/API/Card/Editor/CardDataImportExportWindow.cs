using System;
using Database.API.Card.Extra;
using UnityEditor;
using UnityEngine;

namespace Database.API.Card.Editor
{
    public class CardDataImportExportWindow : EditorWindow
    {
        string m_Source;
        private CardDatabase m_CardDatabase;
        
        private static CardDataImportExportWindow m_Window;
                
        [MenuItem("Card Database/Import Export")]
        private static void Init()
        {
            m_Window = (CardDataImportExportWindow)GetWindow(typeof(CardDataImportExportWindow));
            m_Window.InitCardDatabase();
            m_Window.Show();
        }
        
        private void InitCardDatabase()
        {
            try
            {
                Type[] CardDatabaseType = typeof(CardDatabase).Assembly.GetTypes();
            
                foreach (Type factoryType in CardDatabaseType)
                {
                    if (typeof(CardDatabase).IsAssignableFrom(factoryType) 
                        && !factoryType.IsInterface 
                        && !factoryType.IsAbstract)
                    {
                        m_CardDatabase = (CardDatabase)Activator.CreateInstance(factoryType);
                        if (m_CardDatabase == null)
                        {
                            ShowOperationFailedDialog("Could not create card database! It should have an empty ctor.", "Init database");
                        }
                        
                        m_CardDatabase?.Init();
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                ShowOperationFailedDialog("Could not init database! See Unity Console for more info", "Init database");
            }
            
            ShowOperationFailedDialog("Could not find database, did you implement CardDatabase?", "Find database");
        }
        
        void OnGUI()
        {
            GUILayout.Label("Card Data Import Export", EditorStyles.boldLabel);
            
            m_Source = EditorGUILayout.TextField("Source", m_Source);
                      
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Import"))
            {
                Import();
            }

            if (GUILayout.Button("Export"))
            {
                Export();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            if (GUILayout.Button("Re-Init Database"))
            {
                InitCardDatabase();
            }
            
            if (GUILayout.Button("Create New CardData Asset"))
            {
                CardData.BuildNewAsset($"New{System.Guid.NewGuid().ToString().Substring(0, 5)}", 0, string.Empty, new MinionExtraData());
            }
        }

        private void Import()
        {
            try
            {
                m_CardDatabase.ImportCardsFromSource(m_Source);
            }
            catch (Exception e)
            {
                ShowOperationFailedDialog("Failed to import from source! See Unity Console for more info.", "Import");
                Debug.Log(e);
            }
        }
        
        private void Export()
        {
            try
            {
                m_CardDatabase.ExportCardsToSource(m_Source);
            }
            catch (Exception e)
            {
                ShowOperationFailedDialog("Failed to export from source! See Unity Console for more info.", "Export");
                Debug.Log(e);
            }
        }
        
        private void ShowOperationFailedDialog(string message, string operation)
        {
            EditorUtility.DisplayDialog(title: $"{operation} Failed", message, "ok");
        }
    }
}
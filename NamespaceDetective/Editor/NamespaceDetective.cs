using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
// This class defines an Editor Window to detect and manage namespaces in the Unity editor
public class NamespaceDetective : EditorWindow
{
    // Reference to the NamespaceInfoManager which stores namespace information
    private NamespaceInfoManager namespaceInfoManager;
    private Vector2 scrollViewVector;

    private string[] nameSpaces;
    private string[] defineSymbols;
    private NamespaceInfo.CheckStates[] states;

    // Creates a new window under the "NamespaceDetective" menu
    [MenuItem("NamespaceDetective/Search")]
    public static void ShowWindow()
    {
        GetWindow<NamespaceDetective>("NamespaceDetective");
    }

    // Initializes arrays for namespaces, symbols, and states from NamespaceInfoManager
    private void SetArrays()
    {
        nameSpaces = new string[namespaceInfoManager.Infos.Count];
        defineSymbols = new string[namespaceInfoManager.Infos.Count];
        states = new NamespaceInfo.CheckStates[namespaceInfoManager.Infos.Count];

        for(int i=0; i < nameSpaces.Length; i++)
        {
            nameSpaces[i] = namespaceInfoManager.Infos[i].searchNamespace;
            defineSymbols[i] = namespaceInfoManager.Infos[i].defineSymbol;
            states[i] = namespaceInfoManager.Infos[i].checkState;
        }
    }

    private void OnFocus() 
    {
        Initialize();
    }

    private void Initialize()
    {
        namespaceInfoManager = AssetDatabase.LoadAssetAtPath("Assets/NamespaceDetective/Prefabs/NamespaceInfoManager.prefab",
        typeof(NamespaceInfoManager)) as NamespaceInfoManager;

        SetArrays();
    }

    private void OnGUI() 
    {
        scrollViewVector = GUI.BeginScrollView(new Rect(25, 45, position.width - 30, position.height), scrollViewVector, new Rect(0, 0, 400, 500));

        for(int i=0; i < namespaceInfoManager.Infos.Count; i++)
        {
            GUILibraryInfoList(i);
        }
        GUILayout.Space(20);

        GUIAddAndRemove();
        GUILayout.Space(20);

        GUIDetectNamespaces();
        GUILayout.Space(20);

        GUI.EndScrollView();
    }

    private void GUILibraryInfoList(int index)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(60);

        GUILayout.Label("Search NameSpace:", EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(125), GUILayout.Height(25) } );

        string nameSpaceBackup = nameSpaces[index];
        nameSpaces[index] = EditorGUILayout.TextField(nameSpaces[index], new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(25) });

        GUILayout.Space(30);
        GUILayout.Label("DefineSymbol:", EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(125), GUILayout.Height(25) } );
        string defineSymbolBackup = defineSymbols[index];
        defineSymbols[index] = EditorGUILayout.TextField(defineSymbols[index], new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(25) });

        GUILayout.Space(20);
        
        Texture icon = SearchIcon(index);
        GUILayout.Box(icon, new GUILayoutOption[] { GUILayout.Width(25), GUILayout.Height(25) });

        if(nameSpaceBackup != nameSpaces[index] || defineSymbolBackup != defineSymbols[index])
        {
            NamespaceInfo info = namespaceInfoManager.Infos[index];
            info.searchNamespace = nameSpaces[index];
            info.defineSymbol = defineSymbols[index];
            SaveItem();
        }

        GUILayout.Space(30);
        GUILayout.EndHorizontal();        
    }

    private void GUIAddAndRemove()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(25) }))
        {
            NamespaceInfo info = new NamespaceInfo();
            info.searchNamespace = "NameSpace";
            info.defineSymbol = "DefineSymbol";
            namespaceInfoManager.Infos.Add(info);
            SetArrays();
            SaveItem();
        }

        if (GUILayout.Button("-", new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(25) }))
        {
            if(namespaceInfoManager.Infos.Count > 0)
            {
                NamespaceInfo removeInfo = namespaceInfoManager.Infos[namespaceInfoManager.Infos.Count - 1];
                namespaceInfoManager.Infos.Remove(removeInfo);
                SetArrays();
                SaveItem();

                NamespaceDetector.DeleteDefineSymbol(removeInfo);
            }
        }

        GUILayout.EndHorizontal();
    }

    private void GUIDetectNamespaces()
    {
        if(GUILayout.Button("Detect Namespaces"))
        {
            NamespaceDetector.DetectNamespaces(namespaceInfoManager.Infos);
            SaveItem();
            OnGUI();
        }
    }

    private void SaveItem()
    {
        EditorUtility.SetDirty(namespaceInfoManager.gameObject);
        AssetDatabase.SaveAssets();
    }

    private Texture SearchIcon(int index)
    {
        Texture icon = null;

        switch(states[index])
        {
            case NamespaceInfo.CheckStates.Unknown:
                icon = namespaceInfoManager.StateIcons[0];
                break;
            case NamespaceInfo.CheckStates.Found:
                icon = namespaceInfoManager.StateIcons[1];
                break;
            case NamespaceInfo.CheckStates.Failed:
                icon = namespaceInfoManager.StateIcons[2];
                break;
        }

        return icon;
    }
}
#endif
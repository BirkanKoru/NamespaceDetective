using System.Collections.Generic;

#if UNITY_EDITOR
// This class is responsible for detecting namespaces and synchronizing scripting define symbols
public static class NamespaceDetector
{
    // Detect namespaces and add define symbols into Player Settings
    public static void DetectNamespaces(List<NamespaceInfo> infos)
    {
        HashSet<string> existingNamespaces  = CheckNamespacesExists(infos);
        List<string> androidDefineSymbols   = GetScriptingDefineSymbols(UnityEditor.BuildTargetGroup.Android);
        List<string> iosDefineSymbols		= GetScriptingDefineSymbols(UnityEditor.BuildTargetGroup.iOS);

        for(int i=0; i < infos.Count; i++)
        {
            NamespaceInfo info = infos[i];

            // Check if we found the indentifier for the sdk in the project
            bool exist = existingNamespaces.Contains(info.searchNamespace);
            SyncScriptingDefineSymbols(exist, androidDefineSymbols, iosDefineSymbols, info);
        }

        SetScriptingDefineSymbols(UnityEditor.BuildTargetGroup.Android, androidDefineSymbols);
        SetScriptingDefineSymbols(UnityEditor.BuildTargetGroup.iOS, iosDefineSymbols);
    }

    //Delete last namespace object
    public static void DeleteDefineSymbol(NamespaceInfo info)
    {
        if(info.checkState == NamespaceInfo.CheckStates.Found)
        {
            bool removed = false;

            List<string> androidDefineSymbols   = GetScriptingDefineSymbols(UnityEditor.BuildTargetGroup.Android);
            List<string> iosDefineSymbols		= GetScriptingDefineSymbols(UnityEditor.BuildTargetGroup.iOS);

            if(androidDefineSymbols.Contains(info.defineSymbol)) 
            {
                androidDefineSymbols.Remove(info.defineSymbol);
                removed = true;
            }

            if(iosDefineSymbols.Contains(info.defineSymbol)) 
            {
                iosDefineSymbols.Remove(info.defineSymbol);
                removed = true;
            } 

            if(removed)
            {
                SetScriptingDefineSymbols(UnityEditor.BuildTargetGroup.Android, androidDefineSymbols);
                SetScriptingDefineSymbols(UnityEditor.BuildTargetGroup.iOS, iosDefineSymbols);
            }
        }
    }

    private static HashSet<string> CheckNamespacesExists(List<NamespaceInfo> infos)
    {
        HashSet<string> existingIdentifiers = new HashSet<string>();

        // Get an array of all the assembiles that are currently compiled
        System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

        for (int i = 0; i < assemblies.Length; i++)
        {
            System.Reflection.Assembly assembly = assemblies[i];

            // Get all the system types for the assembly
            System.Type[] types = assembly.GetTypes();

            for (int j = 0; j < types.Length; j++)
            {
                System.Type type = types[j];

                // Get the namespace string for the current type
                string typeNamespace = type.Namespace;

                // Check for a NamespaceInfo that has either the same Namespace or Class name
                for (int k = 0; k < infos.Count; k++)
                {
                    NamespaceInfo namespaceInfo = infos[k];

                    if(namespaceInfo.checkState != NamespaceInfo.CheckStates.Found)
                        namespaceInfo.checkState = NamespaceInfo.CheckStates.Failed;

                    if (!string.IsNullOrEmpty(namespaceInfo.searchNamespace) && namespaceInfo.searchNamespace == typeNamespace)
                    {
                        if (!existingIdentifiers.Contains(typeNamespace))
                        {
                            existingIdentifiers.Add(typeNamespace);
                        }

                        break;
                    }
                }
            }
        }

        // Namespace not found
        return existingIdentifiers;
    }

    // Gets the scripting define symbols from the Player Settings for a specific build target
    private static List<string> GetScriptingDefineSymbols(UnityEditor.BuildTargetGroup buildTargetGroup)
    {
        return new List<string>(UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';'));
    }

    // Sets the scripting define symbols in Player Settings
    private static void SetScriptingDefineSymbols(UnityEditor.BuildTargetGroup buildTargetGroup, List<string> scriptingDefineSymbols)
    {
        string symbols = "";

        for (int i = 0; i < scriptingDefineSymbols.Count; i++)
        {
            if (i != 0)
            {
                symbols += ";";
            }

            symbols += scriptingDefineSymbols[i];
        }

        UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbols);
    }


    // Syncs the scripting define symbols between Android and iOS build targets
    private static void SyncScriptingDefineSymbols(bool exists, List<string> androidDefineSymbols, List<string> iosDefineSymbols, NamespaceInfo info)
    {
        if (exists)
        {
            if (!androidDefineSymbols.Contains(info.defineSymbol))
            {
                androidDefineSymbols.Add(info.defineSymbol);
                info.checkState = NamespaceInfo.CheckStates.Found;
            }

            if (!iosDefineSymbols.Contains(info.defineSymbol))
            {
                iosDefineSymbols.Add(info.defineSymbol);
                info.checkState = NamespaceInfo.CheckStates.Found;
            }
        }
        else
        {
            androidDefineSymbols.Remove(info.defineSymbol);
            iosDefineSymbols.Remove(info.defineSymbol);
        }
    }
}
#endif
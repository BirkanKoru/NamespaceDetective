using System;

#if UNITY_EDITOR
// This class holds information about namespaces and their related states
[Serializable]
public class NamespaceInfo
{
    // The namespace to be searched
    public string searchNamespace = "NameSpace";

    // The symbol associated with the namespace, used for conditional compilation
    public string defineSymbol = "DefineSymbol";

    // The state of the namespace, whether it is found, failed or unknown
    public CheckStates checkState = CheckStates.Unknown;

    // Enum that defines the state of the namespace
    public enum CheckStates
    {
        Unknown,
        Found,
        Failed
    }
}
#endif
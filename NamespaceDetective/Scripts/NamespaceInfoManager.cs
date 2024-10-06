using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
// This class manages a list of NamespaceInfo objects and their state icons
public class NamespaceInfoManager : MonoBehaviour
{
    // A list that stores information about various namespaces
    [SerializeField] private List<NamespaceInfo> infos;
    public List<NamespaceInfo> Infos { get { return infos; } set { infos = value; }}

    // Icons representing different states of each namespace
    [SerializeField] private Texture[] stateIcons;
    public Texture[] StateIcons { get { return stateIcons; } set { stateIcons = value; }}
}
#endif
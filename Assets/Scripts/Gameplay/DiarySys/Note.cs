using UnityEngine;

[CreateAssetMenu(fileName = "New Note", menuName = "Notes System/new Note")]
public class Note : ScriptableObject
{
    [SerializeField] string label = string.Empty;
    public string Label => label;
    
    [SerializeField] Page[] pages = new Page[0];
    public Page[] Pages => pages;
}

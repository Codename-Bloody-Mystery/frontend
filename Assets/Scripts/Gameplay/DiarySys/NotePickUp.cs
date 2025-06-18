using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NotePickUp : MonoBehaviour
{
    [SerializeField] private Note note = null;

    [SerializeField] private bool autoDisplay = false;
    [SerializeField] private bool add = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (autoDisplay)
            {
                DiarySystem.Display(note);
            }

            if (add)
            {
                DiarySystem.AddNote(note.Label,note);
            }
            
            Destroy(gameObject);
        }
    }
}

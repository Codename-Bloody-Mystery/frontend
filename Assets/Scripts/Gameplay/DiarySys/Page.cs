 using UnityEngine;

 public enum PageType {Text, Texture}

 [CreateAssetMenu(fileName = "New Page", menuName = "Notes System/new Page")]
 public class Page : ScriptableObject
 {
     [SerializeField] private PageType type = PageType.Text;
     public PageType Type => type;

     [TextArea(8, 16)] 
     [SerializeField] private string text = string.Empty;
     public string Text => text;
     
     [SerializeField] Sprite texture = null;
     public Sprite Texture => texture;
     
     [SerializeField] bool useSubscript = true;
     public bool UseSubscript => useSubscript;
     
     [SerializeField] bool displayLines = true;
     public bool DisplayLines => displayLines;

     #region Audio

     [SerializeField] private AudioClip narration = null;
     public AudioClip Narration => narration;

     [SerializeField] private bool narration_PlayOnce = true;
     public bool Narration_PlayOnce => narration_PlayOnce;

     [SerializeField] private bool narrationPlayed = false;
     public bool Narration_Played { get; set; }

     #endregion
 }

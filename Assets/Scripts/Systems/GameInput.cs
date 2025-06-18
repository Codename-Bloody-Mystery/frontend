using UnityEngine;

public class GameInput
{
    private GameInput(){}
    
    public static GameInput Instance { get; private set; }
    public PlayerControls Controls { get; private set; }
    
    public static GameInput GetInstance()
    {
        if (Instance == null)
        {
            Instance = new GameInput();
            Instance.Controls = new PlayerControls();
            Instance.Controls.Enable();
        }
        return Instance;
    }
}

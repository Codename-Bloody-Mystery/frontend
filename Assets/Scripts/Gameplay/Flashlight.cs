using UnityEngine;

public interface IFlashlightState { void Enter(); void Toggle(); }

// включено
public class OnState : IFlashlightState {
    private Flashlight ctx;
    public OnState(Flashlight c) => ctx = c;
    public void Enter() { ctx.Light.enabled = true; }
    public void Toggle() => ctx.SetState(new OffState(ctx));
}

// выключено
public class OffState : IFlashlightState {
    private Flashlight ctx;
    public OffState(Flashlight c) => ctx = c;
    public void Enter() { ctx.Light.enabled = false; }
    public void Toggle() => ctx.SetState(new OnState(ctx));
}

[RequireComponent(typeof(Light))]
public class Flashlight : MonoBehaviour {
    public Light Light { get; private set; }
    private IFlashlightState state;
    public AudioSource switchSound;
    private PlayerControls controls;

    void Awake() {
        controls = new PlayerControls();
        Light = GetComponentInChildren<Light>();
        state = new OffState(this);
        state.Enter();

    }
    
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void Update() {
        if(controls.Player.Flashlight.triggered) {
            Debug.Log("Toggeled flashlight");
            state.Toggle();
            state.Enter();
            //switchSound?.Play();
        }
    }
    public void SetState(IFlashlightState newState) => state = newState;
}
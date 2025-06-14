using UnityEngine;

/// <summary>
/// Interface defining common behavior for flashlight states.
/// </summary>
public interface IFlashlightState
{
    /// <summary>
    /// Called when entering this state.
    /// </summary>
    void Enter(); 
    
    /// <summary>
    /// Called to toggle the flashlight state.
    /// </summary>
    void Toggle();
}

/// <summary>
/// State representing the flashlight turned on.
/// </summary>
public class OnState : IFlashlightState {
    private Flashlight ctx;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="OnState"/> class.
    /// </summary>
    /// <param name="c">Reference to the flashlight context.</param>
    public OnState(Flashlight c) => ctx = c;
    
    /// <summary>
    /// Enables the light when entering the On state.
    /// </summary>
    public void Enter() { ctx.Light.enabled = true; }
    
    /// <summary>
    /// Switches to the Off state when toggled.
    /// </summary>
    public void Toggle() => ctx.SetState(new OffState(ctx));
}

// выключено
public class OffState : IFlashlightState {
    private Flashlight ctx;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="OffState"/> class.
    /// </summary>
    /// <param name="c">Reference to the flashlight context.</param>
    public OffState(Flashlight c) => ctx = c;
    
    /// <summary>
    /// Disables the light when entering the Off state.
    /// </summary>
    public void Enter() { ctx.Light.enabled = false; }
    
    /// <summary>
    /// Switches to the On state when toggled.
    /// </summary>
    public void Toggle() => ctx.SetState(new OnState(ctx));
}

/// <summary>
/// Main component controlling the flashlight and handling input.
/// </summary>
[RequireComponent(typeof(Light))]
public class Flashlight : MonoBehaviour {
    /// <summary>
    /// Reference to the Light component attached to this object.
    /// </summary>
    public Light Light { get; private set; }
    
    private IFlashlightState state;
    
    /// <summary>
    /// Sound played when toggling the flashlight.
    /// </summary>
    public AudioSource switchSound;
    
    /// <summary>
    /// Defines which keys need to be pressed
    /// to trigger changing of flashlight's states.
    /// </summary>
    private PlayerControls controls;

    /// <summary>
    /// Initializes input controls and light state.
    /// </summary>
    void Awake() {
        controls = new PlayerControls();
        Light = GetComponentInChildren<Light>();
        state = new OffState(this);
        state.Enter();

    }
    
    /// <summary>
    /// Enables input when this component becomes active.
    /// </summary>
    private void OnEnable()
    {
        controls.Enable();
    }

    /// <summary>
    /// Disables input when this component becomes inactive.
    /// </summary>
    private void OnDisable()
    {
        controls.Disable();
    }

    /// <summary>
    /// Checks for toggle input and updates light state each frame.
    /// </summary>
    void Update() {
        if(controls.Player.Flashlight.triggered) {
            Debug.Log("Toggeled flashlight");
            state.Toggle();
            state.Enter();
            //switchSound?.Play();
        }
    }
    
    /// <summary>
    /// Sets the current flashlight state.
    /// </summary>
    /// <param name="newState">The new state to apply.</param>
    public void SetState(IFlashlightState newState) => state = newState;
}
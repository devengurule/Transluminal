public enum EventType
{
    // Movement
    Move,
    SprintOn,
    SprintOff,
    ZeroVelocityOn,
    ZeroVelocityOff,
    Rotate,

    // Interaction
    Interact,
    Confirm,
    Quit,

    // Collision
    PlayerCollidingEnter,
    PlayerCollidingExit,
    ShipCollidingWithScrap,
    ShipCollidingWithSalvage,

    // Pausing
    PauseOn,
    PauseOff,
    Pause,

    // Nav Nodes
    HomeNodeEnter,
    HomeNodeExit,
    NodeSelected,
    NodeDeselected,
    ArrivedAtHomeNode,
    LeftHomeNode,

    // Misc
    Restart,
    DestroyScrap,
    DestroySalvage,
    HealthChange,

    // UI
    ScrollVert,
    ScrollHori
}
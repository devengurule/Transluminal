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

    // Collision
    PlayerCollidingEnter,
    PlayerCollidingExit,
    ShipCollidingWithScrap,

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
    DestroyScrap
}
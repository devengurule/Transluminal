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
    ShipCollidingWithDebris,

    // Pausing
    PauseOn,
    PauseOff,
    Pause,
    FreezePlayerMove,
    UnFreezePlayerMove,

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
    OnEnterCloset,
    OnExitCloset,
    PlayerHiding,

    // UI
    ScrollVert,
    ScrollHori,
    TransitionOn,
    TransitionOff,
    TransitionOnFinished,
    TransitionOffFinished,

    // ALIENS
    SpawnHunter,
    SpawnRat,
    KillAlien,
    AttackPlayer,
    AddCreature,
    SpawnCreature,

    // DIALOGUE
    NoHelmAccess,
    NoShopAccess,
    FinishedDialogue
}
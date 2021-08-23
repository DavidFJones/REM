public enum InteractionType {
    Key,
    Item,
    Door,
    Pop,
    Mirror_lock,
    Drag,
    Null
}

public enum DoorState {
    OpenFront,
    Closed,
    OpenBack
}

public enum HUDMessages {
    Item,//Have we looked at an item
    InventoryFull,//Our inventory was full
    Interact,//We displayed the interact message
    LockedDoor,//We tried to open a locked door
    UnlockDoor,//We unlocked a door
    None//Last thing we did was clear the hud
}

//What state of the paused hud we are currently viewing
public enum PauseState {
    Pause,
    Options,
    Quit,
    None
}

public enum mGhostState {
    Waiting,
    Hunting,
    Cooldown,
    Idle
}
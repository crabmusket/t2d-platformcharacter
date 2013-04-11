# t2d-platformcharacter

A platformer character behavior for the [Torque 2D][] game engine.

  [Torque 2D]: https://github.com/GarageGames/Torque2D

# Installation

I recommend cloning this repository right into your T2D folder:

```
cd modules/
git clone git@github.com:eightyeight/t2d-platformcharacter PlatformCharacter
```

Or you can achieve the same effect by [downloading a ZIP file][Download].
Note that this will download the latest version of the `master` branch, which will be stable
but may be out of date.

  [Download]: https://github.com/eightyeight/t2d-platformcharacter/archive/master.zip

# Use

To spawn a platformer character, use the `create` function:

```
%player = PlatformCharacter::spawn();
%player.position = "0 2";
%scene.add(%player);
```

The `create` function takes one argument, which can be used to control the character.
The strings `"primary"` and `"secondary"` refer to keyboard inputs.
For example:

```
%playerOne = PlatformCharacter::spawn("primary");
%playerOne = PlatformCharacter::spawn("secondary");
```

This will create one character who moves with WASD and one who moves with the arrow keys.

# Customisation

There are certain default properties you can edit that will affect all subsequently-created characters.
The default values are shown below:

```
$PlatformCharacter::DefaultspeedLimit = 5; // Maximum horizontal speed due to input (units/second)
$PlatformCharacter::DefaultMoveForce = 100; // Force applied when moving.
$PlatformCharacter::DefaultJumpSpeed = 8; // Vertical speed applied at start of jump.
$PlatformCharacter::DefaultAirControl = 0.3; // Multiplier on movement forces while in air.
$PlatformCharacter::DefaultIdleDamping = 10; // Slowing factor when the character is now moving.
$PlatformCharacter::DefaultGravityScale = 2; // Multiplier on gravity (tested with gravity=10).
%p1 = PlatformCharacter::spawn("primary");
%p2 = PlatformCharacter::spawn("secondary");
```

You can also edit these properties after a character has been created:

```
%p = PlatformCharacter::spawn("primary");
%p.airControl = 0.5; // More air control!
```

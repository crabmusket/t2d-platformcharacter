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

  [Download]: https://github.com/eightyeight/t2d-platformcharacter/archive/development.zip

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

Once a character is created, you can edit certain properties to make them behave differently.
The defaults are as follows:

```
%p = PlatformCharacter::spawn("primary");
%p.speedLimit = 5; // Maximum horizontal speed due to input (units/second)
%p.moveForce = 100; // Force applied when moving.
%p.jumpSpeed = 8; // Vertical speed applied at start of jump.
%p.airControl = 0.3; // Multiplier on movement forces while in air.
%p.idleDamping = 10; // Slowing factor when the character is now moving.
```

There are also global variables that set the default values - for example:

```
$PlatformCharacter::DefaultSpeedLimit = 7; // Applied to both characters created below.
%p1 = PlatformCharacter::spawn("primary");
%p2 = PlatformCharacter::spawn("secondary");
```

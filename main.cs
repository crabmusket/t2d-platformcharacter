function PlatformCharacter::create(%this) {
   /*    Behavior template
    * The template is used to instantiate actual behaviors that control objects
    * in the simulation. It has a name because it needs to have its own namespace,
    * unfortunately. Chose a name very similar to the module name.
    */
   if(!isObject(PlatformCharacterControls)) {
      %bt = new BehaviorTemplate(PlatformCharacterControls);

      %bt.FriendlyName = "Platformer Character Controls";
      %bt.BehaviorType = "Input";
      %bt.Description = "Physically-based side-scrolling character movement.";

      %bt.addBehaviorField(leftKey, "Key to bind to left movement", keybind, "keyboard a");
      %bt.addBehaviorField(rightKey, "Key to bind to right movement", keybind, "keyboard d");
      %bt.addBehaviorField(jumpKey, "Key to bind to jumping", keybind, "keyboard w");
      %bt.addBehaviorField(controlsEnabled, "Enable direct control", bool, true);
   }

   /*    Action map
    * This module gets its own ActionMap. It doesn't have a name, instead being referred
    * to as PlatformCharacter.ActionMap. This way, exterior code can turn these keybinds
    * on and off nondestructively by pushing and popping the action map.
    */
   if(!isObject(PlatformCharacter.ActionMap)) {
      %am = new ActionMap();
      PlatformCharacter.ActionMap = %am;
      %am.push();
   }

   /*    Defaults
    * These default values are not global variables, but properties of the module object.
    * No use polluting the global namespace. These global properties allow the end-user
    * to easily configure the module if there are defaults they like better than the current
    * ones. Then they don't have to set them explicitly every time they create a new character.
    */
   PlatformCharacter.DefaultMoveForce = 100;
   PlatformCharacter.DefaultJumpSpeed = 8;
   PlatformCharacter.DefaultIdleDamping = 10;
   PlatformCharacter.DefaultAirControl = 0.2;
   PlatformCharacter.DefaultSpeedLimit = 5;
   PlatformCharacter.DefaultJumpBoostForce = 18;
   PlatformCharacter.DefaultJumpBoostTime = 0.75;
   PlatformCharacter.DefaultGravityScale = 2;
}

function PlatformCharacter::destroy(%this) {
   if(isObject(PlatformCharacter.ActionMap)) {
      PlatformCharacter.ActionMap.pop();
      PlatformCharacter.ActionMap.delete();
   }
}

/*    Spawning
 * This is a convenience function intended to make the process of creating a character
 * easier. It also provides access to the default keybind configuration.
 */
function PlatformCharacter::spawn(%input) {
   %p = new Sprite();

   %b = PlatformCharacterControls.createInstance();
   switch$(%input) {
      case "primary":
         %b.leftKey = "keyboard a";
         %b.rightKey = "keyboard d";
         %b.jumpKey = "keyboard w";
      case "secondary":
         %b.leftKey = "keyboard left";
         %b.rightKey = "keyboard right";
         %b.jumpKey = "keyboard up";
      default:
         %b.controlsEnabled = false;
   }
   %p.addBehavior(%b);

   return %p;
}

function PlatformCharacterControls::onBehaviorAdd(%this) {
   // Control
   %am = PlatformCharacter.ActionMap;
   %am.bindObj(getWord(%this.leftKey, 0), getWord(%this.leftKey, 1), "left", %this);
   %am.bindObj(getWord(%this.rightKey, 0), getWord(%this.rightKey, 1), "right", %this);
   %am.bindObj(getWord(%this.jumpKey, 0), getWord(%this.jumpKey, 1), "jump", %this);

   %p = %this.owner;

   // Collision/physics
   %sizeX = 1;
   %sizeY = 2;
   %p.setSize(%sizeX, %sizeY);
   %p.setBodyType(dynamic);
   %p.fixedAngle = true;
   %p.groundCollisionShape = %p.createPolygonBoxCollisionShape(%sizeX, %sizeY);
   %p.setGatherContacts(true);
   %p.setGravityScale(PlatformCharacter.DefaultGravityScale);

   // Movement
   %p.moveX = 0;
   %p.jumping = false;
   %p.jumpStart = 0;
   %p.setUpdateCallback(true);

   // Character properties
   %p.speedLimit = PlatformCharacter.DefaultSpeedLimit;
   %p.moveForce = PlatformCharacter.DefaultMoveForce;
   %p.jumpSpeed = PlatformCharacter.DefaultJumpSpeed;
   %p.jumpBoostForce = PlatformCharacter.DefaultJumpBoostForce;
   %p.jumpBoostTime = PlatformCharacter.DefaultJumpBoostTime;
   %p.airControl = PlatformCharacter.DefaultAirControl;
   %p.idleDamping = PlatformCharacter.DefaultIdleDamping;
}

function PlatformCharacterControls::left(%this, %val) {
   %p = %this.owner;
   %p.moveX -= %p.moveForce * (%val ? 1 : -1);
}

function PlatformCharacterControls::right(%this, %val) {
   %p = %this.owner;
   %p.moveX += %p.moveForce * (%val ? 1 : -1);
}

function PlatformCharacterControls::jump(%this, %val) {
   %p = %this.owner;
   if(%val) {
      if(%this.isTouchingGround()) {
         %p.setLinearVelocityY(%p.jumpSpeed);
         %p.jumping = true;
         %p.jumpStart = getSimTime();
      }
   } else {
      %p.jumping = false;
   }
}

function mSign(%val) { return %val >= 0 ? 1 : -1; }

function PlatformCharacterControls::onUpdate(%this) {
   %p = %this.owner;
   // Allow jumping status to time out
   if(%p.jumping) {
      if(getSimTime() - %p.jumpStart > %p.jumpBoostTime * 1000) {
         %p.jumping = false;
      }
   }
   // Update movement force
   %forceX = 0;
   %forceY = 0;
   %ground = %this.isTouchingGround();
   if(%p.moveX != 0) {
      // Apply movement force on the ground and in the air
      %forceX = %p.moveX;
      if(!%ground) {
         %forceX *= %p.airControl;
      }
      %velX = %p.getLinearVelocityX();
      if(mAbs(%velX) > %p.speedLimit && mSign(%velX) == mSign(%p.moveX)) {
         %forceX = 0;
      }
   } else {
      // Simulate ground friction
      if(%ground) {
         %forceX = -1 * %p.getLinearVelocityX() * %p.idleDamping;
      }
   }
   if(%p.jumping && !%ground) {
      %forceY = %p.jumpBoostForce;
   }
   %p.applyForce(%forceX SPC %forceY, %p.getPosition());
}

function PlatformCharacterControls::isTouchingGround(%this) {
   %p = %this.owner;
   %i = 0;
   while(%i < %p.getContactCount()) {
      if(getWord(%p.getContact(%i), 1) == %p.groundCollisionShape) {
         return true;
      }
      %i++;
   }
   return false;
}

function PlatformCharacter::create(%this) {
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

   if(!isObject(PlatformCharacter.ControlsMap)) {
      %am = new ActionMap();
      PlatformCharacter.ControlsMap = %am;
      %am.push();

      PlatformCharacter.DefaultMoveForce = 100;
      PlatformCharacter.DefaultJumpSpeed = 8;
      PlatformCharacter.DefaultIdleDamping = 10;
      PlatformCharacter.DefaultAirControl = 0.2;
      PlatformCharacter.DefaultSpeedLimit = 5;
      PlatformCharacter.DefaultJumpBoostForce = 18;
      PlatformCharacter.DefaultJumpBoostTime = 0.75;
      PlatformCharacter.DefaultGravityScale = 2;
   }
}

function PlatformCharacter::destroy(%this) {
   if(isObject(PlatformCharacter.ControlsMap)) {
      PlatformCharacter.ControlsMap.pop();
      PlatformCharacter.ControlsMap.delete();
   }
}

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
   %am = PlatformCharacter.ControlsMap;
   %am.bindObj(getWord(%this.leftKey, 0), getWord(%this.leftKey, 1), "left", %this);
   %am.bindObj(getWord(%this.rightKey, 0), getWord(%this.rightKey, 1), "right", %this);
   %am.bindObj(getWord(%this.jumpKey, 0), getWord(%this.jumpKey, 1), "jump", %this);

   %p = %this.owner;

   // Collision/physics
   %sizeX = 1;
   %sizeY = 2;
   %p.setSize(%sizeX SPC %sizeY);
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

function PlatformCharacter::create(%this) {
   if(!isObject(PlatformControls)) {
      new ActionMap(PlatformControls);
      PlatformControls.bindObj(keyboard, a, "primaryLeft", %this);
      PlatformControls.bindObj(keyboard, d, "primaryRight", %this);
      PlatformControls.bindObj(keyboard, w, "primaryJump", %this);
      PlatformControls.bindObj(keyboard, left, "secondaryLeft", %this);
      PlatformControls.bindObj(keyboard, right, "secondaryRight", %this);
      PlatformControls.bindObj(keyboard, up, "secondaryJump", %this);
      PlatformControls.push();

      PlatformCharacter.DefaultMoveForce = 100;
      PlatformCharacter.DefaultJumpSpeed = 8;
      PlatformCharacter.DefaultIdleDamping = 10;
      PlatformCharacter.DefaultAirControl = 0.2;
      PlatformCharacter.DefaultSpeedLimit = 5;
      PlatformCharacter.DefaultJumpBoostForce = 18;
      PlatformCharacter.DefaultGravityScale = 2;
   }
}

function PlatformCharacter::destroy(%this) {
   if(isObject(PlatformControls)) {
      PlatformControls.pop();
      PlatformControls.delete();
   }
}

function PlatformCharacter::spawn(%input) {
   // Appearance
   %p = new Sprite() { class = "PlatformCharacterSprite"; };
   %p.setSize("1 2");

   // Collision/physics
   %p.setBodyType(dynamic);
   %p.FixedAngle = true;
   %p.groundCollisionShape = %p.createPolygonBoxCollisionShape(1, 2);
   %p.setGatherContacts(true);
   %p.setGravityScale(PlatformCharacter.DefaultGravityScale);

   // Movement
   %p.moveX = 0;
   %p.jumping = false;
   %p.setUpdateCallback(true);

   // Character properties
   %p.speedLimit = PlatformCharacter.DefaultSpeedLimit;
   %p.moveForce = PlatformCharacter.DefaultMoveForce;
   %p.jumpSpeed = PlatformCharacter.DefaultJumpSpeed;
   %p.jumpBoostForce = PlatformCharacter.DefaultJumpBoostForce;
   %p.airControl = PlatformCharacter.DefaultAirControl;
   %p.idleDamping = PlatformCharacter.DefaultIdleDamping;

   // Control
   switch$(%input) {
      case "primary":
         PlatformCharacter.Primary = %p;
      case "secondary":
         PlatformCharacter.Secondary = %p;
   }

   return %p;
}

function PlatformCharacter::left(%this, %p, %val) {
   if(isObject(%p)) {
      %p.moveX -= %p.moveForce * (%val ? 1 : -1);
   }
}

function PlatformCharacter::primaryLeft(%this, %val) { %this.left(PlatformCharacter.Primary, %val); }
function PlatformCharacter::secondaryLeft(%this, %val) { %this.left(PlatformCharacter.Secondary, %val); }

function PlatformCharacter::right(%this, %p, %val) {
   if(isObject(%p)) {
      %p.moveX += %p.moveForce * (%val ? 1 : -1);
   }
}

function PlatformCharacter::primaryRight(%this, %val) { %this.right(PlatformCharacter.Primary, %val); }
function PlatformCharacter::secondaryRight(%this, %val) { %this.right(PlatformCharacter.Secondary, %val); }

function PlatformCharacter::jump(%this, %p, %val) {
   if(isObject(%p)) {
      if(%val) {
         if(%p.isTouchingGround()) {
            %p.setLinearVelocityY(%p.jumpSpeed);
            %p.jumping = true;
         }
      } else {
         %p.jumping = false;
      }
   }
}

function PlatformCharacter::primaryJump(%this, %val) { %this.jump(PlatformCharacter.Primary, %val); }
function PlatformCharacter::secondaryJump(%this, %val) { %this.jump(PlatformCharacter.Secondary, %val); }

function mSign(%val) { return %val >= 0 ? 1 : -1; }

function PlatformCharacterSprite::onUpdate(%this) {
   // Update movement force
   %forceX = 0;
   %forceY = 0;
   %ground = %this.isTouchingGround();
   if(%this.moveX != 0) {
      %forceX = %this.moveX;
      if(!%ground) {
         %forceX *= %this.airControl;
      }
      %velX = %this.getLinearVelocityX();
      if(mAbs(%velX) > %this.speedLimit && mSign(%velX) == mSign(%this.moveX)) {
         %forceX = 0;
      }
   } else {
      if(%ground) {
         %forceX = -1 * %this.getLinearVelocityX() * %this.idleDamping;
      }
   }
   if(%this.jumping && !%ground) {
      %forceY = %this.jumpBoostForce;
   }
   %this.applyForce(%forceX SPC %forceY, %this.getPosition());
}

function PlatformCharacterSprite::isTouchingGround(%this) {
   %i = 0;
   while(%i < %this.getContactCount()) {
      if(getWord(%this.getContact(%i), 1) == %this.groundCollisionShape) {
         return true;
      }
      %i++;
   }
   return false;
}

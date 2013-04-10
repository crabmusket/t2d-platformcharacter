function PlatformCharacter::create(%this) {
   if(!isObject(PlatformControls)) {
      new ActionMap(PlatformControls);
      PlatformControls.bindObj(keyboard, a, "left", %this);
      PlatformControls.bindObj(keyboard, d, "right", %this);
      PlatformControls.bindObj(keyboard, w, "jump", %this);
      PlatformControls.push();
   }
}

function PlatformCharacter::destroy(%this) {
   if(isObject(PlatformControls)) {
      PlatformControls.pop();
      PlatformControls.delete();
   }
}

$PlatformCharacter::DefaultMoveSpeed = 50;
$PlatformCharacter::DefaultJumpSpeed = 5;
$PlatformCharacter::DefaultFriction = 1.1;
$PlatformCharacter::DefaultAirControl = 0.3;

function PlatformCharacter::spawn(%input) {
   // Appearance
   %p = new Sprite() { class = "PlatformCharacterSprite"; };
   %p.setSize("1 2");

   // Collision/physics
   %p.setBodyType(dynamic);
   %p.FixedAngle = true;
   %p.setDefaultFriction($PlatformCharacter::DefaultFriction);
   %p.groundCollisionShape = %p.createPolygonBoxCollisionShape(1, 2);
   %p.setGatherContacts(true);

   // Movement
   %p.moveX = 0;
   %p.setUpdateCallback(true);

   // Character properties
   %p.moveSpeed = $PlatformCharacter::DefaultMoveSpeed;
   %p.jumpSpeed = $PlatformCharacter::DefaultJumpSpeed;

   // Character properties
   %p.moveSpeed = $PlatformCharacter::DefaultMoveSpeed;
   %p.jumpSpeed = $PlatformCharacter::DefaultJumpSpeed;
   %p.airControl = $PlatformCharacter::DefaultAirControl;

   // Control
   switch$(%input) {
      case "primary":
         PlatformCharacter.Primary = %p;
      case "secondary":
         PlatformCharacter.Secondary = %p;
   }

   return %p;
}

function PlatformCharacter::left(%this, %val) {
   if(isObject(PlatformCharacter.Primary)) {
      %p = PlatformCharacter.Primary;
      %p.moveX -= %p.moveSpeed * (%val ? 1 : -1);
   }
}

function PlatformCharacter::right(%this, %val) {
   if(isObject(PlatformCharacter.Primary)) {
      %p = PlatformCharacter.Primary;
      %p.moveX += %p.moveSpeed * (%val ? 1 : -1);
   }
}

function PlatformCharacter::jump(%this, %val) {
   if(%val && isObject(PlatformCharacter.Primary)) {
      %p = PlatformCharacter.Primary;
      %p.setLinearVelocityY(%p.jumpSpeed);
   }
}

function PlatformCharacterSprite::onUpdate(%this) {
   // Update movement force
   if(%this.moveX != 0) {
      %velX = %this.getLinearVelocityX();
      %force = %this.moveX / (mAbs(%velX) + 1);
      if(!%this.isTouchingGround()) {
         echo("not on ground" SPC %this.airControl);
         %force *= %this.airControl;
      }
      %this.applyForce(%force SPC 0, %this.getPosition());
   }
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

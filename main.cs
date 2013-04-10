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

$PlatformCharacter::DefaultMoveSpeed = 3;
$PlatformCharacter::DefaultJumpSpeed = 5;

function PlatformCharacter::spawn(%input) {
   // Appearance
   %p = new Sprite() { class = "PlatformCharacterSprite"; };
   %p.setSize("1 2");

   // Collision/physics
   %p.setBodyType(dynamic);
   %p.FixedAngle = true;
   %p.setDefaultFriction(0);
   %p.createPolygonBoxCollisionShape(1, 2);

   // Character properties
   %p.moveSpeed = $PlatformCharacter::DefaultMoveSpeed;
   %p.jumpSpeed = $PlatformCharacter::DefaultJumpSpeed;

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
      %p.updateMovement();
   }
}

function PlatformCharacter::right(%this, %val) {
   if(isObject(PlatformCharacter.Primary)) {
      %p = PlatformCharacter.Primary;
      %p.moveX += %p.moveSpeed * (%val ? 1 : -1);
      %p.updateMovement();
   }
}

function PlatformCharacter::jump(%this, %val) {
   if(%val && isObject(PlatformCharacter.Primary)) {
      %p = PlatformCharacter.Primary;
      %p.setLinearVelocityY(%p.jumpSpeed);
   }
}

function PlatformCharacterSprite::updateMovement(%this) {
   %this.setLinearVelocityX(%this.moveX);
}

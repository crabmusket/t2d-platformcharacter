function PlatformCharacter::create(%this) {
   if(!isObject(PlatformControls)) {
      new ActionMap(PlatformControls);
      PlatformControls.bindObj(keyboard, a, "left", %this);
      PlatformControls.bindObj(keyboard, d, "right", %this);
      PlatformControls.push();
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
   %p.setDefaultFriction(0);
   %p.createPolygonBoxCollisionShape(1, 2);

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
      PlatformCharacter.Primary.moveX -= %val ? 1 : -1;
      PlatformCharacter.Primary.updateMovement();
   }
}

function PlatformCharacter::right(%this, %val) {
   if(isObject(PlatformCharacter.Primary)) {
      PlatformCharacter.Primary.moveX += %val ? 1 : -1;
      PlatformCharacter.Primary.updateMovement();
   }
}

function PlatformCharacterSprite::updateMovement(%this) {
   %this.setLinearVelocityX(%this.moveX);
}

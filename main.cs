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

function PlatformCharacter::left(%this, %p, %val) {
   if(isObject(%p)) {
      %p.moveX -= %p.moveSpeed * (%val ? 1 : -1);
   }
}

function PlatformCharacter::primaryLeft(%this, %val) { %this.left(PlatformCharacter.Primary, %val); }
function PlatformCharacter::secondaryLeft(%this, %val) { %this.left(PlatformCharacter.Secondary, %val); }

function PlatformCharacter::right(%this, %p, %val) {
   if(isObject(%p)) {
      %p.moveX += %p.moveSpeed * (%val ? 1 : -1);
   }
}

function PlatformCharacter::primaryRight(%this, %val) { %this.right(PlatformCharacter.Primary, %val); }
function PlatformCharacter::secondaryRight(%this, %val) { %this.right(PlatformCharacter.Secondary, %val); }

function PlatformCharacter::jump(%this, %p, %val) {
   if(%val && isObject(%p)) {
      %p.setLinearVelocityY(%p.jumpSpeed);
   }
}

function PlatformCharacter::primaryJump(%this, %val) { %this.jump(PlatformCharacter.Primary, %val); }
function PlatformCharacter::secondaryJump(%this, %val) { %this.jump(PlatformCharacter.Secondary, %val); }

function PlatformCharacterSprite::onUpdate(%this) {
   // Update movement force
   if(%this.moveX != 0) {
      %velX = %this.getLinearVelocityX();
      %force = %this.moveX / (mAbs(%velX) + 1);
      if(!%this.isTouchingGround()) {
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

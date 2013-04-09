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

function PlatformCharacter::left(%this, %val) {
   echo("left" SPC %val);
}

function PlatformCharacter::right(%this, %val) {
   echo("right" SPC %val);
}

function PlatformCharacter::spawn(%input) {
   %p = new Sprite();
   %p.setSize("1 2");
   return %p;
}


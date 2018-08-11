//treeGun.cs

datablock ProjectileData(treeGunProjectile)
{
    projectileShapeName = "./bullet.dts";
    directDamage        = 1;
    directDamageType    = $DamageType::Gun;
    radiusDamageType    = $DamageType::Gun;

    brickExplosionRadius = 0;
    brickExplosionImpact = true;          //destroy a brick if we hit it directly?
    brickExplosionForce  = 10;
    brickExplosionMaxVolume = 1;          //max volume of bricks that we can destroy
    brickExplosionMaxVolumeFloating = 2;  //max volume of bricks that we can destroy if they aren't connected to the ground

    impactImpulse	     = 400;
    verticalImpulse	  = 400;
    explosion           = gunExplosion;
    particleEmitter     = ""; //bulletTrailEmitter;

    muzzleVelocity      = 200;
    velInheritFactor    = 1;

    armingDelay         = 00;
    lifetime            = 60000;
    fadeDelay           = 3500;
    bounceElasticity    = 0.5;
    bounceFriction      = 0.20;
    isBallistic         = true;
    gravityMod = 1.0;

    hasLight    = false;
    lightRadius = 3.0;
    lightColor  = "0 0 0.5";

    uiName = "Tree Gun Seed";
};

//////////
// item //
//////////
datablock ItemData(treeGunItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./pistol.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "Tree Gun";
	iconName = "./icon_gun";
	doColorShift = true;
	colorShiftColor = "0.25 1.0 0.25 1.000";

	 // Dynamic properties defined by the scripts
	image = treeGunImage;
	canDrop = true;
};

////////////////
//weapon image//
////////////////
datablock ShapeBaseImageData(treeGunImage)
{
   // Basic Item properties
   shapeFile = "./pistol.dts";
   emap = true;

   // Specify mount point & offset for 3rd person, and eye offset
   // for first person rendering.
   mountPoint = 0;
   offset = "0 0 0";
   eyeOffset = 0; //"0.7 1.2 -0.5";
   rotation = eulerToMatrix( "0 0 0" );

   // When firing from a point offset from the eye, muzzle correction
   // will adjust the muzzle vector to point to the eye LOS point.
   // Since this weapon doesn't actually fire from the muzzle point,
   // we need to turn this off.  
   correctMuzzleVector = true;

   // Add the WeaponImage namespace as a parent, WeaponImage namespace
   // provides some hooks into the inventory system.
   className = "WeaponImage";

   // Projectile && Ammo.
   item = BowItem;
   ammo = " ";
   projectile = treeGunProjectile;
   projectileType = Projectile;

	casing = gunShellDebris;
	shellExitDir        = "1.0 -1.3 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;	
	shellVelocity       = 7.0;

   //melee particles shoot from eye node for consistancy
   melee = false;
   //raise your arm up or not
   armReady = true;

   doColorShift = true;
   colorShiftColor = treeGunItem.colorShiftColor;//"0.400 0.196 0 1.000";

   //casing = " ";

   // Images have a state system which controls how the animations
   // are run, which sounds are played, script callbacks, etc. This
   // state system is downloaded to the client so that clients can
   // predict state changes and animate accordingly.  The following
   // system supports basic ready->fire->reload transitions as
   // well as a no-ammo->dryfire idle state.

   // Initial start up state
	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.15;
	stateTransitionOnTimeout[0]       = "Ready";
	stateSound[0]					= weaponSwitchSound;

	stateName[1]                     = "Ready";
	stateTransitionOnTriggerDown[1]  = "Fire";
	stateAllowImageChange[1]         = true;
	stateSequence[1]	= "Ready";

	stateName[2]                    = "Fire";
	stateTransitionOnTimeout[2]     = "Smoke";
	stateTimeoutValue[2]            = 0.01;
	stateFire[2]                    = true;
	stateAllowImageChange[2]        = false;
	stateSequence[2]                = "Fire";
	stateScript[2]                  = "onFire";
	stateWaitForTimeout[2]			= true;
	stateEmitter[2]					= gunFlashEmitter;
	stateEmitterTime[2]				= 0.05;
	stateEmitterNode[2]				= "muzzleNode";
	stateSound[2]					= gunShot1Sound;
	stateEjectShell[2]       = true;

	stateName[3] = "Smoke";
	stateEmitter[3]					= gunSmokeEmitter;
	stateEmitterTime[3]				= 0.05;
	stateEmitterNode[3]				= "muzzleNode";
	stateTimeoutValue[3]            = 0.01;
	stateTransitionOnTimeout[3]     = "Reload";

	stateName[4]			= "Reload";
	stateSequence[4]                = "Reload";
	stateTransitionOnTriggerUp[4]     = "Ready";
	stateSequence[4]	= "Ready";

};

function treeGunImage::onFire(%this,%obj,%slot)
{
	if(%obj.getDamagePercent() < 1.0)
		%obj.playThread(2, shiftAway);
	Parent::onFire(%this,%obj,%slot);	
}

package ServerRPG
{
    function treeGunProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity)
    {
        if(%col.getClassName() $= "fxDTSBrick")
        {
            //Get client
            %client = %obj.client;

            if(%client.isAdmin)
            {
                //Tree details
                %trunkColorID = getRandom(59, 61);
                %leafColorID = $RPG::treeColor[getRandom(0,10)];
                %pinkColorID = 10;
                %angleID = getRandom(0,3);
                %isDeadTreeChance = getRandom(0,100);
                %isDead = false;
                %trunkDatablock = $RPG::treeTrunk[getRandom(0,2)];
                %leafID = getRandom(0,7);
                %leafDatablock = $RPG::treeLeafs[%leafID];

                %pinkHeight = 1;
                switch(%leafID)
                {
                    case 0:
                        %pinkHeight = 5;
                    case 1:
                        %pinkHeight = 5;
                    case 2:
                        %pinkHeight = 5;
                    case 3:
                        %pinkHeight = 5;
                    case 4:
                        %pinkHeight = 7;
                    case 5:
                        %pinkHeight = 5;
                    case 6:
                        %pinkHeight = 6;
                    case 7:
                        %pinkHeight = 6;
                }

                //Is it a dead tree?
                if(%isDeadTreeChance > 80)
                    %isDead = true;

                %validModTer = false;
                %isRamp = true;
                for(%i = 0; %i < 9 && !%validModTer; %i++)
                {
                    if(%col.getDatablock().getName() $= $RPG::validModTer[%i])
                    {
                        %validModTer = true;
                        if(%i == 0 || %i == 1 || %i == 8)
                            %isRamp = false;
                    }
                }

                if(%validModTer)
                {
                    //Check tree overlap
                    %adjustPos = "0 0 " @ %trunkDatablock.brickSizeZ * 0.1;
                    if(%isRamp)
                    {
                        %adjustPos = "0 0 " @ %trunkDatablock.brickSizeZ * 0.1 - 1;
                        %px = getWord(%pos, 0);
                        %py = getWord(%pos, 1);
                        %pz = getWord(%pos, 2);

                        %pz = mFloor(%pz*10);
                        if(%pz & 1)
                            %pz-=1;
                        %pz/=10;
                        %pos = %px SPC %py SPC %pz;
                    }
                    
                    %newPos = vectorAdd(%pos, %adjustPos);

                    %newBrick = createBrick(%client, %trunkDatablock, %newPos, %trunkColorID, %angleID);
                    %err = getField(%newBrick, 1);
                    %brick = getField(%newBrick, 0);
                    if(%err == 0 || %err == 2)
                    {
                        %brickList = %brick;
                        %newPos = vectorAdd(%newPos, "0 0 " @ %trunkDatablock.brickSizeZ * 0.1);
                        %newBrick = createBrick(%client, %leafDatablock, %newPos, %leafColorID, %angleID);
                        %err = getField(%newBrick, 1);
                        %brick = getField(%newBrick, 0);
                        if(%err == 0 || %err == 2)
                            %brickList = %brickList SPC %brick;
                        else
                            %brick.delete();

                        %newPos = vectorAdd(%newPos, "0 0 " @ -0.8);
                        for(%i = 0; %i < %pinkHeight; %i++)
                        {
                            %newPos = vectorAdd(%newPos, "0 0 " @ brick4xCubeData.brickSizeZ * 0.2);
                            %newBrick = createBrick(%client, "brick4xCubeData", %newPos, 10, 0);
                            %err = getField(%newBrick, 1);
                            %brick = getField(%newBrick, 0);
                            %brick.setRendering(0);

                            if(%err == 0 || %err == 2)
                                %brickList = %brickList SPC %brick;
                            else
                                %brick.delete();
                        }

                        if(isObject(%brick))
                            %client.undoStack.push(0 TAB "GROUP_PLANT" TAB %brickList);
                    }
                    else
                        %brick.delete();                    
                }
            }  
        }
        return parent::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity);
    }
};

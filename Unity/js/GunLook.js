#pragma strict
/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/

///This is similar to mouseLook, but is only for visual sway on weapons

	private var sensitivityX : float = 15F;
	private var sensitivityY : float = 15F;
	var sensitivityStandardX : float = 15F;
	var sensitivityStandardZ : float = 15F;
	var sensitivityStandardY : float = 15F;
	var sensitivityAimingX : float = 15F;
	var sensitivityAimingZ : float = 15F;
	var sensitivityAimingY : float = 15F;
	var retSensitivity : float = -.5F;

	@HideInInspector
	var minimumX : float = 5F;
	@HideInInspector
	var maximumX : float = 3F;
	
	var xRange : float = 5F;
	var xRangeAim : float = 3F;
	
	var zRange : float = 5F;
	var zRangeAim : float = 3F;
	
	private var actualZRange : float;
	private var sensitivityZ : float;

	var yRange : float = 5F;
	var yRangeAim : float = 3F;
	
	var zMoveRange : float = 10;
	var zMoveSensitivity : float = .5f;
	var zMoveAdjustSpeed : float = 4;
	
	var xMoveRange : float = 10;
	var xMoveSensitivity : float = .5f;
	var xMoveAdjustSpeed : float = 4;
	
	var xAirMoveRange : float = 10;
	var xAirMoveSensitivity : float = .5f;
	var xAirAdjustSpeed : float = 4;
	
	var zPosMoveRange : float = .13;
	var zPosMoveSensitivity : float = .5f;
	var zPosAdjustSpeed : float = 4;
	
	var xPosMoveRange : float = .13;
	var xPosMoveSensitivity : float = .5f;
	var xPosAdjustSpeed : float = 4;

	private var minimumY : float = -60F;
	private var maximumY : float = 60F;
	
	//added by dw to pause camera when in store
	@HideInInspector
	var freeze : boolean = false;
	@HideInInspector
	var rotationX : float = 0F;
	@HideInInspector
	var rotationY : float = 0F;
	var rotationZ : float = 0F;
	
	private var startPos : Vector3;
	private var lastOffset : Vector3;
	private var posOffset : Vector3;
	
	private var curZ : float;
	private var curX : float;
	private var curX2 : float;
	private var lastZ : float;
	private var lastX : float;
	private var tX : float;
	
	var useLookMotion : boolean = true;
	var useWalkMotion : boolean = true;
	
	var lookMotionOpen : boolean = false;
	var walkMotionOpen : boolean = false;
	
	private var originalRotation : Quaternion;
	
	static var jostleAmt : Vector3;
	var curJostle : Vector3;
	var lastJostle : Vector3;
	
	private var targetPosition : Vector3;
	private var curTarget : Vector3;
	private var lastTarget : Vector3;
	
	function Freeze() {
		freeze = true;
	}
	
	function UnFreeze() {
		freeze = false;
	}
	
	function Update () {
		if(freeze || !PlayerWeapons.playerActive) return;
		
		if(retSensitivity > 0)
			retSensitivity*=-1;
	
		if(useLookMotion && PlayerWeapons.canLook) {
			var xQuaternion : Quaternion;
			var yQuaternion : Quaternion;
		
			// Read the mouse input axis
			rotationX += InputDB.GetAxis("Mouse X") * sensitivityX;
			rotationY += InputDB.GetAxis("Mouse Y") * sensitivityY;
			rotationZ += InputDB.GetAxis("Mouse X") * sensitivityZ;
	
			rotationX = ClampAngle (rotationX, minimumX, maximumX);
			rotationY = ClampAngle (rotationY, minimumY, maximumY);
			rotationZ = ClampAngle (rotationZ, -actualZRange,  actualZRange);
			if(Mathf.Abs(Input.GetAxis("Mouse X")) <.05){
				if(sensitivityX > 0){
					rotationX -= rotationX*Time.deltaTime*retSensitivity*7;
					rotationZ -= rotationZ*Time.deltaTime*retSensitivity*7;
					rotationY += rotationY*Time.deltaTime*retSensitivity*7;
				} else {
					rotationX += rotationX*Time.deltaTime*retSensitivity*7;
					rotationZ += rotationZ*Time.deltaTime*retSensitivity*7;
					rotationY += rotationY*Time.deltaTime*retSensitivity*7;
				}
			}
				
			xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
			var zQuaternion : Quaternion = Quaternion.AngleAxis (rotationZ, Vector3.forward);
			yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);
				
			transform.localRotation = Quaternion.Lerp(transform.localRotation ,originalRotation * xQuaternion * yQuaternion *zQuaternion, Time.deltaTime*10);
		}
		
		if(useWalkMotion){
			//Velocity-based changes
			var relVelocity : Vector3 = transform.InverseTransformDirection(PlayerWeapons.CM.movement.velocity);			
			var zVal : float;
			var xVal : float;
			var xVal2 : float;
			
			lastOffset = posOffset;
			
			var s : float = Vector3(PlayerWeapons.CM.movement.velocity.x, 0, PlayerWeapons.CM.movement.velocity.z).magnitude/14;
	
			
			if(!AimMode.staticAiming){				
				var xPos : float = Mathf.Clamp(relVelocity.x*xPosMoveSensitivity, -xPosMoveRange*s, xPosMoveRange*s);
				posOffset.x = Mathf.Lerp(posOffset.x, xPos, Time.deltaTime*xPosAdjustSpeed);// + startPos.x;
				
				var zPos : float = Mathf.Clamp(relVelocity.z*zPosMoveSensitivity, -zPosMoveRange*s, zPosMoveRange*s);
				posOffset.z = Mathf.Lerp(posOffset.z, zPos, Time.deltaTime*zPosAdjustSpeed);// + startPos.z;
				
			} else {
				posOffset.x = Mathf.Lerp(posOffset.x, 0, Time.deltaTime*xPosAdjustSpeed*3);// + startPos.x;
				posOffset.z = Mathf.Lerp(posOffset.z, 0, Time.deltaTime*zPosAdjustSpeed*3);// + startPos.z;
							
			}
			
			//Apply Jostle
			lastJostle = curJostle;
			curJostle = Vector3.Lerp(curJostle, jostleAmt, Time.deltaTime*10);
			jostleAmt = Vector3.Lerp(jostleAmt, Vector3.zero, Time.deltaTime*3);
						
			lastTarget = curTarget;
			curTarget = Vector3.Lerp(curTarget, posOffset, Time.deltaTime*8);
			
			transform.localPosition += curTarget - lastTarget;
			transform.localPosition += curJostle - lastJostle;
		}
	}
	
	function Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
		originalRotation = transform.localRotation;
		startPos = transform.localPosition;
		StopAiming();
	}
	
	static function ClampAngle (angle : float, min : float, max : float) : float
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}
	
	function Aiming() {
		sensitivityX = sensitivityAimingX;
		sensitivityY = sensitivityAimingY;
		
		minimumX = -xRangeAim;
		maximumX = xRangeAim;
		minimumY = -yRangeAim;
		maximumY = yRangeAim;
		sensitivityZ = sensitivityAimingZ;
		actualZRange = zRangeAim;
	}
	function StopAiming() {
		sensitivityX = sensitivityStandardX;
		sensitivityY = sensitivityStandardY;
		
		minimumX = -xRange;
		maximumX = xRange;
		minimumY = -yRange;
		maximumY = yRange;
		sensitivityZ = sensitivityStandardZ;
		actualZRange = zRange;

	}


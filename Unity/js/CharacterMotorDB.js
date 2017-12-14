//#pragma strict
//#pragma implicit
//#pragma downcast

// Does this script currently respond to input?
var canControl : boolean = true;

static var paused : boolean = false;

var useFixedUpdate : boolean = true;

var crouchDeltaHeight : float = 1.0;
var proneDeltaHeight : float = 1.5;
var useProne : boolean = true;
private var weaponCamera : GameObject;
private var standardCamHeight : float;
private var proneCamHeight : float;
private var crouchingCamHeight : float;
static var crouching : boolean = false;
static var prone : boolean = false;
static var walking : boolean = false;
static var sprinting : boolean = false;
@HideInInspector
var stopCrouching : boolean = false;
@HideInInspector
var stopProne : boolean = false;
var velocityFactor : float = 2;
private var canSprint : boolean = true;
static var maxSpeed : float;
private var diving : boolean = false;
private var standardHeight : float;
private var standardCenter : float;
var camSpeed : float = 8;
private var lastCamSpeed : float = 8;
var jumpSound : AudioClip;
var jumpSoundVolume : float = .2;
var landSound : AudioClip;
var landSoundVolume : float = .13;
var proneLandSound : AudioClip;
var proneLandSoundVolume : float = .8;

var hitProne : boolean = false;
var proneFrame : boolean = false;


var aim : boolean = false;

// For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
// Very handy for organization!

// The current global direction we want the character to move in.
@System.NonSerialized
var inputMoveDirection : Vector3 = Vector3.zero;

// Is the jump button held down? We use this interface instead of checking
// for the jump button directly so this script can also be used by AIs.
@System.NonSerialized
var inputJump : boolean = false;

class CharacterMotorDBMovement {
	// The maximum horizontal speed when moving
	@HideInInspector
	var maxForwardSpeed : float = 10.0;
	@HideInInspector
	var defaultForwardSpeed : float = 10;
	@HideInInspector
	var maxCrouchSpeed : float = 6;
	@HideInInspector
	var maxSprintSpeed : float = 13;
	@HideInInspector
	var minSprintSpeed : float = 10;
	@HideInInspector
	var maxAimSpeed : float = 4;
	@HideInInspector
	var maxProneSpeed : float = 4;
	
	@HideInInspector
	var maxSidewaysSpeed : float = 10.0;
	@HideInInspector
	var defaultSidewaysSpeed : float = 10;
	@HideInInspector
	var sprintSidewaysSpeed : float = 15;
	@HideInInspector
	var crouchSidewaysSpeed : float = 6;
	@HideInInspector
	var aimSidewaysSpeed : float = 4;
	@HideInInspector
	var proneSidewaysSpeed : float = 2;
	
	@HideInInspector
	var maxBackwardsSpeed : float = 10.0;
	@HideInInspector
	var defaultBackwardsSpeed : float = 10;
	@HideInInspector
	var crouchBackwardsSpeed : float = 6;
	@HideInInspector
	var aimBackwardsSpeed : float = 4;
	@HideInInspector
	var proneBackwardsSpeed : float = 2;
		
	// Curve for multiplying speed based on slope (negative = downwards)
	var slopeSpeedMultiplier : AnimationCurve = AnimationCurve(Keyframe(-90, 1), Keyframe(0, 1), Keyframe(90, 0));
	
	// How fast does the character change speeds?  Higher is faster.
	var maxGroundAcceleration : float = 30.0;
	var defaultGroundAcceleration : float = 30;
	var sprintGroundAcceleration : float = 50;
	var maxAirAcceleration : float = 20.0;
	var defaultAirAcceleration : float = 20;
	var sprintAirAcceleration : float = 25;
	var useDive : boolean = false;
	
	var crouchedTime : float;
	var proneHoldTime : float = .3;


	// The gravity for the character
	var gravity : float = 10.0;
	var maxFallSpeed : float = 20.0;
	var fallDamageStart : float = 9;
	var fallDamageEnd : float = 50;
	var fallDamageMax : float = 100;
	
	// For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
	// Very handy for organization!

	// The last collision flags returned from controller.Move
	@System.NonSerialized
	var collisionFlags : CollisionFlags; 

	// We will keep track of the character's current velocity,
	@System.NonSerialized
	var velocity : Vector3;
	
	// This keeps track of our current velocity while we're not grounded
	@System.NonSerialized
	var frameVelocity : Vector3 = Vector3.zero;
	
	@System.NonSerialized
	var hitPoint : Vector3 = Vector3.zero;
	
	@System.NonSerialized
	var lastHitPoint : Vector3 = Vector3(Mathf.Infinity, 0, 0);
}

var movement : CharacterMotorDBMovement = CharacterMotorDBMovement();

enum MovementTransferOnJumpDB {
	None, // The jump is not affected by velocity of floor at all.
	InitTransfer, // Jump gets its initial velocity from the floor, then gradualy comes to a stop.
	PermaTransfer, // Jump gets its initial velocity from the floor, and keeps that velocity until landing.
	PermaLocked // Jump is relative to the movement of the last touched floor and will move together with that floor.
}

// We will contain all the jumping related variables in one helper class for clarity.
class CharacterMotorDBJumping {
	// Can the character jump?
	var enabled : boolean = true;

	// How high do we jump when pressing jump and letting go immediately
	var baseHeight : float = 1.0;
	
	// We add extraHeight units (meters) on top when holding the button down longer while jumping
	var extraHeight : float = 4.1;
	
	// How much does the character jump out perpendicular to the surface on walkable surfaces?
	// 0 means a fully vertical jump and 1 means fully perpendicular.
	var perpAmount : float = 0.0;
	
	// How much does the character jump out perpendicular to the surface on too steep surfaces?
	// 0 means a fully vertical jump and 1 means fully perpendicular.
	var steepPerpAmount : float = 0.5;
	
	// For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
	// Very handy for organization!

	// Are we jumping? (Initiated with jump button and not grounded yet)
	// To see if we are just in the air (initiated by jumping OR falling) see the grounded variable.
	@System.NonSerialized
	var jumping : boolean = false;
	
	@System.NonSerialized
	var holdingJumpButton : boolean = false;

	// the time we jumped at (Used to determine for how long to apply extra jump power after jumping.)
	@System.NonSerialized
	var lastStartTime : float = 0.0;
	
	@System.NonSerialized
	var lastButtonDownTime : float = -100;
	
	@System.NonSerialized
	var jumpDir : Vector3 = Vector3.up;
}

var jumping : CharacterMotorDBJumping = CharacterMotorDBJumping();

class CharacterMotorDBMovingPlatform {
	var enabled : boolean = true;
	
	var movementTransfer : MovementTransferOnJumpDB = MovementTransferOnJumpDB.PermaTransfer;
	
	@System.NonSerialized
	var hitPlatform : Transform;
	
	@System.NonSerialized
	var activePlatform : Transform;
	
	@System.NonSerialized
	var activeLocalPoint : Vector3;
	
	@System.NonSerialized
	var activeGlobalPoint : Vector3;
	
	@System.NonSerialized
	var activeLocalRotation : Quaternion;
	
	@System.NonSerialized
	var activeGlobalRotation : Quaternion;
	
	@System.NonSerialized
	var lastMatrix : Matrix4x4;
	
	@System.NonSerialized
	var platformVelocity : Vector3;
	
	@System.NonSerialized
	var newPlatform : boolean;
}

var movingPlatform : CharacterMotorDBMovingPlatform = CharacterMotorDBMovingPlatform();

class CharacterMotorDBSliding {
	// Does the character slide on too steep surfaces?
	var enabled : boolean = true;
	
	// How fast does the character slide on steep surfaces?
	var slidingSpeed : float = 15;
	
	// How much can the player control the sliding direction?
	// If the value is 0.5 the player can slide sideways with half the speed of the downwards sliding speed.
	var sidewaysControl : float = 1.0;
	
	// How much can the player influence the sliding speed?
	// If the value is 0.5 the player can speed the sliding up to 150% or slow it down to 50%.
	var speedControl : float = 0.4;
}

var sliding : CharacterMotorDBSliding = CharacterMotorDBSliding();

@System.NonSerialized
var grounded : boolean = true;

@System.NonSerialized
var groundNormal : Vector3 = Vector3.zero;

private var lastGroundNormal : Vector3 = Vector3.zero;

private var tr : Transform;

private var controller : CharacterController;

function Awake () {
	var values : MovementValues = this.GetComponent(MovementValues);
	if(values != null){
		movement.defaultForwardSpeed = values.defaultForwardSpeed;
		movement.maxCrouchSpeed = values.maxCrouchSpeed;
		movement.maxSprintSpeed = values.maxSprintSpeed;
		movement.minSprintSpeed = values.minSprintSpeed;
		movement.maxAimSpeed = values.maxAimSpeed;
		movement.maxProneSpeed = values.maxProneSpeed;
		
		movement.defaultSidewaysSpeed = values.defaultSidewaysSpeed;
		movement.sprintSidewaysSpeed = values.sprintSidewaysSpeed;
		movement.crouchSidewaysSpeed = values.crouchSidewaysSpeed;
		movement.aimSidewaysSpeed = values.aimSidewaysSpeed;
		movement.proneSidewaysSpeed = values.proneSidewaysSpeed;
		
		movement.defaultBackwardsSpeed = values.defaultBackwardsSpeed;
		movement.crouchBackwardsSpeed = values.crouchBackwardsSpeed;
		movement.aimBackwardsSpeed = values.aimBackwardsSpeed;
		movement.proneBackwardsSpeed = values.proneBackwardsSpeed;
	}
	
	controller = GetComponent (CharacterController);
	standardHeight = controller.height;
	standardCenter = controller.center.y;
	tr = transform;
	weaponCamera = gameObject.FindWithTag("WeaponCamera");
	crouching = false;
	prone = false;
	standardCamHeight = weaponCamera.transform.localPosition.y;
	crouchingCamHeight = standardCamHeight - crouchDeltaHeight;
	proneCamHeight = standardCamHeight - proneDeltaHeight;
	NormalSpeed();
}

private function UpdateFunction () {
	if(paused)
		return;
	if(diving){
		var yVeloc : float = movement.velocity.y;
		SetVelocity(transform.forward*20 + Vector3.up*yVeloc);
	}
	
	// We copy the actual velocity into a temporary variable that we can manipulate.
	var velocity : Vector3 = movement.velocity;
	
	// Update velocity based on input
	velocity = ApplyInputVelocityChange(velocity);
	
	// Apply gravity and jumping force
	velocity = ApplyGravityAndJumping (velocity);
	
	// Moving platform support
	var moveDistance : Vector3 = Vector3.zero;
	if (MoveWithPlatform()) {
		var newGlobalPoint : Vector3 = movingPlatform.activePlatform.TransformPoint(movingPlatform.activeLocalPoint);
		moveDistance = (newGlobalPoint - movingPlatform.activeGlobalPoint);
		if (moveDistance != Vector3.zero)
			controller.Move(moveDistance);
		
		// Support moving platform rotation as well:
        var newGlobalRotation : Quaternion = movingPlatform.activePlatform.rotation * movingPlatform.activeLocalRotation;
        var rotationDiff : Quaternion = newGlobalRotation * Quaternion.Inverse(movingPlatform.activeGlobalRotation);
        
        var yRotation = rotationDiff.eulerAngles.y;
        if (yRotation != 0) {
	        // Prevent rotation of the local up vector
	        tr.Rotate(0, yRotation, 0);
        }
	}
	
	// Save lastPosition for velocity calculation.
	var lastPosition : Vector3 = tr.position;
	
	// We always want the movement to be framerate independent.  Multiplying by Time.deltaTime does this.
	var currentMovementOffset : Vector3 = velocity * Time.deltaTime;
	
	// Find out how much we need to push towards the ground to avoid loosing grouning
	// when walking down a step or over a sharp change in slope.
	var pushDownOffset : float = Mathf.Max(controller.stepOffset, Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);
	if (grounded)
		currentMovementOffset -= pushDownOffset * Vector3.up;
	
	// Reset variables that will be set by collision function
	movingPlatform.hitPlatform = null;
	groundNormal = Vector3.zero;
	
   	// Move our character!
	movement.collisionFlags = controller.Move (currentMovementOffset);
	
	movement.lastHitPoint = movement.hitPoint;
	lastGroundNormal = groundNormal;
	
	if (movingPlatform.enabled && movingPlatform.activePlatform != movingPlatform.hitPlatform) {
		if (movingPlatform.hitPlatform != null) {
			movingPlatform.activePlatform = movingPlatform.hitPlatform;
			movingPlatform.lastMatrix = movingPlatform.hitPlatform.localToWorldMatrix;
			movingPlatform.newPlatform = true;
		}
	}
	
	// Calculate the velocity based on the current and previous position.  
	// This means our velocity will only be the amount the character actually moved as a result of collisions.
	var oldHVelocity : Vector3 = new Vector3(velocity.x, 0, velocity.z);
	movement.velocity = (tr.position - lastPosition) / Time.deltaTime;
	var newHVelocity : Vector3 = new Vector3(movement.velocity.x, 0, movement.velocity.z);
	
	// The CharacterController can be moved in unwanted directions when colliding with things.
	// We want to prevent this from influencing the recorded velocity.
	if (oldHVelocity == Vector3.zero) {
		movement.velocity = new Vector3(0, movement.velocity.y, 0);
	}
	else {
		var projectedNewVelocity : float = Vector3.Dot(newHVelocity, oldHVelocity) / oldHVelocity.sqrMagnitude;
		movement.velocity = oldHVelocity * Mathf.Clamp01(projectedNewVelocity) + movement.velocity.y * Vector3.up;
	}
	
	if (movement.velocity.y < velocity.y - 0.001) {
		if (movement.velocity.y < 0) {
			// Something is forcing the CharacterController down faster than it should.
			// Ignore this
			movement.velocity.y = velocity.y;
		}
		else {
			// The upwards movement of the CharacterController has been blocked.
			// This is treated like a ceiling collision - stop further jumping here.
			jumping.holdingJumpButton = false;
		}
	}
	
	// We were grounded but just loosed grounding
	if (grounded && !IsGroundedTest()) {
		grounded = false;
		
		// Apply inertia from platform
		if (movingPlatform.enabled &&
			(movingPlatform.movementTransfer == MovementTransferOnJumpDB.InitTransfer ||
			movingPlatform.movementTransfer == MovementTransferOnJumpDB.PermaTransfer)
		) {
			movement.frameVelocity = movingPlatform.platformVelocity;
			movement.velocity += movingPlatform.platformVelocity;
		}
		
		SendMessage("OnFall", SendMessageOptions.DontRequireReceiver);
		// We pushed the character down to ensure it would stay on the ground if there was any.
		// But there wasn't so now we cancel the downwards offset to make the fall smoother.
		tr.position += pushDownOffset * Vector3.up;
	}
	// We were not grounded but just landed on something
	else if (!grounded && IsGroundedTest()) {
		grounded = true;
		jumping.jumping = false;
		SubtractNewPlatformVelocity();
		
		if(diving){
			diving = false;
			SetVelocity(transform.forward*6);
			audio.volume = proneLandSoundVolume;
			audio.PlayOneShot(proneLandSound);
			camSpeed = lastCamSpeed;
			//Prone();
		}
		
		//Fall Damage!!!
		
		SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
		BroadcastMessage("Landed", SendMessageOptions.DontRequireReceiver);
		var fallVal : float = ((-1*velocity.y)-movement.fallDamageStart)/(movement.fallDamageEnd-movement.fallDamageStart);
		fallVal *= movement.fallDamageMax;
		fallVal = Mathf.Round(fallVal);
		GunLook.jostleAmt = Vector3(-.06,-.1,0);
		CamSway.jostleAmt = Vector3(-.01,-.07,0);
		if(fallVal > 0 && PlayerWeapons.playerActive && !paused)
			BroadcastMessage("ApplyFallDamage", fallVal);
		//Debug.Log(fallVal);
		audio.volume = landSoundVolume;
		audio.PlayOneShot(landSound);

	}
	
	// Moving platforms support
	if (MoveWithPlatform()) {
		// Use the center of the lower half sphere of the capsule as reference point.
		// This works best when the character is standing on moving tilting platforms. 
		movingPlatform.activeGlobalPoint = tr.position + Vector3.up * (controller.center.y - controller.height*0.5 + controller.radius);
		movingPlatform.activeLocalPoint = movingPlatform.activePlatform.InverseTransformPoint(movingPlatform.activeGlobalPoint);
		
		// Support moving platform rotation as well:
        movingPlatform.activeGlobalRotation = tr.rotation;
        movingPlatform.activeLocalRotation = Quaternion.Inverse(movingPlatform.activePlatform.rotation) * movingPlatform.activeGlobalRotation; 
	}
}

function FixedUpdate () {
	if (movingPlatform.enabled) {
		if (movingPlatform.activePlatform != null) {
			if (!movingPlatform.newPlatform) {
				var lastVelocity : Vector3 = movingPlatform.platformVelocity;
				
				movingPlatform.platformVelocity = (
					movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)
					- movingPlatform.lastMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)
				) / Time.deltaTime;
			}
			movingPlatform.lastMatrix = movingPlatform.activePlatform.localToWorldMatrix;
			movingPlatform.newPlatform = false;
		}
		else {
			movingPlatform.platformVelocity = Vector3.zero;	
		}
	}
	
	if (useFixedUpdate)
		UpdateFunction();
}

function Update () {
	if(paused){
		movement.velocity = Vector3(0,0,0);
		return;
	}
		
	if((inputMoveDirection.x != 0 || inputMoveDirection.y != 0 ||inputMoveDirection.z != 0) && grounded && PlayerWeapons.canMove){ 
		if(!walking)
			BroadcastMessage("Walking", SendMessageOptions.DontRequireReceiver);
		walking = true;
	} else {
		if(walking)
			BroadcastMessage("StopWalking", SendMessageOptions.DontRequireReceiver);
		walking = false;
	}

	if (!useFixedUpdate)
		UpdateFunction();
		
	/*if(weaponCamera.transform.localPosition.y > standardCamHeight){
		weaponCamera.transform.localPosition.y = standardCamHeight;
	} else if(weaponCamera.transform.localPosition.y < crouchingCamHeight){
		weaponCamera.transform.localPosition.y = crouchingCamHeight;
	}*/

	if (grounded) {	
		if (InputDB.GetButtonUp ("Crouch") && PlayerWeapons.canCrouch){
			if(!proneFrame){
				if(!crouching && !prone){
					/*if(PlayerWeapons.sprinting && movement.useDive && !diving){
						SetVelocity(transform.forward*25 + Vector3.up*12);
						audio.volume = jumpSoundVolume;
						audio.PlayOneShot(jumpSound);
						diving = true;
						lastCamSpeed = camSpeed;
						crouching = false;
						Prone();
						camSpeed = 1;
						movement.crouchedTime = -1;
						//prone = true;
					} else {*/
						
					Crouch();
				}else if(crouching && AboveIsClear()){
					crouching = false;
					stopCrouching = true;
					NormalSpeed();
				} else if(prone && AboveIsClearProne()){
					//crouching = false;
					//stopProne = true;
					prone = false;
					Crouch();
				}
			}
			proneFrame = false;
		} else if(InputDB.GetButton("Crouch")){
			if(movement.crouchedTime < 0)
				movement.crouchedTime = Time.time;
			if(Time.time > movement.crouchedTime+ movement.proneHoldTime && movement.crouchedTime >0 && !prone){
				if(useProne){
					Prone();
					proneFrame = true;
				}
				movement.crouchedTime = -1;
			}
		} else {
			movement.crouchedTime = -1;
		}
	}
	
	if(crouching && !prone){
		if(weaponCamera.transform.localPosition.y > crouchingCamHeight){
				weaponCamera.transform.localPosition.y = Mathf.Clamp (weaponCamera.transform.localPosition.y - crouchDeltaHeight*Time.deltaTime*camSpeed, crouchingCamHeight, standardCamHeight);
		} else {
				weaponCamera.transform.localPosition.y = Mathf.Clamp (weaponCamera.transform.localPosition.y + crouchDeltaHeight*Time.deltaTime*camSpeed, proneCamHeight, crouchingCamHeight);
		}
	} else if (prone) {
		if(weaponCamera.transform.localPosition.y > proneCamHeight){
			weaponCamera.transform.localPosition.y = Mathf.Clamp (weaponCamera.transform.localPosition.y - crouchDeltaHeight*Time.deltaTime*camSpeed, proneCamHeight, weaponCamera.transform.localPosition.y);
		} else if(!hitProne) {
			GunLook.jostleAmt = Vector3(-.075,-.12,0);
			CamSway.jostleAmt = Vector3(-.01,-.03,0);
			hitProne = true;
			audio.volume = proneLandSoundVolume;
			audio.PlayOneShot(proneLandSound);
		}
	} else {
		if(weaponCamera.transform.localPosition.y < standardCamHeight){
				weaponCamera.transform.localPosition.y = Mathf.Clamp(weaponCamera.transform.localPosition.y + standardCamHeight*Time.deltaTime*camSpeed,proneCamHeight, standardCamHeight);
		}
	}

}

function StandUp () {
	if(!AboveIsClear())
		return;
	if(crouching){
		crouching = false;
		stopCrouching = true;
		NormalSpeed();
	} else if(prone){
		stopProne = true;
		NormalSpeed();
	}
}

//Check if it is clear above us to stand up
function AboveIsClear () {
	if(!crouching && !prone)
		return true;
		
	return !Physics.Raycast(transform.TransformPoint(controller.center)+(controller.height/2)*Vector3.up,Vector3.up, standardHeight-controller.height);
}

//Check if it is clear for us to go to crouch
function AboveIsClearProne () {
	return !Physics.Raycast(transform.TransformPoint(controller.center)+(controller.height/2)*Vector3.up,Vector3.up, standardHeight-controller.height-crouchDeltaHeight);
}

private function ApplyInputVelocityChange (velocity : Vector3) {	
	if (!canControl || !PlayerWeapons.canMove)
		inputMoveDirection = Vector3.zero;
	
	// Find desired velocity
	var desiredVelocity : Vector3;
	if (grounded && TooSteep()) {
		// The direction we're sliding in
		desiredVelocity = Vector3(groundNormal.x, 0, groundNormal.z).normalized;
		// Find the input movement direction projected onto the sliding direction
		var projectedMoveDir = Vector3.Project(inputMoveDirection, desiredVelocity);
		// Add the sliding direction, the spped control, and the sideways control vectors
		desiredVelocity = desiredVelocity + projectedMoveDir * sliding.speedControl + (inputMoveDirection - projectedMoveDir) * sliding.sidewaysControl;
		// Multiply with the sliding speed
		desiredVelocity *= sliding.slidingSpeed;
	}
	else
		desiredVelocity = GetDesiredHorizontalVelocity();
	
	if (movingPlatform.enabled && movingPlatform.movementTransfer == MovementTransferOnJumpDB.PermaTransfer) {
		desiredVelocity += movement.frameVelocity;
		desiredVelocity.y = 0;
	}
	
	if (grounded)
		desiredVelocity = AdjustGroundVelocityToNormal(desiredVelocity, groundNormal);
	else
		velocity.y = 0;
	
	// Enforce max velocity change
	var maxVelocityChange : float = GetMaxAcceleration(grounded) * Time.deltaTime * velocityFactor;
	var velocityChangeVector : Vector3 = (desiredVelocity - velocity);
	if (velocityChangeVector.sqrMagnitude > maxVelocityChange * maxVelocityChange) {
		velocityChangeVector = velocityChangeVector.normalized * maxVelocityChange;
	}
	// If we're in the air and don't have control, don't apply any velocity change at all.
	// If we're on the ground and don't have control we do apply it - it will correspond to friction.
	if (grounded || canControl)
		velocity += velocityChangeVector;
	
	if (grounded) {
		// When going uphill, the CharacterController will automatically move up by the needed amount.
		// Not moving it upwards manually prevent risk of lifting off from the ground.
		// When going downhill, DO move down manually, as gravity is not enough on steep hills.
		velocity.y = Mathf.Min(velocity.y, 0);
	}
	
	return velocity;
}

private function ApplyGravityAndJumping (velocity : Vector3) {
	
	if (!inputJump || !canControl) {
		jumping.holdingJumpButton = false;
		jumping.lastButtonDownTime = -100;
	}
	
	if (inputJump && jumping.lastButtonDownTime < 0 && canControl && !prone)
		jumping.lastButtonDownTime = Time.time;
	
	if (grounded)
		velocity.y = Mathf.Min(0, velocity.y) - movement.gravity * Time.deltaTime;
	else {
		velocity.y = movement.velocity.y - movement.gravity * Time.deltaTime;
		
		// When jumping up we don't apply gravity for some time when the user is holding the jump button.
		// This gives more control over jump height by pressing the button longer.
		if (jumping.jumping && jumping.holdingJumpButton) {
			// Calculate the duration that the extra jump force should have effect.
			// If we're still less than that duration after the jumping time, apply the force.
			if (Time.time < jumping.lastStartTime + jumping.extraHeight / CalculateJumpVerticalSpeed(jumping.baseHeight)) {
				// Negate the gravity we just applied, except we push in jumpDir rather than jump upwards.
				velocity += jumping.jumpDir * movement.gravity * Time.deltaTime;
			}
		}
		
		// Make sure we don't fall any faster than maxFallSpeed. This gives our character a terminal velocity.
		velocity.y = Mathf.Max (velocity.y, -movement.maxFallSpeed);
	}
		
	if (grounded) {
		// Jump only if the jump button was pressed down in the last 0.2 seconds.
		// We use this check instead of checking if it's pressed down right now
		// because players will often try to jump in the exact moment when hitting the ground after a jump
		// and if they hit the button a fraction of a second too soon and no new jump happens as a consequence,
		// it's confusing and it feels like the game is buggy.
		if (jumping.enabled && canControl && PlayerWeapons.canMove && (Time.time - jumping.lastButtonDownTime < 0.2)) {
			grounded = false;
			jumping.jumping = true;
			jumping.lastStartTime = Time.time;
			jumping.lastButtonDownTime = -100;
			jumping.holdingJumpButton = true;
			
			// Calculate the jumping direction
			if (TooSteep())
				jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.steepPerpAmount);
			else
				jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.perpAmount);
			
			// Apply the jumping force to the velocity. Cancel any vertical velocity first.
			velocity.y = 0;
			velocity += jumping.jumpDir * CalculateJumpVerticalSpeed (jumping.baseHeight);
			if(crouching){
				stopCrouching = true;
				NormalSpeed();
			}
			if(prone){
				stopProne = true;
				NormalSpeed();
				return;
			}

			
			// Apply inertia from platform
			if (movingPlatform.enabled &&
				(movingPlatform.movementTransfer == MovementTransferOnJumpDB.InitTransfer ||
				movingPlatform.movementTransfer == MovementTransferOnJumpDB.PermaTransfer)
			) {
				movement.frameVelocity = movingPlatform.platformVelocity;
				velocity += movingPlatform.platformVelocity;
			}
			
			SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
			audio.volume = jumpSoundVolume;
			audio.PlayOneShot(jumpSound);
			BroadcastMessage("Airborne", SendMessageOptions.DontRequireReceiver);

		}
		else {
			jumping.holdingJumpButton = false;
		}
	}
	
	return velocity;
}

function OnControllerColliderHit (hit : ControllerColliderHit) {
	if (hit.normal.y > 0 && hit.normal.y > groundNormal.y && hit.moveDirection.y < 0) {
		if ((hit.point - movement.lastHitPoint).sqrMagnitude > 0.001 || lastGroundNormal == Vector3.zero)
			groundNormal = hit.normal;
		else
			groundNormal = lastGroundNormal;
		
		movingPlatform.hitPlatform = hit.collider.transform;
		movement.hitPoint = hit.point;
		movement.frameVelocity = Vector3.zero;
	}
}

private function SubtractNewPlatformVelocity () {
	// When landing, subtract the velocity of the new ground from the character's velocity
	// since movement in ground is relative to the movement of the ground.
	if (movingPlatform.enabled &&
		(movingPlatform.movementTransfer == MovementTransferOnJumpDB.InitTransfer ||
		movingPlatform.movementTransfer == MovementTransferOnJumpDB.PermaTransfer)
	) {
		// If we landed on a new platform, we have to wait for two FixedUpdates
		// before we know the velocity of the platform under the character
		if (movingPlatform.newPlatform) {
			var platform : Transform = movingPlatform.activePlatform;
			yield WaitForFixedUpdate();
			yield WaitForFixedUpdate();
			if (grounded && platform == movingPlatform.activePlatform)
				yield 1;
		}
		movement.velocity -= movingPlatform.platformVelocity;
	}
}

private function MoveWithPlatform () : boolean {
	return (
		movingPlatform.enabled
		&& (grounded || movingPlatform.movementTransfer == MovementTransferOnJumpDB.PermaLocked)
		&& movingPlatform.activePlatform != null
	);
}

private function GetDesiredHorizontalVelocity () {
	// Find desired velocity
	var desiredLocalDirection : Vector3 = tr.InverseTransformDirection(inputMoveDirection);
	maxSpeed = MaxSpeedInDirection(desiredLocalDirection);
	if (grounded) {
		// Modify max speed on slopes based on slope speed multiplier curve
		var movementSlopeAngle = Mathf.Asin(movement.velocity.normalized.y)  * Mathf.Rad2Deg;
		maxSpeed *= movement.slopeSpeedMultiplier.Evaluate(movementSlopeAngle);
	}
	return tr.TransformDirection(desiredLocalDirection * maxSpeed);
}

private function AdjustGroundVelocityToNormal (hVelocity : Vector3, groundNormal : Vector3) : Vector3 {
	var sideways : Vector3 = Vector3.Cross(Vector3.up, hVelocity);
	return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
}

private function IsGroundedTest () {
	return (groundNormal.y > 0.01);
}

function GetMaxAcceleration (grounded : boolean) : float {
	// Maximum acceleration on ground and in air
	if (grounded)
		return movement.maxGroundAcceleration;
	else
		return movement.maxAirAcceleration;
}

function CalculateJumpVerticalSpeed (targetJumpHeight : float) {
	// From the jump height and gravity we deduce the upwards speed 
	// for the character to reach at the apex.
	return Mathf.Sqrt (2 * targetJumpHeight * movement.gravity);
}

function IsJumping () {
	return jumping.jumping;
}

function IsSliding () {
	return (grounded && sliding.enabled && TooSteep());
}

function IsTouchingCeiling () {
	return (movement.collisionFlags & CollisionFlags.CollidedAbove) != 0;
}

function IsGrounded () {
	return grounded;
}

function TooSteep () {
	return (groundNormal.y <= Mathf.Cos(controller.slopeLimit * Mathf.Deg2Rad));
}

function GetDirection () {
	return inputMoveDirection;
}

function SetControllable (controllable : boolean) {
	canControl = controllable;
}

// Project a direction onto elliptical quater segments based on forward, sideways, and backwards speed.
// The function returns the length of the resulting vector.
function MaxSpeedInDirection (desiredMovementDirection : Vector3) : float {
	if (desiredMovementDirection == Vector3.zero)
		return 0;
	else {
		var zAxisEllipseMultiplier : float = (desiredMovementDirection.z > 0 ? movement.maxForwardSpeed : movement.maxBackwardsSpeed) / movement.maxSidewaysSpeed;
		var temp : Vector3 = new Vector3(desiredMovementDirection.x, 0, desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
		var length : float = new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * movement.maxSidewaysSpeed;
		return length;
	}
}

function SetVelocity(velocity : Vector3){
	grounded = false;
	movement.velocity = velocity;
	movement.frameVelocity = Vector3.zero;
	SendMessage("OnExternalVelocity", SendMessageOptions.DontRequireReceiver);
}

function Aiming(){
	movement.maxForwardSpeed = Mathf.Min(movement.maxForwardSpeed ,movement.maxAimSpeed);
	movement.maxSidewaysSpeed = Mathf.Min(movement.maxSidewaysSpeed, movement.aimSidewaysSpeed);
	movement.maxBackwardsSpeed = Mathf.Min(movement.maxBackwardsSpeed, movement.aimBackwardsSpeed);
	aim = true;
}

function StopAiming(){
	aim = false;
	NormalSpeed();
}

function Crouch(){
	weaponCamera.BroadcastMessage("Crouching", SendMessageOptions.DontRequireReceiver);
	movement.maxForwardSpeed = movement.maxCrouchSpeed;
	movement.maxSidewaysSpeed = movement.crouchSidewaysSpeed;
	movement.maxBackwardsSpeed = movement.crouchBackwardsSpeed;
	controller.height = standardHeight-crouchDeltaHeight;
	controller.center.y = standardCenter - crouchDeltaHeight/2;
	crouching = true;
	if(aim){
		Aiming();
	}
}

function Prone(){
	hitProne = false;
	weaponCamera.BroadcastMessage("Prone", SendMessageOptions.DontRequireReceiver);
	crouching = false;
	stopCrouching = false;
	movement.maxForwardSpeed = movement.maxProneSpeed;
	movement.maxSidewaysSpeed = movement.proneSidewaysSpeed;
	movement.maxBackwardsSpeed = movement.proneBackwardsSpeed;
	controller.height = standardHeight-proneDeltaHeight;
	controller.center.y = standardCenter - proneDeltaHeight/2;
	prone = true;
	if(aim){
		Aiming();
	}
}

function NormalSpeed(){
	sprinting = false;
	if(stopCrouching){
		crouching = false;
		controller.height += crouchDeltaHeight;
		BroadcastMessage("StopWalking", SendMessageOptions.DontRequireReceiver);
		controller.center += Vector3(0,crouchDeltaHeight/2, 0);
		stopCrouching = false;	
	}else if(crouching){
		movement.maxForwardSpeed = movement.maxCrouchSpeed;
		movement.maxSidewaysSpeed = movement.crouchSidewaysSpeed;
		movement.maxBackwardsSpeed = movement.crouchBackwardsSpeed;
		if(aim){
			Aiming();
		}
		return;
	} else if (stopProne) {
		prone = false;
		controller.height += proneDeltaHeight;
		BroadcastMessage("StopWalking", SendMessageOptions.DontRequireReceiver);
		controller.center += Vector3(0,proneDeltaHeight/2, 0);
		stopProne = false;
	} else if (prone) {
		movement.maxForwardSpeed = movement.maxProneSpeed;
		movement.maxSidewaysSpeed = movement.proneSidewaysSpeed;
		movement.maxBackwardsSpeed = movement.proneBackwardsSpeed;
		if(aim){
			Aiming();
		}
		return;
	}
	movement.maxBackwardsSpeed = movement.defaultBackwardsSpeed;
	movement.maxSidewaysSpeed = movement.defaultSidewaysSpeed;
	movement.maxForwardSpeed = movement.defaultForwardSpeed;
	movement.maxAirAcceleration = movement.defaultAirAcceleration;
	movement.maxGroundAcceleration = movement.defaultGroundAcceleration;
	if(aim){
		Aiming();
	}
}

function Sprinting(){
	sprinting = true;
	if(crouching){
		crouching = false;
		controller.height += crouchDeltaHeight;
		BroadcastMessage("StopWalking", SendMessageOptions.DontRequireReceiver);
		controller.center += Vector3(0,crouchDeltaHeight/2, 0);
	}
	/*if(prone){
		prone = false;
		controller.height += proneDeltaHeight;
		BroadcastMessage("StopWalking", SendMessageOptions.DontRequireReceiver);
		controller.center += Vector3(0,proneDeltaHeight/2, 0);
	}*/
	if(canSprint){
		movement.maxForwardSpeed = movement.maxSprintSpeed;
		movement.maxGroundAcceleration = movement.sprintGroundAcceleration;
		movement.maxAirAcceleration = movement.defaultAirAcceleration;
		movement.maxBackwardsSpeed = movement.defaultBackwardsSpeed;
		movement.maxSidewaysSpeed = movement.sprintSidewaysSpeed;
	}
}

// Require a character controller to be attached to the same game object
@script RequireComponent (CharacterController)

@script RequireComponent (MovementValues)
//@script AddComponentMenu ("Character/Character Motor")

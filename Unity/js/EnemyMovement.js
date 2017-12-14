//#pragma strict
private var target : Transform;
var waypoint : Transform;
private var targetRotation : Quaternion;
private var targetVector : Vector3;
private var move : boolean = false;
private var playerLastSeen : Vector3;
private var visitedLastSeen : boolean = true;
private var targetPriority : int = 0;

private var curTarget : Vector3;
private var loseTime : float = 0;

var turnSpeed : float;
var attackRange : float; 
var targetBuffer : float;
var desiredSpeed : float;
var forceConstant : float;
var viewAngle : float;
var viewRange : float;
var hearRange : float;
var blocksVision : LayerMask;
var moves : boolean = true;
private var sees : boolean;

private var hitOverride : boolean = false;

static var enemies : int = 0;

function Start () {
	target = PlayerWeapons.weaponCam.transform;
	this.GetComponent(EnemyDamageReceiver).isEnemy = true;
}

function Update () {
	if(MouseLookDBJS.freeze)
		return;
	sees = CanSeeTarget();
	if(hitOverride){
		playerLastSeen = target.position;
		visitedLastSeen = false;
	}
	hitOverride = false;
	var relativePos : Vector3;
	if(sees){
		curTarget = target.position;
	} else if (!visitedLastSeen) { 
		curTarget = playerLastSeen;
		loseTime += Time.deltaTime;
		if(loseTime > 3){
			visitedLastSeen = true;
			loseTime = 0;
			waypoint = Waypoint.GetClosestWaypoint(transform.position);
		}
	} else {
		if(waypoint) {
			curTarget = waypoint.position;
		} else {
			curTarget = Vector3(0,0,0);
		}
	}
	
	relativePos = curTarget - transform.position;
	targetRotation = Quaternion.LookRotation(relativePos);
	ToRotation(targetRotation.eulerAngles);
			
	if(move && moves){
   	 // this reduces the amount of force that acts on the object if it is already
   	 // moving at speed.
    	var forceMultiplier : float = Mathf.Clamp01((desiredSpeed - rigidbody.velocity.magnitude) / desiredSpeed);
    	// now we actually perform the push
    	rigidbody.AddForce(transform.forward * (forceMultiplier * Time.deltaTime * forceConstant));
    }
    
    if(Vector3.Distance(transform.position, target.position) < attackRange && sees)
    	this.SendMessage("Attack");
    	
    if(Vector3.Distance(transform.position, curTarget) < targetBuffer) {
    	visitedLastSeen = true;
    	move = false;
    } else {
    	move = true;
    }
}

function ToRotation(v3 : Vector3){
	var xtarget : float = transform.localEulerAngles.x;
	var ztarget : float = transform.localEulerAngles.z;
	var ytarget : float = Mathf.MoveTowardsAngle(transform.localEulerAngles.y, v3.y, Time.deltaTime*turnSpeed);
	transform.localEulerAngles = Vector3(xtarget,ytarget,ztarget);
}

function CanSeeTarget () : boolean {
	//checks if target is visible, within field of view, or close enough to be heard
	
	var canSee = false;
	var hit: RaycastHit;
	
	var targetAngle : float = Vector3.Angle(target.position - transform.position, transform.forward);
	var targetDistance : float = Vector3.Distance(transform.position, target.position);
	//is target within range and angle
	if(targetDistance < viewRange && Mathf.Abs(targetAngle) < viewAngle/2) {
		if(!Physics.Linecast(transform.position, (target.position), blocksVision)){
			playerLastSeen = target.position;
		 	canSee=true;
		 	visitedLastSeen = false;
		}
	}
	//is target close enough to hear?
	if(targetDistance < hearRange){
		playerLastSeen = target.position;
		canSee=true;
		visitedLastSeen = false;
	}
	return canSee;
}

function OnDrawGizmosSelected (){
	Gizmos.color = Color.green;

	//Draw field of view
	var leftRayRotation : Quaternion = Quaternion.AngleAxis( -viewAngle/2, Vector3.up );
    var rightRayRotation : Quaternion = Quaternion.AngleAxis( viewAngle/2, Vector3.up );
    
    var leftRayDirection : Vector3 = leftRayRotation * transform.forward;
    var rightRayDirection : Vector3 = rightRayRotation * transform.forward;
    
    Gizmos.DrawRay( transform.position, leftRayDirection * viewRange );
    Gizmos.DrawRay( transform.position, rightRayDirection * viewRange );
    
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, attackRange);
    
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, hearRange);
}

function ApplyDamagePlayer(damage : float){
	hitOverride = true;
}
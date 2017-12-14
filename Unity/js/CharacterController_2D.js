public var f_speed : float = 5.0;
public var loopSprites : SpriteManager[];
public var jumpSprite : JumpSpriteManager;
public var doorOpenTexture : Texture2D;
public var doorCloseTexture : Texture2D;
public var doorOpenSound : AudioClip;
public var getKeySound : AudioClip;
public var jumpSound : AudioClip;
public var layerMask : LayerMask;

private var restartButton : GUITexture;
private var in_direction : int;
private var b_isJumping : boolean;
private var f_height : float;
private var f_lastY : float;
private var b_hasKey : boolean;

public function Start() : void {
	//Get restartButton from the Game Scene
	restartButton = GameObject.FindWithTag("RestartButton").guiTexture;
	//make restart Button disabled
	restartButton.enabled = false;
	
	//Start with no Key
	b_hasKey = false;
	
	//Get mesh from the character MeshFilter
	mesh = GetComponent(MeshFilter).sharedMesh;
	//Get hight from the top of our character to the bottom of our character
	f_height = mesh.bounds.size.y * transform.localScale.y;
	//Set up the last y-axis position of our character
	f_lastY = transform.position.y;
	b_isJumping = false;
	
	in_direction = 1;
	//Initialization Sprite Manager
	for (var i : int = 0; i < loopSprites.length; i++) {
		loopSprites[i].Init();
	}
	//Update Camera to the character position
	Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
}

// Update is called once per frame
public function Update () : void {
	//If our character isn’t jumping
	if (!b_isJumping) {	
		if (Input.GetButton("Horizontal")) {
			//Walking
			in_direction = Input.GetAxis("Horizontal") < 0 ? -1 : 1;
			rigidbody.velocity = new Vector3((in_direction*f_speed), rigidbody.velocity.y, 0);
			//Reset Stay animation frame back to the first frame
			loopSprites[0].ResetFrame();
			//Update Walking animation while the character is walking
			loopSprites[1].UpdateAnimation(in_direction, renderer.material);
		} else {
			//Stay
			//Reset Walking animation frame back to the first frame
			loopSprites[1].ResetFrame();
			//Update Stay animation while the character is not walking
			loopSprites[0].UpdateAnimation(in_direction, renderer.material);
		}
		//Jump
		if (Input.GetButton("Jump")) {
			b_isJumping = true;
			//Then make it Jump
			audio.volume = 0.3;
			audio.PlayOneShot(jumpSound);
			loopSprites[0].ResetFrame();
			loopSprites[1].ResetFrame();
			rigidbody.velocity = new Vector3(rigidbody.velocity.x, -Physics.gravity.y, 0);
		}
	} else {
		//update animation while it Jump
		jumpSprite.UpdateJumpAnimation(in_direction, rigidbody.velocity.y, renderer.material);
	}
}

public function LateUpdate() : void {
	//Checking Jumping by using Raycast
	var hit : RaycastHit;
	var v3_hit : Vector3 = transform.TransformDirection (-Vector3.up) * (f_height * 0.5);
	var v3_right : Vector3 = new Vector3(transform.position.x + (collider.bounds.size.x*0.45), transform.position.y, transform.position.z);
	var v3_left : Vector3 = new Vector3(transform.position.x - (collider.bounds.size.x*0.45), transform.position.y, transform.position.z);

    if (Physics.Raycast (transform.position, v3_hit, hit, 2.5, layerMask.value)) {
        b_isJumping = false;
    } else if (Physics.Raycast (v3_right, v3_hit, hit, 2.5, layerMask.value)) {
   		if (b_isJumping) {
        	b_isJumping = false;
        }
    } else if (Physics.Raycast (v3_left, v3_hit, hit, 2.5, layerMask.value)) {
        if (b_isJumping) {
        	b_isJumping = false;
        }
    } else {
		if (!b_isJumping) {
	    	if (Mathf.Floor(transform.position.y) == f_lastY) {
	    		b_isJumping = false;
	    	} else {
	    		b_isJumping = true;
	    	}
	    }
	}
    f_lastY = Mathf.Floor(transform.position.y);
    //Update Camera
	Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
}

public function OnTriggerEnter (hit : Collider) : IEnumerator {
	if (hit.collider.tag == "Key") {
		if (!b_hasKey) {
			//We hit our Key
			audio.volume = 1.0;
			audio.PlayOneShot(getKeySound);
			b_hasKey = true;
			Destroy (hit.gameObject);
		}
	}
	
	if (hit.collider.tag == "Door") {
		if (b_hasKey) {
			audio.volume = 1.0;
			audio.PlayOneShot(doorOpenSound);
			//If we had Key and hit door the door will open
			hit.gameObject.renderer.material.mainTexture = doorOpenTexture;
			//wait for 1 second and destroy our character
			yield WaitForSeconds(1);
			Destroy (gameObject);
			//We close the door
			hit.gameObject.renderer.material.mainTexture = doorCloseTexture;
			//Show Restart Button
			restartButton.enabled = true;
		}
	}
}

public function OnDrawGizmos() : void {
	mesh = GetComponent(MeshFilter).sharedMesh;
	f_height = mesh.bounds.size.y * transform.localScale.y;
	var v3_right : Vector3 = new Vector3(transform.position.x + (collider.bounds.size.x*0.45), transform.position.y, transform.position.z);
	var v3_left : Vector3 = new Vector3(transform.position.x - (collider.bounds.size.x*0.45), transform.position.y, transform.position.z);
	Gizmos.color = Color.red;
	Gizmos.DrawRay(transform.position, transform.TransformDirection (-Vector3.up) * (f_height * 0.5));
	Gizmos.DrawRay(v3_right, transform.TransformDirection (-Vector3.up) * (f_height * 0.5));
	Gizmos.DrawRay(v3_left, transform.TransformDirection (-Vector3.up) * (f_height * 0.5));
}

class SpriteManager {
	public var spriteTexture : Texture2D; //Set Texture use for a loop animation such as walking, stay, etc.
	public var in_framePerSec : int; //Get frame per sec to calculate time
	public var in_gridX : int; //Get max number of Horizontal images
	public var in_gridY : int; //Get max number of Vertical images
	
	private var f_timePercent : float;
	private var f_nextTime : float; //Update time by using frame persecond
	private var f_gridX : float;
	private var f_gridY : float;
	private var in_curFrame : int;
	
	public function Init () : void {
		f_timePercent = 1.0/in_framePerSec;
		f_nextTime = f_timePercent; //Update time by using frame persecond
		f_gridX = 1.0/in_gridX;
		f_gridY = 1.0/in_gridY;
		in_curFrame = 1;
	}
	
	public function UpdateAnimation (_direction : int, _material : Material) : void {
		//Update material
		_material.mainTexture = spriteTexture;
		//Update frame by time
		if (Time.time > f_nextTime) {
			f_nextTime = Time.time + f_timePercent;
			in_curFrame++;
			if (in_curFrame > in_framePerSec) {
				in_curFrame = 1;
			}
		}
		_material.mainTextureScale = new Vector2 (_direction * f_gridX, f_gridY);
		var in_col : int = 0;
		if (in_gridY > 1) {
			//If there is more than one grid on the y-axis update the texture
			in_col = Mathf.Ceil(in_curFrame/in_gridX);
		}
		if (_direction == 1) { //Right 
			_material.mainTextureOffset = new Vector2(((in_curFrame)%in_gridX) * f_gridX, in_col*f_gridY);
		} else { //Left
			//Flip Texture
			_material.mainTextureOffset = new Vector2(((in_gridX + (in_curFrame)%in_gridX)) * f_gridX, in_col*f_gridY);
		}
	}
	
	public function ResetFrame () :void {
		in_curFrame = 1;
	}
}

class JumpSpriteManager {
	public var t_jumpStartTexture : Texture2D; //Alternative Jump Texture play after t_jumpReadyTextures
	public var t_jumpAirTexture : Texture2D; //Alternative Jump Texture play when the player in the air at the top position of projectile
	public var t_jumpDownTexture : Texture2D; //Alternative Jump Texture play when the player fall to the ground
	
	public function UpdateJumpAnimation (_direction : int, _velocityY : float, _material : Material) : void {
		//Checking for the player position in the air
		if ((_velocityY >= -2.0) && (_velocityY <= 2.0)) { //Top of the projectile
			_material.mainTexture = t_jumpAirTexture;
		} else if (_velocityY > 2.0) { //Start Jump
			_material.mainTexture = t_jumpStartTexture;
		} else {  //Fall
			_material.mainTexture = t_jumpDownTexture;
		}
		_material.mainTextureScale = new Vector2 (_direction * 1, 1);
		_material.mainTextureOffset = new Vector2 (_direction * 1, 1);
	}
}

@script RequireComponent (AudioSource)
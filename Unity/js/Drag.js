#pragma strict

enum dirs {x, y} //which rotation axis should we use?
var direction : dirs = dirs.x;

var invert : boolean; //should it be inverted?

var input : InputItem;
var sensitivity : float;
var buttons : TouchButton[];
private var touch : int;
private var inButtons : boolean;

function FixedUpdate () {
	var x : float;
    var y : float;
    
    var t : int = 0;
    
    if (Input.touches.Length > 0) { //user is touching
    	for (t = 0; t < Input.touches.length; t++) { //for each touch
    	
    		//check if that touch is currently touching a button
    		inButtons = false;
    		for (var b = 0; b < buttons.length; b++) {
    			if(Input.touches[t].fingerId == buttons[b].curTouch){
    				inButtons = true;
    				break;
    			}
    		}
    		if(!inButtons) //if it wasn't, then we have found our touch
    			break;

   		}
   		
   		//if no touch was viable
		if(inButtons)
			return;
		
   		if (Input.touches[t].phase == TouchPhase.Moved) { //the touch moved
      		x = Input.touches[t].deltaPosition.x *sensitivity* Time.deltaTime;
      		y = Input.touches[t].deltaPosition.y *sensitivity* Time.deltaTime;	
    	}
  	} else {
  		// zero out axis
  		x = 0;
  		y = 0;
  	}
  	
  	//invert if needed
	if(invert){ 
		x *= -1;
		y *= -1;
	}
	
	
	//set proper axis
	if(direction == dirs.x) {
		input.axis = x;
	} else {
		input.axis = y;
	}
}

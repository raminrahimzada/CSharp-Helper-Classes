#pragma strict
var key : String;
var input : InputItem;

function UpdateInput () {
	//Just get the axis value from Unity's input
	input.axis = Input.GetAxisRaw(key);
}

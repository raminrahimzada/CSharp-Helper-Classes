#pragma strict
var amplitude : float;
var time : float;

function Update () {
	CameraShake.ShakeCam(amplitude, 10, time);
}
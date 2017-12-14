@script ExecuteInEditMode()

var enableMiniMap : boolean	= true;
var MiniMapRender : RenderTexture;
var MapCompass : Texture2D;
var MapFrame : Texture2D;
var compassCamera : Transform;
var RegionName : String;
var OnscreenLabelStyle : GUIStyle = new GUIStyle();

function OnGUI () {	
	if (enableMiniMap){
		drawMiniMap();
	}
}

function drawMiniMap () {
	var Xoffset = Screen.width - 185;
	var Yoffset = 5;
	
	GUI.DrawTexture ( Rect( Xoffset + 25, Yoffset + 28, 128, 128 ), MiniMapRender, ScaleMode.ScaleToFit, true, 1.0 );
	GUI.DrawTexture ( Rect( Xoffset + 0, Yoffset + 0, 179, 179 ), MapFrame, ScaleMode.ScaleToFit, true, 1.0 );
	
	var newRotation = transform.rotation.eulerAngles.y;
	GUI.matrix = Matrix4x4.TRS ( Vector3( Xoffset + 90, Yoffset + 92, 0 ), Quaternion.Euler(0, 0, newRotation), Vector3.one );
	GUI.DrawTexture ( Rect( -16, -16, 32, 32 ), MapCompass, ScaleMode.ScaleToFit, true, 1.0 );
	GUI.matrix = Matrix4x4.TRS ( Vector3.zero, Quaternion.identity, Vector3.one );
	
	compassCamera.position.x = transform.position.x;
	compassCamera.position.z = transform.position.z;
	
	GUI.Label (Rect (Xoffset-10, Yoffset-1, 200, 30), RegionName, OnscreenLabelStyle);
	
}
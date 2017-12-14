
var Score : int = 0;
var TotalAmount : int = 6;

var ScoreBarBackground : Texture2D;

var SpawnCrystals : GameObject[];
function Start() {
	SpawnCrystals = GameObject.FindGameObjectsWithTag ("Respawn");
	TotalAmount = SpawnCrystals.Length;
}

function OnGUI() {
	
	if ( Score == TotalAmount ) {
		
		var WinString : String = "Congratulations.. you win!";
		GUI.Label (Rect (25, 25, 200, 30), WinString);
		
		if (GUI.Button (Rect (25, 65, 200, 30), "Reset Game")) {
			
			for (var respawn in SpawnCrystals) {
   				respawn.active = true;
   				Score = 0;
			}
		}
	} else {
		ScoreBarGUI();
	}
}

function ScoreBarGUI() {
	GUI.BeginGroup ( new Rect ( 10, 10, 192, 96), ScoreBarBackground ) ;
	//IdentBarDynamicGUI();
	
	GUI.Label (Rect ( 110, 30, 200, 20), Score + " / " + TotalAmount );
	
	GUI.EndGroup ();
}

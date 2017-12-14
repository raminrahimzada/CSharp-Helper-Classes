var FX : Transform[];

function OnGUI () {
	for(i = 0; i < FX.length; i ++){
		if(GUI.Button(Rect(120*i,0,120,80), FX[i].gameObject.name)){
			gameObject.GetComponent("ExplosionAtPoint").explosionPrefab = FX[i];
		}
	}
	Time.timeScale = GUI.HorizontalSlider(Rect(0, 130, Screen.width, 10), Time.timeScale, 0, 15);
}
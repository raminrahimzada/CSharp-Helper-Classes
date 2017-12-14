/*
 FPS Constructor - Weapons
 Copyright© Dastardly Banana Productions 2011-2012
 This script, and all others contained within the Dastardly Banana Weapons Package are licensed under the terms of the
 Unity Asset Store End User License Agreement at http://download.unity3d.com/assetstore/customer-eula.pdf 
 
  For additional information contact us info@dastardlybanana.com.
*/
/*
	private var sensitivityX : float = 15F;
	private var sensitivityY : float = 15F;
	var sensitivityStandardX : float = 15F;
	var sensitivityStandardY : float = 15F;
	var sensitivityAimingX : float = 15F;
	var sensitivityAimingY : float = 15F;
	var sensitivityZ : float = 0.7F;
	var retSensitivity : float = -.5F;

	@HideInInspector
	var minimumX : float = 5F;
	@HideInInspector
	var maximumX : float = 3F;
	
	var xRange : float = 5F;
	var xRangeAim : float = 3F;
	

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
*/
@CustomEditor (GunLook)
class GunLookEditor extends Editor {
	function OnInspectorGUI(){
		EditorGUIUtility.LookLikeInspector();	
		EditorGUILayout.Separator();	
		
		EditorGUILayout.BeginVertical("toolbar");
		target.lookMotionOpen = EditorGUILayout.Foldout(target.lookMotionOpen, "Look Motion");
		EditorGUILayout.EndVertical();
		
		if(target.lookMotionOpen){
			EditorGUILayout.BeginVertical("textField");
			
				target.useLookMotion = EditorGUILayout.Toggle("Use Look Motion", target.useLookMotion);
			
				if(target.useLookMotion){
					EditorGUILayout.Separator();
					EditorGUILayout.BeginVertical("toolbar");
						EditorGUILayout.LabelField("Standard");
					EditorGUILayout.EndVertical();
					
					target.sensitivityStandardX = EditorGUILayout.FloatField(GUIContent("X Sensitivity","Sensitivity for x look movement"), target.sensitivityStandardX);
					target.sensitivityStandardY = EditorGUILayout.FloatField(GUIContent("Y Sensitivity","Sensitivity for y look movement"), target.sensitivityStandardY);
					target.sensitivityStandardZ = EditorGUILayout.FloatField(GUIContent("Z Sensitivity","Sensitivity for z look movement"), target.sensitivityStandardZ);
					target.retSensitivity = EditorGUILayout.FloatField(GUIContent("Return Sensitivity","Speed at which weapon returns to standard position"), target.retSensitivity);
					
					EditorGUILayout.Separator();
					
					target.xRange = EditorGUILayout.FloatField(GUIContent("X Range","Range of motion in degrees for x motion"), target.xRange);
					target.yRange = EditorGUILayout.FloatField(GUIContent("Y Range","Range of motion in degrees for y motion"), target.yRange);
					target.zRange = EditorGUILayout.FloatField(GUIContent("Z Range","Range of motion in degrees for z motion"), target.zRange);
	
					EditorGUILayout.Separator();
					
					EditorGUILayout.BeginVertical("toolbar");
						EditorGUILayout.LabelField("Aim");
					EditorGUILayout.EndVertical();
					
					target.sensitivityAimingX = EditorGUILayout.FloatField(GUIContent("X Sensitivity","Sensitivity for x look movement when aiming"), target.sensitivityAimingX);
					target.sensitivityAimingY = EditorGUILayout.FloatField(GUIContent("Y Sensitivity","Sensitivity for y look movement when aiming"), target.sensitivityAimingY);
					target.sensitivityAimingZ = EditorGUILayout.FloatField(GUIContent("Z Sensitivity","Sensitivity for z look movement when aiming"), target.sensitivityAimingZ);
					
					EditorGUILayout.Separator();
					
					target.xRangeAim = EditorGUILayout.FloatField(GUIContent("X Range","Range of motion in degrees for x motion when aiming"), target.xRangeAim);
					target.yRangeAim = EditorGUILayout.FloatField(GUIContent("Y Range","Range of motion in degrees for y motion when aiming"), target.yRangeAim);
					target.zRangeAim = EditorGUILayout.FloatField(GUIContent("Z Range","Range of motion in degrees for z motion when aiming"), target.zRangeAim);
				}
				
			EditorGUILayout.EndVertical();
		}
		
		EditorGUILayout.BeginVertical("toolbar");
		target.walkMotionOpen = EditorGUILayout.Foldout(target.walkMotionOpen, "Walk Motion");
		EditorGUILayout.EndVertical();
		
		if(target.walkMotionOpen){
			EditorGUILayout.BeginVertical("textField");
			
				target.useWalkMotion = EditorGUILayout.Toggle("Use Walk Motion", target.useWalkMotion);
			
				if(target.useWalkMotion){
					/*EditorGUILayout.Separator();
					EditorGUILayout.BeginVertical("toolbar");
						EditorGUILayout.LabelField("Rotation");
					EditorGUILayout.EndVertical();
					
					target.zMoveRange = EditorGUILayout.FloatField(GUIContent("X Range","Range of angle in degrees through which weapon will move for z motion"), target.zMoveRange);
					target.zMoveSensitivity = EditorGUILayout.FloatField(GUIContent("X Sensitivity","Determines how much the weapons move based on z movement"), target.zMoveSensitivity);
					target.zMoveAdjustSpeed = EditorGUILayout.FloatField(GUIContent("X Speed","Determines how quickly the weapons move"), target.zMoveAdjustSpeed);
					EditorGUILayout.Separator();	
					
					target.xMoveRange = EditorGUILayout.FloatField(GUIContent("Z Range","Range of angle in degrees through which weapon will move for x motion"), target.xMoveRange);
					target.xMoveSensitivity = EditorGUILayout.FloatField(GUIContent("Z Sensitivity","Determines how much the weapons move based on x movement"), target.xMoveSensitivity);
					target.xMoveAdjustSpeed = EditorGUILayout.FloatField(GUIContent("Z Speed","Determines how quickly the weapons move"), target.xMoveAdjustSpeed);
					EditorGUILayout.Separator();	
					
					target.xAirMoveRange = EditorGUILayout.FloatField(GUIContent("Z Air Range","Range of angle in degrees through which weapon will move when airborne"), target.xAirMoveRange);
					target.xAirMoveSensitivity = EditorGUILayout.FloatField(GUIContent("Z Air Sensitivity","Determines how much the weapons move whe airborne"), target.xAirMoveSensitivity);
					target.xAirAdjustSpeed = EditorGUILayout.FloatField(GUIContent("Z Air Speed","Determines how quickly the weapons move"), target.xAirAdjustSpeed);
					EditorGUILayout.Separator();
					*/
					EditorGUILayout.BeginVertical("toolbar");
						EditorGUILayout.LabelField("Position");
					EditorGUILayout.EndVertical();
					
					target.zPosMoveRange = EditorGUILayout.FloatField(GUIContent("Z Range","Range of distance through which weapon will move for z motion"), target.zPosMoveRange);
					target.zPosMoveSensitivity = EditorGUILayout.FloatField(GUIContent("Z Sensitivity","Determines how much the weapons move based on z movement"), target.zPosMoveSensitivity);
					target.zPosAdjustSpeed = EditorGUILayout.FloatField(GUIContent("Z Speed","Determines how quickly the weapons move"), target.zPosAdjustSpeed);
					EditorGUILayout.Separator();
					
					target.xPosMoveRange = EditorGUILayout.FloatField(GUIContent("X Range","Range of distance through which weapon will move for x motion"), target.xPosMoveRange);
					target.xPosMoveSensitivity = EditorGUILayout.FloatField(GUIContent("X Sensitivity","Determines how much the weapons move based on z movement"), target.xPosMoveSensitivity);
					target.xPosAdjustSpeed = EditorGUILayout.FloatField(GUIContent("X Speed","Determines how quickly the weapons move"), target.xPosAdjustSpeed);
				}				
				
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.Separator();
		
	}
}
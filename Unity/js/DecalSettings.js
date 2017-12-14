
var FriendlyDecalImage : Texture2D;
var EnemyDecalImage : Texture2D;
var NeutralDecalImage : Texture2D;
var SelfDecalImage : Texture2D;

function SetDecalImage( imageType : String ) {
	var MyDecal : Projector = GetComponent( Projector );
	var SelectedDecal : Texture2D;
	switch( imageType ) {
	case "friend" :
		SelectedDecal = FriendlyDecalImage;
		break;
	case "enemy" :
		SelectedDecal = EnemyDecalImage;
		break;
	case "neutral" :
		SelectedDecal = NeutralDecalImage;
		break;
	case "self" :
		SelectedDecal = SelfDecalImage;
		break;
	}
	MyDecal.material.SetTexture("_MainTex", SelectedDecal);
}
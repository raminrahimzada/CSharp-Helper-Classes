 #pragma strict
var upgrade : Upgrade;
static var WeaponArray : WeaponInfo[];
private var Upgrades : Upgrade[];
private var applied : boolean = false;
var classesAllowed : boolean[];

function Start () {
	if(WeaponArray == null)
		WeaponArray = FindObjectsOfType(WeaponInfo) as WeaponInfo[];
}

function Apply () {
	applied = true;
	var temp : Transform;
	var up : Upgrade;
	var upgradeArray = new Array();
	for(var i : int = 0; i < WeaponArray.length; i++){
		var enumIndex : int = WeaponArray[i].weaponClass;
		
		if(classesAllowed[enumIndex]){
			temp = Instantiate(upgrade.gameObject, transform.position, transform.rotation).transform;
			temp.parent = WeaponArray[i].transform;
			temp.name = upgrade.upgradeName;
			up = temp.GetComponent(Upgrade);
			up.Init();
			up.ApplyUpgrade();
			up.showInStore = false;
			upgradeArray.Push(up);
		}
	}
	Upgrades = upgradeArray.ToBuiltin(Upgrade) as Upgrade[];
	
	this.SendMessage("Apply");
}

function UnApply () {
	applied = false;
	this.SendMessage("Remove");
	for(var i : int = 0; i < Upgrades.length; i++){
		Upgrades[i].DeleteUpgrade();
	}
}
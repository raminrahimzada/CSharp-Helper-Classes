//FPS Constructor - Weapons
//Copyright© Dastardly Banana Productions 2010
//This script is distributed exclusively through ActiveDen and it's use is restricted to the terms of the ActiveDen 
//licensing agreement.
//
// Questions should be addressed to info@dastardlybanana.com
//
class WeaponClassArrayType{
	var weaponClass : String;
	var WeaponInfoArray : WeaponInfo[];
}

static var storeActive : boolean = false;
static var canActivate : boolean = true;
static var singleton : DBStoreController;

 var balance: float; // Store account balance 
 
 //var scrollPosition : Vector2;
@HideInInspector var WeaponInfoArray : WeaponInfo[] ;
@HideInInspector var WeaponInfoByClass :WeaponClassArrayType[];
@HideInInspector var weaponClassNames : String[];
@HideInInspector var weaponClassNamesPopulated : String [];
@HideInInspector var playerW : PlayerWeapons;
@HideInInspector var nullWeapon : GameObject; //there must be one null weapon as a placeholder to put in an empty slot.
@HideInInspector var slotInfo: SlotInfo;
var canExitWhileEmpty : boolean = false;
static var inStore : boolean = false;
 
function Initialize() {
	singleton = this;
	playerW = FindObjectOfType(PlayerWeapons) as PlayerWeapons;
	slotInfo = FindObjectOfType(SlotInfo) as SlotInfo;		
	WeaponInfoArray = FindObjectsOfType(WeaponInfo) as WeaponInfo[];
	for(var w : WeaponInfo in WeaponInfoArray) {
		if(w.weaponClass == weaponClasses.Null) 
			nullWeapon = w.gameObject;
	}
	setupWeaponClassNames();
	setupWeaponInfoByClass();
}


function getNumOwned(slot: int) {
	//will use the slot info later to restrict count
	var n : int = 0;
	for (var i: int = 0; i < WeaponInfoArray.length; i++) {
		if(WeaponInfoArray[i].owned && slotInfo.isWeaponAllowed(slot,WeaponInfoArray[i]))
			n++;
	}
	return n;
}

function getWeaponNamesOwned(slot : int) : String[] {
	var names : String[] = new String[getNumOwned(slot)];
	var n : int = 0;
	for (var i: int = 0; i <  WeaponInfoArray.length; i++) {
		if(WeaponInfoArray[i].owned && slotInfo.isWeaponAllowed(slot,WeaponInfoArray[i])){
			names[n] = WeaponInfoArray[i].gunName;
			n++;
		}
	}
	return names;
}

function getWeaponsOwned(slot : int) : WeaponInfo[] {
	var w : WeaponInfo[] = new WeaponInfo[getNumOwned(slot)];
	var n : int = 0;
	for (var i: int = 0; i <  WeaponInfoArray.length; i++) {
		if(WeaponInfoArray[i].owned && slotInfo.isWeaponAllowed(slot,WeaponInfoArray[i])){
			w[n] = WeaponInfoArray[i];
			n++;
		}
	}
	return w;
}



function Update() {
	if(InputDB.GetButtonDown("Store")){
		if(!storeActive && canActivate && !GunScript.takingOut && !GunScript.puttingAway){
			activateStore();
		}else if(storeActive) {
			deActivateStore();
		}
	}
}

function setupWeaponClassNames() {
	var names : String[];
	var nameArray = new Array();
	
	for (var w : weaponClasses in weaponClasses.GetValues(weaponClasses) ) {
		nameArray.push(w.ToString().Replace("_", " " ));
	}
	weaponClassNames = nameArray.ToBuiltin(String);
}

//Organize Weapon Information by Weapon Class for use within the GUI
//Note: the code assumes the last weapon class is "Null" so the array is one element shorter than the number of
//		weapon classes.

function setupWeaponInfoByClass() {

	//check to see how many Weapon Classes have one or more weapons

	var n : int = 0;
	for (var i : int = 0; i< weaponClassNames.length - 1; i++) {
		for (var j : int = 0; j < WeaponInfoArray.length; j++) {
			if(WeaponInfoArray[j].weaponClass == i) {
				n++;
				break;
			}
		}
	}
	weaponClassNamesPopulated = new String[n];
	WeaponInfoByClass = new WeaponClassArrayType[n]; // size array to hold all non-Null weapon classes with at least one weapon

		n = 0;
		for(i =0; i < weaponClassNames.length - 1; i++) {
		var arr = new Array();	
			for (j = 0 ; j<WeaponInfoArray.Length; j++) {
				if(WeaponInfoArray[j].weaponClass == i) {
					arr.push(WeaponInfoArray[j]);
				}
			}
			if(arr.length > 0) {
				WeaponInfoByClass[n] = new WeaponClassArrayType();
				WeaponInfoByClass[n].WeaponInfoArray = arr.ToBuiltin(WeaponInfo);
				WeaponInfoByClass[n].weaponClass = weaponClassNames[i];
				weaponClassNamesPopulated[n] = weaponClassNames[i];
				n++;
			}
	}
}

function activateStore() {
	PlayerWeapons.HideWeaponInstant();
	Time.timeScale = 0;
//	GUIController.state = GUIStates.store;
	inStore = true;
	//BroadcastMessage("Unlock");
	storeActive = true;
	Screen.lockCursor = false;
	LockCursor.canLock = false;
	//playerW.BroadcastMessage("DeselectWeapon"); //turn off graphics/weapons on entering store
	var player = GameObject.FindWithTag("Player");
	player.BroadcastMessage("Freeze", SendMessageOptions.DontRequireReceiver);
}

function deActivateStore() {
	if(PlayerWeapons.HasEquipped() <= 0 && !canExitWhileEmpty)
		return;
	storeActive = false;
	inStore = false;
	//GUIController.state = GUIStates.playing;
	//BroadcastMessage("Lock");
	Time.timeScale = 1;
	Screen.lockCursor=true;
	LockCursor.canLock = true;
	PlayerWeapons.ShowWeapon();
	//playerW.SelectWeapon(playerW.selectedWeapon); // activate graphics on selected weapon
	var player = GameObject.FindWithTag("Player");
	player.BroadcastMessage("UnFreeze", SendMessageOptions.DontRequireReceiver);
	
}

function equipWeapon(g : WeaponInfo, slot: int) {
	//if the weapon is equipped in another slot, unequip it
	if(slot < 0)
		return;
	for(var i : int = 0; i < playerW.weapons.length; i++) {
		if(g.gameObject == playerW.weapons[i]) {
			unEquipWeapon(g, i);
		}
	}
	if(playerW.weapons[playerW.selectedWeapon] == null)
		playerW.selectedWeapon = slot;
	playerW.BroadcastMessage("DeselectWeapon", SendMessageOptions.DontRequireReceiver);
	var tempGScript : GunScript = g.gameObject.GetComponent(GunScript).GetPrimaryGunScript();
	tempGScript.ammoLeft = tempGScript.ammoPerClip;
	playerW.SetWeapon(g.gameObject,slot);
}
function unEquipWeapon(g : WeaponInfo, slot: int) {
	playerW.weapons[slot] = null;
}

function buyUpgrade(g : WeaponInfo, u : Upgrade) {
	withdraw(u.buyPrice);
	u.owned = true;
	g.ApplyUpgrade();
}

function buyWeapon(g : WeaponInfo) {
	withdraw(g.buyPrice);
	g.owned = true;
	g.ApplyUpgrade();
	equipWeapon(g, autoEquipWeapon(g, false));
}

function BuyAmmo(g : WeaponInfo) {
	withdraw(g.ammoPrice);
	var temp : GunScript = g.gameObject.GetComponent(GunScript).GetPrimaryGunScript();
	temp.clips = Mathf.Min(temp.clips+temp.ammoPerClip, temp.maxClips);
	temp.ApplyToSharedAmmo();
}

function sellWeapon(g: WeaponInfo) {
	if(!g.canBeSold)
		return;
	for(var i : int = 0; i < g.upgrades.length ; i++ ) {
		g.upgrades[i].owned = false;
		g.upgrades[i].RemoveUpgrade();	
	}
	DropWeapon(g);
	deposit(g.sellPriceUpgraded);
	g.owned = false;
}

function getBalance(): float {
	return balance;
}

//Function to deposit money - returns the new balance
function deposit(amt : float) : float {
		balance += amt;
		return balance;
}

// Function to withdraw money  - returns amount withdrawn 
// You can't withdraw more than the balance.
function withdraw(amt : float) : float {
	if (amt <= balance) {
		balance -= amt;
		return amt;
	} else {
		var oldBalance: float = balance;
		balance = 0;
		return oldBalance;
	}
		
}

function autoEquipWeapon(w : WeaponInfo, auto : boolean) : int {
	//Slot the weapon is equipped in, -1 if not equipped
	var slot : int = -1; 
	
	//find the first empty slot that can hold w
	for (var i: int = 0; i < playerW.weapons.length; i ++) {
		if(slotInfo.isWeaponAllowed(i,w) && playerW.weapons[i] ==null) {
			//equipWeapon(w,i);
			slot = i;
		}
		if(slot >= 0) 
			return slot;
	}
	if(!auto)
		return slot;
		
	if(slotInfo.isWeaponAllowed(playerW.selectedWeapon,w)) {
		//equipWeapon(w,i);
		slot = playerW.selectedWeapon;
		return slot;
	}
		
	for ( i = 0; i < playerW.weapons.length; i++) {
		if(slotInfo.isWeaponAllowed(i,w)) {
			//equipWeapon(w,i);
			slot = i;
		}
		if(slot >= 0)
			return slot;	
	}
	return slot;
}

function autoEquipWeaponWithReplacement(w : WeaponInfo, auto : boolean) : int {
	//Slot the weapon is equipped in, -1 if not equipped
	var slot : int = -1; 
	
	//See if the weapon is already equipped and can be replaced
	for (var i: int = 0; i < PlayerWeapons.PW.weapons.length; i ++) {
		if(slotInfo.isWeaponAllowed(i,w) && (playerW.weapons[i] == null || (playerW.weapons[i].gameObject == w.gameObject))) {
			//equipWeapon(w,i);
			slot = i;
		}
		if(slot >= 0) 
			return slot;
	}
	
	//find the first empty slot that can hold w
	for (i = 0; i < playerW.weapons.length; i ++) {
		if(slotInfo.isWeaponAllowed(i,w) && PlayerWeapons.PW.weapons[i] ==null) {
			//equipWeapon(w,i);
			slot = i;
		}
		if(slot >= 0) 
			return slot;
	}
	if(!auto)
		return slot;
		
	if(slotInfo.isWeaponAllowed(PlayerWeapons.PW.selectedWeapon,w)) {
		//equipWeapon(w,i);
		slot = playerW.selectedWeapon;
		return slot;
	}
	for ( i = 0; i < PlayerWeapons.PW.weapons.length; i++) {
		if(slotInfo.isWeaponAllowed(i,w)) {
			//equipWeapon(w,i);
			slot = i;
		}
		if(slot >= 0)
			return slot;	
	}
	return slot;
}

//Drops weapon at given index in Weapons[]
function DropWeapon (g: WeaponInfo){
	//Weapon Drop
	var wep : int = -1;
	for(var i : int = 0; i<playerW.weapons.length;i++){
		if(playerW.weapons[i].gameObject == g.gameObject){
			wep = i;
			break;
		}
	}
	if(wep < 0) return;
	
	playerW.weapons[wep] = null;
	
	//Ceck if we have a weapon to switch to
	var prevWeapon : int = -1;
	for(i = wep-1; i >= 0; i--){
		if(playerW.weapons[i] != null){
			prevWeapon = i;
			break;
		}
	}
	
	var nextWeapon : int = -1;
	if(prevWeapon == -1){
		for(i = wep+1; i < playerW.weapons.length; i++){
			if(playerW.weapons[i] != null){
				nextWeapon = i;
				break;
			}
		}
		prevWeapon = nextWeapon;
		
		if(nextWeapon == -1)
			return;
	}
		
	playerW.selectedWeapon = prevWeapon;
	playerW.SelectWeapon(prevWeapon);
}
// Class to hold common GUI Panel Configuration Information
class DBStorePanel {
	var x : float;
	var y : float;
	var width : float;
	var height : float;
	var r : Rect;
	var buttonWidth : float;
	var buttonHeight : float;
	var navButtonWidth : float;
	var navButtonHeight : float;
	var nextIcon: Texture;
	var prevIcon: Texture;
	var checkedIcon : Texture;
	var lockedIcon : Texture;
	var titleHeight : float;
	var xPad : float;
	var yPad : float;
	var title : String;
	var titleIcon : Texture;
	var content : String[];
	var wi : WeaponInfo[];
	var contentIcon : Texture[];
	var columns : int;
	var rows : int;
	var buttonsPerPage : int;
	var page: int = 1;
	var nPages : int = 1;
	var selection: int;
	var store: DBStoreController;
}

// Store State Variables
enum HeaderModes {Buy = 0,Equip = 1};

var skin : GUISkin;
var checkedIcon : Texture;
var lockedIcon: Texture;

private var wcIcons : WeaponClassIcons;
private var slotIcons : Texture[] = new Texture[10];
private var store : DBStoreController;

//Position of the upper left corner of the store, if negative the store will be centered
var sPosX = -1;
var sPosY = -1;

//configuration parameters fpr the Header (H) GUI Component
var HWidth = 400;
var HHeight = 100;
var HYpad = 10;
var HButtonWidth = 120;
var HButtonHeight = 50;
var HTitleHeight = 40;
var HTitle = "WEAPONS DEPOT";
var HTitleImage : Texture;


//Configuration for Left Selection (LS) GUI Component
var LSHeight : float = 300;
var LSWidth : float = 140;
var LSButtonHeight : float = 50.0;
var LSButtonWidth: float = 120;
var LSxPad : float  = 5;
var LSyPad : float  = 5;
var LSNextIcon : Texture;
var LSPrevIcon : Texture;

// Configuration for Main Display (MD) GUI Component
var MDButtonWidth : float = 120;
var MDButtonHeight : float = 50.0; 
var MDxPad : float = 5;
var MDyPad : float = 5;

private var header :DBStorePanel = new DBStorePanel();
private var lsBuy : DBStorePanel = new DBStorePanel();	// Buy Weapons Left Selection GUI (LS) Component
private var lsEquip : DBStorePanel = new DBStorePanel();	// Equip Slots Left Selection (LS) GUI Componenent
private var mdBuy : DBStorePanel = new DBStorePanel();		// Buy Weapons Main Display (MD)GUI Component
private var mdEquip : DBStorePanel = new DBStorePanel(); // Equip Slot Main Display (MD) GUI Componenet

private var popupRect : Rect;
private var MDContent : String[]; 
private var MDSelection : int;

private var clicked : int; // temp variable used to track selections
private var viewUpgrades : boolean = true;

//variables used for the Weapon Info/Buy/Upgrade Popup Window
var popupUpgradeWidth : float = 500;
var popupUpgradeHeight : float = 275;
private var popupUpgradeRect : Rect;	 //Expanded Rectangle for the popup Buy/Upgrade Window
private var popupActive : boolean = false;
private var popupBuyActive: boolean = false;
private var popupBuyScroll1 : Vector2;
private var upgradeSelected : boolean[] = new boolean[20]; //allows a maximum of 20 upgrades per weapon

private var popupLockedRect : Rect;
private var popupLockedActive : boolean = false;

var maskTexture: Texture; //Texture that's drawn over the scene when the store is active
var maskTexturePopup : Texture; //Texture that's drawn behind the Buy/Sell/Upgrade window
private var upgradeDisplayBuy : boolean = true;
private var upgradeDisplayEquip : boolean = false;
var upgradeBuyIcon : Texture;
var upgradeInfoIcon : Texture;

function Start() {
	store = FindObjectOfType(DBStoreController);
	store.Initialize();
	if(sPosX < 0  || sPosY < 0) { // center the store
		sPosX = Screen.width/2 - (HWidth + LSWidth)/2;
		sPosY = Screen.height/2 - (HHeight + LSHeight)/2;
	}
	popupUpgradeRect = new Rect(sPosX + 30,sPosY + 60, popupUpgradeWidth, popupUpgradeHeight);	
	popupLockedRect = new Rect(sPosX + LSWidth + 50, sPosY + HHeight + 50, 200,200);
	wcIcons = FindObjectOfType(WeaponClassIcons);
	setupLS(lsBuy);
	setuplsBuyContent();
	setupLS(lsEquip);
	setuplsEquipContent();
	setupHeader();
	setupMD(mdBuy);
	setupMD(mdEquip);	
}

function OnGUI() {
	if(!DBStoreController.inStore)
		return;
	//GUI.skin = skin;
	if(DBStoreController.inStore) {
		if(maskTexture)
			GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), maskTexture);
		
		clicked = DisplayHeader(header); //Draw Header Area
		if(!popupActive && clicked != -1) {
			header.selection = clicked; // If Popup is Active Don't respond to clicks
			setmdBuyContent(lsBuy.selection);
			setmdEquipContent(lsEquip.selection);			
		}
		if(header.selection == HeaderModes.Buy) {
			clicked = DisplayLS(lsBuy); // Draw Left Selection Area
			if(clicked != -1 && !popupActive) { 
				setmdBuyContent(lsBuy.selection);
			}
			clicked = DisplayMD(mdBuy,lsBuy.selection, header.selection);
			if(clicked != -1 && mdBuy.wi[clicked].locked ){
				mdBuy.selection = clicked;
				popupLockedActive = true;
				popupActive = true;
			} else if(clicked != -1 && !popupActive) {
				mdBuy.selection = clicked;
				popupActive = true;
				popupBuyActive = true;
			}			
		} else if (header.selection == HeaderModes.Equip) {
			clicked = DisplayLS(lsEquip);
			if(clicked != -1 && !popupActive) {
				mdEquip.selection = clicked; 
				setmdEquipContent(lsEquip.selection);
			}
			clicked = DisplayMD(mdEquip, lsEquip.selection, header.selection);
			if(clicked != -1 && !popupActive) {
				mdEquip.selection = clicked;
				popupActive = true;
				popupBuyActive = true;
			}
		}
		if(popupBuyActive) {
			if(maskTexture)
				GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), maskTexturePopup);
				popupRect = popupUpgradeRect; 				 
			if(maskTexture)
				GUI.DrawTexture(popupRect, maskTexture);
			GUI.Window(1, popupUpgradeRect, WeaponBuyWindow,"Weapon Info");
		} else if(popupLockedActive) {
			popupRect = popupLockedRect;
			if(maskTexturePopup){
				GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), maskTexturePopup);
				GUI.DrawTexture(popupRect, maskTexture);
			}
		 	GUI.Window(0, popupRect, WeaponLockedWindow,"Sorry!!!");
		 }
	}
}

function setupMD(md : DBStorePanel) {
	md.selection = -1;
	md.buttonHeight = MDButtonHeight;
	md.buttonWidth = MDButtonWidth;
	md.xPad = MDxPad;
	md.yPad = MDyPad;
	md.checkedIcon = checkedIcon;
	md.lockedIcon = lockedIcon;
	md.r = new Rect(sPosX + LSWidth,sPosY + HHeight,HWidth, LSHeight);
	md.columns = Mathf.Floor((md.r.width - md.xPad)/(md.buttonWidth + md.xPad));
	md.rows = Mathf.Floor((md.r.height-md.titleHeight - md.yPad)/(md.buttonHeight - md.yPad));
	md.content = null;
	md.store = store;
}

//Set the content for the Buy MD panel based on weapon class selected
function setmdBuyContent(sel : int){
	mdBuy.content = new String[store.WeaponInfoByClass[sel].WeaponInfoArray.Length];
	mdBuy.wi = new WeaponInfo[store.WeaponInfoByClass[sel].WeaponInfoArray.Length];
	for(var i : int = 0; i< mdBuy.content.length; i++) {
		mdBuy.content[i] = store.WeaponInfoByClass[sel].WeaponInfoArray[i].gunName;
		mdBuy.wi[i] = store.WeaponInfoByClass[sel].WeaponInfoArray[i];
	}
}

//set the content for the Equip MD panel based on the slot selected
//for now it's the same for all slots but we'll be adding the ability to restrict slots to different types of weapons

function setmdEquipContent(slot: int) {
	mdEquip.content = store.getWeaponNamesOwned(slot);
	mdEquip.wi = store.getWeaponsOwned(slot);
}

function setupLS(ls : DBStorePanel) {
	ls.selection = 0;
	ls.buttonHeight = LSButtonHeight;
	ls.buttonWidth = LSButtonWidth;
	ls.xPad = LSxPad;
	ls.yPad = LSyPad;	
	ls.navButtonWidth = ls.buttonWidth/2.0;
	ls.navButtonHeight = ls.buttonHeight/2.0;
	ls.nextIcon = LSNextIcon;
	ls.prevIcon = LSPrevIcon;
	ls.r = new Rect(sPosX, sPosY + HHeight, LSWidth,LSHeight);
	ls.buttonsPerPage = Mathf.Floor((ls.r.height - ls.titleHeight - ls.navButtonHeight - 2*ls.yPad)/(ls.buttonHeight + ls.yPad));
	ls.store = store;
}

function setuplsBuyContent() {
	lsBuy.title = "Weapon Classes";
	lsBuy.content = new String[store.WeaponInfoByClass.length];
	lsBuy.contentIcon = new Texture[store.WeaponInfoByClass.length];
	
	System.Array.Copy(store.weaponClassNamesPopulated,lsBuy.content,store.WeaponInfoByClass.length);
	for(var i: int = 0; i < lsBuy.content.Length; i++) {
		lsBuy.content[i] = lsBuy.content[i] + " (" + store.WeaponInfoByClass[i].WeaponInfoArray.Length + ")";
		var ic : int = store.WeaponInfoByClass[i].WeaponInfoArray[0].weaponClass;
		lsBuy.contentIcon[i] = wcIcons.weaponClassTextures[ic];
	}
	
	lsBuy.nPages = Mathf.Ceil(parseFloat(lsBuy.content.length)/parseFloat(lsBuy.buttonsPerPage));
}

function setuplsEquipContent() {
	lsEquip.title = "Weapon Slots";
	lsEquip.content = new String[store.playerW.weapons.length];
	lsEquip.contentIcon = new Texture[10];
	for (var i : int = 0; i < lsEquip.content.length; i++) {
		lsEquip.content[i] = store.slotInfo.slotName[i];
		lsEquip.contentIcon[i] = slotIcons[i];
	}
	lsEquip.nPages = Mathf.Ceil(parseFloat(lsEquip.content.length)/parseFloat(lsEquip.buttonsPerPage));
}

function setupHeader() {
	header.r = new Rect(sPosX + LSWidth, sPosY, HWidth, HHeight);
	header.yPad = HYpad;
	header.content = ["Buy", "Equip"];
	header.title = HTitle;
	header.titleHeight = HTitleHeight;
	header.buttonHeight = HButtonHeight;
	header.buttonWidth = HButtonWidth;
	header.checkedIcon = checkedIcon;
	header.lockedIcon = lockedIcon;
	header.store = store;
	header.titleIcon = HTitleImage;
}

//Function to display the Header Panel for the store

static function DisplayHeader(cfg : DBStorePanel) {
	var clicked : int = -1;
	var rect : Rect;
	var gc : GUIContent;
	GUILayout.BeginArea(cfg.r);
		GUI.Box(Rect(0,0,cfg.r.width,cfg.r.height),"");
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				gc = setGUIContent(cfg.titleIcon,cfg.title,"");		
				GUILayout.Label(gc,GUILayout.Height(cfg.titleHeight), GUILayout.Width(cfg.titleHeight/cfg.titleIcon.height * cfg.titleIcon.width));
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				for(var i: int = 0; i< cfg.content.length; i ++) {
					if(GUILayout.Button(cfg.content[i],GUILayout.Width(cfg.buttonWidth),GUILayout.Height(cfg.buttonHeight)))
						clicked = i;
				}
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
		GUILayout.EndVertical();	
		GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label("Balance: $" + cfg.store.getBalance());
				GUILayout.Space(5);
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			var msg : String;
			if(cfg.selection == HeaderModes.Buy)
				msg = "Owned";
			if(cfg.selection == HeaderModes.Equip)
				msg = "Equipped";
			GUILayout.BeginHorizontal();
				GUILayout.Label(cfg.checkedIcon, GUILayout.Width(40), GUILayout.Height(20));
				var r : Rect = GUILayoutUtility.GetLastRect();
				r.x += 25;
				r.width +=15;
				GUI.Label(r,msg);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				GUILayout.Label(cfg.lockedIcon, GUILayout.Width(40), GUILayout.Height(20));
				r = GUILayoutUtility.GetLastRect();
				r.x +=25;
				r.width +=10;
				GUI.Label(r, "Locked");
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	GUILayout.EndArea();
	return clicked;
}

static function DisplayLS(cfg : DBStorePanel) {
		// function to display Left Selection Box
		
		var clicked : int = -1; //local variable to keep track of selections
		var startInt : int = ((cfg.page-1)*cfg.buttonsPerPage);
		var endInt : int = Mathf.Min(startInt+cfg.buttonsPerPage, cfg.content.length);

			GUILayout.BeginArea(cfg.r);
			GUI.Box(Rect(0,0,cfg.r.width,cfg.r.height),"");
				GUILayout.BeginVertical();
					GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label(cfg.title, GUILayout.Height(cfg.titleHeight));
						GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					for(var i: int = startInt; i< endInt; i++) {
						GUILayout.BeginHorizontal();
							GUILayout.FlexibleSpace();
							if(cfg.contentIcon[i]) {
								if(GUILayout.Button(cfg.contentIcon[i],GUILayout.Width(cfg.buttonWidth),GUILayout.Height(cfg.buttonHeight))) {
									clicked = i;
									cfg.selection = i;
								}								
							 }
							 else {	
								if(GUILayout.Button(cfg.content[i],GUILayout.Width(cfg.buttonWidth),GUILayout.Height(cfg.buttonHeight))) {
									clicked = i;
									cfg.selection = i;
								}
							}
							GUILayout.FlexibleSpace();
						GUILayout.EndHorizontal();
					}
				GUILayout.EndVertical();
				
				var bx: float = cfg.r.width -cfg.navButtonWidth -cfg.xPad;
				var by : float = cfg.r.height - cfg.navButtonHeight - cfg.yPad;
				if(cfg.page < cfg.nPages){
					if(cfg.nextIcon) {
						if(GUI.Button(Rect(bx,by,cfg.navButtonWidth,cfg.navButtonHeight), cfg.nextIcon)){
							cfg.page++;
						}
					} else {
						if(GUI.Button(Rect(bx,by,cfg.navButtonWidth,cfg.navButtonHeight), "next")){
							cfg.page++;
						}
					}
				}
				if(cfg.page > 1){
					bx = cfg.xPad/2.0;
					if(cfg.prevIcon) {
						if(GUI.Button(Rect(bx,by,cfg.navButtonWidth,cfg.navButtonHeight), cfg.prevIcon)){
							cfg.page--;
						}
					} else {
						if(GUI.Button(Rect(bx,by,cfg.navButtonWidth,cfg.navButtonHeight), "Prev")){
							cfg.page--;
						}
					}
			}	
		GUILayout.EndArea();


		return clicked;
}

static function DisplayMD(cfg : DBStorePanel, sel : int, mode : HeaderModes ) {
	var clicked = -1;
	var msg : String; //used to hold temporary string messages
	var gc : GUIContent; 
	GUILayout.BeginArea(cfg.r);
	GUI.Box(new Rect(0,0,cfg.r.width,cfg.r.height),""); 
		if(cfg.content == null){
			GUILayout.EndArea();
			 return clicked;
		}
		GUILayout.BeginVertical();
			GUILayout.Space(cfg.yPad);
			if(cfg.content.length == 0) {
				DrawLabelHCenter("Slot: " + cfg.store.slotInfo.slotName[sel]);		
				GUILayout.Space(20);
					DrawLabelHCenter("You Don't Own any Weapons For This Slot");
				GUILayout.EndVertical();
				GUILayout.EndArea();
				return clicked;
			} else {
				if(mode == HeaderModes.Equip)
					DrawLabelHCenter("Slot: " + cfg.store.slotInfo.slotName[sel]);		
			}
			var count : int = 0;
			var cl : int = -1;
			for(var i : int = 0; i< cfg.rows; i++) {
				GUILayout.BeginHorizontal();
					GUILayout.Space(cfg.xPad);
						for(var j: int = 0; j< cfg.columns; j++) {	
							if(count >= cfg.content.length) break;
							if(HeaderModes.Buy && cfg.wi[count].owned){
								msg = cfg.content[count];
							} else{
								msg = cfg.content[count] + "\n$" + cfg.wi[count].buyPrice;
							}
							gc = setGUIContent(cfg.wi[count].icon, msg,"");
							if(GUILayout.Button(gc ,GUILayout.Width(cfg.buttonWidth),GUILayout.Height(cfg.buttonHeight))){
									clicked = count;								
							}
 							// now draw the overlay icons if the weapon is owned or locked when in buy mode
							// and equipped in slot in equip mode
							var r: Rect = GUILayoutUtility.GetLastRect();
							if(mode == HeaderModes.Equip) {
								if((cfg.store.playerW.weapons[sel] == cfg.wi[count].gameObject) && cfg.checkedIcon){ 
									GUI.Label(r,cfg.checkedIcon);
								}
							} else {
								if(cfg.wi[count].owned && cfg.checkedIcon) {
									GUI.Label(r,cfg.checkedIcon);
								}
								if(cfg.wi[count].locked && cfg.lockedIcon) {
									GUI.Label(r,cfg.lockedIcon);
								}
							}
							count++;

						}
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
		GUILayout.EndVertical();
 	GUILayout.EndArea();
 	return clicked;
}

// This function draws the popup window to buy or equip a weapon.
function WeaponBuyWindow(windowID : int) {
	var g : WeaponInfo; 
	var msg : String;
	var rLeft : Rect = new Rect(5,20,popupRect.width/2-7, popupRect.height-25);
	var rRight : Rect = new Rect(popupRect.width/2 + 2,20,popupRect.width/2-7, popupRect.height-25);
	var slot : int = lsEquip.selection; // only used for equiping
	if(header.selection == HeaderModes.Buy) {
		g  = mdBuy.wi[mdBuy.selection];
	} else {
		g = mdEquip.wi[mdEquip.selection];
	}
	var upgrades : Upgrade[] = g.getUpgrades();
	//var upgradeToggles : boolean[] = new boolean[upgrades.length];

			GUI.Box(rLeft, "");
			GUILayout.BeginArea(rLeft);
			GUILayout.BeginHorizontal();
			GUILayout.Space(5);
			GUILayout.BeginVertical();
			GUILayout.Space(5);
			if(header.selection == HeaderModes.Buy) {
				msg = getBuyMsg(g);
			} else {
				msg = getEquipMsg(g,slot);
			}

				GUILayout.Label(msg);
				GUILayout.Label("Available Balance = $" + store.getBalance());

				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if(header.selection == HeaderModes.Equip ) {
						if(store.playerW.weapons[slot] != g.gameObject) {
							if(GUILayout.Button("Equip", GUILayout.Width(65))) {
								store.equipWeapon(g,slot);
							}
						} else {
							if(GUILayout.Button("Un-Equip", GUILayout.Width(65))) {
								store.unEquipWeapon(g,slot);
							}
						}		
						if(!g.gun.infiniteAmmo){
						if(g.gun.clips < g.gun.maxClips){
							if(store.getBalance()>= g.ammoPrice) {
								if (GUILayout.Button (GUIContent("Ammo ($"+g.ammoPrice+")", "Current Ammo: "+g.gun.clips), GUILayout.Width(85))) {
									store.BuyAmmo(g);
								}
							} else {
								GUILayout.Button (GUIContent("Ammo ($"+g.ammoPrice+")", "Can't Afford"), GUILayout.Width(85));
							}
						} else {
							GUILayout.Button (GUIContent("Ammo ($"+g.ammoPrice+")", "Ammo Full: "+g.gun.clips), GUILayout.Width(85));
						}
					}				
				} else {
				if(g.owned) {
					if(g.canBeSold){
						if(GUILayout.Button("Sell", GUILayout.Width(70))) {
							store.sellWeapon(g);
						}
					} else {
						GUILayout.Button("Can't Sell", GUILayout.Width(70));
					}
					if(!g.gun.infiniteAmmo){
						if(g.gun.clips < g.gun.maxClips){
							if(store.getBalance()>= g.ammoPrice) {
								if (GUILayout.Button (GUIContent("Ammo ($"+g.ammoPrice+")", "Current Ammo: "+g.gun.clips), GUILayout.Width(85))) {
									store.BuyAmmo(g);
								}
							} else {
								GUILayout.Button (GUIContent("Ammo ($"+g.ammoPrice+")", "Can't Afford"), GUILayout.Width(85));
							}
						} else {
							GUILayout.Button (GUIContent("Ammo ($"+g.ammoPrice+")", "Ammo Full: "+g.gun.clips), GUILayout.Width(85));
						}
					}
				} else {
					if(store.getBalance()>= g.buyPrice) {
						if(GUILayout.Button("Buy", GUILayout.Width(70))){
							store.buyWeapon(g);
						}
					} else {
						GUILayout.Button(GUIContent("Buy", "Insufficient Funds"), GUILayout.Width(70));
					}
				}
			}
			
			if (GUILayout.Button ("Close",GUILayout.Width(70))) {
				MDSelection = -1;
				popupActive = false;
				popupBuyActive = false;
			}
			GUILayout.FlexibleSpace();	
			GUILayout.EndHorizontal();
		GUILayout.Label(GUI.tooltip);
		GUILayout.EndVertical();
		GUILayout.Space(5);
		GUILayout.EndHorizontal();
	GUILayout.EndArea();
	
	
// Display for upgrades in the right side of the window
// The display changes for buying upgrades or equipping upgrades.

	GUI.Box(rRight,"");
	GUILayout.BeginArea(rRight);
	GUILayout.BeginHorizontal();
	GUILayout.Space(5);
	GUILayout.BeginVertical();
	GUILayout.Space(5);
	if(upgrades== null){
		GUILayout.Label("No Upgrades Available for this Weapon");
	} else if(upgrades.length < 0) {
		GUILayout.Label("No Upgrades Available for this Weapon");
	} else {
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		upgradeDisplayBuy = GUILayout.Toggle(upgradeDisplayBuy, "Buy Upgrades"); 
		if (upgradeDisplayBuy) {
			upgradeDisplayEquip = false;
		}
		upgradeDisplayEquip = GUILayout.Toggle(upgradeDisplayEquip, "Equip Upgrades");
		if(upgradeDisplayEquip) {
			upgradeDisplayBuy = false;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		if(upgradeDisplayBuy) {
		GUILayout.Space(5);
		var upgradeMsg : String;
		if(g.owned)  {	
			upgradeMsg  = "View or Buy Upgrades";
		} else {
			upgradeMsg = "You must Purchase the Weapon before buying Upgrades";
		}
		GUILayout.Label(upgradeMsg);
		popupBuyScroll1 = GUILayout.BeginScrollView(popupBuyScroll1);
		if(upgrades != null){
		for(var i : int = 0 ; i < upgrades.length; i++) {
			if(!upgrades[i].owned) {
				GUILayout.BeginHorizontal();
				GUILayout.Label(upgrades[i].upgradeName);
				GUILayout.Space(5);
				if(GUILayout.Button(setGUIContent(upgradeInfoIcon,"View",""), GUILayout.Width(40),GUILayout.Height(20))) {
					if(upgradeSelected[i]) {
						upgradeSelected[i] = false;
					} else {
						upgradeSelected[i] = true;
					}
				 }
				GUILayout.Space(5);
				
				var gc : GUIContent;
				
				if(upgrades[i].locked) {
					GUILayout.Label("(Locked)");
					upgradeMsg = upgrades[i].lockedDescription;
				} else {
					if(g.owned) {
						var balance = store.getBalance();
					if(balance > upgrades[i].buyPrice)
						gc = setGUIContent(upgradeBuyIcon, "Buy","");
					else 
						gc = new GUIContent(upgradeBuyIcon,"Insufficient Funds");

						if(GUILayout.Button(gc, GUILayout.Width(40), GUILayout.Height(20))) {
							
							if(balance > upgrades[i].buyPrice)
								store.buyUpgrade(g,upgrades[i]);
						}
					}
				}
				upgradeMsg = "Price :\t$" + upgrades[i].buyPrice + "\n";
				upgradeMsg += "Description:\t" + upgrades[i].description;
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				if(upgradeSelected[i]) {
					closeOtherSelections(i);
					GUILayout.BeginHorizontal();
					GUILayout.Space(10);
					GUILayout.BeginVertical();
						GUILayout.Label(upgradeMsg);
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();	
				}	
			} // upgrade not owned
		} // loop over upgrades
		}
			GUILayout.EndScrollView();
			GUILayout.Label(GUI.tooltip);
		} else { // Displaying Equip Window
			GUILayout.Space(5);
			GUILayout.Label("Select Upgrades To Apply - Note: Some upgrades disable others");
			var before : boolean;
//			var upgradesApplied : boolean[] = g.getUpgradesApplied();
			popupBuyScroll1 = GUILayout.BeginScrollView(popupBuyScroll1);
			for (var j = 0; j < upgrades.length; j++){
				if(upgrades[j].owned) {				
					before = g.upgradesApplied[j]; // keep track of current state
					g.upgradesApplied[j] = GUILayout.Toggle(g.upgradesApplied[j],upgrades[j].upgradeName);
					if(before != g.upgradesApplied[j]) {
						if(before) {
							upgrades[j].RemoveUpgrade();
						} else {
							upgrades[j].ApplyUpgrade();
							PlayerWeapons.HideWeaponInstant(); //turn off graphics for applied upgrade
						}
						g.updateApplied();
					}
				}
			}
		GUILayout.EndScrollView();
		GUILayout.Space(8);
		} // Displaying Equip Window
		} //upgrades.length !=0

		GUILayout.EndVertical();
		GUILayout.Space(5);
		GUILayout.EndHorizontal();
		GUILayout.EndArea();

}

//This function displays popup messages, including the weapon locked message.
function WeaponLockedWindow(windowID : int) {
	var g : WeaponInfo = mdBuy.wi[mdBuy.selection];
	GUILayout.BeginArea(new Rect(5,10,popupLockedRect.width-10, popupLockedRect.height-20));
		GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.Label(g.lockedDescription);
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Close")) {
				MDSelection = -1;
				popupActive = false;
				popupLockedActive = false;
			}
		GUILayout.EndVertical();
	GUILayout.EndArea();
}

// This function is used to make the GUILayout.Toggle() function act like a togglegroup for the Buy/Equip popup window.
function closeOtherSelections(sel : int) {
	for(var i: int; i < upgradeSelected.length; i ++) {
		if(i != sel) upgradeSelected[i] = false;
	}
}

//the following two functions are used just to cleanup the logic in the Buy/Upgrade popup window
function getBuyMsg(g : WeaponInfo) {
	var msg : String;
	msg  = "Weapon Name: " + g.gunName +"\n\n";			
	if(g.owned) {
		msg += "Weapon Not Owned\n";
	} else {
		msg += "Weapon Owned\n";
	}
	msg += "Description: " + g.gunDescription + "\n\n";
	if(g.owned) {
		msg += "Sell Price: $" + g.sellPriceUpgraded + "\n";
	} else {
		msg += "Price: $" + g.buyPrice;
	}
	return msg;
}

function getEquipMsg(g : WeaponInfo, slot: int) {
	var msg : String;
	msg  = "Equipping for Slot " + slot + " \n";
	msg += "Weapon Name " + g.gunName + "\n";			
	if(store.playerW.weapons[slot] as WeaponInfo == g.gameObject) {
		msg += "Weapon Equiped in Slot\n";
	} else {
		msg += "Weapon not Equiped in Slot\n";
	}
	msg += "Description: " + g.gunDescription + "\n";
	if(g.owned) {
		msg += "Sell Price: $" + g.sellPriceUpgraded + "\n";
	} else {
		msg += "Price: $" + g.buyPrice;
	}
	return msg;
}

//This utility function just keeps the code a little simpler. It centers a GUILayout Label Horizontally
static function DrawLabelHCenter(s : String) {
	GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
			GUILayout.Label(s);
		GUILayout.FlexibleSpace();
	GUILayout.EndHorizontal();
}

// Utility function to create GUIContent
// If texture existing dont set the string value
// If tool tip is not null set it
static function setGUIContent(t : Texture, label : String, tip :String) : GUIContent {
	var gc : GUIContent;
	if(tip !="") {
		if(t) {
			gc = GUIContent(t, tip);
		} else {
			gc = GUIContent(label, tip);
		}
	} else {
		if(t) {
			gc = GUIContent(t);
		} else {
			gc = GUIContent(label);
		}
	}
	return gc;		
}

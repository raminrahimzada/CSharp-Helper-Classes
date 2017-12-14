

function OnMouseDown () {
    var AllCreatures = FindObjectsOfType (PuppetController);
    for (var Creature : PuppetController in AllCreatures) {
        Creature.SetDecal( false );
    }
}
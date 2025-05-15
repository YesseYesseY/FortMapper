const LP = require("./LP.json");
const LPC = require("./LPC.json");
const LTD = require("./LTD.json");

function parse_name(name) {
    const translation_table = {
        "WorldPKG.AthenaLoot.Weapon.HighCrossbow": "Crossbows",
        "WorldPKG.AthenaLoot.Weapon.HighHandgun": "Pistols",
        "WorldPKG.AthenaLoot.Weapon.HighRocket": "Explosives",
        "WorldPKG.AthenaLoot.Weapon.HighLMG": "LMGs",
        "WorldPKG.AthenaLoot.Weapon.HighHandCannon": "Hand Cannons",
        "WorldPKG.AthenaLoot.Weapon.HighSniper": "Snipers",
        "WorldPKG.AthenaLoot.Weapon.HighAssaultAuto": "Assault Rifles",
        "WorldPKG.AthenaLoot.Weapon.HighSMG": "SMGs",
        "WorldPKG.AthenaLoot.Weapon.HighShotgun": "Shotguns",
        "WorldPKG.AthenaLoot.Weapon.Crossbow": "Crossbows",
        "WorldPKG.AthenaLoot.Weapon.Handgun": "Pistols",
        "WorldPKG.AthenaLoot.Weapon.Rocket": "Explosives",
        "WorldPKG.AthenaLoot.Weapon.LMG": "LMGs",
        "WorldPKG.AthenaLoot.Weapon.HandCannon": "Hand Cannons",
        "WorldPKG.AthenaLoot.Weapon.Sniper": "Snipers",
        "WorldPKG.AthenaLoot.Weapon.AssaultAuto": "Assault Rifles",
        "WorldPKG.AthenaLoot.Weapon.SMG": "SMGs",
        "WorldPKG.AthenaLoot.Weapon.Shotgun": "Shotguns",
        "WorldPKG.AthenaLoot.Consumable": "Consumables",
        "WorldPKG.AthenaLoot.Trap": "Traps",
        "WorldPKG.AthenaLoot.Ammo": "Ammo",
        "WorldPKG.AthenaLoot.Resources": "Resources",
        "WorldList.AthenaHighConsumables": "Consumables",
        "WorldList.AthenaLoot.Resources": "Resources",
        "WorldList.AthenaWadChests": "Gold",
        "WorldList.AthenaTraps": "Traps",
        "Handmade": "Common",
        "Sturdy": "Rare",
        "Quality": "Epic",
        "Fine": "Legendary"
    }

    return name in translation_table ? translation_table[name] : name;
}

function static_parse(ToParse, OverwriteWeight = -1) {
    var TotalWeight;
    if (OverwriteWeight < 0) {
        TotalWeight = 0.0;
        for (let i = 0; i < ToParse.length; i++) {
            TotalWeight += ToParse[i].weight;
        }
    }
    else {
        TotalWeight = OverwriteWeight;
    }

    for (let i = 0; i < ToParse.length; i++) {
        console.log(` * ${((ToParse[i].weight / TotalWeight) * 100).toFixed(2).padStart(5, " ")}% ${ToParse[i].item_count.toString().padStart(2, " ")}x ${parse_name(ToParse[i].item_rarity)} ${ToParse[i].item_name}`);
    }

    console.log();
}

function parse_treasure(RelativePercent = false) {
    var LAT = LTD.Loot_AthenaTreasure;

    var static_len = LP[LAT[0].loot_package].length;
    for (let i = 1; i < static_len; i++) {
        if (LP[LAT[0].loot_package][i].loot_package_call.includes(".Ammo.")) continue; 
        console.log(`${parse_name(LP[LAT[0].loot_package][i].loot_package_call)}: (100%)`); // TODO: Check if gamemode drops gold
        static_parse(LPC[LP[LAT[0].loot_package][i].loot_package_call]);
    }

    {
        var LATWeight = 0.0;
        var TotalWeight = 0.0;
        for (let i = 0; i < LAT.length; i++) {
            LATWeight += LAT[i].weight;
            for (let j = 0; j < LPC[LP[LAT[i].loot_package][0].loot_package_call].length; j++) {
                TotalWeight += LPC[LP[LAT[i].loot_package][0].loot_package_call][j].weight;
            }
        }

        for (let i = 0; i < LAT.length; i++) {
            console.log(`${parse_name(LAT[i].loot_package)}: (${((LAT[i].weight / LATWeight) * 100).toFixed(2).padStart(5, " ")}%)`);
            static_parse(LPC[LP[LAT[i].loot_package][0].loot_package_call], RelativePercent ? -1 : TotalWeight);
        }
    }
}

function parse_floorloot() {
    var LAF = LTD.Loot_AthenaFloorLoot;

    var LAFWeight = 0.0;
    var TotalWeight = 0.0;
    for (let i = 0; i < LAF.length; i++) {
        LAFWeight += LAF[i].weight;
        if (LAF[i].loot_package != "WorldList.AthenaLoot.Empty") {
            console.log(LAF[i].loot_package);
            for (let j = 0; j < LPC[LP[LAF[i].loot_package][0].loot_package_call].length; j++) {
                TotalWeight += LPC[LP[LAF[i].loot_package][0].loot_package_call][j].weight;
            }
        }
    }

    for (let i = 0; i < LAF.length; i++) {

        if (LAF[i].loot_package == "WorldList.AthenaLoot.Empty") {
            console.log(`Empty: (${((LAF[i].weight / LAFWeight) * 100).toFixed(2)}%)\n`);
        } else {
            console.log(`${parse_name(LAF[i].loot_package)}: (${((LAF[i].weight / LAFWeight) * 100).toFixed(2)}%):`);
            static_parse(LPC[LP[LAF[i].loot_package][0].loot_package_call], RelativePercent ? -1 : TotalWeight);
        }
    }
}

parse_treasure();
const LP = require("./LP.json");
const LPC = require("./LPC.json");
const LTD = require("./LTD.json");

function parse_name(name) {
    const translation_table = {
        "WorldPKG.AthenaLoot.Weapon.HighCrossbow": "Crossbows",
        "WorldPKG.AthenaLoot.Weapon.HighHandgun": "Pistols",
        "WorldPKG.AthenaLoot.Weapon.HighRocket": "Explosives",
        "WorldPKG.AthenaLoot.Weapon.HighLMG": "LMGs", // ?
        "WorldPKG.AthenaLoot.Weapon.HighHandCannon": "Hand Cannons", // ?
        "WorldPKG.AthenaLoot.Weapon.HighSniper": "Snipers",
        "WorldPKG.AthenaLoot.Weapon.HighAssaultAuto": "Assault Rifles",
        "WorldPKG.AthenaLoot.Weapon.HighSMG": "SMGs",
        "WorldPKG.AthenaLoot.Weapon.HighShotgun": "Shotguns",
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

    for (let i = 2; i <= 5; i++) {
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

parse_treasure();
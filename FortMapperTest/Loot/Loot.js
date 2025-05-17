const data_input = document.getElementById("data-input");
const ltd_input = document.getElementById("ltd-input");
const extra_input = document.getElementById("extra-input");
const loot_place = document.getElementById("loot-place");

var data = {};

data_input.addEventListener("change", (e) => {
    Object.values(e.target.files).forEach((f) => {
        const reader = new FileReader();
        
        reader.addEventListener("load", (e) => {
            const nametoadd = f.name.replace(".json", "");
            data[nametoadd] = JSON.parse(e.target.result);

            // TODO: IDK WHERE TF WorldPKG.AthenaLoot.Ammo IS OR IF IT EXISTS AT ALL SO THIS WILL HAVE TO DO
            if (nametoadd == "LP") {
                data[nametoadd]["WorldPKG.AthenaLoot.Ammo"] = [
                    {
                        "weight": 1,
                        "loot_package_call": "WorldList.AthenaLoot.Ammo"
                    }
                ];
                data[nametoadd]["WorldPKG.AthenaLoot.Resources"] = [
                    {
                        "weight": 1,
                        "loot_package_call": "WorldList.AthenaLoot.Resources"
                    }
                ];
            }
        });

        reader.readAsText(f);

        ltd_input.style.display = "inline";
        extra_input.style.display = "inline";
        data_input.style.display = "none";
    });
});

const rarity_colors = {
    "Handmade": ["#D9D9D9", "#B7BFC5", "#747A80", "#40464D", "#272834"],
    "Uncommon": ["#A1FE00", "#61BF00", "#008005", "#024F03", "#023302"],
    "Sturdy": ["#00FFFB", "#00AFFF", "#0058BF", "#00458A", "#1A2A39"],
    "Quality": ["#EC26FF", "#CE59FF", "#762CD3", "#4C197B", "#2D2039"],
    "Fine": ["#FBC363", "#FF8B19", "#BF4F00", "#8A3C1D", "#381C17"],
};

const translation_table = {
    "WorldList.AthenaLoot.Weapon.HighCrossbow": "Crossbows",
    "WorldList.AthenaLoot.Weapon.HighHandgun": "Pistols",
    "WorldList.AthenaLoot.Weapon.HighRocket": "Explosives",
    "WorldList.AthenaLoot.Weapon.HighLMG": "LMGs",
    "WorldList.AthenaLoot.Weapon.HighHandCannon": "Hand Cannons",
    "WorldList.AthenaLoot.Weapon.HighSniper": "Snipers",
    "WorldList.AthenaLoot.Weapon.HighAssaultAuto": "Assault Rifles",
    "WorldList.AthenaLoot.Weapon.HighSMG": "SMGs",
    "WorldList.AthenaLoot.Weapon.HighShotgun": "Shotguns",
    "WorldList.AthenaLoot.Weapon.Crossbow": "Crossbows",
    "WorldList.AthenaLoot.Weapon.Handgun": "Pistols",
    "WorldList.AthenaLoot.Weapon.Rocket": "Explosives",
    "WorldList.AthenaLoot.Weapon.LMG": "LMGs",
    "WorldList.AthenaLoot.Weapon.HandCannon": "Hand Cannons",
    "WorldList.AthenaLoot.Weapon.Sniper": "Snipers",
    "WorldList.AthenaLoot.Weapon.AssaultAuto": "Assault Rifles",
    "WorldList.AthenaLoot.Weapon.SMG": "SMGs",
    "WorldList.AthenaLoot.Weapon.Shotgun": "Shotguns",
    "WorldList.AthenaLoot.Consumable": "Consumables",
    "WorldList.AthenaLoot.Trap": "Traps",
    "WorldList.AthenaLoot.Ammo": "Ammo",
    "WorldList.AthenaLoot.Resources": "Resources",
    "WorldList.AthenaHighConsumables": "Consumables",
    "WorldList.AthenaLoot.Resources": "Resources",
    "WorldList.AthenaWadChests": "Gold",
    "WorldList.AthenaTraps": "Traps",
    "WorldList.AthenaSupplyDropConsumables": "Consumables",
    "WorldList.AthenaLoot.SupplyDropResources": "Resources",
    "WorldList.AthenaSupplyDropTraps": "Traps",
    "WorldList.AthenaSupplyDrop.Weapon.Assault": "Assault Rifles",
    "WorldList.AthenaSupplyDrop.Weapon.Rocket": "Explosives",
    "WorldList.AthenaSupplyDrop.Weapon.Sniper": "Snipers",
    "WorldList.AthenaSupplyDrop.Weapon.Shotgun": "Shotguns",
    "WorldList.AthenaSupplyDrop.Weapon.Handgun": "Pistols",
    "WorldList.AthenaSupplyDrop.Weapon.SMG": "SMGs",
    "WorldList.AthenaLoot.Empty": "Empty",
    "Handmade": "Common",
    "Sturdy": "Rare",
    "Quality": "Epic",
    "Fine": "Legendary"
};

function parse_name(name) {
    return name in translation_table ? translation_table[name] : name;
}

function parse_rarity(rarity) {
    return rarity in rarity_colors ? `radial-gradient(${rarity_colors[rarity][0]}, ${rarity_colors[rarity][3]})` : "#888888";
}

function create_item_card(e, ltd_prob, weight2) {
    const item_card = Object.assign(document.createElement("div"), { className: "item-card" });
    const item_card_img = Object.assign(document.createElement("img"), { src: e.item_icon, draggable: false, });
    item_card_img.style.background = parse_rarity(e.item_rarity);
    const item_name_label = Object.assign(document.createElement("label"), { innerText: (e.item_count != 1 ? `${e.item_count}x ` : "") + e.item_name });
    item_card.append(
        Object.assign(document.createElement("label"), { innerText: ((((e.weight / weight2) * (ltd_prob)) * 100).toFixed(2)) + "%" }),
        item_card_img,
        item_name_label
    );
    return item_card;
}

function parse_lpc(lpc, item_container, ltd_prob) {
    var w2 = 0;
    lpc.forEach(e => w2 += e.weight);
    lpc.forEach(e => item_container.appendChild(create_item_card(e, ltd_prob, w2)));
}

ltd_input.addEventListener("change", (e) => {
    const LTD = data["LTD"];
    const LP = data["LP"];
    const LPC = data["LPC"];

    loot_place.innerHTML = "";

    if (e.target.value == "") return;

    const ltdinputval = e.target.value;

    var ltd_total_weight = 0;
    LTD[e.target.value].forEach((e) => ltd_total_weight += e.weight);

    if (e.target.value == "Loot_AthenaTreasure" || e.target.value == "Loot_AthenaSupplyDrop")
    {
        const current_lp = LP[LTD[e.target.value][0].loot_package];
        for (let i = 2; i < current_lp.length; i++) {
            const current_lpc = LPC[current_lp[i].loot_package_call];
            loot_place.innerHTML += `<label>${parse_name(current_lp[i].loot_package_call)}</label>`;
            const item_container = document.createElement("div");
            item_container.className = "item-container";
            loot_place.appendChild(item_container);

            var total_weight = 0;
            current_lpc.forEach(thing => total_weight += thing.weight);
            parse_lpc(current_lpc, item_container, 1);
        }
    }

    var total_weight = 0;
    var combine_container;

    if (extra_input.value != "local") {
        LTD[e.target.value].forEach((e2) => {
            if (e2.loot_package.startsWith("WorldList")) {
                LPC[e2.loot_package].forEach(e3 => total_weight += e3.weight);
            }
            else {
                LPC[LP[e2.loot_package][0].loot_package_call].forEach(e3 => total_weight += e3.weight);
            }
        });
    }

    const isLlama = e.target.value == "Loot_AthenaLlama";

    LTD[e.target.value].forEach((e) => {
        console.log(e);
        const current_lp = LP[e.loot_package];
        const lp_is_lpc = e.loot_package.startsWith("WorldList");
        var current_lpc = lp_is_lpc ? LPC[e.loot_package] : LPC[current_lp[0].loot_package_call];

        if (!e.loot_package.startsWith("WorldList"))
        {
            for (let i = 0; i < current_lp.length; i++) {
                const e2 = current_lp[i];
                if (extra_input.value == "local")
                    current_lpc.forEach(thing => total_weight += thing.weight);

                current_lpc = LPC[e2.loot_package_call];

                loot_place.innerHTML += `<label>${parse_name(e2.loot_package_call)} (${((e.weight / ltd_total_weight) * 100).toFixed(2)}%)</label>`;
                const item_container = document.createElement("div");
                item_container.className = "item-container";
                parse_lpc(current_lpc, item_container, e.weight / ltd_total_weight);
                loot_place.appendChild(item_container);

                if (extra_input.value == "local")
                    total_weight = 0;

                if (ltdinputval != "Loot_AthenaLlama") break;
            }
        }
    });
    
    
});
const data_input = document.getElementById("data-input");
const ltd_input = document.getElementById("ltd-input");
const loot_place = document.getElementById("loot-place");

var data = {};

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
    "Fine": "Legendary",
};

data_input.addEventListener("change", (e) => {
    Object.values(e.target.files).forEach((f) => {
        const reader = new FileReader();

        reader.addEventListener("load", (e) => {
            data = JSON.parse(e.target.result);

            // TODO: IDK WHERE TF WorldPKG.AthenaLoot.Ammo IS OR IF IT EXISTS AT ALL SO THIS WILL HAVE TO DO
            data["LP"]["WorldPKG.AthenaLoot.Ammo"] = [
                {
                    "weight": 1,
                    "loot_package_call": "WorldList.AthenaLoot.Ammo"
                }
            ];
            data["LP"]["WorldPKG.AthenaLoot.Resources"] = [
                {
                    "weight": 1,
                    "loot_package_call": "WorldList.AthenaLoot.Resources"
                }
            ];
        });
        reader.readAsText(f);

        ltd_input.style.display = "inline";
        data_input.style.display = "none";
    });
});

function parse_name(name) {
    return name in translation_table ? translation_table[name] : name;
}

function parse_rarity(rarity) {
    return rarity in rarity_colors ? `radial-gradient(${rarity_colors[rarity][0]}, ${rarity_colors[rarity][3]})` : "#FF00FF";
}

function to_percent(val) {
    return (val * 100).toFixed(2) + "%"
}

function create_item_card2(img_path, top_text, bottom_text, rarity) {
    const item_card = Object.assign(document.createElement("div"), { className: "item-card" });
    const item_img = Object.assign(document.createElement("img"), { draggable: false, });
    if (img_path !== "")
        item_img.src = img_path;
    item_img.style.background = parse_rarity(rarity);
    item_card.append(
        Object.assign(document.createElement("label"), { innerText: top_text }),
        item_img,
        Object.assign(document.createElement("label"), { innerText: bottom_text })
    );
    return item_card;
}

function create_item_card(e, ltd_prob, weight2) {
    const is_empty = e.item_count == 0;
    const item_card = Object.assign(document.createElement("div"), { className: "item-card" });
    const item_card_img = Object.assign(document.createElement("img"), { draggable: false, });
    if (!is_empty)
        item_card_img.src = e.item_icon;
    item_card_img.style.background = parse_rarity(e.item_rarity);
    const item_name_label = Object.assign(document.createElement("label"), { innerText: is_empty ? "Empty" : (e.item_count != 1 ? `${e.item_count}x ` : "") + e.item_name });
    item_card.append(
        Object.assign(document.createElement("label"), { innerText: ((((e.weight / weight2) * (ltd_prob)) * 100).toFixed(2)) + "%" }),
        item_card_img,
        item_name_label
    );
    return item_card;
}

ltd_input.addEventListener("change", (change_event) => {
    const LTD = data["LTD"];
    const LP = data["LP"];
    const LPC = data["LPC"];

    loot_place.innerHTML = "";

    if (change_event.target.value == "") return;

    var ltd_total_weight = 0;
    LTD[change_event.target.value].forEach(e => ltd_total_weight += e.weight);

    const lpcs = {};
    function addtolpcs(weight, name) {
        if (name in lpcs)
            lpcs[name] += weight;
        else
            lpcs[name] = weight;
    }
    LTD[change_event.target.value].forEach(e => {
        if (e.loot_package.startsWith("WorldList")) {
            addtolpcs(e.weight, e.loot_package);
        } else {
            LP[e.loot_package].forEach(e2 => addtolpcs(e.weight, e2.loot_package_call));
        }
    })

    Object.entries(lpcs).forEach(e => {
        loot_place.innerHTML += `<label>${parse_name(e[0])} (${to_percent(e[1] / ltd_total_weight)})</label>`;
        const item_container = document.createElement("div");
        item_container.className = "item-container";
        var lpc_total_weight = 0;
        LPC[e[0]].forEach(thing => lpc_total_weight += thing.weight);
        LPC[e[0]].forEach(thing => item_container.appendChild(create_item_card2(
            thing.item_count > 0 ? thing.item_icon : "",
            to_percent((thing.weight / lpc_total_weight) * (e[1] / ltd_total_weight)),
            thing.item_count > 0 ? ((thing.item_count != 1 ? `${thing.item_count}x ` : "") + thing.item_name) : "Empty",
            thing.item_rarity
        )));
        loot_place.appendChild(item_container);
    })
});
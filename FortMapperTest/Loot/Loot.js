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
    "Elegant": ["#FFF0A6", "#E5C74F", "#BF9323", "#7C5D26", "#392C1A"],
    "Exotic": ["#00DBFB", "#00FFAC", "#76BFFF", "#6B3F8A", "#381F38"],
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
    "WorldList.AthenaConsumables": "Consumables",
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
    "WorldList.AthenaLlama.Wood": "Wood",
    "WorldList.AthenaLlama.Stone": "Stone",
    "WorldList.AthenaLlama.Metal": "Metal",
    "WorldList.AthenaLoot.Ammo.Rockets": "Rocket Ammo",
    "WorldList.AthenaLoot.Ammo.Heavy": "Heavy Ammo",
    "WorldList.AthenaLoot.Ammo.Medium": "Medium Ammo",
    "WorldList.AthenaLoot.Ammo.Light": "Light Ammo",
    "WorldList.AthenaLoot.Ammo.Shells": "Shells",
    "WorldList.AthenaLoot.SupplyAmmo.Rockets": "Rocket Ammo",
    "WorldList.AthenaLoot.SupplyAmmo.Heavy": "Heavy Ammo",
    "WorldList.AthenaLoot.SupplyAmmo.Medium": "Medium Ammo",
    "WorldList.AthenaLoot.SupplyAmmo.Light": "Light Ammo",
    "WorldList.AthenaLoot.SupplyAmmo.Shells": "Shells",
    "WorldList.AthenaLoot.SupplyResources": "Resources",
    "WorldList.ClaimPOI.Reward.HealingConsumables": "Healing Consumables",
    "WorldList.AthenaSupplyDropHeals": "Healing Consumables",
    "WorldList.Consumable.Mobility": "Mobility Consumables",
    "WorldList.AthenaSupplyDropUtility": "Utility Consumables",
    "WorldList.Weapons.Rarity.Epic": "Epic Weapons",
    "WorldList.GoldLlama.GoldBars": "Bars",
    "WorldList.MythicRandom": "Random Mythic",
    "WorldList.ExoticRandom": "Random Exotic",
    "WorldList.MythicGFish": "Mythic Goldfish",
    "WorldList.ExoticSupply1": "Exotic",
    "WorldList.ExoticSupply2": "Exotic",
    "WorldList.ExoticSupply3": "Exotic",
    "WorldList.ExoticSupply4": "Exotic",
    "WorldList.ExoticSupply5": "Exotic",
    "WorldList.ApolloLoot.Weapon.HighShotgun": "Shotguns",
    "WorldList.AthenaHighConsumablesRare": "Consumables",
    "WorldList.ApolloLoot.Weapon.SMG": "SMGs",
    "WorldList.ApolloLoot.Weapon.AssaultAuto": "Assault Rifles",
    "WorldList.ApolloLoot.Weapon.Sniper": "Snipers",
    "WorldList.ApolloLoot.Weapon.Rocket": "Explosives",
    "WorldList.ApolloLoot.Weapon.HighHandgun": "Pistols",
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

            let to_remove = [];
            for (let i = 0; i < ltd_input.children.length; i++) {
                let child = ltd_input.children[i];
                if (child.value !== "" && !(child.value in data["LTD"]))
                    to_remove.push(child);
            }
            to_remove.forEach(e => ltd_input.removeChild(e));
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

function get_total_weight(thing) {
    if (thing === undefined)
        return 0;
    var ret = 0;
    thing.forEach(e => ret += e.weight);
    return ret;
}

function create_item_container(name, chance) {
    loot_place.innerHTML += `<label>${parse_name(name)} (${to_percent(chance)})</label>`;
    return Object.assign(document.createElement("div"), { className: "item-container" });
}

function create_item_card(img_path, top_text, bottom_text, background) {
    const item_card = Object.assign(document.createElement("div"), { className: "item-card" });
    const item_img = Object.assign(document.createElement("img"), { draggable: false, });
    if (img_path !== "")
        item_img.src = img_path;
    item_img.style.background = background;
    item_card.append(
        Object.assign(document.createElement("label"), { innerText: top_text }),
        item_img,
        Object.assign(document.createElement("label"), { innerText: bottom_text })
    );
    return item_card;
}

function item_to_card(item, ltd_chance, lpc_chance) {
    return create_item_card(
        item.item_count > 0 ? item.item_icon : "",
        to_percent(lpc_chance * ltd_chance),
        item.item_count > 0 ? ((item.item_count != 1 ? `${item.item_count}x ` : "") + item.item_name) : "Empty",
        parse_rarity(item.item_rarity)
    );
}

function parse_default(ltd_name, worldpkg_counts = []) {
    const ltd_total_weight = get_total_weight(data["LTD"][ltd_name]);
    let lpcma = data["LTD"][ltd_name][0].loot_package_category_min_array;
    for (let i = 0; i < lpcma.length; i++) {
        if (worldpkg_counts[i] === undefined || worldpkg_counts[i] === -1) {
            worldpkg_counts[i] = lpcma[i];
        }
    }

    const lpcs = {};
    function addtolpcs(weight, name, count = 1) {
        if (name in lpcs) {
            lpcs[name].weight += weight
        }
        else {
            lpcs[name] = {
                "weight": weight,
                "count": count
            };
        }
    }

    data["LTD"][ltd_name].forEach(e => {
        if (e.loot_package.startsWith("WorldList")) {
            addtolpcs(e.weight, e.loot_package);
        } else {
            const thingy = data["LP"][e.loot_package];
            for (let i = 0; i < thingy.length; i++) {
                if (worldpkg_counts[i] == 0) continue
                const e2 = thingy[i];
                addtolpcs(e.weight, e2.loot_package_call, worldpkg_counts[i] == undefined ? 1 : worldpkg_counts[i]);
            }
        }
    })

    for (const [lpc_name, lpc_data] of Object.entries(lpcs)) {
        if (data["LPC"][lpc_name] === undefined) continue;
        const item_container = create_item_container(`${(lpc_data.count > 1 ? `${lpc_data.count}x ` : "")}${parse_name(lpc_name)}`, lpc_data.weight / ltd_total_weight);
        const lpc_total_weight = get_total_weight(data["LPC"][lpc_name]);

        data["LPC"][lpc_name].forEach(thing => item_container.appendChild(item_to_card(thing, lpc_data.weight / ltd_total_weight, thing.weight / lpc_total_weight)));
        loot_place.appendChild(item_container);
    }
}

function parse_vending(ltd_name) {
    const vending_weights = [
        {
            name: "Common",
            weight: 10,
            stuff: []
        },
        {
            name: "Uncommon",
            weight: 20,
            stuff: []
        },
        {
            name: "Rare",
            weight: 20,
            stuff: []
        },
        {
            name: "Epic",
            weight: 7.5,
            stuff: []
        },
        {
            name: "Legendary",
            weight: 5.0,
            stuff: []
        }
    ];
    const thingyes = {
        "Common": 0,
        "Uncommon": 1,
        "Rare": 2,
        "Epic": 3,
        "Legendary": 4,
    }

    data["LTD"][ltd_name].forEach(e => {
        const rarity = e.loot_package.split('.')[4];
        if (e.loot_package.startsWith("WorldList"))
            vending_weights[thingyes[rarity]].stuff.push(e);
        else
            vending_weights[thingyes[rarity]].stuff.push({
                weight: e.weight,
                loot_package: data["LP"][e.loot_package][0].loot_package_call
            });
    });

    const vending_total_weight = get_total_weight(Object.values(vending_weights));
    for (let i = 0; i < vending_weights.length; i++) {
        const rarity_chance = vending_weights[i].weight / vending_total_weight;
        const item_container = create_item_container(vending_weights[i].name, rarity_chance);

        loot_place.appendChild(item_container);

        const stuff_total_weight = get_total_weight(vending_weights[i].stuff);
        vending_weights[i].stuff.forEach(e => {
            const stuff_chance = e.weight / stuff_total_weight;
            const lpc_chance = get_total_weight(data["LPC"][e.loot_package]);
            data["LPC"][e.loot_package].forEach(e2 => {
                item_container.appendChild(item_to_card(e2, stuff_chance, e2.weight / lpc_chance));
            });
        });
    }
}

ltd_input.addEventListener("change", (change_event) => {
    if (ltd_input.children[0].value == "")
        ltd_input.removeChild(ltd_input.children[0]);

    loot_place.innerHTML = "";
    if (change_event.target.value == "") return;

    switch (change_event.target.value) {
        case "Loot_AthenaTreasure":
            parse_default(change_event.target.value, [-1, -1, -1, -1, -1, 1]);
            break;
        case "Loot_AthenaSupplyDrop":
        case "Loot_AthenaLlama":
        case "Loot_AthenaFloorLoot":
        case "Loot_AthenaFloorLoot_Warmup":
        case "Loot_Golden_Llama":
            parse_default(change_event.target.value);
            break;
        case "Loot_AthenaVending":
            parse_vending(change_event.target.value);
            break;
        default:
            parse_default(change_event.target.value);
            break;
    }
});
const data_input = document.getElementById("data-input");
const ltd_input = document.getElementById("ltd-input");
const extra_input = document.getElementById("extra-input");
const loot_place = document.getElementById("loot-place");

var data = {};

data_input.addEventListener("change", (e) => {
    Object.values(e.target.files).forEach((f) => {
        console.log(f);

        const reader = new FileReader();
        
        reader.addEventListener("load", (e) => {
            console.log(f.name.replace(".json", ""));
            data[f.name.replace(".json", "")] = JSON.parse(e.target.result);
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

function create_item_card(e, total_weight) {
    const item_card = Object.assign(document.createElement("div"), { className: "item-card" });
    const item_card_img = Object.assign(document.createElement("img"), { src: e.item_icon, draggable: false, });
    item_card_img.style.background = parse_rarity(e.item_rarity);
    item_card.append(
        Object.assign(document.createElement("label"), { innerText: (((e.weight / total_weight) * 100).toFixed(2)) + "%" }),
        item_card_img,
        Object.assign(document.createElement("label"), { innerText: e.item_name })
    );
    return item_card;
}

function parse_lpc(lpc, total_weight, item_container) {
    lpc.forEach(e => item_container.appendChild(create_item_card(e, total_weight)));
}

ltd_input.addEventListener("change", (e) => {
    console.log("change");
    const LTD = data["LTD"];
    const LP = data["LP"];
    const LPC = data["LPC"];
    loot_place.innerHTML = "";

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
            parse_lpc(current_lpc, total_weight, item_container);
        }
    }

    {
        var total_weight = 0;
        var combine_container;

        if (extra_input.value != "local")
            LTD[e.target.value].forEach((e2) => LPC[LP[e2.loot_package][0].loot_package_call].forEach(e3 => total_weight += e3.weight));

        if (extra_input.value == "combine") {
            loot_place.innerHTML += `<label>Weapons</label>`;
            combine_container = document.createElement("div");
            combine_container.className = "item-container";
        }

        LTD[e.target.value].forEach((e) => {
            const current_lp = LP[e.loot_package];
            const current_lpc = LPC[current_lp[0].loot_package_call];

            if (extra_input.value == "combine") {
                parse_lpc(current_lpc, total_weight, combine_container);
                loot_place.appendChild(combine_container);
            } else {
                if (extra_input.value == "local")
                    current_lpc.forEach(thing => total_weight += thing.weight);

                loot_place.innerHTML += `<label>${parse_name(current_lp[0].loot_package_call)}</label>`;
                const item_container = document.createElement("div");
                item_container.className = "item-container";
                parse_lpc(current_lpc, total_weight, item_container);
                loot_place.appendChild(item_container);
                if (extra_input.value == "local")
                    total_weight = 0;
            }
        });
    }
});
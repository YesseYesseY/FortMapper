/** @type HTMLCanvasElement */
const canvas = document.getElementById("canvas");
/** @type HTMLDivElement */
const actorButtons = document.getElementById("actor-buttons");
/** @type CanvasRenderingContext2D */
const ctx = canvas.getContext("2d");

const coords_x = document.getElementById("coords-x");
const coords_y = document.getElementById("coords-y");

canvas.width = window.innerWidth;
canvas.height = window.innerHeight;
window.addEventListener("resize", (e) => {
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;
    drawMap();
});
canvas.addEventListener("mousedown", (e) => {
    canvas.style.cursor = "all-scroll";
    if (e.button == 0)
        dragging = "map";
    else if (e.button == 1)
        dragging = "zone"
});
canvas.addEventListener("mouseup", (e) => {
    canvas.style.cursor = "default";
    dragging = "";
});
canvas.addEventListener("mousemove", (e) => {
    if (dragging == "map") {
        img_pos.X += e.movementX;
        img_pos.Y += e.movementY;
    } else if (dragging == "zone") {
        const new_zone_pos = img_to_world({
            X: e.offsetX,
            Y: e.offsetY
        });
        zone_pos = new_zone_pos;
    }

    drawMap();

    if (world.camera !== undefined) {
        const worldmousepos = img_to_world({
            X: e.offsetX,
            Y: e.offsetY
        });
        coords_x.innerText = worldmousepos.X.toFixed(1);
        coords_y.innerText = worldmousepos.Y.toFixed(1);

        //const uwu = world_to_img(worldmousepos);
        //ctx.fillStyle = "red";
        //ctx.fillRect(uwu.X, uwu.Y, 20, 20);
    }

});

var world = {};

const actorimages = {};
let actornames = [];

const zoomMult = 1.15;
const iconSize = 32;
const baseImageSize = 1000;

var img_pos = {
    X: canvas.width / 2 - (baseImageSize / 2),
    Y: canvas.height / 2 - (baseImageSize / 2)
};
var imgZoom = 1;

var dragging = "";
var drawPois = false;
var drawZiplines = false;

var zone_pos = {
    X: 0,
    Y: 0
}

const jsoninput = document.getElementById("jsoninput");
jsoninput.addEventListener("change", (e) => {
    const file = e.target.files[0];
    if (!file) return;

    const reader = new FileReader();

    reader.addEventListener("load", (e) => {
        world = JSON.parse(e.target.result);
        actorButtons.style.display = "block";
        maindraw();
    });

    reader.readAsText(file);
});

function world_to_img(pos) {
    let relative = [pos.X - world.camera.pos.X, pos.Y - world.camera.pos.Y];

    let rot = -((world.camera.rot.Roll + world.camera.relrot.Roll) * (Math.PI / 180));
    let cos = Math.cos(rot);
    let sin = Math.sin(rot);
    let rotatedX = relative[0] * cos - relative[1] * sin;
    let rotatedY = relative[0] * sin + relative[1] * cos;

    return {
        X: (((rotatedY) / world.camera.ortho_width + 0.5) * baseImageSize * imgZoom) + img_pos.X,
        Y: ((-(rotatedX) / world.camera.ortho_width + 0.5) * baseImageSize * imgZoom) + img_pos.Y,
    }
}

function img_to_world(pos) {
    let rotatedY = (((pos.X - img_pos.X) / (baseImageSize * imgZoom)) - 0.5) * world.camera.ortho_width;
    let rotatedX = -(((pos.Y - img_pos.Y) / (baseImageSize * imgZoom)) - 0.5) * world.camera.ortho_width;
    let rot = -((world.camera.rot.Roll + world.camera.relrot.Roll) * (Math.PI / 180));
    let cos = Math.cos(rot);
    let sin = Math.sin(rot);

    return {
        X: (rotatedX * cos + rotatedY * sin) + world.camera.pos.X,
        Y: (-rotatedX * sin + rotatedY * cos) + world.camera.pos.Y,
    };
}

function drawMap() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.drawImage(mapimage, img_pos.X, img_pos.Y, baseImageSize * imgZoom, baseImageSize * imgZoom);

    ctx.font = "bold 18px arial";
    ctx.fillStyle = "black";

    for (let j = 0; j < world.actors.length; j++) {
        for (let i = 0; i < world.actors[j].positions.length; i++) {
            if (world.actors[j].disabled) continue;

            let map_pos = world_to_img(world.actors[j].positions[i]);
            ctx.drawImage(actorimages[world.actors[j].name], map_pos.X - (iconSize / 2), map_pos.Y - (iconSize / 2), iconSize, iconSize);
        }
    }

    if (drawPois) {
        for (const [poiname, poilocation] of Object.entries(world.pois)) {
            let map_pos = world_to_img(poilocation);
            ctx.textAlign = "center";
            ctx.fillText(poiname, map_pos.X, map_pos.Y);
        }
    }

    ctx.lineWidth = 2;

    if (drawZiplines) {
        for (let i = 0; i < world.ziplines.length; i++) {
            const pos1 = world_to_img(world.ziplines[i].Item1);
            const pos2 = world_to_img(world.ziplines[i].Item2);
            ctx.beginPath();
            ctx.moveTo(pos1.X, pos1.Y);
            ctx.lineTo(pos2.X, pos2.Y);
            ctx.stroke();
        }
    }
}

function toggleActor(actor) {
    world.actors[actor].disabled = !world.actors[actor].disabled;
    drawMap();
}

var mapimage;

function setupMapImage(map_path) {
    mapimage = new Image(2048, 2048);
    mapimage.src = map_path;
    mapimage.addEventListener("load", () => {
        drawMap();
        canvas.style.display = "Block";
        canvas.addEventListener("wheel", (e) => {
            const oldZoom = imgZoom;
            const canvasCenterX = canvas.width / 2;
            const canvasCenterY = canvas.height / 2;

            e.deltaY < 0 ? imgZoom *= zoomMult : imgZoom /= zoomMult;

            img_pos.X = canvasCenterX - ((canvasCenterX - img_pos.X) / oldZoom) * imgZoom;
            img_pos.Y = canvasCenterY - ((canvasCenterY - img_pos.Y) / oldZoom) * imgZoom;
            drawMap();
        });

    });
}

function maindraw() {
    setupMapImage(world.minimap_path);
    actornames = Object.keys(world.actors);
    zone_pos.X = world.camera.pos.X;
    zone_pos.Y = world.camera.pos.Y;
    actorButtons.innerHTML = "";
    drawPois = false;
    drawZiplines = false;

    for (let i = 0; i < world.actors.length; i++) {
        if (world.actors[i].positions.length == 0) continue;

        world.actors[i].disabled = true;
        actorButtons.innerHTML += `<input id="toggle-${i}" onclick="toggleActor(${i})" type="checkbox"><label onclick="document.getElementById('toggle-${i}').click()">${world.actors[i].name}</label><br>`;

        actorimages[world.actors[i].name] = (() => {
            const actorimg = new Image();
            actorimg.src = `./Images/${world.actors[i].name}.png`;
            actorimg.addEventListener("load", () => drawMap());
            return actorimg;
        })();
    }

    actorButtons.innerHTML += `<input id="toggle-poi" onclick="drawPois = !drawPois;drawMap();" type="checkbox"><label onclick="document.getElementById('toggle-poi').click()">Draw POIs</label><br>`;
    actorButtons.innerHTML += `<input id="toggle-zipline" onclick="drawZiplines = !drawZiplines;drawMap();" type="checkbox"><label onclick="document.getElementById('toggle-zipline').click()">Draw Ziplines</label><br>`;

}

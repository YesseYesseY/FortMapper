/** @type HTMLCanvasElement */
const canvas = document.getElementById("canvas");
/** @type HTMLDivElement */
const actorButtons = document.getElementById("actor-buttons");
/** @type CanvasRenderingContext2D */
const ctx = canvas.getContext("2d");

canvas.width = window.innerWidth;
canvas.height = window.innerHeight;
window.addEventListener("resize", (e) => {
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;
    drawMap();
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

// TODO: Rotation, works fine on og but not main br
function img_to_world(pos) {
    return {
        X: -(((pos.Y - img_pos.Y) / (baseImageSize * imgZoom)) - 0.5) * world.camera.ortho_width + world.camera.pos.X,
        Y: (((pos.X - img_pos.X) / (baseImageSize * imgZoom)) - 0.5) * world.camera.ortho_width + world.camera.pos.Y,
    }
}

function drawMap() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.drawImage(mapimage, img_pos.X, img_pos.Y, baseImageSize * imgZoom, baseImageSize * imgZoom);

    ctx.font = "bold 18px arial";
    ctx.fillStyle = "black";
    if (drawPois) {
        for (const [poiname, poilocation] of Object.entries(world.pois)) {
            let map_pos = world_to_img(poilocation);
            ctx.textAlign = "center";
            ctx.fillText(poiname, map_pos.X, map_pos.Y);
        }
    }

    for (let j = 0; j < actornames.length; j++) {
        for (let i = 0; i < world.actors[actornames[j]].length; i++) {
            if (world.actors[actornames[j]].disabled) continue;

            let map_pos = world_to_img(world.actors[actornames[j]][i]);
            ctx.drawImage(actorimages[actornames[j]], map_pos.X - (iconSize / 2), map_pos.Y - (iconSize / 2), iconSize, iconSize);
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
    for (let i = 0; i < actornames.length; i++) {
        if (Object.values(world.actors[actornames[i]]).length == 0) continue;

        world.actors[actornames[i]].disabled = true;
        actorButtons.innerHTML += `<input id="toggle-${i}" onclick="toggleActor('${actornames[i]}')" type="checkbox"><label onclick="document.getElementById('toggle-${i}').click()">${actornames[i]}</label><br>`;

        actorimages[actornames[i]] = (() => {
            const actorimg = new Image();
            actorimg.src = `./Images/${actornames[i]}.png`;
            actorimg.addEventListener("load", () => drawMap());
            return actorimg;
        })();
    }

    actorButtons.innerHTML += `<input id="toggle-poi" onclick="drawPois = !drawPois;drawMap();" type="checkbox"><label onclick="document.getElementById('toggle-poi').click()">Draw POIs</label><br>`;

}

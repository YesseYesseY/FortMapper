/** @type HTMLCanvasElement */
const canvas = document.getElementById("canvas");
/** @type HTMLDivElement */
const actorButtons = document.getElementById("actor-buttons");
/** @type CanvasRenderingContext2D */
const ctx = canvas.getContext("2d");

canvas.width = window.innerWidth;
canvas.height = window.innerHeight;

const mapimage = new Image(2048, 2048);
mapimage.src = "./map.png";

var world = {};

const jsoninput = document.getElementById("jsoninput");
jsoninput.addEventListener("change", (e) => {
    const file = e.target.files[0];
    if (!file) return;

    const reader = new FileReader();

    reader.addEventListener("load", (e) => {
        world = JSON.parse(e.target.result);
        jsoninput.style.display = "None";
        canvas.style.display = "Block";
        actorButtons.style.display = "block";
        maindraw();

    });

    reader.readAsText(file);
});

const actorimages = {};
let actornames = [];

const manual_rotation = 90;
const zoomMult = 1.15;
const iconSize = 32;
const baseImageSize = 1000;

var imgX = canvas.width / 2 - (baseImageSize / 2);
var imgY = canvas.height / 2 - (baseImageSize / 2);
var imgZoom = 1;
var dragging = false;
var drawPois = false;

function get_map_pos(pos) {
    let relative = [pos.X - world.camera.pos.X, pos.Y - world.camera.pos.Y];

    let rot = -((world.camera.rot.Roll + manual_rotation) * (Math.PI / 180));
    let cos = Math.cos(rot);
    let sin = Math.sin(rot);
    let rotatedX = relative[0] * cos - relative[1] * sin;
    let rotatedY = relative[0] * sin + relative[1] * cos;

    return {
        x: (rotatedX / world.camera.ortho_width + 0.5) * baseImageSize * imgZoom,
        y: (rotatedY / world.camera.ortho_width + 0.5) * baseImageSize * imgZoom,
    }
}

function drawMap() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.drawImage(mapimage, imgX, imgY, baseImageSize * imgZoom, baseImageSize * imgZoom);

    ctx.font = "bold 18px arial";
    ctx.fillStyle = "black";
    if (drawPois) {
        for (const [poiname, poilocation] of Object.entries(world.pois)) {
            let map_pos = get_map_pos(poilocation);
            ctx.textAlign = "center";
            ctx.fillText(poiname, map_pos.x + imgX, map_pos.y + imgY);
            //ctx.strokeText(poiname, map_pos.x + imgX, map_pos.y + imgY);
        }
    }

    for (let j = 0; j < actornames.length; j++) {
        for (let i = 0; i < world.actors[actornames[j]].length; i++) {
            if (world.actors[actornames[j]].disabled) continue;

            let map_pos = get_map_pos(world.actors[actornames[j]][i]);
            ctx.drawImage(actorimages[actornames[j]], map_pos.x + imgX - (iconSize / 2), map_pos.y + imgY - (iconSize / 2), iconSize, iconSize);
        }
    }
}

function toggleActor(actor) {
    world.actors[actor].disabled = !world.actors[actor].disabled;
    drawMap();
}

function maindraw() {
    actornames = Object.keys(world.actors);

    for (let i = 0; i < actornames.length; i++) {
        world.actors[actornames[i]].disabled = true;
        actorButtons.innerHTML += `<input id="toggle-${i}" onclick="toggleActor('${actornames[i]}')" type="checkbox"><label onclick="document.getElementById('toggle-${i}').click()">${actornames[i]}</label><br>`;

        actorimages[actornames[i]] = (() => {
            const actorimg = new Image();
            actorimg.src = `./${actornames[i]}.png`;
            actorimg.addEventListener("load", () => drawMap());
            return actorimg;
        })();
    }

    actorButtons.innerHTML += `<input id="toggle-poi" onclick="drawPois = !drawPois;drawMap();" type="checkbox"><label onclick="document.getElementById('toggle-poi').click()">Draw POIs</label><br>`;

    canvas.addEventListener("wheel", (e) => {
        const oldZoom = imgZoom;
        const canvasCenterX = canvas.width / 2;
        const canvasCenterY = canvas.height / 2;

        e.deltaY < 0 ? imgZoom *= zoomMult : imgZoom /= zoomMult;

        imgX = canvasCenterX - ((canvasCenterX - imgX) / oldZoom) * imgZoom;
        imgY = canvasCenterY - ((canvasCenterY - imgY) / oldZoom) * imgZoom;
        drawMap();
    });
    canvas.addEventListener("mousedown", (e) => {
        canvas.style.cursor = "all-scroll";
        dragging = true;
    });
    canvas.addEventListener("mouseup", (e) => {
        canvas.style.cursor = "default";
        dragging = false;
    });
    canvas.addEventListener("mousemove", (e) => {
        if (dragging) {
            imgX += e.movementX;
            imgY += e.movementY;
            drawMap();
        }
    });
    window.addEventListener("resize", (e) => {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
        drawMap();
    });
}

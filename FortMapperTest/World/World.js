/** @type HTMLCanvasElement */
const canvas = document.getElementById("canvas");
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
        maindraw();

    });

    reader.readAsText(file);
});

const actorimages = {};
let actornames = [];

const manual_rotation = 90;
const zoomMult = 0.15;
const iconSize = 32;
const baseImageSize = 1000;

const canvasCenterX = canvas.width / 2;
const canvasCenterY = canvas.height / 2;

var imgX = canvas.width / 2 - (baseImageSize / 2);
var imgY = canvas.height / 2 - (baseImageSize / 2);
var imgZoom = 1;
var dragging = false;

function get_map_pos(pos) {
    let relative = [pos.X - world.camera_pos.X, pos.Y - world.camera_pos.Y];

    let rot = -((world.camera_rot.Roll + manual_rotation) * (Math.PI / 180));
    let cos = Math.cos(rot);
    let sin = Math.sin(rot);
    let rotatedX = relative[0] * cos - relative[1] * sin;
    let rotatedY = relative[0] * sin + relative[1] * cos;

    return {
        x: (rotatedX / world.camera_ortho_width + 0.5) * baseImageSize * imgZoom,
        y: (rotatedY / world.camera_ortho_width + 0.5) * baseImageSize * imgZoom,
    }
}

function drawMap() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.drawImage(mapimage, imgX, imgY, baseImageSize * imgZoom, baseImageSize * imgZoom);
    for (let j = 0; j < actornames.length; j++) {
        for (let i = 0; i < world.actors[actornames[j]].length; i++) {
            let map_pos = get_map_pos(world.actors[actornames[j]][i]);

            ctx.drawImage(actorimages[actornames[j]], map_pos.x + imgX - (iconSize / 2), map_pos.y + imgY - (iconSize / 2), iconSize, iconSize);
        }
    }
}

function maindraw() {
    actornames = Object.keys(world.actors);

    for (let i = 0; i < actornames.length; i++) {
        actorimages[actornames[i]] = (() => {
            const actorimg = new Image();
            actorimg.src = `./${actornames[i]}.png`;
            actorimg.addEventListener("load", () => drawMap());
            return actorimg;
        })();
    }

    canvas.addEventListener("wheel", (e) => {
        const oldZoom = imgZoom;

        e.deltaY < 0 ? imgZoom += zoomMult : imgZoom -= zoomMult;

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
    })
}

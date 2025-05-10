/** @type HTMLCanvasElement */
const canvas = document.getElementById("canvas");
/** @type CanvasRenderingContext2D */
const ctx = canvas.getContext("2d");

const mapimage = new Image(2048, 2048);
mapimage.src = "./map.png";

var world = {};

const jsoninput = document.getElementById("jsoninput");
jsoninput.addEventListener("change", (e) => {
    console.log(e);
    const file = e.target.files[0];
    if (!file) return;

    const reader = new FileReader();

    reader.addEventListener("load", (e) => {
        world = JSON.parse(e.target.result);
        jsoninput.style.display = "None";
        maindraw();
    });

    reader.readAsText(file);
});

const manual_rotation = 90;

function get_map_pos(pos) {
    let relative = [pos.X - world.camera_pos.X, pos.Y - world.camera_pos.Y];

    let rot = -((world.camera_rot.Roll + manual_rotation) * (Math.PI / 180));
    let cos = Math.cos(rot);
    let sin = Math.sin(rot);
    let rotatedX = relative[0] * cos - relative[1] * sin;
    let rotatedY = relative[0] * sin + relative[1] * cos;

    return {
        x: (rotatedX / world.camera_ortho_width + 0.5) * 2048,
        y: (rotatedY / world.camera_ortho_width + 0.5) * 2048,
    }
}

function maindraw() {
    ctx.drawImage(mapimage, 0, 0);
    let actornames = Object.keys(world.actors);
    for (let j = 0; j < actornames.length; j++) {
        ctx.fillStyle = '#' + Math.floor(Math.random() * 0xFFFFFF).toString(16).padStart(6, '0');
        for (let i = 0; i < world.actors[actornames[j]].length; i++) {
            let map_pos = get_map_pos(world.actors[actornames[j]][i]);
            ctx.beginPath();
            ctx.arc(map_pos.x, map_pos.y, 2.5, 0, 2 * Math.PI);
            ctx.fill();
        }
    }
}
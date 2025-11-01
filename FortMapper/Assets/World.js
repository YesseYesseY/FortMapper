const canvas = document.getElementById("main");
canvas.width = window.innerWidth;
canvas.height = window.innerHeight;

const zoomAmount = 1.15;

let movingCanvas = false;
let imgPos = {
    X: 0,
    Y: 0
}
let imgZoom = 1;

function waitForData() {
    if (typeof data === "undefined") {
        setTimeout(waitForData, 250);
    } else {
        main();
    }
}

function init() {
    var url = new URLSearchParams(window.location.search);
    var name = url.get("name");
    if (name === null) {
        console.error("No name specified in query");
    }

    const lol = document.createElement("script");
    lol.src = `World/${name}.json`;
    document.head.append(lol);
    waitForData();
}

function main() {
    window.addEventListener("resize", () => {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
        mainDraw();
    });
    
    canvas.addEventListener("wheel", (e) => {
        const oldZoom = imgZoom;
        if (e.deltaY < 0) {
            imgZoom *= zoomAmount;
        } else {
            imgZoom /= zoomAmount;
        }
    
        const canvasCenterX = canvas.width / 2;
        const canvasCenterY = canvas.height / 2;
    
        imgPos.X = canvasCenterX - ((canvasCenterX - imgPos.X) / oldZoom) * imgZoom;
        imgPos.Y = canvasCenterY - ((canvasCenterY - imgPos.Y) / oldZoom) * imgZoom;
        mainDraw();
    });
    
    canvas.addEventListener("mousedown", () => {
        canvas.style.cursor = "all-scroll";
        movingCanvas = true;
    });
    
    // If the event is on canvas then it won't detect mouseup on when outside of it which is just weird
    window.addEventListener("mouseup", () => {
        canvas.style.cursor = "default";
        movingCanvas = false;
    });
    
    canvas.addEventListener("mousemove", (e) => {
        if (!movingCanvas) return;
    
        imgPos.X += e.movementX;
        imgPos.Y += e.movementY;
        mainDraw();
        // console.log(e);
    });
    
    const ctx = canvas.getContext("2d");
    ctx.fillRectCentered = function(x, y, w, h) {
        ctx.fillRect(x - (w / 2), y - (h / 2), w, h);
    };
    
    const mapimg = new Image();
    mapimg.src = `./World/${data.DisplayName}.png`;
    mapimg.onload = () => {
        mainDraw();
    };

    function vecSub(vec1, vec2) {
        return {
            X: vec1.X - (vec2.X ?? 0),
            Y: vec1.Y - (vec2.Y ?? 0),
            Z: vec1.Z - (vec2.Z ?? 0)
        }
    }
    
    function rotatePos(pos, rot) {
        rot = -(rot * (Math.PI / 180));
        let cos = Math.cos(rot);
        let sin = Math.sin(rot);
        return {
            X: pos.X * cos - pos.Y * sin,
            Y: pos.X * sin + pos.Y * cos
        };
    }
    
    function worldToImg(pos) {
        const rotPos = rotatePos(vecSub(pos, data.Camera.Position), 90);
        return {
            X: ((rotPos.X / data.Camera.Width + 0.5) * 2048 * imgZoom) + imgPos.X,
            Y: ((rotPos.Y / data.Camera.Width + 0.5) * 2048 * imgZoom) + imgPos.Y
        }
    }
    
    function mainDraw() {
        ctx.fillStyle = "white";
        ctx.fillRect(0, 0, canvas.width, canvas.height)
    
        const imgSize = 2048 * imgZoom
        ctx.drawImage(mapimg, imgPos.X, imgPos.Y, imgSize, imgSize);
    
        ctx.fillStyle = "red";
        data.Actors.Tiered_Chest_6_Figment_C.forEach(e => {
            const pointPos = worldToImg(e);
            ctx.fillRectCentered(
                pointPos.X, pointPos.Y,
                5, 5
            );
        });
    }
}

init();

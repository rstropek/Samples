import { Planes, Radar } from "radar";
import p5 from "p5";

let planes: Planes;
let radar: Radar;

const p = new p5((sketch) => {
    sketch.setup = setup;
    sketch.draw = draw;
});

function setup() {
    planes = Planes.new();
    radar = Radar.new();

    p.createCanvas(1000, 1000);

    setInterval(() => {
        radar.update();
    }, 25);
}

function draw() {
    p.background('lightblue');

    p.push();
    p.stroke('black');
    p.noFill();
    p.circle(radar.center.x, radar.center.y, radar.reach_radius * 2);
    p.pop();

    p.push();
    p.noStroke();
    p.fill('red');
    const current_angle = radar.current_angle; // in degrees; 0 = north
    const beam_width_angle = radar.beam_width_angle; // in degrees
    const center = radar.center;
    const startAngle = p.radians(current_angle - beam_width_angle/2 + 270); // Adjust to p5 coordinate system
    const endAngle = p.radians(current_angle + beam_width_angle/2 + 270);
    p.arc(center.x, center.y, radar.reach_radius * 2, radar.reach_radius * 2, startAngle, endAngle);
    p.pop();

    p.push();
    p.noStroke();
    p.fill('white');
    for (let i = 0; i < planes.number_of_planes(); i++) {
        const pl = planes.get_plane(i);

        if (radar.can_detect(pl.position)) {
            p.fill('black');
        } else {
            p.fill('white');
        }

        p.circle(pl.position.x, pl.position.y, 10);
        pl.free();
    }
    p.pop();
}
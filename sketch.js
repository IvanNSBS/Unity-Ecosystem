// The Nature of Code
// Daniel Shiffman
// http://natureofcode.com

// Seeking "vehicle" follows the mouse position

// Implements Craig Reynold's autonomous steering behaviors
// One vehicle "seeks"
// See: http://www.red3d.com/cwr/

let v;
let predator;
let targets = []
let preys = []

function rnd_int_in_range(min, max) {
  return Math.floor(Math.random() * (max - min) ) + min;
}

function setup() {
  createCanvas(640, 360);
  v = new Vehicle(width / 2, height / 2, createVector(0,220,0), -2, 5.5);
  predator = new Vehicle(width/4, height/4, createVector(220,0,0), -0.5, 3)
  preys[0] = v.position;
  for(var i = 0; i < 20; i++)
  {
    var x = rnd_int_in_range(60, width);
    var y = rnd_int_in_range(60, height);
    targets.push(createVector(x, y));
  }
}

function draw() {
  background(51);
  // Call the appropriate steering behaviors for our agents

  let mouse = createVector(mouseX, mouseY);
  if(random(1) < 0.01)
  {
    var x = rnd_int_in_range(60, width);
    var y = rnd_int_in_range(60, height);
    targets.push(createVector(x, y));
  }

  for(var i = targets.length - 1; i >= 0; i--)
  {
    // Draw an ellipse at the mouse position
    fill(0,255,0);
    stroke(0,180,0);
    strokeWeight(1);
    ellipse(targets[i].x, targets[i].y, 10, 10);
  }

  v.apply_behaviours(targets);
  
  v.update();
  v.display();
  // var seek = predator.seek_n_arrive(v.position, 10, 4000);
  predator.apply_behaviours(preys);
  predator.update();
  predator.display();
}
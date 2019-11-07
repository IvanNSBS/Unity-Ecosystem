// The Nature of Code
// Daniel Shiffman
// http://natureofcode.com

// Seeking "vehicle" follows the mouse position

// Implements Craig Reynold's autonomous steering behaviors
// One vehicle "seeks"
// See: http://www.red3d.com/cwr/

let v = [];
let predator;
let targets = []

let preys = []
let predators = []

function rnd_int_in_range(min, max) {
  return Math.floor(Math.random() * (max - min) ) + min;
}

function setup() {
  createCanvas(640, 360);
  
  for(var i = 0; i < 1; i++)
    v.push(new Vehicle(rnd_int_in_range(0,width), rnd_int_in_range(0,height), createVector(0,220,0), -2, 2, 100));

  predator = new Vehicle(width/4, height/4, createVector(220,0,0), -0.5, 3.2, 100);

  predators[0] = predator.position;

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

  for(var i = 0; i < v.length; i++)
  {
      v[i].apply_behaviours(targets, predators);
      v[i].update();
      v[i].display();
      preys[i] = v[i].position;
  }
  predators[0] = predator.position;
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

  for(var i = 0; i < v.length; i++)
  {
      v[i].apply_behaviours(targets, predators);
      v[i].update();
      v[i].display();
  }
  // var seek = predator.seek_n_arrive(v.position, 10, 4000);
  predator.apply_behaviours(preys, undefined);
  predator.update();
  predator.display();
}
// The Nature of Code
// Daniel Shiffman
// http://natureofcode.com

// Seeking "vehicle" follows the mouse position

// Implements Craig Reynold's autonomous steering behaviors
// One vehicle "seeks"
// See: http://www.red3d.com/cwr/

let v;
let targets = []

function setup() {
  createCanvas(640, 360);
  v = new Vehicle(width / 2, height / 2);

  for(var i = 0; i < 40; i++)
  {
    targets[i] = createVector(random(60, width), random(60,height));
  }
}

function draw() {
  background(51);
  // Call the appropriate steering behaviors for our agents
  for(var i = 0; i < targets.length; i++)
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
  
}
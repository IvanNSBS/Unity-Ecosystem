// The Nature of Code
// Daniel Shiffman
// http://natureofcode.com

// Seeking "vehicle" follows the mouse position

// Implements Craig Reynold's autonomous steering behaviors
// One vehicle "seeks"
// See: http://www.red3d.com/cwr/

/**
 * TODO: Make fleeing better
 *        - Use memory range (will flee until predator is within that range)
 *        - Reset wander point once start fleeing
 */

let preys = [];
let predators = [];
let targets = []

let predator_amnt = 1;
let prey_amnt = 3;

function rnd_int_in_range(min, max) {
  var val = Math.floor(Math.random() * (max - min) ) + min;
  if(val < min)
    val = min;
  if(val > max)
    val = max;
  return val;
}

function setup() {
  createCanvas(1280, 640);
  
  for(var i = 0; i < prey_amnt; i++)
    preys.push(new Vehicle( rnd_int_in_range(60, width), rnd_int_in_range(60, height), createVector(0,220,0), -2, 5.0, 80));

  for(var i = 0; i < predator_amnt; i++){
    predators.push(new Vehicle( rnd_int_in_range(60, width), rnd_int_in_range(60, height), createVector(220,0,0), -2, 8.0, 120));
  }


  for(var i = 0; i < 20; i++)
  {
    var x = rnd_int_in_range(60, width-60);
    var y = rnd_int_in_range(60, height-60);
    targets.push(createVector(x, y));
  }
}

function draw() {
  background(51);
  // Call the appropriate steering behaviors for our agents

  if(random(1) < 0.01)
  {
    var x = rnd_int_in_range(60, width-60);
    var y = rnd_int_in_range(60, height-60);
    targets.push(createVector(x, y));

    // var x2 = rnd_int_in_range(60, width-60);
    // var y2 = rnd_int_in_range(60, height-60);
    // targets.push(createVector(x2, y2));
  }



  
  for(var i = targets.length - 1; i >= 0; i--)
  {
    // Draw an ellipse at the mouse position
    fill(0,0,255);
    stroke(0,0,180);
    strokeWeight(1);
    ellipse(targets[i].x, targets[i].y, 10, 10);
  }


  for(var i = 0; i < preys.length; i++)
  {
    let avoids = [];
    for(var j = 0; j < preys.length; j++)
      if(j !== i)
        avoids.push(preys[j]);
    preys[i].apply_behaviours(targets, predators, avoids);
    preys[i].update();
    preys[i].display();
  }

  for(var i = 0; i < predators.length; i++){

    let avoids = [];
    for(var j = 0; j < predators.length; j++)
      if(j !== i)
        avoids.push(predators[j].position);

    predators[i].apply_behaviours(preys, undefined, avoids);
    predators[i].update();
    predators[i].display();
  }
  
  console.log("Prey length: ", preys.length);
  // var seek = predator.seek_n_arrive(v.position, 10, 4000);
}
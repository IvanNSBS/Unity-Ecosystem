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

let v = [];
let predator_s = [];
let targets = []

let preys = []
let predator_list = []

let predator_amnt = 15;
let prey_amnt = 200;

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
    v.push(new Vehicle( rnd_int_in_range(60, width), rnd_int_in_range(60, height), createVector(0,220,0), -2, 5.0, 80));

  for(var i = 0; i < predator_amnt; i++){
    predator_s.push(new Vehicle( rnd_int_in_range(60, width), rnd_int_in_range(60, height), createVector(220,0,0), -1.5, 3.0, 120));
    predator_list[i] = predator_s[predator_s.length - 1].position;
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


  for(var i = 0; i < predator_s.length; i++){
    predator_list[i] = predator_s[i].position;
  }

  if(random(1) < 0.8)
  {
    var x = rnd_int_in_range(60, width-60);
    var y = rnd_int_in_range(60, height-60);
    targets.push(createVector(x, y));

    var x2 = rnd_int_in_range(60, width-60);
    var y2 = rnd_int_in_range(60, height-60);
    targets.push(createVector(x2, y2));
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
    let avoids = [];
    for(var j = 0; j < preys.length; j++)
      if(j !== i)
        avoids.push(preys[j]);
    v[i].apply_behaviours(targets, predator_list, avoids);
    v[i].update();
    v[i].display();
    preys[i] = v[i].position;
  }

  for(var i = 0; i < predator_s.length; i++){
    predator_list[i] = predator_s[i].position;
  }

  for(var i = 0; i < predator_s.length; i++){

    let avoids = [];
    for(var j = 0; j < predator_s.length; j++)
      if(j !== i)
        avoids.push(predator_s[j].position);

    predator_s[i].apply_behaviours(preys, undefined, avoids);
    predator_s[i].update();
    predator_s[i].display();
  }
  

  // var seek = predator.seek_n_arrive(v.position, 10, 4000);
}
// The Nature of Code
// Daniel Shiffman
// http://natureofcode.com

// The "Vehicle" class

class Vehicle {
  constructor(x, y) {
    this.acceleration = createVector(0, 0);
    this.velocity = createVector(0, -2);
    this.position = createVector(x, y);
    this.r = 6;

    this.maxspeed = 5.5;
    this.maxforce = 0.25;
    this.maxbrake = 0.6;
    this.sight_range = 100; 

    this.wander_points = [];
  }

  // Method to update location
  update() {
    // Update velocity
    this.velocity.add(this.acceleration);
    // Limit speed
    this.velocity.limit(this.maxspeed);
    this.position.add(this.velocity);
    // Reset accelerationelertion to 0 each cycle
    this.acceleration.mult(0);
  }

  applyForce(force) {
    // We could add mass here if we want A = F / M
    this.acceleration.add(force);
  }

  // A method that calculates a steering force towards a target
  // STEER = DESIRED MINUS VELOCITY
  seek_n_arrive(target, arrive_tol, sight_radius) {

    var desired = p5.Vector.sub(target, this.position); // A vector pointing from the location to the target
    var distance = desired.mag();

    if(distance < arrive_tol){
      var actual = this.maxspeed*distance/arrive_tol; 
      desired.setMag( actual );
      var steer = p5.Vector.sub(desired, this.velocity);
      steer.limit(this.maxbrake); // Limit to maximum steering force
      console.log("arrived!")
      return steer;
    }
    else if(distance < sight_radius){
      desired.setMag(this.maxspeed);
      var steer = p5.Vector.sub(desired, this.velocity);
      steer.limit(this.maxforce); // Limit to maximum steering force
      console.log("wandering to ( " + target.x + ", " + target.y + " )");
      return steer;
    }
    return createVector(0,0);
    // this.applyForce(steer);
  }

  seek_n_arrive_multiple(target_list, arrive_tol, sight_radius)
  {
    var biggest_d = Infinity;
    var target_idx = null;
    for(var i = target_list.length - 1; i >= 0; i--)
    {
      var d = p5.Vector.dist(this.position, target_list[i]);
      if(d < biggest_d){
        biggest_d = d;
        target_idx = i;
      }
    }
    if(biggest_d < arrive_tol && target_list.length > 0)
    {
      target_list.splice(target_idx, 1);
      return createVector(0,0);
    }
    if(target_idx != null)
      return this.seek_n_arrive(target_list[target_idx], 50, sight_radius);
  }

  wander()
  {
    if(this.wander_points.length === 0)
    {
      this.wander_points.push(createVector(random(60, width), random(60,height)));
      this.wander_points.push(createVector(random(60, width), random(60,height)));
    }
    else if(this.wander_points.lenght === 1)
      this.wander_points.push(createVector(random(60, width), random(60,height)));
    
    if(this.wander_points !== null && this.wander_point.length > 0)
      console.log("wander point[0] = ( " + this.wander_points[0].x + ", " + this.wander_points[0].y + " )");

    return this.seek_n_arrive_multiple(this.wander_points, 40, 10000);
  }

  stay_within_walls(tolerance)
  {
    let desired = null;

    if (this.position.x < tolerance) {
      desired = createVector(this.maxspeed, this.velocity.y);
    } else if (this.position.x > width - tolerance) {
      desired = createVector(-this.maxspeed, this.velocity.y);
    }

    if (this.position.y < tolerance) {
      desired = createVector(this.velocity.x, this.maxspeed);
    } else if (this.position.y > height - tolerance) {
      desired = createVector(this.velocity.x, -this.maxspeed);
    }

    if (desired !== null) {
      desired.normalize();
      desired.mult(this.maxspeed);
      let steer = p5.Vector.sub(desired, this.velocity);
      steer.limit(this.maxforce);
      return steer;
    }

    return createVector(0,0);
  }

  apply_behaviours(target)
  {
    var sna = null;
    if(target != null && !Array.isArray(target))
      sna = this.seek_n_arrive(target, 100, this.sight_radius);
    else if(target != null)
      sna = this.seek_n_arrive_multiple(target, 10, this.sight_range);

    var within = this.stay_within_walls(30);
    if(within.mag() === 0)
    {
      //cant see anything to seek
      if(sna == null || sna.mag() === 0)
      {
        this.applyForce(this.wander());
      }
      //seek target
      
      else
      {
        this.wander_point = [];
        this.applyForce(sna);
      }
    }
    else
      this.applyForce(within);
  }
  
  display() {
    // Draw a triangle rotated in the direction of velocity
    //draw sight_radius
    fill(150,150,150, 100);
    stroke(100,100,100,100);
    strokeWeight(1);
    ellipse(this.position.x, this.position.y, this.sight_range, this.sight_range);

    let theta = this.velocity.heading() + PI / 2;
    fill(127,0,0);
    stroke(0,0,200);
    strokeWeight(1);

    
    translate(this.position.x, this.position.y);
    rotate(theta);

    beginShape();
    vertex(0, -this.r * 2);
    vertex(-this.r, this.r * 2);
    vertex(this.r, this.r * 2);
    endShape(CLOSE);

    pop();
  }
}
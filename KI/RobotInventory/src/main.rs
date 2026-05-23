use robot::XyRobot;
use screw_shelf::ScrewShelf;

mod screw_shelf;
mod robot;

fn main() {
    let shelf = ScrewShelf::new(10, 10, 80.0, 100.0);

    let mut robot = XyRobot::new(4.0, 4.0, 120.0);
    robot.home();

    loop {
        let current_position = robot.get_current_position();
        if current_position.is_none() {
            panic!("Robot is not homed, this should never happen");
        }

        let current_position = current_position.unwrap();

        let ix = shelf.get_shelf_ix(current_position);
        if ix.is_err() {
            panic!("Robot is not in a valid shelf position, this should never happen. Maybe a missing homing operation?");
        }

        let ix = ix.unwrap();

        println!("Currently the robot is at shelf {}/{}", ix.0 + 1, ix.1 + 1);

        let mut target_x = String::new();
        let mut target_y = String::new();
        let target_ix: (u16, u16);

        loop {
            println!("Enter target x index (1-10): ");
            target_x.clear();
            std::io::stdin().read_line(&mut target_x).expect("Failed to read line");
            
            println!("Enter target y index (1-10): ");
            target_y.clear();
            std::io::stdin().read_line(&mut target_y).expect("Failed to read line");

            // Parse and convert from 1-based to 0-based indexing
            let x = match target_x.trim().parse::<u16>() {
                Ok(num) if (1..=10).contains(&num) => num - 1,
                _ => {
                    println!("Invalid x index. Please enter a number between 1 and 10.");
                    continue;
                }
            };

            let y = match target_y.trim().parse::<u16>() {
                Ok(num) if (1..=10).contains(&num) => num - 1,
                _ => {
                    println!("Invalid y index. Please enter a number between 1 and 10.");
                    continue;
                }
            };

            target_ix = (x, y);
            break;
        }

        let target_position = shelf.get_shelf_position(target_ix).unwrap();
        match robot.get_rotation_for_movement(target_position) {
            Ok(m) => println!("Movement: {:?}", m),
            Err(e) => {
                println!("Error: {}", e);
            }
        }

        robot.ack_position(target_position);
    }
}

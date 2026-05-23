pub struct XyRobot {
    current_position: Option<(f32, f32)>, //mm
    lead_x: f32, //mm/rev
    lead_y: f32, //mm/rev
    max_rpm: f32, //rpm
}

#[derive(Default, Debug)]
pub struct Movement {
    rotation_x: f32,
    rpm_x: f32,
    rotation_y: f32,
    rpm_y: f32,
    movement_duration: f32, //seconds
    speed: f32, //mm/s
}

impl XyRobot {
    pub fn new(lead_x: f32, lead_y: f32, max_rpm: f32) -> Self {
        Self { current_position: None, lead_x, lead_y, max_rpm }
    }

    pub fn home(&mut self) {
        self.current_position = Some((0.0, 0.0));
    }

    pub fn get_rotation_for_movement(&self, target_position: (f32, f32)) -> Result<Movement, &'static str> {
        if self.current_position.is_none() {
            return Err("Robot is not homed");
        }

        let mut result = Movement::default();

        let dx = target_position.0 - self.current_position.unwrap().0;
        let dy = target_position.1 - self.current_position.unwrap().1;
        if dx == 0.0 && dy == 0.0 {
            return Ok(result);
        }

        let distance = ((dx).powi(2) + (dy).powi(2)).sqrt();

        result.rotation_x = dx / self.lead_x;
        result.rotation_y = dy / self.lead_y;

        let duration_x = result.rotation_x.abs() / (self.max_rpm / 60.0);
        let duration_y = result.rotation_y.abs() / (self.max_rpm / 60.0);

        if duration_x > duration_y {
            result.movement_duration = duration_x;
            result.rpm_x = self.max_rpm;
            result.rpm_y = self.max_rpm * duration_y / duration_x;
        } else {
            result.movement_duration = duration_y;
            result.rpm_x = self.max_rpm * duration_x / duration_y;
            result.rpm_y = self.max_rpm;
        }

        result.speed = distance / result.movement_duration;
        Ok(result)
    }

    pub fn ack_position(&mut self, position: (f32, f32)) {
        self.current_position = Some(position);
    }

    pub fn get_current_position(&self) -> Option<(f32, f32)> {
        self.current_position
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use rstest::rstest;

    #[test]
    fn test_new_robot() {
        let robot = XyRobot::new(5.0, 4.0, 3000.0);
        assert!(robot.get_current_position().is_none());
        assert_eq!(robot.lead_x, 5.0);
        assert_eq!(robot.lead_y, 4.0);
    }

    #[test]
    fn test_home_robot() {
        let mut robot = XyRobot::new(5.0, 4.0, 3000.0);
        robot.home();
        assert_eq!(robot.get_current_position(), Some((0.0, 0.0)));
    }

    #[test]
    fn test_get_rotation_unhomed_robot() {
        let robot = XyRobot::new(5.0, 4.0, 600.0);
        let result = robot.get_rotation_for_movement((10.0, 8.0));
        assert!(result.is_err());
    }

    #[rstest]
    #[case((100.0, 120.0), 20.0, 30.0, 30.0, 40.0, 60.0)]  // y dominant case
    #[case((150.0, 80.0), 30.0, 20.0, 30.0, 60.0, 40.0)]   // x dominant case
    #[case((-150.0, -80.0), -30.0, -20.0, 30.0, 60.0, 40.0)] // negative movement case
    fn test_get_rotation(
        #[case] target: (f32, f32),
        #[case] expected_rotation_x: f32,
        #[case] expected_rotation_y: f32,
        #[case] expected_duration: f32,
        #[case] expected_rpm_x: f32,
        #[case] expected_rpm_y: f32
    ) {
        let mut robot = XyRobot::new(5.0, 4.0, 60.0);
        robot.home();
        
        let result = robot.get_rotation_for_movement(target);
        assert!(result.is_ok());
        let movement = result.unwrap();
        
        assert_eq!(movement.rotation_x, expected_rotation_x);
        assert_eq!(movement.rotation_y, expected_rotation_y);
        assert_eq!(movement.movement_duration, expected_duration);
        assert_eq!(movement.rpm_x, expected_rpm_x);
        assert_eq!(movement.rpm_y, expected_rpm_y);
        
        let expected_speed = (target.0.powi(2) + target.1.powi(2)).sqrt() / expected_duration;
        assert_eq!(movement.speed, expected_speed);
    }

    #[test]
    fn test_ack_position() {
        let mut robot = XyRobot::new(5.0, 4.0, 3000.0);
        robot.ack_position((15.0, 12.0));
        assert_eq!(robot.get_current_position(), Some((15.0, 12.0)));
    }

    #[test]
    fn test_get_rotation_no_movement() {
        let mut robot = XyRobot::new(5.0, 4.0, 60.0);
        robot.home();
        
        let result = robot.get_rotation_for_movement((0.0, 0.0));
        assert!(result.is_ok());
        let movement = result.unwrap();
        
        assert_eq!(movement.rotation_x, 0.0);
        assert_eq!(movement.rotation_y, 0.0);
        assert_eq!(movement.movement_duration, 0.0);
        assert_eq!(movement.rpm_x, 0.0);
        assert_eq!(movement.rpm_y, 0.0);
        assert_eq!(movement.speed, 0.0);
    }
}

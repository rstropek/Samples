import { WebApi } from './webapi';

(() => {
  const api = new WebApi();
  api.getCourses((cerr, courses) => {
    if (cerr) {
      console.error(cerr);
      process.exit();
    }

    for (const course of courses) {
      api.getAttendees(course.id, (aerr, attendees) => {
        if (aerr) {
          console.error(aerr);
          process.exit();
        }

        if (!attendees || !attendees.length) {
          api.cancelCourse(course.id, (cancelerr) => {
            if (cancelerr) {
              console.error(cancelerr);
              process.exit();
            }

            console.log(
                `Course ${course.id} ${course.name} has been cancelled`);
          });
        }
      });
    }
  });
})();

(async () => {
  try {
    const api = new WebApi();
    const courses = await api.getCourses();
    for (var course of courses) {
      const attendees = await api.getAttendees(course.id);
      if (!attendees || !attendees.length) {
        await api.cancelCourse(course.id);
        console.log(`Course ${course.id} ${course.name} has been cancelled`);
      }
    }
  } catch (err) {
    console.error(err);
  }
})();

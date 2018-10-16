import { Observable } from "rxjs";

export interface ICourse {
  id: number;
  name: string;
}

export interface IAttendee {
  firstName: string;
  lastName: string;
}

const courses: ICourse[] = [{id: 1, name: 'Angular'}, {id: 2, name: 'Blazor'}, {id: 3, name: 'Cobol'}];
const attendees = new Map<number, IAttendee[]>([
    [1, [{firstName: 'Jane', lastName: 'Smith'}]],
    [2, [{firstName: 'John', lastName: 'Doe'}]],
    [3, []]
]);;

const invalidCourseIdError = new Error('Invalid course ID');
const courseCannotBeCancelledError = new Error('Course cannot be cancelled');

export class WebApi {
  public getCourses(callback: (err: any, courses: ICourse[]) => void): void;
  public getCourses(): Promise<ICourse[]>;
  public getCourses(callback?: (err: any, courses: ICourse[]) => void) {
      if (callback) {
          setTimeout(() => callback(null, courses), 250);
          return;
      } else {
          return new Promise<ICourse[]>((resolve) => {
              setTimeout(() => resolve(courses), 250);
          });
      }
  }

  public getCourses$(): Observable<ICourse[]> {
    return new Observable<ICourse[]>(observer => {
      setTimeout(() => {
        observer.next(courses);
        observer.complete();
      }, 250);
    });
  }

  public getAttendees(courseId: number, callback: (err: any, attendees: IAttendee[]) => void): void;
  public getAttendees(courseId: number): Promise<IAttendee[]>;
  public getAttendees(courseId: number, callback?: (err: any, attendees: IAttendee[]) => void) {
      const result: IAttendee[] = attendees.get(courseId);
      if (callback) {
          setTimeout(() => {
              if (result) callback(null, result);
              else callback(invalidCourseIdError, null);
          }, 250);
          return;
      } else {
          return new Promise<IAttendee[]>((resolve, reject) => {
              setTimeout(() => {
                  if (result) resolve(result);
                  else reject(invalidCourseIdError);
              }, 250);
          })
      }
  }

  public getAttendees$(courseId: number): Observable<IAttendee[]> {
    return new Observable<IAttendee[]>(observer => {
      setTimeout(() => {
        const result: IAttendee[] = attendees.get(courseId);
        if (result) {
          observer.next(result);
          observer.complete();
        } else {
          observer.error(invalidCourseIdError);
        }
      }, 250);
    });
  }

  public cancelCourse(courseId: number, callback: (err: any) => void): void;
  public cancelCourse(courseId: number): Promise<void>;
  public cancelCourse(courseId: number, callback?: (err: any) => void) {
      if (callback) {
          setTimeout(() => {
              if (!attendees.get(courseId).length) callback(null);
              else callback(courseCannotBeCancelledError);
          }, 250);
          return;
      } else {
          return new Promise<void>((resolve, reject) => {
              setTimeout(() => {
                  if (!attendees.get(courseId).length) resolve();
                  else reject(courseCannotBeCancelledError);
              }, 250);
          })
      }
  }

  public cancelCourse$(courseId: number): Observable<void> {
    return new Observable<void>(observer => {
      setTimeout(() => {
        if (!attendees.get(courseId).length) {
          observer.next();
          observer.complete();
        } else {
          observer.error(courseCannotBeCancelledError);
        }
      }, 250);
    });
  }
}
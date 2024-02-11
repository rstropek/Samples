import fs from "fs";
import path from "path";

export interface Session {
  name: string;
  localizedStartDate: string;
  localizedEndDate: string;
  details: string;
  sessionType: SessionType;
  longAbstract: string;
  workshopShortLabel: string;
  roomName: string;
  language: string;
  slugNames: string[];
  primaryTrack: PrimaryTrack;
  tracks: Track[];
  experts: Expert[];
}

export interface SessionType {
  type: string;
  name: string;
  slug: string;
}

export interface PrimaryTrack {
  name: string;
  slug: string;
}

export interface Track {
  name: string;
  uniqueId: string;
  slug: string;
}

export interface Expert {
  surname: string;
  forename: string;
  company: string;
  bio: string;
  slug: string;
}

export class ConferenceProgram {
    public static readFromFile(): ConferenceProgram {
        const sessionsFile = fs.readFileSync(path.join(__dirname, "sessions.json"), "utf8");
        return new ConferenceProgram(JSON.parse(sessionsFile));
    }

    constructor(public sessions: Session[]) { }

    public getSessionTitles(): string[] {
        return this.sessions
            .filter(session => !!session.name)
            .map((session) => session.name);
    }
}

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

export interface ExpertSummary {
    surname: string;
    forename: string;
    company: string;
}

export type ExpertFilter = {
    surname?: string;
    forename?: string;
};

export type FindSessionFilter = {
    surname?: string;
    forename?: string;
    title: string;
};

export interface SessionSummary {
    name: string;
    localizedStartDate: string;
    localizedEndDate: string;
    sessionTypeName: string;
    experts: string[];
  }

export class ConferenceProgram {
    public static readFromFile(): ConferenceProgram {
        const sessionsFile = fs.readFileSync(path.join(__dirname, "sessions.json"), "utf8");
        return new ConferenceProgram(JSON.parse(sessionsFile));
    }

    constructor(public sessions: Session[]) {}

    public findExperts(filter?: ExpertFilter): ExpertSummary[] {
        const expertNameMap = new Map<string, ExpertSummary>();
        this.sessions
            .filter((s) => !!s.experts)
            .flatMap((s) => s.experts)
            .filter((e) => !filter || ((filter.forename === undefined || e!.forename === filter.forename) && (filter.surname === undefined || e!.surname === filter.surname)))
            .map((e) => {
                return { forename: e.forename, surname: e.surname, company: e.company };
            })
            .forEach((e) => expertNameMap.set(`${e.forename} ${e.surname}`, e));
        return Array.from(expertNameMap.values());
    }

    public findSessions(filter?: FindSessionFilter): SessionSummary[] {
        let summaries = this.sessions
            .filter((s) => !!s.experts)
            .map((s) => {
                return {
                    name: s.name,
                    localizedStartDate: s.localizedStartDate,
                    localizedEndDate: s.localizedEndDate,
                    sessionTypeName: s.sessionType?.name,
                    experts: s.experts.map((e) => {
                        return { surname: e.surname, forename: e.forename, company: e.company };
                    }),
                };
            });

        if (filter) {
            if (filter.title) {
                summaries = summaries.filter((s) => s.name.indexOf(filter.title) != -1);
            }
            if (filter.forename || filter.surname) {
                summaries = summaries.filter((s) =>
                    s.experts.some((e) => (filter.forename === undefined || e.forename === filter.forename) && (filter.surname === undefined || e.surname === filter.surname))
                );
            }
        }

        return summaries.map((s) => {
            return {
                name: s.name,
                localizedStartDate: s.localizedStartDate,
                localizedEndDate: s.localizedEndDate,
                sessionTypeName: s.sessionTypeName,
                experts: s.experts.map((e) => `${e.forename} ${e.surname}`),
            };
        });
    }
}

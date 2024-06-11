export type ProgramActions = {
  actions: Action[];
};

export type Action = FindExpertAction | FindSessionAction | UnknownAction;

export type FindExpertAction = {
  actionType: "find expert";
  filter?: ExpertFilter;
};

export type ExpertFilter = {
  surname?: string;
  forename?: string;
};

export type FindSessionAction = {
  actionType: "find session";
  filter?: ExpertFilter | SessionTitleFilter;
};

export type SessionTitleFilter = {
  // string that must be part of the session title
  title: string;
};

// if the user types text that can not easily be understood as a calendar action, this action is used
export interface UnknownAction {
  actionType: "unknown";
  // text typed by the user that the system did not understand
  text: string;
}

export interface ExpertSummary {
  surname: string;
  forename: string;
  company: string;
}

export interface SessionSummary {
  name: string;
  localizedStartDate: string;
  localizedEndDate: string;
  sessionTypeName: string;
  experts: string[];
}

export type API = {
  findExperts(filter?: ExpertFilter): ExpertSummary[];
  findSessions(filter?: ExpertFilter | SessionTitleFilter): SessionSummary[];
  extractTitlesFromSessionSummaries(sessionSummaries: SessionSummary[]): string[];
  addSessionsToCalendar(sessionTitles: string[]): void;
};

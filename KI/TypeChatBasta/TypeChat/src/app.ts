import fs from "fs";
import path from "path";
import { createJsonTranslator, createLanguageModel, createProgramTranslator, evaluateJsonProgram, getData, processRequests } from "typechat";
import dotenv from "dotenv";

import { ConferenceProgram } from "./sessions";
import { ProgramActions, SessionSummary } from "./programActionsSchema";
import { argv } from "process";

dotenv.config({ path: path.join(__dirname, "../../.env") });

const conferenceProgram = ConferenceProgram.readFromFile();
const model = createLanguageModel(process.env);
const schema = fs.readFileSync(path.join(__dirname, "programActionsSchema.ts"), "utf8");

if (argv.length === 0 || argv[0] === "translate") {
    const translator = createJsonTranslator<ProgramActions>(model, schema, "ProgramActions");
    translator.validator.stripNulls = true;

    // Process requests interactively or from the input file specified on the command line
    processRequests("BASTA> ", undefined, async (request: any) => {
        const response = await translator.translate(request);
        if (!response.success) {
            console.log(response.message);
            return;
        }
        const programActions = response.data;
        console.log(JSON.stringify(programActions, undefined, 2));
        console.log("===================");
        for (const action of programActions.actions) {
            switch (action.actionType) {
                case "find expert":
                    console.log("Finding expert with filter", action.filter);
                    console.log(conferenceProgram.findExperts(action.filter));
                    break;
                case "find session":
                    console.log("Finding session with filter", action.filter);
                    console.log(conferenceProgram.findSessions(action.filter));
                    break;
                case "unknown":
                    console.log("Unknown action", action.text);
                    break;
            }
        }
    });
} else {
    const translator = createProgramTranslator(model, schema);
    translator.validator.stripNulls = true;

    processRequests("BASTA> ", undefined, async (request: any) => {
        const response = await translator.translate(request);
        if (!response.success) {
            console.log(response.message);
            return;
        }
        const program = response.data;
        console.log(getData(translator.validator.createModuleTextFromJson(program)));
        console.log("Running program:");
        const result = await evaluateJsonProgram(program, handleCall);
        console.log(result);

        async function handleCall(func: string, args: any[]): Promise<unknown> {
            console.log(`${func}(${args})`);
            switch (func) {
                case "findExperts":
                    return conferenceProgram.findExperts(args[0]);
                case "findSessions":
                    return conferenceProgram.findSessions(args[0]);
                case "addSessionsToCalendar":
                    console.log("Adding to calendar:");
                    for (const session of args[0]) {
                        console.log(`  * ${session}`);
                    }
                case "extractTitlesFromSessionSummaries":
                    return args[0].map((s: SessionSummary) => s.name);
            }
        }
    });
}

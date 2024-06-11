import express from "express";
import logger from "./logging.js";
import cors from "cors";
import health from "./health.js";
import longRunning from "./long-running.js";
import sse from "./server-sent-events.js";
import * as openai from "./openai.js";

logger.info("Creating or updating AI assistant");
const assistant = await openai.createOrUpdateAssistant();

const app = express();

// Add the following lines to log all incoming requests
// import pinoHTTP from "pino-http";
// app.use(pinoHTTP.default({ logger }));

app.use(cors());
app.use(express.static("public"));
app.use("/health", health);
app.use("/long-running", longRunning);
app.use("/sse", sse);
app.use("/openai", openai.route(assistant));

const PORT = process.env["PORT"] || 3000;
app.listen(PORT, () => {
  logger.info({ PORT }, "Listening");
});

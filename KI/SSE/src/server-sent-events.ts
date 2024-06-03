import express from "express";
import logger from "./logging.js";

const router = express.Router();

// Tip: Don't forget to look at the client-side network traffic with
// Chrome's DevTools to see how the server-sent events are being sent
// ("Network" section, "EventStream" tab).

router.get("/data-only", async (_, response) => {
  logger.info("Received SSE request");

  response.writeHead(200, { "Content-Type": "text/event-stream" });

  // We can send comments to the client by starting the line with a colon.
  // This might be useful for keeping the connection alive.
  response.write(`; Let's get started\n\n`);

  for (let i = 0; i < 10; i++) {
    logger.info(`Getting chunk ${i}`);
    await sleep(1000);

    response.write(`data: Data ${i}\n\n`);
    // Note that we have to end each message with TWO newlines.
    // Two consecutive lines starting with "data:" will be treated as two
    // lines (separated by \n) of the SAME message. Try it be commenting
    // out the previous line and replacing it with the following one:
    //response.write(`data: Data\ndata: ${i}\n\n`);
  }

  // We have to signal the end of the stream to the client (in this case by sending
  // and empty message). If we don't and simply end the response, EventSource's automatic
  // reconnection logic will kick in and the client will keep on reconnecting - which
  // will lead to executing the same operation over and over again. You can test that
  // by commenting out the following line and observing the server logs.
  //
  // Note that the following line does not contain a colon. In this case,
  // "data" is the field name and an empty string is the field value.
  response.write(`data\n\n`);

  logger.info("Streaming complete");
  response.end();
});

router.get("/custom-events", async (_, response) => {
  logger.info("Received SSE request");

  response.writeHead(200, { "Content-Type": "text/event-stream" });

  for (let i = 0; i < 10; i++) {
    logger.info(`Getting chunk ${i}`);
    await sleep(1000);

    // The first line contains the event name. The second line contains the data.
    // Note that you can mix custom events and data-only events.
    response.write(`event: ${i % 2 === 0 ? "even" : "odd"}\n`);
    response.write(`data: { "value": ${i} }\n\n`);
  }

  response.write(`event: eom\ndata\n\n`);

  logger.info("Streaming complete");
  response.end();
});

router.get("/custom-events-with-id", async (request, response) => {
  logger.info("Received SSE request");

  response.writeHead(200, { "Content-Type": "text/event-stream" });

  const lastEventId = request.header("Last-Event-Id");
  let i = 0;
  if (lastEventId) {
    i = parseInt(lastEventId, 10) + 1;
    logger.info(`Resuming from event ID ${i}`);
  }
  for (; i < 10; i++) {
    logger.info(`Getting chunk ${i}`);
    await sleep(1000);

    // The first line contains the event name. The second line contains the data.
    // Note that you can mix custom events and data-only events.
    response.write(`event: ${i % 2 === 0 ? "even" : "odd"}\n`);
    response.write(`data: { "value": ${i} }\n`);
    response.write(`id: ${i}\n\n`);

    // When we reach i == 5, we simulate an error by closing the connection
    // without sending the "end of message" marker.
    if (i === 5) {
      break;
    }
  }

  if (i === 10) {
    // Send the end of message marker only if we have sent all the messages.
    response.write(`event: eom\ndata\n\n`);
  }

  logger.info("Streaming complete");
  response.end();
});

function sleep(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

export default router;

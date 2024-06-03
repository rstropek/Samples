// Various healthchecks for the server

import express from 'express';

const router = express.Router();
router.get("/ping", (_, response) => response.send("pong"));

export default router;

import { Context, HttpRequest } from "@azure/functions"
import { ACCEPTED, BAD_REQUEST } from 'http-status-codes';
import { validateTokenAndGetUser } from "./auth";

//-- HELPER FUNCTIONS ------------------------------------------------------------------------------
//   Internal helper functions 
//
function wait(duration: number): Promise<void> {
    return new Promise<void>(res => setTimeout(() => res(), duration));
}

//-- ORDER PROCESSING FUNCTIONS---------------------------------------------------------------------
//   These functions are only simulating the processing of incoming orders. In real life, this
// would be where the interesting stuff happens (e.g. accessing backend databases, doing
// calculations, calling backend microservices, etc.). However, this demo focusses on communication
// protocolls and technologies. Therefore, the processing functions only simulate heavy lifting
// by waiting for some time.
//
export interface IOrder {
    customerId: number,
    product: string,
    amount: number,
    userId?: string
}

function isOrder(oAny: any): oAny is IOrder {
    const o = oAny as IOrder;
    return o.customerId !== undefined && o.product !== undefined && !!o.amount;
}

async function validateIncomingOrder(order: IOrder): Promise<boolean> {
    // Simulate some processing time. Assumption: Initial checking of
    // incoming order is rather fast (e.g. check of master data like 
    // referenced customer exists).
    await wait(100);

    return true;
}

async function processIncomingOrder(order: IOrder): Promise<boolean> {
    // Simulate processing time. Assumption: This is the heavy lifting
    // for order processing. Might include complex calculations, talking
    // to multiple slow backend services, executing complex queries, etc.
    await wait(2500);

    return true;
}

//-- HANDLER FUNCTIONS FOR AZURE FUNCTIONS ---------------------------------------------------------
//   Functions for handling Azure Functions invocations.

/**
 * HTTP-triggered function receiving orders from Angular app.
 */
export async function receiveIncomingOrder(context: Context, req: HttpRequest): Promise<void> {
    // Get user name from token
    const user = validateTokenAndGetUser(context, req);
    if (!user) { return; }

    // Make sure incoming order is a valid order. Validation is known to be pretty fast.
    const order = req.body;
    if (!isOrder(order) || !await validateIncomingOrder(order)) {
        context.res = { status: BAD_REQUEST };
        return;
    }

    // Add user from token to order
    Object.assign(order, { userId: user })

    // Send service bus message that triggers async processing of incoming order.
    context.bindings.outputSbQueue = req.body;

    context.res = { status: ACCEPTED };
};

/**
 * ServiceBus-triggered function doing long-running processing of order.
 */
export async function processOrder(context: Context, order: any): Promise<void> {
    // Trigger complex order processing
    if (await processIncomingOrder(order)) {
        // Order processing was successfull. Send notification to browser client
        // by using a SignalR output binding.
        context.bindings.signalRMessages = [{
            "target": "orderProcessed",
            "userId": order.userId,
            "arguments": [order]
        }];
    }
};

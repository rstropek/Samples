import { Context, HttpRequest } from "@azure/functions"
import { validateTokenAndGetUser } from "./auth";
import { UNAUTHORIZED, BAD_REQUEST } from "http-status-codes";

export async function negotiate(context: Context, req: HttpRequest, connectionInfo: any): Promise<void> {
    // Validate token. Note that this is a naive implementation. In practise, use OpenID Connect
    // ideally in conjunction with Azure Active Directory. This sample is not about auth, so we
    // keep it simple here.
    const user = validateTokenAndGetUser(context, req);
    if (!user) {
        context.res = { statusCode: UNAUTHORIZED };
        return;
    }

    // Verify that user from token and header are identical.
    if (user !== req.headers['x-ms-signalr-userid']) {
        context.res = { statusCode: BAD_REQUEST };
        return;
    }

    context.res.body = connectionInfo;
};

export async function receiveEvent(context: Context, req: HttpRequest): Promise<void> {
    // See also https://docs.microsoft.com/en-us/azure/azure-signalr/concept-upstream#request-header

    // Note that you should make sure that events come from Azure SignalR by configuring
    // Azure Managed Identity for SignalR upstream events. This sample is not about auth, so we
    // keep it simple here.
    
    const type = req.body.type || req.body.Type;
    if (type) {
        console.log(`Received type ${type}`);
        switch (type) {
            case 10:
                console.log('Connected');
                console.log(`Connection ID=${req.headers['x-asrs-connection-id']}, User ID=${req.headers['x-asrs-user-id']}`);
                break;
            case 11:
                console.log('Disconnected');
                console.log(`Connection ID=${req.headers['x-asrs-connection-id']}, User ID=${req.headers['x-asrs-user-id']}`);
                if (req.body.Error) {
                    console.error(req.body.Error);
                }
                break;
            case 1:
                console.log('Received message from client');
                console.log(`Target=${req.body.target}, Connection ID=${req.headers['x-asrs-connection-id']}, User ID=${req.headers['x-asrs-user-id']}`);
                break;
            default:
                break;
        }
    }
};
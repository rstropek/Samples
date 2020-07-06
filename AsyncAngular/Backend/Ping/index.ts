import { Context } from "@azure/functions"

export async function ping(context: Context): Promise<void> {
    context.res = { body: 'Pong' };
};
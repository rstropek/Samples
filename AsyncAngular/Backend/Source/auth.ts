import { Context, HttpRequest } from "@azure/functions";
import * as jwt from 'jsonwebtoken';
import { StatusCodes } from "http-status-codes";

// Obviously, NEVER store token signing secrets in code. This is for demo purposes only!!
const secretForTokenSigning = 'secret';

//-- HANDLER FUNCTIONS FOR AZURE FUNCTIONS ---------------------------------------------------------
//   Note that this is a naive implementation of a login mechanism because this
// sample should not demonstrate proper authentication. In practice, use OpenID Connect,
// preferrably with Azure Active Directory, to get access tokens.
//
export async function login(context: Context, req: HttpRequest): Promise<void> {
    // Get authorization header and make sure it is basic auth.
    const authHeaderBase64 = req.headers['Authorization'] || req.headers['authorization'];
    if (!authHeaderBase64 || !authHeaderBase64.startsWith('Basic ')) {
        context.res = { statusCode: StatusCodes.BAD_REQUEST };
        return;
    }

    // Decode Base64 encoded user:password
    const authHeader = Buffer.from(authHeaderBase64.substr(6), 'base64').toString();
    const indexOfColon = authHeader.indexOf(':');
    if (indexOfColon <= 0) {
        context.res = { statusCode: StatusCodes.BAD_REQUEST };
        return;
    }

    // Create a JWT access token.
    const token = jwt.sign({ sub: authHeader.substr(0, indexOfColon) }, secretForTokenSigning);
    context.res = { body: { accessToken: token }, headers: { 'content-type': 'application/json' } };
}

//-- HELPER FUNCTION FOR VALIDATING TOKENS ---------------------------------------------------------
//   Note that this is a naive implementation of a token validation function because this
// sample should not demonstrate proper authentication. In practice, use a ready-made JWT
// validation library together with OpenID Connect, preferrably with Azure Active Directory.
// 
export function validateTokenAndGetUser(context: Context, req: HttpRequest): string {
    // Get authorization header and make sure it is bearer auth.
    const authHeaderBase64 = req.headers['Authorization'] || req.headers['authorization'];
    if (!authHeaderBase64 || !authHeaderBase64.startsWith('Bearer ')) {
        context.res = { statusCode: StatusCodes.BAD_REQUEST };
        return '';
    }

    // Get and verify JWT token
    const token = authHeaderBase64.substr(7);
    const decodedToken: any = jwt.verify(token, secretForTokenSigning);
    if (!decodedToken || !decodedToken.sub) {
        context.res = { statusCode: StatusCodes.UNAUTHORIZED };
        return '';
    }

    // Get user (=sub) from token and return it.
    return decodedToken.sub;
}
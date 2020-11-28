const apiConfig = {
  b2cScopes: ["https://wishlistsappb2c.onmicrosoft.com/api/execute"],
  webApi: "https://wishlists.azurewebsites.net/api",
};

const b2cPolicies = {
  names: {
    signIn: "B2C_1_signin",
    forgotPassword: "B2C_1_reset",
  },
  authorities: {
    signIn: {
      authority:
        "https://wishlistsappb2c.b2clogin.com/wishlistsappb2c.onmicrosoft.com/B2C_1_signin",
    },
    forgotPassword: {
      authority:
        "https://wishlistsappb2c.b2clogin.com/wishlistsappb2c.onmicrosoft.com/B2C_1_reset",
    },
  },
  authorityDomain: "wishlistsappb2c.b2clogin.com",
};

const msalConfig = {
  auth: {
    clientId: "624f988c-fd38-461a-93f9-9be88085d9c8", // This is the ONLY mandatory field; everything else is optional.
    authority: b2cPolicies.authorities.signIn.authority, // Choose sign-up/sign-in user-flow as your default.
    knownAuthorities: [b2cPolicies.authorityDomain], // You must identify your tenant's domain as a known authority.
    redirectUri: "https://localhost:44314/", // You must register this URI on Azure Portal/App Registration. Defaults to "window.location.href".
    postLogoutRedirectUri: "https://localhost:44314/",
  },
  cache: {
    cacheLocation: "sessionStorage", // Configures cache location. "sessionStorage" is more secure, but "localStorage" gives you SSO.
    storeAuthStateInCookie: true, // If you wish to store cache items in cookies as well as browser cache, set this to "true".
  },
};

/**
 * Scopes you add here will be prompted for user consent during sign-in.
 * By default, MSAL.js will add OIDC scopes (openid, profile, email) to any login request.
 * For more information about OIDC scopes, visit:
 * https://docs.microsoft.com/azure/active-directory/develop/v2-permissions-and-consent#openid-connect-scopes
 */
const loginRequest = {
  scopes: [...apiConfig.b2cScopes],
};

/**
 * Scopes you add here will be used to request a token from Azure AD B2C to be used for accessing a protected resource.
 * To learn more about how to work with scopes and resources, see:
 * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/resources-and-scopes.md
 */
const tokenRequest = {
  scopes: [...apiConfig.b2cScopes], // e.g. ["https://fabrikamb2c.onmicrosoft.com/helloapi/demo.read"]
  forceRefresh: false, // Set this to "true" to skip a cached token and go to the server to get a new token
};

export { b2cPolicies, msalConfig, loginRequest, tokenRequest };

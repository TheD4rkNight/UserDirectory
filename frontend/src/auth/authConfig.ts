import { PublicClientApplication, type Configuration } from '@azure/msal-browser';

const tenantId = import.meta.env.VITE_ENTRA_TENANT_ID;
const clientId = import.meta.env.VITE_ENTRA_CLIENT_ID;
const apiScope = import.meta.env.VITE_API_SCOPE;

export const msalConfig: Configuration = {
  auth: {
    clientId: clientId || '00000000-0000-0000-0000-000000000000',
    authority: `https://login.microsoftonline.com/${tenantId ?? 'common'}`,
    redirectUri: window.location.origin,
    postLogoutRedirectUri: window.location.origin,
  },
  cache: {
    cacheLocation: 'sessionStorage',
  },
};

export const loginRequest = {
  scopes: apiScope ? [apiScope] : [],
};

export const msalInstance = new PublicClientApplication(msalConfig);

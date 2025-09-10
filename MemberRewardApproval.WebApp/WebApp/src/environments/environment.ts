export const environment = {
  production: false,
  apiBaseUrl: 'http://localhost:5172',
  azure: {
    tenantId: 'bf5fbc3e-afe3-4db0-a323-d90f4fa8c415',
    authorityBase: 'https://login.microsoftonline.com',
    // Angular SPA app registration
    spaClientId: 'f1999bd0-4d92-4823-b458-e0bfbc5372d1',
    redirectUri: 'http://localhost:4200',

    // Backend API scopes
    scopes: ['api://36542c62-9dd6-4ef8-a8d0-bc3711548751/mma-net-api.read'],
  },
};

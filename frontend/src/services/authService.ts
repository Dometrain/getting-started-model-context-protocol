import { PublicClientApplication, type AuthenticationResult, InteractionRequiredAuthError } from "@azure/msal-browser";
import { msalConfig, loginRequest } from "../config/msalConfig";

class AuthService {
  private msalInstance: PublicClientApplication;

  constructor() {
    this.msalInstance = new PublicClientApplication(msalConfig);
  }

  async initialize() {
    await this.msalInstance.initialize();
  }

  async login(): Promise<AuthenticationResult | null> {
    try {
      return await this.msalInstance.loginPopup(loginRequest);
    } catch (error) {
      console.error("Login failed:", error);
      throw error;
    }
  }

  async logout() {
    await this.msalInstance.logoutPopup();
  }

  async getToken(): Promise<string | null> {
    const accounts = this.msalInstance.getAllAccounts();
    if (accounts.length === 0) return null;

    try {
      const result = await this.msalInstance.acquireTokenSilent({
        ...loginRequest,
        account: accounts[0],
      });
      return result.accessToken;
    } catch (error) {
      if (error instanceof InteractionRequiredAuthError) {
        // Token expired or interaction required
        return null;
      }
      console.error("Token acquisition failed:", error);
      return null;
    }
  }

  getAccount() {
    const accounts = this.msalInstance.getAllAccounts();
    return accounts.length > 0 ? accounts[0] : null;
  }

  getInstance() {
    return this.msalInstance;
  }
}

export const authService = new AuthService();

import { useState, useEffect } from 'react';
import { useMsal, useIsAuthenticated } from '@azure/msal-react';
import { loginRequest } from '../config/msalConfig';
import type { UserProfile } from '../types/auth';

export const useAuth = () => {
  const { instance, accounts } = useMsal();
  const isAuthenticated = useIsAuthenticated();
  const [user, setUser] = useState<UserProfile | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [manualToken, setManualToken] = useState<string | null>(localStorage.getItem('mcp_manual_token'));
  const [isAuthEnabled, setIsAuthEnabled] = useState<boolean>(() => {
    const saved = localStorage.getItem('mcp_auth_enabled');
    return saved === null ? true : saved === 'true';
  });

  useEffect(() => {
    if (isAuthEnabled) {
      if (isAuthenticated && accounts.length > 0) {
        const account = accounts[0];
        setUser({
          name: account.name,
          username: account.username,
          localAccountId: account.localAccountId,
        });
      } else if (manualToken) {
        setUser({
          name: 'Manual User',
          username: 'manual@local',
        });
      } else {
        setUser(null);
      }
    } else {
      setUser(null);
    }
    setIsLoading(false);
  }, [isAuthenticated, accounts, manualToken, isAuthEnabled]);

  const login = async () => {
    try {
      await instance.loginPopup(loginRequest);
    } catch (error) {
      console.error('Login failed:', error);
    }
  };

  const setToken = (token: string) => {
    localStorage.setItem('mcp_manual_token', token);
    setManualToken(token);
  };

  const toggleAuth = (enabled: boolean) => {
    localStorage.setItem('mcp_auth_enabled', String(enabled));
    setIsAuthEnabled(enabled);
  };

  const logout = async () => {
    try {
      localStorage.removeItem('mcp_manual_token');
      setManualToken(null);
      if (isAuthenticated) {
        await instance.logoutPopup();
      }
    } catch (error) {
      console.error('Logout failed:', error);
    }
  };

  return {
    isAuthenticated: !isAuthEnabled || isAuthenticated || !!manualToken,
    user: isAuthEnabled ? user : null,
    isLoading,
    login,
    logout,
    setToken,
    manualToken,
    isAuthEnabled,
    toggleAuth
  };
};

export interface UserProfile {
  name?: string;
  username?: string;
  localAccountId?: string;
}

export interface AuthState {
  isAuthenticated: boolean;
  user: UserProfile | null;
  token: string | null;
  isLoading: boolean;
  error: string | null;
}

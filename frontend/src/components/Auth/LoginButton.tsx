import React, { useState } from 'react';
import { LogIn, LogOut, User, Key } from 'lucide-react';
import { useAuth } from '../../hooks/useAuth';

export const LoginButton: React.FC = () => {
  const { isAuthenticated, user, login, logout, isLoading, setToken, isAuthEnabled } = useAuth();
  const [showManual, setShowManual] = useState(false);
  const [tempToken, setTempToken] = useState('');

  if (!isAuthEnabled) {
    return null;
  }

  if (isLoading) {
    return (
      <div className="animate-pulse bg-slate-800 h-10 w-32 rounded-lg"></div>
    );
  }

  if (isAuthenticated && user) {
    return (
      <div className="flex items-center gap-4">
        <div className="flex items-center gap-2 bg-slate-800 px-3 py-1.5 rounded-full border border-slate-700">
          <User size={16} className="text-blue-400" />
          <span className="text-sm font-medium text-slate-200">{user.name || user.username}</span>
        </div>
        <button
          onClick={logout}
          className="flex items-center gap-2 text-slate-400 hover:text-white transition-colors duration-200"
          title="Logout"
        >
          <LogOut size={20} />
        </button>
      </div>
    );
  }

  return (
    <div className="flex flex-col items-end gap-2">
      <div className="flex items-center gap-2">
        <button
          onClick={() => setShowManual(!showManual)}
          className="flex items-center gap-2 bg-slate-800 hover:bg-slate-700 text-slate-300 px-4 py-2 rounded-lg font-medium transition-all duration-200 border border-slate-700"
        >
          <Key size={18} />
          <span>Manual Token</span>
        </button>
        <button
          onClick={login}
          className="flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-medium transition-all duration-200 shadow-lg shadow-blue-500/20"
        >
          <LogIn size={20} />
          <span>Sign in with Entra ID</span>
        </button>
      </div>
      
      {showManual && (
        <div className="absolute top-16 right-0 mt-2 p-4 bg-slate-800 border border-slate-700 rounded-xl shadow-2xl w-96 z-50 animate-in slide-in-from-top-2">
          <p className="text-xs text-slate-400 mb-2 font-medium">Paste the token from your PowerShell script:</p>
          <textarea
            value={tempToken}
            onChange={(e) => setTempToken(e.target.value)}
            className="w-full h-24 bg-slate-900 border border-slate-700 rounded-lg p-2 text-xs font-mono text-blue-300 mb-3 focus:outline-none focus:border-blue-500"
            placeholder="eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6In..."
          />
          <button
            onClick={() => {
              if (tempToken.trim()) {
                setToken(tempToken.trim());
                setShowManual(false);
              }
            }}
            className="w-full bg-blue-600 hover:bg-blue-500 text-white py-2 rounded-lg text-sm font-bold transition-colors"
          >
            Apply Token
          </button>
        </div>
      )}
    </div>
  );
};

import React from 'react';
import { Bot, Shield, ShieldOff } from 'lucide-react';
import { LoginButton } from '../Auth/LoginButton';
import { useAuth } from '../../hooks/useAuth';

interface AppLayoutProps {
  children: React.ReactNode;
}

export const AppLayout: React.FC<AppLayoutProps> = ({ children }) => {
  const { isAuthEnabled, toggleAuth } = useAuth();

  return (
    <div className="min-h-screen bg-slate-900 text-slate-100 flex flex-col">
      <header className="border-b border-slate-800 bg-slate-900/50 backdrop-blur-sm sticky top-0 z-10">
        <div className="max-w-5xl mx-auto px-4 h-16 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="bg-blue-600 p-2 rounded-xl">
              <Bot size={24} className="text-white" />
            </div>
            <div>
              <h1 className="text-lg font-bold tracking-tight">MCP Agent</h1>
              <p className="text-xs text-slate-400 font-medium uppercase tracking-wider">Modern Client</p>
            </div>
          </div>
          <div className="flex items-center gap-6">
            <div className="flex items-center gap-2">
              <label className="relative inline-flex items-center cursor-pointer">
                <input 
                  type="checkbox" 
                  checked={isAuthEnabled}
                  onChange={(e) => toggleAuth(e.target.checked)}
                  className="sr-only peer"
                />
                <div className="w-11 h-6 bg-slate-700 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full rtl:peer-checked:after:-translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:start-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
                <span className="ml-3 text-sm font-medium text-slate-300 flex items-center gap-1.5">
                  {isAuthEnabled ? (
                    <><Shield size={14} className="text-blue-400" /> Authenticated</>
                  ) : (
                    <><ShieldOff size={14} className="text-slate-500" /> Unauthenticated</>
                  )}
                </span>
              </label>
            </div>
            <LoginButton />
          </div>
        </div>
      </header>
      
      <main className="flex-1 flex flex-col max-w-5xl mx-auto w-full px-4 py-6 overflow-hidden">
        {children}
      </main>
      
      <footer className="py-4 text-center text-slate-500 text-xs border-t border-slate-800/50">
        &copy; {new Date().getFullYear()} MCP Course &bull; Powered by .NET 10 & React
      </footer>
    </div>
  );
};

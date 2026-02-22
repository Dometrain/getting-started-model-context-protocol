import React from 'react';
import { MessageList } from './MessageList';
import { MessageInput } from './MessageInput';
import { useChat } from '../../hooks/useChat';
import { useAuth } from '../../hooks/useAuth';
import { AlertCircle, Trash2, Info } from 'lucide-react';

export const ChatContainer: React.FC = () => {
  const { messages, isLoading, error, sendMessage, clearChat } = useChat();
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return (
      <div className="flex-1 flex flex-col items-center justify-center p-8 space-y-6">
        <div className="bg-slate-800/50 p-8 rounded-3xl border border-slate-700/50 max-w-md w-full text-center space-y-4">
          <div className="w-16 h-16 bg-blue-600/10 rounded-2xl flex items-center justify-center mx-auto mb-2">
            <Info size={32} className="text-blue-500" />
          </div>
          <h2 className="text-2xl font-bold">Welcome to MCP Chat</h2>
          <p className="text-slate-400 font-medium">
            Please sign in with your Entra ID account to start chatting with the agent and using its tools.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="flex-1 flex flex-col min-h-0 bg-slate-900/40 rounded-3xl border border-slate-800/50 shadow-xl overflow-hidden relative">
      <div className="absolute top-4 right-4 z-10">
        <button
          onClick={clearChat}
          className="p-2 text-slate-500 hover:text-red-400 hover:bg-red-400/10 rounded-lg transition-all duration-200"
          title="Clear Conversation"
        >
          <Trash2 size={18} />
        </button>
      </div>
      
      <MessageList messages={messages} isLoading={isLoading} />
      
      {error && (
        <div className="mx-4 mb-2 p-3 bg-red-900/20 border border-red-500/30 rounded-xl flex items-start gap-3 text-red-400 animate-in slide-in-from-bottom-2">
          <AlertCircle size={18} className="mt-0.5 flex-shrink-0" />
          <div className="text-xs font-medium">{error}</div>
        </div>
      )}
      
      <div className="px-4 pb-4">
        <MessageInput onSendMessage={sendMessage} disabled={isLoading} />
      </div>
    </div>
  );
};

import React, { useRef, useEffect } from 'react';
import type { Message } from '../../types/chat';
import { MessageBubble } from './MessageBubble';
import { Bot, Loader2 } from 'lucide-react';

interface MessageListProps {
  messages: Message[];
  isLoading: boolean;
}

export const MessageList: React.FC<MessageListProps> = ({ messages, isLoading }) => {
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages, isLoading]);

  if (messages.length === 0) {
    return (
      <div className="flex-1 flex flex-col items-center justify-center text-center p-8 opacity-50">
        <div className="bg-slate-800 p-6 rounded-3xl mb-4 border border-slate-700/50 shadow-inner">
          <Bot size={48} className="text-blue-500 mb-2 mx-auto" />
          <h2 className="text-xl font-bold text-slate-200">How can I help you today?</h2>
          <p className="text-sm text-slate-400 max-w-xs mt-2 font-medium">
            I'm your MCP Agent. I have access to various tools to help you with your tasks.
          </p>
        </div>
        <div className="grid grid-cols-2 gap-3 max-w-md w-full">
          {['What tools do you have?', 'Help me with a task', 'Tell me a joke', 'Search something'].map((prompt) => (
            <div key={prompt} className="p-3 bg-slate-800/30 rounded-xl border border-slate-700/30 text-xs font-semibold text-slate-400">
              "{prompt}"
            </div>
          ))}
        </div>
      </div>
    );
  }

  return (
    <div className="flex-1 overflow-y-auto px-2 py-4 space-y-4 custom-scrollbar">
      {messages.map((message) => (
        <MessageBubble key={message.id} message={message} />
      ))}
      {isLoading && (
        <div className="flex items-center gap-3 text-slate-400 animate-pulse ml-1">
          <div className="w-8 h-8 rounded-lg bg-slate-800 flex items-center justify-center border border-slate-700">
            <Loader2 size={18} className="animate-spin text-blue-500" />
          </div>
          <span className="text-xs font-semibold tracking-wide uppercase">Agent is thinking...</span>
        </div>
      )}
      <div ref={messagesEndRef} />
    </div>
  );
};

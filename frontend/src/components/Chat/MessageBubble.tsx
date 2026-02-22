import React from 'react';
import { User, Bot } from 'lucide-react';
import type { Message } from '../../types/chat';
import { ToolInvocation } from './ToolInvocation';
import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';

function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

interface MessageBubbleProps {
  message: Message;
}

export const MessageBubble: React.FC<MessageBubbleProps> = ({ message }) => {
  const isUser = message.role === 'user';

  return (
    <div className={cn(
      "flex w-full mb-6 gap-3 group animate-in fade-in slide-in-from-bottom-2 duration-300",
      isUser ? "flex-row-reverse" : "flex-row"
    )}>
      <div className={cn(
        "flex-shrink-0 w-8 h-8 rounded-lg flex items-center justify-center shadow-md",
        isUser ? "bg-blue-600" : "bg-slate-700"
      )}>
        {isUser ? <User size={18} className="text-white" /> : <Bot size={18} className="text-blue-400" />}
      </div>
      
      <div className={cn(
        "flex flex-col max-w-[85%]",
        isUser ? "items-end" : "items-start"
      )}>
        <div className={cn(
          "px-4 py-3 rounded-2xl shadow-sm text-sm leading-relaxed",
          isUser 
            ? "bg-blue-600 text-white rounded-tr-none" 
            : "bg-slate-800 text-slate-100 border border-slate-700 rounded-tl-none"
        )}>
          <div className="whitespace-pre-wrap">{message.content}</div>
          
          {!isUser && message.toolInvocations && message.toolInvocations.length > 0 && (
            <div className="mt-4 pt-3 border-t border-slate-700/50">
              <div className="text-[10px] font-bold uppercase tracking-wider text-slate-500 mb-2">Tool Executions</div>
              {message.toolInvocations.map((invocation, index) => (
                <ToolInvocation key={index} invocation={invocation} />
              ))}
            </div>
          )}
        </div>
        <span className="text-[10px] text-slate-500 mt-1 px-1 font-medium">
          {message.timestamp.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
        </span>
      </div>
    </div>
  );
};

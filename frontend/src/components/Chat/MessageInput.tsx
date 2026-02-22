import React, { useState, useRef, useEffect } from 'react';
import { ArrowUp } from 'lucide-react';

interface MessageInputProps {
  onSendMessage: (content: string) => void;
  disabled: boolean;
}

export const MessageInput: React.FC<MessageInputProps> = ({ onSendMessage, disabled }) => {
  const [content, setContent] = useState('');
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  const handleSend = () => {
    if (content.trim() && !disabled) {
      onSendMessage(content.trim());
      setContent('');
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  useEffect(() => {
    if (textareaRef.current) {
      textareaRef.current.style.height = 'auto';
      textareaRef.current.style.height = `${Math.min(textareaRef.current.scrollHeight, 200)}px`;
    }
  }, [content]);

  return (
    <div className="mt-auto pt-4">
      <div className="relative group">
        <div className="absolute -inset-0.5 bg-gradient-to-r from-blue-500 to-purple-600 rounded-2xl blur opacity-20 group-focus-within:opacity-40 transition duration-1000 group-hover:duration-200"></div>
        <div className="relative bg-slate-800 border border-slate-700 rounded-2xl p-2 shadow-2xl overflow-hidden transition-all duration-300 focus-within:border-slate-500 focus-within:ring-1 focus-within:ring-slate-500">
          <textarea
            ref={textareaRef}
            rows={1}
            value={content}
            onChange={(e) => setContent(e.target.value)}
            onKeyDown={handleKeyDown}
            placeholder="Message the MCP agent..."
            className="w-full bg-transparent text-slate-100 text-sm focus:outline-none py-3 px-4 resize-none min-h-[44px] max-h-[200px]"
            disabled={disabled}
          />
          <div className="flex justify-between items-center px-4 pb-2">
            <div className="text-[10px] text-slate-500 font-medium">
              Shift + Enter for new line
            </div>
            <button
              onClick={handleSend}
              disabled={!content.trim() || disabled}
              className="bg-blue-600 hover:bg-blue-500 disabled:bg-slate-700 disabled:text-slate-500 text-white p-2 rounded-xl transition-all duration-200 shadow-lg"
            >
              <ArrowUp size={20} />
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

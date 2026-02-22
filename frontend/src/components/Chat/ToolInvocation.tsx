import React, { useState } from 'react';
import { Settings, ChevronDown, ChevronUp, Clock } from 'lucide-react';
import type { ToolInvocation as ToolInvocationType } from '../../types/chat';

interface ToolInvocationProps {
  invocation: ToolInvocationType;
}

export const ToolInvocation: React.FC<ToolInvocationProps> = ({ invocation }) => {
  const [isExpanded, setIsExpanded] = useState(false);

  return (
    <div className="my-2 bg-slate-800/50 rounded-lg border border-slate-700 overflow-hidden text-sm">
      <button
        onClick={() => setIsExpanded(!isExpanded)}
        className="w-full flex items-center justify-between px-3 py-2 hover:bg-slate-700/50 transition-colors duration-150"
      >
        <div className="flex items-center gap-2">
          <div className="bg-purple-500/20 p-1 rounded">
            <Settings size={14} className="text-purple-400" />
          </div>
          <span className="font-mono text-xs font-semibold text-slate-300">
            {invocation.toolName}
          </span>
          <div className="flex items-center gap-1 text-[10px] text-slate-500 bg-slate-900/50 px-1.5 py-0.5 rounded ml-2">
            <Clock size={10} />
            {invocation.duration}
          </div>
        </div>
        {isExpanded ? <ChevronUp size={14} className="text-slate-500" /> : <ChevronDown size={14} className="text-slate-500" />}
      </button>
      
      {isExpanded && (
        <div className="p-3 bg-slate-900/50 border-t border-slate-700 space-y-3 animate-in slide-in-from-top-1 duration-200">
          <div>
            <div className="text-[10px] font-bold uppercase tracking-wider text-slate-500 mb-1">Arguments</div>
            <pre className="bg-slate-950 p-2 rounded text-xs overflow-x-auto text-blue-300 border border-slate-800">
              {invocation.arguments || '{}'}
            </pre>
          </div>
          <div>
            <div className="text-[10px] font-bold uppercase tracking-wider text-slate-500 mb-1">Result</div>
            <pre className="bg-slate-950 p-2 rounded text-xs overflow-x-auto text-emerald-300 border border-slate-800 whitespace-pre-wrap">
              {invocation.result || 'No result returned'}
            </pre>
          </div>
        </div>
      )}
    </div>
  );
};

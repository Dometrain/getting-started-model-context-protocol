export interface ChatRequest {
  message: string;
  conversationId?: string;
}

export interface ToolInvocation {
  toolName: string;
  arguments?: string;
  result?: string;
  duration: string;
}

export interface ChatResponse {
  answer: string;
  conversationId: string;
  toolInvocations: ToolInvocation[];
}

export interface Message {
  id: string;
  role: 'user' | 'agent';
  content: string;
  timestamp: Date;
  toolInvocations?: ToolInvocation[];
}

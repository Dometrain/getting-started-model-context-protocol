import { useState, useCallback } from 'react';
import type { Message, ChatResponse } from '../types/chat';
import { chatService } from '../services/chatService';

export const useChat = () => {
  const [messages, setMessages] = useState<Message[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [conversationId, setConversationId] = useState<string | undefined>();
  const [error, setError] = useState<string | null>(null);

  const sendMessage = useCallback(async (content: string) => {
    if (!content.trim()) return;

    const userMessage: Message = {
      id: Date.now().toString(),
      role: 'user',
      content,
      timestamp: new Date(),
    };

    setMessages((prev) => [...prev, userMessage]);
    setIsLoading(true);
    setError(null);

    try {
      const response: ChatResponse = await chatService.sendMessage(content, conversationId);
      
      const agentMessage: Message = {
        id: (Date.now() + 1).toString(),
        role: 'agent',
        content: response.answer,
        timestamp: new Date(),
        toolInvocations: response.toolInvocations,
      };

      setConversationId(response.conversationId);
      setMessages((prev) => [...prev, agentMessage]);
    } catch (err: any) {
      setError(err.message || 'An error occurred while communicating with the agent.');
    } finally {
      setIsLoading(false);
    }
  }, [conversationId]);

  const clearChat = useCallback(() => {
    setMessages([]);
    setConversationId(undefined);
    setError(null);
  }, []);

  return {
    messages,
    isLoading,
    conversationId,
    error,
    sendMessage,
    clearChat,
  };
};

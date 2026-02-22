import type { ChatRequest, ChatResponse } from "../types/chat";
import { authService } from "./authService";

const API_BASE_URL = "https://localhost:5001";

class ChatService {
  async sendMessage(message: string, conversationId?: string): Promise<ChatResponse> {
    const isAuthEnabled = localStorage.getItem('mcp_auth_enabled') !== 'false';
    let token: string | null = null;

    if (isAuthEnabled) {
      // Check manual token first
      token = localStorage.getItem('mcp_manual_token');
      
      // Fallback to MSAL
      if (!token) {
        token = await authService.getToken();
      }

      if (!token) {
        throw new Error("Not authenticated or token expired. Please login or provide a manual token.");
      }
    }

    const request: ChatRequest = {
      message,
      conversationId,
    };

    const headers: Record<string, string> = {
      "Content-Type": "application/json",
    };

    if (token) {
      headers["Authorization"] = `Bearer ${token}`;
    }

    const response = await fetch(`${API_BASE_URL}/api/chat`, {
      method: "POST",
      headers,
      body: JSON.stringify(request),
    });

    if (response.status === 401) {
      throw new Error("Unauthorized. Your session may have expired.");
    }

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.detail || "Failed to send message to the agent.");
    }

    return await response.json();
  }
}

export const chatService = new ChatService();

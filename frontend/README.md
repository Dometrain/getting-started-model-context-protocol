# MCP Chat Frontend

A modern, dark-mode React application for chatting with the MCP Client API.

## Features

- **Modern UI**: Built with React, Tailwind CSS 4, and Lucide icons.
- **Entra ID Authentication**: Secure login via MSAL.js.
- **Tool Invocations**: Visual feedback and detailed logs for MCP tool executions.
- **Dark Mode**: High-contrast, easy-on-the-eyes design by default.
- **Responsive**: Works great on various screen sizes.

## Prerequisites

- Node.js (v20 or later recommended)
- MCP Client API running on `https://localhost:5001`

## Setup

1. **Install Dependencies**:
   ```bash
   cd frontend
   npm install
   ```

2. **Configuration**:
   Before running, update `src/config/msalConfig.ts` with your Entra ID values:
   - Replace `<FRONTEND_CLIENT_ID>` with your MCP Client API app registration's Application (client) ID.
   - Replace `<AZURE_TENANT_ID>` with your Microsoft Entra tenant ID.

3. **Run Development Server**:
   ```bash
   npm run dev
   ```
   The app will be available at [http://localhost:5173](http://localhost:5173).

## Authentication

The application uses Microsoft Entra ID (formerly Azure AD) for authentication. 
- You must sign in to send messages.
- The application will request a token with the `access_as_user` scope for the MCP Client API.
- If your session expires, you will be prompted to sign in again.

## Project Structure

- `src/components`: UI components (Chat, Auth, Layout)
- `src/services`: API and Auth services
- `src/hooks`: React hooks for shared logic
- `src/types`: TypeScript definitions
- `src/config`: MSAL and app configuration

## Technologies

- [Vite](https://vitejs.dev/)
- [React](https://reactjs.org/)
- [Tailwind CSS 4](https://tailwindcss.com/)
- [MSAL.js](https://github.com/AzureAD/microsoft-authentication-library-for-js)
- [Lucide Icons](https://lucide.dev/)

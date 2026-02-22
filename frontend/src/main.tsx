import ReactDOM from 'react-dom/client'
import { PublicClientApplication, EventType } from '@azure/msal-browser'
import { MsalProvider } from '@azure/msal-react'
import { msalConfig } from './config/msalConfig'
import App from './App.tsx'
import './index.css'

console.log("Main.tsx loaded");

const msalInstance = new PublicClientApplication(msalConfig);

const root = ReactDOM.createRoot(document.getElementById('root')!);

const renderApp = () => {
  console.log("Rendering app...");
  root.render(
    <MsalProvider instance={msalInstance}>
      <App />
    </MsalProvider>
  );
};

msalInstance.initialize()
  .then(() => {
    console.log("MSAL initialized");
    
    if (!msalInstance.getActiveAccount() && msalInstance.getAllAccounts().length > 0) {
      msalInstance.setActiveAccount(msalInstance.getAllAccounts()[0]);
    }

    msalInstance.addEventCallback((event) => {
      if (event.eventType === EventType.LOGIN_SUCCESS && event.payload) {
        const payload = event.payload as any;
        const account = payload.account;
        msalInstance.setActiveAccount(account);
      }
    });

    renderApp();
  })
  .catch(err => {
    console.error("MSAL init error:", err);
    root.render(
      <div style={{ color: 'white', padding: '20px' }}>
        <h1>Initialization Error</h1>
        <pre>{err.message}</pre>
      </div>
    );
  });

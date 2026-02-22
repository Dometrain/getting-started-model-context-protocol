# Demo MCP Server (.NET 10)

**With Azure AD (Entra ID) Authentication.** Builds on section 08. This section adds **MCP Resources** - read-only contextual data such as `todo://schema` (JSON schema) and `todo://stats/dashboard` (real-time statistics). Demonstrates the separation of Resources (context) vs Tools (actions).

## Before you run

1. **Azure AD (Entra ID)**: Replace `<AZURE_TENANT_ID>` and `<AZURE_CLIENT_ID>` in `src/McpServerResources/appsettings.json` and `.vscode/launch.json` (for F5 debugging).
2. **Azure Storage Connection String**: Replace `<AZURE_STORAGE_CONNECTION_STRING>` in the same files.

---

# MCP Server - Azure Table Storage Setup (Cheapest & Simplest)

This guide explains how to set up **Azure Table Storage** and connect it to a **.NET MCP Server** using a **storage account connection string**.

This approach is deliberately:
- ✅ Low cost
- ✅ Low ceremony
- ✅ Easy to understand
- ❌ Not production-grade security (by design, for teaching)

---

## Overview

We will:

1. Create a **low-cost Azure Storage Account**
2. Enable **Table Storage**
3. Copy the **connection string**
4. Store it in `appsettings.json`
5. Use it from C# to read/write todos

---

## 1. Create the Azure Storage Account

In the Azure Portal:

1. **Create resource** → **Storage account**
2. Use the following settings:

### Basics
- **Resource group**: your existing MCP server resource group
- **Storage account name**: any globally unique name (e.g. `jcmpserverdemo01`)
- **Region**: same as your Container App
- **Performance**: `Standard`
- **Replication**: `Locally-redundant storage (LRS)`
- **Preferred storage type**:  
  👉 **Other (tables and queues)**

> This is the cheapest possible configuration that supports Azure Table Storage.

---

## 2. Security & Networking (keep it simple)

You can leave most settings as default.

Important points:
- **Public network access**: Enabled
- **Secure transfer required**: Enabled
- **Storage account key access**: Enabled

We intentionally allow key-based access to avoid managed identity and RBAC complexity in this demo.

---

## 3. Create the Table

After the storage account is created:

## MCP Resources best practices

This server demonstrates the following MCP resource patterns:

### Resources vs Tools

A key best practice in MCP is the separation of **Resources** and **Tools**:

*   **Resources** are for **read-only contextual information**. This includes:
    *   System schemas and metadata (`todo://schema`)
    *   Real-time dashboards and statistics (`todo://stats/dashboard`)
    *   Documentation or background context that helps the AI understand the environment.
*   **Tools** are for **actions and data manipulation**. This includes:
    *   CRUD operations on individual records (`GetTodo`, `AddTodo`, `UpdateTodo`)
    *   Executing specific tasks or workflows.

**Why this matters:** Resources are designed to provide the AI model with "context" to its environment. Using resources to access individual records from a database is an anti-pattern because:
1.  It bloats the resource list with thousands of items.
2.  It doesn't provide meaningful context for the model's reasoning process.
3.  Tools are better suited for targeted data retrieval and modification.

### Dynamic Resources

The `todo://stats/dashboard` resource demonstrates the **Dynamic Resource** pattern. These are resources that change as the underlying data changes and support **Subscriptions**, allowing the client to receive updates whenever the system state changes.


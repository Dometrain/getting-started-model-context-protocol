# Demo MCP Server (.NET 10)

Builds on section 02 by adding **todo list tools** (AddTodo, GetTodoList, DeleteTodo) backed by **Azure Table Storage**. No authentication. Introduces Azure Storage setup used in later sections.

## Before you run

1. **Azure Storage Connection String**: Replace `<AZURE_STORAGE_CONNECTION_STRING>` in:
   - `src/McpServerTodoList/appsettings.json`
   - `.vscode/launch.json` (env section, for F5 debugging)
   
   Get the value from Azure Portal → Storage Account → Access keys.

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

1. Go to **Storage account → Storage browser**
2. Open **Tables**
3. Click **Add table**
4. Name it: **Todos**


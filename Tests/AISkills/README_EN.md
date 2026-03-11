# F8 Integration with unity-mcp

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## Import Plugin (Requires unity-mcp first)
Note! unity-mcp: https://github.com/CoplayDev/unity-mcp  
Method 1: Download files directly and place them in Unity  
Method 2: Unity -> Click Menu Bar -> Window -> Package Manager -> Click + button -> Add Package from git URL -> Enter: https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main

## Introduction (Want to start making games with just one click on F8, without extra worries)
Integrated with the unity-mcp library.
1. After importing using this [Official Tutorial (Quick Start)](https://github.com/CoplayDev/unity-mcp?tab=readme-ov-file#quick-start), install [Python 3.10+](https://www.python.org/) and the [uv](https://docs.astral.sh/uv/getting-started/installation/) library.  
2. Once MCP for Unity is ready.
3. Copy the `F8Framework/Tests/AISkills/f8framework-skills` directory to the corresponding skills directory of your AI assistant in the project root.
   * Refer to the table below to copy `f8framework-skills` to the appropriate directory:
4. If everything is correct, chatting directly with the AI will prioritize using F8Framework features.

### Correspondence Between AI Tools' Skills Features and Directories

| Tool Name | Skills Support | Corresponding SKILLS Directory / Description |
| :--- | :--- | :--- |
| **Antigravity** | Yes | `~/.gemini/antigravity/skills/` (Global)<br>`.antigravity/skills/` (Project-level) |
| **Cherry Studio** | To be verified | Support for Agent Skills is not explicitly mentioned in currently public materials. |
| **Claude Code / Claude Desktop** | Yes | `~/.claude/skills/` (Personal)<br>`.claude/skills/` (Project)<br>(Skills were introduced by Anthropic and are core to the ecosystem) |
| **Cline** | Yes | No explicit default path found, but as a key tool in this ecosystem, it generally adopts the Agent Skills standard. |
| **CodeBuddy** | Yes | `~/.codebuddy/skills/` (User-level)<br>`.codebuddy/skills/` (Project-level)<br>(The first product in China to support Skills) |
| **CLI Codex** | Yes | `~/.codex/skills/` (Global)<br>`.codex/skills/` (Project-level) |
| **Codex** | Yes | `~/.codex/skills/` (Global)<br>`.codex/skills/` (Project-level) |
| **Cursor** | Yes | `~/.cursor/skills/` or similar directory within the project (Official changelog has announced support) |
| **Gemini CLI** | Yes | `.gemini/skills/` or `.agents/skills/` |
| **GitHub Copilot CLI** | Yes | `~/.copilot/skills/` (Personal)<br>`.github/skills/` (Project)<br>(Explicitly supported by official documentation) |
| **Kilo Code** | Yes | `~/.kilocode/skills/` (Global)<br>`.kilocode/skills/` (Project)<br>(Supports mode-specific directories like `skills-code/`) |
| **Kiro** | Yes | `~/.kiro/skills/` (Global)<br>`.kiro/skills/` (Workspace/Project) |
| **OpenCode** | To be verified | Support for Agent Skills is not explicitly mentioned in currently public materials. |
| **Qwen Code Rider** | To be verified | Support for Agent Skills is not explicitly mentioned in currently public materials. |
| **Trae / Trae CN** | To be verified | Support for Agent Skills is not explicitly mentioned in currently public materials. |
| **GitHub Copilot / VSCode / VSCode Insiders** | Supported (Partial) | These are essentially code editors or plugin platforms. Skills features are implemented by specific AI assistants running within them (e.g., Cline, Copilot CLI), rather than the editor itself. Their directories are defined by the specific AI assistant. For example, Cline configured in VSCode following Cline's specifications for its skills directory. |
| **Windsurf** | To be verified | Support for Agent Skills is not explicitly mentioned in currently public materials. |

# F8 接入unity-mcp

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## 导入插件（需要首先导入unity-mcp）
注意！unity-mcp：https://github.com/CoplayDev/unity-mcp  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
接入unity-mcp库。
1. 使用这个[ 官方教程（快速开始） ](https://github.com/CoplayDev/unity-mcp?tab=readme-ov-file#quick-start)导入后，安装 [Python 3.10+](https://www.python.org/) 和 [uv](https://docs.astral.sh/uv/getting-started/installation/) 库。  
2. MCP for Unity 准备就绪后。
3. 复制 `F8Framework/Tests/AISkills/f8framework-skills` 目录，到项目根目录的 AI 助手对应 skills 目录中。
   * 参考下表将 `f8framework-skills` 复制到对应目录：
4. 如无问题，直接与AI对话，会优先使用 F8Framework 的功能。

### 各工具的 Skills 功能与目录对应关系

| 工具名称 | 是否支持 Skills 功能 | 对应 SKILLS 目录名称 / 说明 |
| :--- | :--- | :--- |
| **Antigravity** | 是 | `~/.gemini/antigravity/skills/` (全局)<br>`.antigravity/skills/` (项目级) |
| **Cherry Studio** | 待核实 | 目前公开资料中未明确提及对 Agent Skills 的支持。 |
| **Claude Code / Claude Desktop** | 是 | `~/.claude/skills/` (个人)<br>`.claude/skills/` (项目)<br>(Skills 功能由 Anthropic 提出，是生态的核心) |
| **Cline** | 是 | 未找到明确的默认路径，但作为该生态的重要工具，普遍采用 Agent Skills 标准。 |
| **CodeBuddy** | 是 | `~/.codebuddy/skills/` (用户级)<br>`.codebuddy/skills/` (项目级)<br>(国内首家支持 Skills 的产品) |
| **CLI Codex** | 是 | `~/.codex/skills/`(全局)<br>`.codex/skills/` (项目级)|
| **Codex** | 是 | `~/.codex/skills/`(全局)<br>`.codex/skills/` (项目级)|
| **Cursor** | 是 | `~/.cursor/skills/` 或项目内类似目录 (官方 changelog 已宣布支持) |
| **Gemini CLI** | 是 | `.gemini/skills/` 或 `.agents/skills/` |
| **GitHub Copilot CLI** | 是 | `~/.copilot/skills/` (个人)<br>`.github/skills/` (项目)<br>(官方文档明确支持) |
| **Kilo Code** | 是 | `~/.kilocode/skills/` (全局)<br>`.kilocode/skills/` (项目)<br>(支持模式专属目录如 `skills-code/`) |
| **Kiro** | 是 | `~/.kiro/skills/` (全局)<br>`.kiro/skills/` (工作区/项目) |
| **OpenCode** | 待核实 | 目前公开资料中未明确提及对 Agent Skills 的支持。 |
| **Qwen Code Rider** | 待核实 | 目前公开资料中未明确提及对 Agent Skills 的支持。 |
| **Trae / Trae CN** | 待核实 | 目前公开资料中未明确提及对 Agent Skills 的支持。 |
| **GitHub Copilot / VSCode / VSCode Insiders** | 支持 (部分) | 这些本质是代码编辑器 or 插件平台。Skills 功能是由运行在其中的特定 AI 助手（如 Cline, Copilot CLI）实现的，而非编辑器本身。其目录由具体 AI 助手定义。例如，在 VSCode 中配置的 Cline，其技能目录会遵循 Cline 的规范。 |
| **Windsurf** | 待核实 | 目前公开资料中未明确提及对 Agent Skills 的支持。 |

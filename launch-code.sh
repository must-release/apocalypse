#!/bin/bash

# 파일 경로를 인자로 받음
SCRIPT_FILE="$1"

# 워크스페이스 파일 경로 (필요 시 절대경로로 수정 가능)
WORKSPACE_PATH="$(cd "$(dirname "$0")"; pwd)/Scripts.code-workspace"

# VS Code로 워크스페이스 + 스크립트 파일 열기
open -a "Visual Studio Code" --args "$WORKSPACE_PATH" "$SCRIPT_FILE"
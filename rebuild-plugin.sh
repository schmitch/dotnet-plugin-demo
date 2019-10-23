#!/usr/bin/env bash

set -e

RESET="\033[0m"
YELLOW="\033[0;33m"
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
pushd $DIR >/dev/null

publish_dir="$DIR/bin/plugins/Finder.Plugin.Demo/"

publish() {
    echo ""
    dotnet publish --no-restore Finder.Plugin.Demo/ -o $publish_dir -nologo
    echo ""
}

echo -e "${YELLOW}run.sh:${RESET} Compiling apps"
dotnet build Finder.Plugin.Host.Demo -nologo -clp:NoSummary
publish

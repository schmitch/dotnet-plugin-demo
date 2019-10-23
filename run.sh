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

trap "kill 0" EXIT

echo -e "${YELLOW}run.sh:${RESET} Use CTRL+C to exit"

dotnet run --no-build -p Finder.Plugin.Host.Demo -- "$publish_dir/Finder.Plugin.Demo.dll" &

while true
do
    sleep 5
    echo -e "${YELLOW}run.sh:${RESET} Rebuilding plugin..."
    publish
done
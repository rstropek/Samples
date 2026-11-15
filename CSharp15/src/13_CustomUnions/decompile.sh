#!/usr/bin/env bash
# Show that the compiler uses TryGetValue/HasValue (no boxing) for the custom
# ValueOption<T> union, but box + get_Value + isinst for the shorthand union.
# Requires: .NET 11 SDK + `dotnet tool install -g ilspycmd`.
set -euo pipefail
cd "$(dirname "$0")"

dotnet build -c Release
IL=$(ilspycmd "bin/Release/net11.0/13_CustomUnions.dll" -il)

show() { # $1 = method name
  echo "===================  Probes::$1  ==================="
  echo "$IL" | awk "/ $1 \\(/,/end of method Probes::$1/" \
    | grep -E "box |isinst |unbox|ValueOption|BoxedOption" || true
  echo
}
show SumBoxed
show SumValueOption
show Describe

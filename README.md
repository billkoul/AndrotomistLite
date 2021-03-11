# AndrotomistLite
A lightweight CLI version of the Androtomist tool (https://github.com/billkoul/Androtomist, Androtomist.com)

AndrotomistLite consists of a console application written in .NET Core 3.1 which allows:

1. Static analysis of .apk files using apkTool<br>
2. Dynamic instrumentation using Frida<br>

Static analysis is ready-to-go with Apktool (https://ibotpeaches.github.io/Apktool/), just place the apktool in the same folder as the Runner.exe (bin\Debug / bin/Release)<br>

Dynamic instrumentation requires extra configuration in the appsettings.json file, such as the android platform tools folder path, the frida folder path, instrumentation script path, and remote address & port to allow connections with network VMs<br>

This is a lightweight tool and is better suited for smaller projects that don't require a database connection.

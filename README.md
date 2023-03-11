# AndrotomistLite
A lightweight CLI version of the Androtomist tool (https://github.com/billkoul/Androtomist, Androtomist.com) which performs Android .APK analysis.

AndrotomistLite consists of a console application written in .NET Core 3.1 which allows:

1. Static analysis of .apk files using <a href="https://github.com/Giannisgre/APKProfiler">APKProfiler</a><br>
2. Dynamic instrumentation using Frida<br>

Static analysis requires Apktool (https://ibotpeaches.github.io/Apktool/), just place the apktool in the same folder as the Runner.exe, well as the .bat file to call Apktool for Windows (https://github.com/iBotPeaches/Apktool/blob/master/scripts/windows/apktool.bat). Apktool requires Java. <br>

Dynamic instrumentation requires extra configuration in the appsettings.json file, such as the android platform tools folder path, the frida folder path, instrumentation script path, and remote address & port to allow connections with network VMs<br>

This is a lightweight tool and is better suited for smaller projects that don't require a database connection.

Licence
Androtomist's source code is offered under the European Union Public Licence (https://ec.europa.eu/info/european-union-public-licence_en)

Please cite our paper:
Kouliaridis, V.; Kambourakis, G.; Geneiatakis, D.; Potha, N. Two Anatomists Are Better than One—Dual-Level Android Malware Detection. Symmetry 2020, 12, 1128.

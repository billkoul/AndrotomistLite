# AndrotomistLite
A lightweight CLI version of the Androtomist tool (https://github.com/billkoul/Androtomist, Androtomist.com) which performs Android .APK analysis.

AndrotomistLite consists of a console application written in .NET Core 3.1 which allows:

1. Code analysis of .apk files using <a href="https://apktool.org/docs/install/">ApkTool</a> and <a href="https://github.com/Giannisgre/APKProfiler">APKProfiler</a><br>
2. Taint analysis using <a href="https://github.com/gvieralopez/pyFlowDroid">pyflowdroid</a><br>
3. Dynamic instrumentation using <a href="https://github.com/frida">Frida</a><br>

Static analysis requires Apktool (https://apktool.org/docs/install/). Download the latest version of the apktool, rename it to apktool.jar and place it in the same folder as the Runner.exe (e.g., while debugging Runner/bin/debug/net10.0), well as the .bat file to call Apktool in Windows. Apktool requires Java. After installing Java it is recommended to add it in the Path (e.g., for windows, WIN+R -> sysdm.cpl -> System Properties -> Advanced -> Environment Variables -> System Variables -> Edit Path -> Add -> Add ONLY the folder path to Java’s bin directory, e.g., C:\Program Files\Java\jdk-x.x.x\bin) <br>

Taint analysis requires installation of pyflowdroid:<br>
```
$ pip install pyflowdroid
$ python -m pyflowdroid install
```

Dynamic instrumentation requires extra configuration in the appsettings.json file, such as the android platform tools folder path, the frida folder path, instrumentation script path, and remote address & port to allow connections with network VMs<br>

This is a lightweight tool and is better suited for smaller projects that don't require a database connection.

Licence
Androtomist's source code is offered under the European Union Public Licence (https://ec.europa.eu/info/european-union-public-licence_en)

Please cite our paper:
Kouliaridis, V.; Kambourakis, G.; Geneiatakis, D.; Potha, N. Two Anatomists Are Better than One—Dual-Level Android Malware Detection. Symmetry 2020, 12, 1128.

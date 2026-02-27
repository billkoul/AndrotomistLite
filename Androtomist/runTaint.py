import pyflowdroid
import sys

flowdroid_logs = pyflowdroid.analyze_apk(str(sys.argv[1]))

print(flowdroid_logs)
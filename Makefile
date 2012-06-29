help: ENDAPLCNetLib/bin/Debug/ENDAPLCNetLib.dll
	NDoc3Console.exe -project=PLCNetLib.ndoc ENDAPLCNetLib/bin/Debug/ENDAPLCNetLib.dll
release:
	cd ..; zip -r PLCNetLib/PLCNetLib-`date +"%Y%m%d-%H%M"`.zip PLCNetLib -x "*/.git/*" -x "*/TestResults/*" -x "*/bin/*" -x "*~" -x "PLCNetLib/*.zip"


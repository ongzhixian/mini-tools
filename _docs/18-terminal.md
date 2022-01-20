# Windows Terminal


WARNING! Do not use wt.exe in $PROFILE!
         It will create new Windows continuously!


Dell

wt --title tabname1 `; --title "asd"


wt --title tabname1 `; --title "asd"


wt ping docs.microsoft.com


Quake mode!
wt -w _quake

Open a new tab with the default profile in the current window
`wt -w 0 nt`

# Examples


Create new Window with:
1.  A tab to `D:\src\github\mini-tools` titled `MiniTools`
2.  A tab to `D:\src\github\Dn6Poc` titled `dn6Poc`
3.  A tab to `C:\Apps\kafka` titled `kafka-proc`
4.
split-pane --tabColor "#f59218" 



```ps1
wt -w 0 new-tab --tabColor "#009999" -d D:\src\github\mini-tools --title miniTools `; `
new-tab --tabColor "#0e949a" -d D:\src\github\Dn6Poc --title dn6Poc `; `
new-tab --tabColor "#a09399" -d C:\Apps\kafka --title zoo `; `
split-pane -H --tabColor "#f59218" -d C:\Apps\kafka --title kafka 
```

```
wt -w new-tab -d D:\src\github\mini-tools --title miniTools `; `
new-tab -d D:\src\github\Dn6Poc --title dn6Poc `; `
new-tab -d C:\Apps\kafka --title zoo `; `
split-pane -H -d C:\Apps\kafka --title kafka 
```

// Open a new tab in the terminal window named foo with the default profile. 
If foo does not exist then, create a new window named foo.
wt -w proc nt -d C:\Apps\kafka --title kafka -c ".\bin\windows\kafka-server-start.bat .\config\server.properties"


wt -w proc nt -d C:\Apps\kafka --title kafka "C:\Apps\kafka\bin\windows\kafka-server-start.bat .\config\server.properties"


wt -w proc ping docs.microsoft.com

Dump all processes into a separate "proc" window


wt -w proc C:\Apps\kafka\bin\windows\zookeeper-server-start.bat C:\Apps\kafka\config\zookeeper.properties --title zookeeper
wt -w proc C:\Apps\kafka\bin\windows\kafka-server-start.bat C:\Apps\kafka\config\server.properties --title kafka


# Reference

https://docs.microsoft.com/en-us/windows/terminal/command-line-arguments?tabs=powershell#command-line-argument-examples

https://docs.microsoft.com/en-us/windows/terminal/tips-and-tricks#quake-mode
